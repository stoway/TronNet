using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TronNet
{
    public static class TronNetServiceExtension
    {
        public static StowayNet.IStowayNetBuilder AddTronNet(StowayNet.IStowayNetBuilder builder, Action<TronNetOptions> setupAction)
        {
            builder.Services.AddTronNet(setupAction);

            return builder;
        }

        public static IServiceCollection AddTronNet(this IServiceCollection services, Action<TronNetOptions> setupAction)
        {
            var options = new TronNetOptions();

            setupAction(options);

            services.AddTransient<ITransactionClient, TransactionClient>();
            services.AddTransient<IGrpcChannelClient, GrpcChannelClient>();
            services.AddTransient<ITronClient, TronClient>();
            services.AddTransient<IWalletClient, WalletClient>();
            services.AddSingleton<Contracts.IContractClientFactory, Contracts.ContractClientFactory>();
            services.AddTransient<Contracts.TRC20ContractClient>();
            services.Configure(setupAction);

            return services;
        }
    }
}
