using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    public interface IGrpcChannelClient
    {
        
        Grpc.Core.Channel GetProtocol();
        Grpc.Core.Channel GetSolidityProtocol();
    }
}
