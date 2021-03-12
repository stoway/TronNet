using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Protocol;

namespace TronNet
{
    class WalletClient : IWalletClient
    {
        private readonly IGrpcChannelClient _channelClient;

        public WalletClient(IGrpcChannelClient channelClient)
        {
            _channelClient = channelClient;
        }

        public Wallet.WalletClient GetWallet()
        {
            var channel = _channelClient.GetChannel();
            var wallet = new Wallet.WalletClient(channel);
            return wallet;
        }

        public WalletSolidity.WalletSolidityClient WalletSolidity()
        {
            var channel = _channelClient.GetSolidityChannel();
            var wallet = new WalletSolidity.WalletSolidityClient(channel);

            return wallet;
        }
    }
}
