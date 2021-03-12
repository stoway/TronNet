using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Protocol;

namespace TronNet
{
    class TronClient : ITronClient
    {
        private readonly ILogger<TronClient> _logger;
        private readonly IOptions<TronNetOptions> _options;
        private readonly IGrpcChannelClient _channelClient;
        private readonly IWalletClient _walletClient;

        public TronNetwork TronNetwork => _options.Value.Network;

        public TronClient(ILogger<TronClient> logger, IOptions<TronNetOptions> options, IGrpcChannelClient channelClient, IWalletClient walletClient)
        {
            _logger = logger;
            _options = options;
            _channelClient = channelClient;
            _walletClient = walletClient;
        }
        public Grpc.Core.Channel CreateChannel()
        {
            return _channelClient.GetChannel(); 
        }
        public Wallet.WalletClient GetWalletClient()
        {
            return _walletClient.GetWallet();
        }

        public TronECKey GenerateKey()
        {
            var tronKey = TronECKey.GenerateKey(_options.Value.Network);

            return tronKey;
        }
    }
}
