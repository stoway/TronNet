using TronNet.ABI.Decoders;
using TronNet.ABI.Encoders;

namespace TronNet.ABI
{
    public class Bytes32Type : ABIType
    {
        public Bytes32Type(string name) : base(name)
        {
            Decoder = new Bytes32TypeDecoder();
            Encoder = new Bytes32TypeEncoder();
        }
    }
}