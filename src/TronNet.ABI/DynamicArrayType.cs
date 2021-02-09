using TronNet.ABI.Decoders;
using TronNet.ABI.Encoders;

namespace TronNet.ABI
{
    public class DynamicArrayType : ArrayType
    {
        public DynamicArrayType(string name) : base(name)
        {
            Decoder = new DynamicArrayTypeDecoder(ElementType);
            Encoder = new DynamicArrayTypeEncoder(ElementType);
        }

        public override string CanonicalName => ElementType.CanonicalName + "[]";

        public override int FixedSize => -1;
    }
}