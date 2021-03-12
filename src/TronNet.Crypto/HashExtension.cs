using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Crypto
{
    public static class HashExtension
    {
        public static byte[] ToSHA256Hash(this byte[] value)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(value, 0, value.Length);
            var output = new byte[digest.GetDigestSize()];

            digest.DoFinal(output, 0);
            return output;
        }

        public static byte[] ToKeccakHash(this byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
        public static string ToKeccakHash(this string value)
        {
            var input = Encoding.UTF8.GetBytes(value);
            var output = input.ToKeccakHash();
            return output.ToHex();
        }
    }
}
