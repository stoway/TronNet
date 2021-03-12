using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    public interface IGrpcChannelClient : StowayNet.IStowayDependency
    {
        Grpc.Core.Channel GetChannel();
        Grpc.Core.Channel GetSolidityChannel();
    }
}
