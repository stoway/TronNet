using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.ABI.FunctionEncoding.Attributes;
using TronNet.ABI.Model;

namespace TronNet.ABI
{
    public static class ABITypedRegistry
    {
        private static ConcurrentDictionary<Type, FunctionABI> _functionAbiRegistry = new ConcurrentDictionary<Type, FunctionABI>();
        private static AttributesToABIExtractor _abiExtractor = new AttributesToABIExtractor();

        public static FunctionABI GetFunctionABI<TFunctionMessage>()
        {
            return GetFunctionABI(typeof(TFunctionMessage));
        }

        public static FunctionABI GetFunctionABI(Type functionABIType)
        {
            if (!_functionAbiRegistry.ContainsKey(functionABIType))
            {
                var functionAbi = _abiExtractor.ExtractFunctionABI(functionABIType);
                _functionAbiRegistry[functionABIType] = functionAbi ?? throw new ArgumentException(functionABIType.ToString() + " is not a valid Function Type");
            }
            return _functionAbiRegistry[functionABIType];
        }
    }
}
