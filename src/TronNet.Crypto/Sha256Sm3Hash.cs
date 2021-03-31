using Google.Protobuf;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Crypto
{
    public class Sha256Sm3Hash
    {
        public static int LENGTH = 32; // bytes
        public static Sha256Sm3Hash ZERO_HASH = Wrap(new byte[LENGTH]);

        private byte[] _bytes;

        static Sha256Sm3Hash()
        {
        }

        public Sha256Sm3Hash(byte[] rawHashBytes)
        {
            checkArgument(rawHashBytes.Length == LENGTH);
            _bytes = rawHashBytes;
        }

        public byte[] GetBytes()
        {
            return _bytes;
        }

        public static Sha256Sm3Hash Wrap(byte[] rawHashBytes)
        {
            return new Sha256Sm3Hash(rawHashBytes);
        }
        public static Sha256Sm3Hash Wrap(ByteString rawHashByteString)
        {
            return Wrap(rawHashByteString.ToByteArray());
        }
        public static Sha256Sm3Hash Create(byte[] contents)
        {
            return Of(contents);
        }
        public static Sha256Sm3Hash Of(byte[] contents)
        {
            return Wrap(Hash(contents));
        }
        public static byte[] Hash(byte[] input)
        {
            return Hash(input, 0, input.Length);
        }
        public static byte[] Hash(byte[] input, int offset, int length)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(input, offset, length);
            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            return output;

        }
        private void checkArgument(bool result)
        {
            if (!result) throw new ArgumentException();
        }
    }

}
