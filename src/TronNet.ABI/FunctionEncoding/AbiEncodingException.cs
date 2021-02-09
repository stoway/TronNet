using System;

namespace TronNet.ABI.FunctionEncoding
{
    public class AbiEncodingException:Exception
    {
        public AbiEncodingException(int order, ABIType abiType, object value, string message, Exception innerException):base(message, innerException)
        {
            Order = order;
            ABIType = abiType;
            Value = value;
        }

        public int Order { get;}
        public ABIType ABIType { get; }
        public object Value { get; }
    }
 }

   