using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.ABI.FunctionEncoding.Attributes;

namespace TronNet.Contracts
{
    [Function("decimals", "uint8")]
    public class DecimalsFunction : FunctionMessage
    {
    }
}
