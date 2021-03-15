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

    services.AddStowayNet().AddTronNet(x =>
    {
        x.Network = TronNetwork.MainNet;
        x.Channel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50051 };
        x.SolidityChannel = new GrpcChannelOption { Host = "grpc.shasta.trongrid.io", Port = 50052 };
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

namespace TronNetTest
{
    class Class1
    {
        private readonly ITransactionClient _transactionClient;
        public Class1(ITransactionClient transactionClient)
        {
            _tronClient = tronClient;
            _transactionClient = transactionClient;
        }

        public async Task SignAsync()
        {
            var privateKey = "D95611A9AF2A2A45359106222ED1AFED48853D9A44DEFF8DC7913F5CBA727366";
            var ecKey = new TronECKey(privateKey, TronNetwork.MainNet);
            var from = ecKey.GetPublicAddress();
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 100_000_000L;
            var result = await transactionClient.CreateTransactionAsync(from, to, amount);

            var transactionSigned = transactionClient.GetTransactionSign(result.Transaction, privateKey);

        }
    }
}

```
also see: https://github.com/stoway/TronNet/blob/main/test/TronNet.Test/TransactionSignTest.cs

