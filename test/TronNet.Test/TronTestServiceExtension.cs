using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StowayNet;
using Microsoft.Extensions.Options;

namespace TronNet.Test
{
    public record TronTestRecord(IServiceProvider ServiceProvider, ITronClient TronClient, IOptions<TronNetOptions> Options);
    public static class TronTestServiceExtension
    {
        public static IServiceProvider AddTronNet()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddStowayNet().AddTronNet(x =>
            {
                x.Network = TronNetwork.MainNet;
                x.Channel = new GrpcChannelOption { Host = "grpc.trongrid.io", Port = 50051 };
                //x.SolidityChannel = new GrpcChannelOption { Host = "grpc.trongrid.io", Port = 50052 };
            });
            services.AddLogging();
            return services.BuildServiceProvider();
        }

        public static TronTestRecord GetTestRecord()
        {
            var provider = TronTestServiceExtension.AddTronNet();
            var client = provider.GetService<ITronClient>();
            var options = provider.GetService<IOptions<TronNetOptions>>();

            return new TronTestRecord(provider, client, options);
        }
    }

}
