using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Protocol;

namespace TronNet
{
    public interface IWalletClient : StowayNet.IStowayDependency
    {
        Wallet.WalletClient GetWallet();
    }
}
