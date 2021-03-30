using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;

namespace TronNet.Crypto
{
    /// <summary>
    ///     ECKey based on the implementation of bitcoinj, NBitcoin
    /// </summary>
    public class ECKey
    {
        public static readonly BigInteger HALF_CURVE_ORDER;
        public static readonly BigInteger CURVE_ORDER;
        public static readonly ECDomainParameters CURVE;
        internal static readonly X9ECParameters _secp256k1;
        private readonly ECKeyParameters _Key;
        private ECPublicKeyParameters _ecPublicKeyParameters;
        private byte[] _publicKey;

        private static readonly SecureRandom _secureRandom = new SecureRandom();

        private ECDomainParameters _DomainParameter;

        private static readonly BigInteger PRIME;

        public ECPrivateKeyParameters PrivateKey => _Key as ECPrivateKeyParameters;


        public static X9ECParameters Secp256k1 => _secp256k1;

        public ECDomainParameters DomainParameter
        {
            get
            {
                if (_DomainParameter == null)
                    _DomainParameter = new ECDomainParameters(Secp256k1.Curve, Secp256k1.G, Secp256k1.N, Secp256k1.H);
                return _DomainParameter;
            }
        }


        static ECKey()
        {
            //using Bouncy
            _secp256k1 = SecNamedCurves.GetByName("secp256k1");
            CURVE = new ECDomainParameters(_secp256k1.Curve, _secp256k1.G, _secp256k1.N, _secp256k1.H);
            HALF_CURVE_ORDER = _secp256k1.N.ShiftRight(1);
            CURVE_ORDER = _secp256k1.N;
            PRIME = new BigInteger(1,
               Org.BouncyCastle.Utilities.Encoders.Hex.Decode(
                   "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));
        }

        public ECKey(byte[] vch, bool isPrivate)
        {
            if (isPrivate)
            {
                _Key = new ECPrivateKeyParameters(new BigInteger(1, vch), DomainParameter);
            }
            else
            {
                var q = Secp256k1.Curve.DecodePoint(vch);
                _Key = new ECPublicKeyParameters("EC", q, DomainParameter);
            }
        }

        public ECKey()
        {
            var generator = new ECKeyPairGenerator("EC");
            generator.Init(new ECKeyGenerationParameters(CURVE, _secureRandom));
            var pair = generator.GenerateKeyPair();
            _Key = (ECPrivateKeyParameters)pair.Private;
        }

        public byte[] GetPubKeyNoPrefix()
        {
            var pubKey = GetPubKey();
            var arr = new byte[pubKey.Length - 1];
            //remove the prefix
            Array.Copy(pubKey, 1, arr, 0, arr.Length);
            return arr;
        }
        public byte[] GetPubKey()
        {
            if (_publicKey != null) return _publicKey;

            var q = GetPublicKeyParameters().Q;
            q = q.Normalize();

            _publicKey =
            Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(false);
            return _publicKey;
        }

        public ECPublicKeyParameters GetPublicKeyParameters()
        {
            if (_ecPublicKeyParameters == null)
            {
                if (_Key is ECPublicKeyParameters)
                    _ecPublicKeyParameters = (ECPublicKeyParameters)_Key;
                else
                {
                    var q = Secp256k1.G.Multiply(PrivateKey.D);
                    _ecPublicKeyParameters = new ECPublicKeyParameters("EC", q, DomainParameter);

                }
            }
            return _ecPublicKeyParameters;
        }

        public static ECKey RecoverFromSignature(int recId, ECDSASignature sig, byte[] message, bool compressed)
        {
            if (recId < 0)
                throw new ArgumentException("recId should be positive");
            if (sig.R.SignValue < 0)
                throw new ArgumentException("r should be positive");
            if (sig.S.SignValue < 0)
                throw new ArgumentException("s should be positive");
            if (message == null)
                throw new ArgumentNullException("message");


            var curve = Secp256k1;

            // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
            //   1.1 Let x = r + jn

            var n = curve.N;
            var i = BigInteger.ValueOf((long)recId / 2);
            var x = sig.R.Add(i.Multiply(n));

            //   1.2. Convert the integer x to an octet string X of length mlen using the conversion routine
            //        specified in Section 2.3.7, where mlen = ⌈(log2 p)/8⌉ or mlen = ⌈m/8⌉.
            //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
            //        conversion routine specified in Section 2.3.4. If this conversion routine outputs “invalid”, then
            //        do another iteration of Step 1.
            //
            // More concisely, what these points mean is to use X as a compressed public key.

            //using bouncy and Q value of Point

            if (x.CompareTo(PRIME) >= 0)
                return null;

            // Compressed keys require you to know an extra bit of data about the y-coord as there are two possibilities.
            // So it's encoded in the recId.
            var R = DecompressKey(x, (recId & 1) == 1);
            //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).

            if (!R.Multiply(n).IsInfinity)
                return null;

            //   1.5. Compute e from M using Steps 2 and 3 of ECDSA signature verification.
            var e = new BigInteger(1, message);
            //   1.6. For k from 1 to 2 do the following.   (loop is outside this function via iterating recId)
            //   1.6.1. Compute a candidate public key as:
            //               Q = mi(r) * (sR - eG)
            //
            // Where mi(x) is the modular multiplicative inverse. We transform this into the following:
            //               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
            // Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n). In the above equation
            // ** is point multiplication and + is point addition (the EC group operator).
            //
            // We can find the additive inverse by subtracting e from zero then taking the mod. For example the additive
            // inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and -3 mod 11 = 8.

            var eInv = BigInteger.Zero.Subtract(e).Mod(n);
            var rInv = sig.R.ModInverse(n);
            var srInv = rInv.Multiply(sig.S).Mod(n);
            var eInvrInv = rInv.Multiply(eInv).Mod(n);
            var q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, R, srInv);
            q = q.Normalize();
            if (compressed)
            {
                q = Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger());
                return new ECKey(q.GetEncoded(true), false);
            }
            return new ECKey(q.GetEncoded(false), false);
        }

