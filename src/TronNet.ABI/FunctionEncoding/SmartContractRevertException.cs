using System;

namespace TronNet.ABI.FunctionEncoding
{
    public class SmartContractRevertException : Exception
    {
        private const string ERROR_PREFIX = "Smart contract error: ";
        public SmartContractRevertException(string message) : base(ERROR_PREFIX + message)
        {

        }
    }
}