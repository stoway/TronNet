using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Contracts
{
    public interface IContractClientFactory
    {
        IContractClient CreateClient(ContractProtocol protocol);
    }
}
