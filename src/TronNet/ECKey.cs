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

namespace TronNet.Crypto
{
    public class ECKey
    {
        public static readonly ECDomainParameters CURVE;

        public static readonly BigInteger HALF_CURVE_ORDER;

        private static readonly SecureRandom _secureRandom;
        private static readonly long _serialVersionUID = -728224901792295832L;

        static ECKey()
        {
            X9ECParameters x9ecParams = SecNamedCurves.GetByName("secp256k1");
            CURVE = new ECDomainParameters(x9ecParams.Curve, x9ecParams.G, x9ecParams.N, x9ecParams.H);
            HALF_CURVE_ORDER = x9ecParams.N.ShiftRight(1);
            _secureRandom = new SecureRandom();
        }

        protected readonly ECPoint _pub;

        private readonly ICipherParameters _privKey;


        private byte[] _pubKeyHash;
        private byte[] _nodeId;

        public ECKey() : this(_secureRandom)
        {
        }
        public ECKey(SecureRandom secureRandom)
        {
           
            var gen = new ECKeyPairGenerator("EC");
            var keyGenParam = new KeyGenerationParameters(secureRandom, 256);
            gen.Init(keyGenParam);
            var keyPair = gen.GenerateKeyPair();
            _privKey = keyPair.Private;
            var pubKey = keyPair.Public;
            if (pubKey is ECPublicKeyParameters ecPublicKey)
            {
                _pub = ecPublicKey.Q;
            }
        }

        public ECKey(BigInteger priv, ECPoint pub)
        {
        }

        public static ECKey FromPrivate(byte[] privKeyBytes)
        {
            return FromPrivate(new BigInteger(privKeyBytes));
        }
        public static ECKey FromPrivate(BigInteger privKey)
        {
            return new ECKey(privKey, CURVE.G.Multiply(privKey));
        }
    }
}
