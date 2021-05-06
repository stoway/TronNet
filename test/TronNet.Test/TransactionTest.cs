using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using TronNet.Crypto;
using TronNet.Protocol;
using Xunit;

namespace TronNet.Test
{
    public class TransactionTest
    {
        private readonly TronTestRecord _record;
        private readonly Wallet.WalletClient _wallet;

        public TransactionTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.TronClient.GetWallet();
        }



        [Fact]
        public async Task TestTransferAsync()
        {
            var transactionClient = _record.ServiceProvider.GetService<ITransactionClient>();
            var walletClient = _record.TronClient.GetWallet();
            var privateKey = "8e812436a0e3323166e1f0e8ba79e19e217b2c4a53c970d4cca0cfb1078979df";
            var tronKey = new TronECKey(privateKey, _record.Options.Value.Network);
            var from = tronKey.GetPublicAddress();
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 1_000_000L; // 1 TRX, api only receive trx in Sun, and 1 trx = 1000000 Sun

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var block = await walletClient.GetNowBlock2Async(new EmptyMessage());

            var transaction = await transactionClient.CreateTransactionAsync(from, to, amount);

            Assert.True(transaction.Result.Result);

            var transactionSigned = transactionClient.GetTransactionSign(transaction.Transaction, privateKey);

            var result = await transactionClient.BroadcastTransactionAsync(transactionSigned);

            Assert.True(result.Result);
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

            var remoteTransactionSigned = await _record.TronClient.GetWallet().GetTransactionSign2Async(new Protocol.TransactionSign
            {
                Transaction = result.Transaction,
                PrivateKey = ByteString.CopyFrom(privateKey.HexToByteArray()),
            });

            Assert.True(remoteTransactionSigned.Result.Result);

            Assert.Equal(remoteTransactionSigned.Transaction.Signature[0], transactionSigned.Signature[0]);
        }
    }
}
