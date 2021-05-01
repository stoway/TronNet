using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    public class TronNetOptions
    {
        public TronNetwork Network { get; set; }
        public GrpcChannelOption Channel { get; set; }

        public GrpcChannelOption SolidityChannel { get; set; }

        public string ApiKey { get; set; }

        internal Metadata GetgRPCHeaders()
        {
            return new Metadata
            {
                { "TRON-PRO-API-KEY", ApiKey }
            };
        }
    }
}
