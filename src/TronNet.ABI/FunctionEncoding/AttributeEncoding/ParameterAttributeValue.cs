using System.Reflection;
using TronNet.ABI.FunctionEncoding.Attributes;

namespace TronNet.ABI.FunctionEncoding.AttributeEncoding
{
    public class ParameterAttributeValue
    {
        public ParameterAttribute ParameterAttribute { get; set; }
        public object Value { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}