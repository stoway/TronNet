using System.Linq;
using TronNet.ABI.Decoders;

namespace TronNet.ABI
{
    public class EncoderDecoderHelpers
    {
        public static int GetNumberOfBytes(byte[] encoded)
        {
            var intDecoder = new IntTypeDecoder();
            var numberOfBytesEncoded = encoded.Take(32);
            return intDecoder.DecodeInt(numberOfBytesEncoded.ToArray());
        }
    }
}