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
        private readonly IGrpcChannelClient _channel;

        public TronNetwork TronNetwork => _options.Value.Network;

        public TronClient(ILogger<TronClient> logger, IOptions<TronNetOptions> options, IGrpcChannelClient channel)
        {
            _logger = logger;
            _options = options;
            _channel = channel;
        }
        public Grpc.Core.Channel CreateChannel()
        {
            return _channel.CreateChannel(); 
        }
        public Wallet.WalletClient GetWalletClient()
        {
            var wallet = new Wallet.WalletClient(CreateChannel());
            return wallet;
        }
    }
}
