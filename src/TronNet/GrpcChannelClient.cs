using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    class GrpcChannelClient : IGrpcChannelClient
    {
        private readonly ILogger<GrpcChannelClient> _logger;
        private readonly IOptions<TronNetOptions> _options;

        public GrpcChannelClient(ILogger<GrpcChannelClient> logger, IOptions<TronNetOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        public Channel CreateChannel()
        {
            return new Channel(_options.Value.Channel.Host, _options.Value.Channel.Port, ChannelCredentials.Insecure);
        }
        //public Channel CreateSolidityChannel()
        //{
        //    var option = (_options.Value.SolidityChannel ?? _options.Value.Channel);
        //    return new Channel(option.Host, option.Port, ChannelCredentials.Insecure);
        //}
    }

}