        private ECDSASignature doSign(byte[] input)
        {
            if (input.Length != 32)
            {
                throw new ArgumentException(
                    "Expected 32 byte input to " + "ECDSA signature, not " + input.Length);
            }

            if (PrivateKey == null)
            {
                throw new MissingPrivateKeyException();
            }

            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            var privKeyParams = new ECPrivateKeyParameters(PrivateKey.D, CURVE);
            signer.Init(true, privKeyParams);

            var components = signer.GenerateSignature(input);

            return new ECDSASignature(components[0], components[1]).MakeCanonical();

        }

        public ECDSASignature Sign(byte[] hash)
        {
            var sig = doSign(hash);
            var thisKey = GetPubKey();

            var recId = CalculateRecId(sig, hash, thisKey);
            sig.V = new byte[] { (byte)recId };
            return sig;
        }

        internal int CalculateRecId(ECDSASignature signature, byte[] hash, byte[] uncompressedPublicKey)
        {
            var recId = -1;

            for (var i = 0; i < 4; i++)
            {
                var rec = ECKey.RecoverFromSignature(i, signature, hash, false);
                if (rec != null)
                {
                    var k = rec.GetPubKey();
                    if (k != null && k.SequenceEqual(uncompressedPublicKey))
                    {
                        recId = i;
                        break;
                    }
                }
            }
            if (recId == -1)
                throw new Exception("Could not construct a recoverable key. This should never happen.");
            return recId;
        }

        public bool Verify(byte[] hash, ECDSASignature sig)
        {
            var signer = new ECDsaSigner();
            signer.Init(false, GetPublicKeyParameters());
            return signer.VerifySignature(hash, sig.R, sig.S);
        }

        private void AssertPrivateKey()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException("This key should be a private key for such operation");
        }

        private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
        {
            var curve = Secp256k1.Curve;
            var compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(curve));
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
            return curve.DecodePoint(compEnc);
        }
    }

}
