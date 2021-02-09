using TronNet.ABI.Decoders;
using TronNet.ABI.Encoders;

namespace TronNet.ABI
{
    public class BytesType : ABIType
    {
        public BytesType() : base("bytes")
        {
            Decoder = new BytesTypeDecoder();
            Encoder = new BytesTypeEncoder();
        }

        public override int FixedSize => -1;
    }
}