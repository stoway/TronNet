# TronNet
TronNet is a SDK that includes libraries for working with TRON, TronNet makes it easy to build TRON applications with .net.

## Get Started
### NuGet 

You can run the following command to install the `TronNet` in your project.

```
PM> Install-Package TronNet
```

### Configuration

First,You need to config `TronNet` in your `Startup.cs`:
```c#
......
using StowayNet;
using TronNet;
......

public void ConfigureServices(IServiceCollection services)
{
    ......

    services.AddTronNet(x =>
    {
        x.Network = TronNetwork.MainNet;
        x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
        x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
        x.ApiKey = "input your api key";
    });

    ......
}

```

### Sample

#### Sample 1: Generate Address Offline

```c#
using TronNet;

namespace TronNetTest
{
    class Class1
    {
        private readonly ITronClient _tronClient;

        public Class1(ITronClient tronClient)
        {
            _tronClient = tronClient;
        }

        public void GenerateAddress()
        {
            var key = _tronClient.GenerateKey();

            var address = key.GetPublicAddress();
        }
    }
}


```

#### Sample 2: Transaction Sign Offline
```c#
using TronNet;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace TronNetTest
{
    class Class1
    {
        private readonly ITransactionClient _transactionClient;
        private readonly IOptions<TronNetOptions> _options;
        public Class1(ITransactionClient transactionClient, IOptions<TronNetOptions> options)
        {
            _options = options;
            _transactionClient = transactionClient;
        }

        public async Task SignAsync()
        {
            var privateKey = "D95611A9AF2A2A45359106222ED1AFED48853D9A44DEFF8DC7913F5CBA727366";
            var ecKey = new TronECKey(privateKey, _options.Value.Network);
            var from = ecKey.GetPublicAddress();
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 100_000_000L;
            var transactionExtension = await _transactionClient.CreateTransactionAsync(from, to, amount);

            var transactionSigned = _transactionClient.GetTransactionSign(transactionExtension.Transaction, privateKey);
            
            var result = await _transactionClient.BroadcastTransactionAsync(transactionSigned);
        }
    }
}

```
also see: https://github.com/stoway/TronNet/blob/main/test/TronNet.Test/TransactionSignTest.cs

