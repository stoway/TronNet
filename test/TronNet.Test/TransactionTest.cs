using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TronNet.Test
{
    public class TransactionTest
    {
        private readonly TronTestRecord _record;

        public TransactionTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
        }

        [Fact]
        public async Task TestSignAsync()
        {
            var transactionClient = _record.ServiceProvider.GetService<ITransactionClient>();
            var privateKey = "D95611A9AF2A2A45359106222ED1AFED48853D9A44DEFF8DC7913F5CBA727366";
            var ecKey = new TronECKey(privateKey, _record.Options.Value.Network);
            var from = ecKey.GetPublicAddress();
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 100_000_000L;
            var result = await transactionClient.CreateTransactionAsync(from, to, amount);

            Assert.True(result.Result.Result);

            var transactionSigned = transactionClient.GetTransactionSign(result.Transaction, privateKey);

            var remoteTransactionSigned = await _record.TronClient.GetWalletClient().GetTransactionSign2Async(new Protocol.TransactionSign
            {
                Transaction = result.Transaction,
                PrivateKey = ByteString.CopyFrom(privateKey.HexToByteArray()),
            });

            Assert.True(remoteTransactionSigned.Result.Result);

            Assert.Equal(remoteTransactionSigned.Transaction.Signature[0], transactionSigned.Signature[0]);
        }
    }
}
