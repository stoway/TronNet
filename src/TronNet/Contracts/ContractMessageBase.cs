using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TronNet.Contracts
{
    public abstract class ContractMessageBase
    {
        public BigInteger AmountToSend { get; set; }
        public BigInteger? Gas { get; set; }
        public BigInteger? GasPrice { get; set; }
        public string FromAddress { get; set; }
        public BigInteger? Nonce { get; set; }
    }

}
