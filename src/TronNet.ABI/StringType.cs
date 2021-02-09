using TronNet.ABI.Decoders;
using TronNet.ABI.Encoders;

namespace TronNet.ABI
{
    public class StringType : ABIType
    {
        public StringType() : base("string")
        {
            Decoder = new StringTypeDecoder();
            Encoder = new StringTypeEncoder();
        }

        public override int FixedSize => -1;
    }
}