using TronNet.ABI.Decoders;
using TronNet.ABI.Encoders;

namespace TronNet.ABI
{
    public class BoolType : ABIType
    {
        public BoolType() : base("bool")
        {
            Decoder = new BoolTypeDecoder();
            Encoder = new BoolTypeEncoder();
        }
    }
}