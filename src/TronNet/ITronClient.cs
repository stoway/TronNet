using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    public interface ITronClient
    {
        TronNetwork TronNetwork { get; }
        Grpc.Core.Channel CreateChannel();
        Protocol.Wallet.WalletClient GetWalletClient();
        TronECKey GenerateKey();
    }
}
