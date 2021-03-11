using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Crypto
{

    public static class Base58Encoder
    {
        public static readonly char[] Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static readonly int[] Indexes = new int[128];

        static Base58Encoder()
        {
            for (var i = 0; i < Indexes.Length; i++)
            {
                Indexes[i] = -1;
            }
            for (var i = 0; i < Alphabet.Length; i++)
            {
                Indexes[Alphabet[i]] = i;
            }
        }

        private static byte Divmod58(byte[] number, int startAt)
        {
            var remainder = 0;
            for (var i = startAt; i < number.Length; i++)
            {
                var digit256 = (int)number[i] & 0xFF;
                var temp = remainder * 256 + digit256;

                number[i] = (byte)(temp / 58);

                remainder = temp % 58;
            }

            return (byte)remainder;
        }

        private static byte Divmod256(byte[] number58, int startAt)
        {
            var remainder = 0;
            for (var i = startAt; i < number58.Length; i++)
            {
                var digit58 = (int)number58[i] & 0xFF;
                var temp = remainder * 58 + digit58;
                number58[i] = (byte)(temp / 256);
                remainder = temp % 256;
            }

            return (byte)remainder;
        }

        private static byte[] CopyOfRange(byte[] source, int from, int to)
        {
            var range = new byte[to - from];
            Array.Copy(source, from, range, 0, range.Length);
            return range;
        }

        public static byte[] DecodeFromBase58Check(string addressBase58)
        {
            if (string.IsNullOrWhiteSpace(addressBase58)) return null;

            byte[] decodeCheck = Decode(addressBase58);
            if (decodeCheck.Length <= 4) return null;


            byte[] decodeData = new byte[decodeCheck.Length - 4];
            Array.Copy(decodeCheck, 0, decodeData, 0, decodeData.Length);
            var hash0 = Hash(Hash(decodeData));
            var valid = true;
            for (int i = 0; i < 4; i++)
            {
                if (hash0[i] != decodeCheck[decodeData.Length + i])
                {
                    valid = false;
                    break;
                }
            }
            return (valid ? decodeData : null);
        }

        public static byte[] TwiceHash(byte[] input)
        {
            return Hash(Hash(input));
        }
        public static byte[] Hash(byte[] input)
        {
            byte[] hashBytes;
            var hash = new System.Security.Cryptography.SHA256Managed();
            using var stream = new MemoryStream(input);
            try
            {
                hashBytes = hash.ComputeHash(stream);
            }
            finally
            {
                hash.Clear();
            }

            return hashBytes;
        }

        public static string EncodeFromHex(string hexAddress, byte prefix)
        {
            var hexBytes = hexAddress.HexToByteArray();
            var addressBytes = new byte[21];
            Array.Copy(hexBytes, hexBytes.Length - 20, addressBytes, 1, 20);
            addressBytes[0] = prefix;
            var hash = TwiceHash(addressBytes);
            var bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            var addressChecksum = new byte[25];
            Array.Copy(addressBytes, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);
            return Encode(addressChecksum);
        }
        public static string EncodeFromHex(byte[] hexBytes, byte prefix)
        {
            var addressBytes = new byte[21];
            Array.Copy(hexBytes, hexBytes.Length - 20, addressBytes, 1, 20);
            addressBytes[0] = prefix;
            var hash = TwiceHash(addressBytes);
            var bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            var addressChecksum = new byte[25];
            Array.Copy(addressBytes, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);
            return Encode(addressChecksum);
        }

        public static string Encode(byte[] input)
        {
            if (input.Length == 0)
            {
                return "";
            }
            input = CopyOfRange(input, 0, input.Length);
            // Count leading zeroes.
            var zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
            {
                ++zeroCount;
            }
            // The actual encoding.
            var temp = new byte[input.Length * 2];
            var j = temp.Length;

            var startAt = zeroCount;
            while (startAt < input.Length)
            {
                var mod = Divmod58(input, startAt);
                if (input[startAt] == 0)
                {
                    ++startAt;
                }
                temp[--j] = (byte)Alphabet[mod];
            }

            // Strip extra '1' if there are some after decoding.
            while (j < temp.Length && temp[j] == Alphabet[0])
            {
                ++j;
            }
            // Add as many leading '1' as there were leading zeros.
            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)Alphabet[0];
            }

            var output = CopyOfRange(temp, j, temp.Length);
            return Encoding.ASCII.GetString(output);
        }


        public static byte[] Decode(string input)
        {
            if (input.Length == 0)
            {
                return new byte[0];
            }


            var input58 = new byte[input.Length];
            // Transform the String to a base58 byte sequence
            for (var i = 0; i < input.Length; ++i)
            {
                var c = input[i];

                int digit58 = -1;
                if (c >= 0 && c < 128)
                {
                    digit58 = Indexes[c];
                }
                if (digit58 < 0)
                {
                    throw new ArgumentOutOfRangeException("Illegal character " + c + " at " + i);
                }

                input58[i] = (byte)digit58;
            }

            // Count leading zeroes
            var zeroCount = 0;
            while (zeroCount < input58.Length && input58[zeroCount] == 0)
            {
                ++zeroCount;
            }
            // The encoding
            var temp = new byte[input.Length];
            var j = temp.Length;

            var startAt = zeroCount;
            while (startAt < input58.Length)
            {
                byte mod = Divmod256(input58, startAt);
                if (input58[startAt] == 0)
                {
                    ++startAt;
                }

                temp[--j] = mod;
            }

            // Do no add extra leading zeroes, move j to first non null byte.
            while (j < temp.Length && temp[j] == 0)
            {
                ++j;
            }

            return CopyOfRange(temp, j - zeroCount, temp.Length);
        }
    }

}
