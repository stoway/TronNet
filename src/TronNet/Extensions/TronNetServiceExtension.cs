using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StowayNet;

namespace TronNet
{
    public static class TronNetServiceExtension
    {
        public static IStowayNetBuilder AddTronNet(this IStowayNetBuilder builder, Action<TronNetOptions> setupAction)
        {
            var options = new TronNetOptions();

            setupAction(options);
            builder.Services.Configure(setupAction);

            return builder;
        }

    }
}
