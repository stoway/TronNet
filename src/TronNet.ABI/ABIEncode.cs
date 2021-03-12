using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TronNet.ABI.FunctionEncoding;
using TronNet.ABI.Model;
using TronNet.ABI.Util;
using TronNet.Crypto;

namespace TronNet.ABI
{
    public class ABIEncode
    {

        public byte[] GetSha3ABIEncodedPacked(params ABIValue[] abiValues)
        {
            return GetABIEncodedPacked(abiValues).ToKeccakHash();
        }

        public byte[] GetSha3ABIEncodedPacked(params object[] values)
        {
            return GetABIEncodedPacked(values).ToKeccakHash();
        }

        public byte[] GetSha3ABIEncoded(params ABIValue[] abiValues)
        {
            return GetABIEncoded(abiValues).ToKeccakHash();
        }

        public byte[] GetSha3ABIEncoded(params object[] values)
        {
            return GetABIEncoded(values).ToKeccakHash();
        }

        public byte[] GetSha3ABIParamsEncodedPacked<T>(T input)
        {
            return GetABIParamsEncodedPacked<T>(input).ToKeccakHash();
        }

        public byte[] GetSha3ABIParamsEncoded<T>(T input)
        {
            return GetABIParamsEncoded<T>(input).ToKeccakHash();
        }

        public byte[] GetABIEncodedPacked(params ABIValue[] abiValues)
        {
            var result = new List<byte>();
            foreach (var abiValue in abiValues)
            {
                result.AddRange(abiValue.ABIType.EncodePacked(abiValue.Value));
            }
            return result.ToArray();
        }


        public byte[] GetABIEncoded(params ABIValue[] abiValues)
        {
            var parameters = new List<Parameter>();
            var values = new List<object>();
            var order = 1;
            foreach (var abiValue in abiValues)
            {
                parameters.Add(new Parameter(abiValue.ABIType.Name, order));
                values.Add(abiValue.Value);
                order = order + 1;
            }

            return new ParametersEncoder().EncodeParameters(parameters.ToArray(), values.ToArray());
        }

        public byte[] GetABIEncoded(params object[] values)
        {
            return GetABIEncoded(ConvertValuesToDefaultABIValues(values).ToArray());
        }

        public byte[] GetABIParamsEncodedPacked<T>(T input)
        {
            var type = typeof(T);
            var parametersEncoder = new ParametersEncoder();
            var parameterObjects = parametersEncoder.GetParameterAttributeValues(type, input).OrderBy(x => x.ParameterAttribute.Order);

            var result = new List<byte>();

            foreach (var abiParameter in parameterObjects)
            {
                var abiType = abiParameter.ParameterAttribute.Parameter.ABIType;
                var value = abiParameter.Value;

                result.AddRange(abiType.EncodePacked(value));
                
            }
            return result.ToArray();
        }

        public byte[] GetABIParamsEncoded<T>(T input)
        {
            var type = typeof(T);
            return new ParametersEncoder().EncodeParametersFromTypeAttributes(type, input);
        }

        private List<ABIValue> ConvertValuesToDefaultABIValues(params object[] values)
        {
            var abiValues = new List<ABIValue>();
            foreach (var value in values)
            {
                if (value.IsNumber())
                {
                    var bigInt = BigInteger.Parse(value.ToString());
                    if (bigInt >= 0)
                    {
                        abiValues.Add(new ABIValue(new IntType("uint256"), value));
                    }
                    else
                    {
                        abiValues.Add(new ABIValue(new IntType("int256"), value));
                    }
                }

                if (value is string)
                {
                    abiValues.Add(new ABIValue(new StringType(), value));
                }

                if (value is bool)
                {
                    abiValues.Add(new ABIValue(new BoolType(), value));
                }

                if (value is byte[])
                {
                    abiValues.Add(new ABIValue(new BytesType(), value));
                }
            }

            return abiValues;
        }

        public byte[] GetABIEncodedPacked(params object[] values)
        {
            var abiValues = ConvertValuesToDefaultABIValues(values);
            return GetABIEncodedPacked(abiValues.ToArray());
        }

    }
}