using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TronNet.Protocol;
using Google.Protobuf;
using TronNet.Crypto;

namespace TronNet.Test
{
    public class TransactionSignTest
    {
        private readonly TronTestRecord _record;
        private readonly Wallet.WalletClient _wallet;
        public TransactionSignTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.TronClient.GetWalletClient();
        }


        [Fact]
        public async Task TestTransactionSignAsync()
        {
            var privateStr = "8e812436a0e3323166e1f0e8ba79e19e217b2c4a53c970d4cca0cfb1078979df";
            var tronKey = new TronECKey(privateStr, _record.Options.Value.Network);
            var from = tronKey.GetPublicAddress();
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 100_000_000L; // 100 TRX, api only receive trx in Sun, and 1 trx = 1000000 Sun

            var transaction = await CreateTransactionAsync(from, to, amount);

            var transactionExtention = await _wallet.GetTransactionSign2Async(new TransactionSign
            {
                PrivateKey = ByteString.CopyFrom(privateStr.HexToByteArray()),
                Transaction = transaction
            });
            Assert.NotNull(transactionExtention);

            Assert.True(transactionExtention.Result.Result);

            var transactionSigned = transactionExtention.Transaction;

            var transactionBytes = transaction.ToByteArray();

            var transaction4 = SignTransaction2Byte(transactionBytes, privateStr.HexToByteArray(), transactionSigned);


            var transaction5 = transactionSigned.ToByteArray();

            Assert.Equal(transaction4.Signature[0], transactionSigned.Signature[0]);
        }

        private async Task<Transaction> CreateTransactionAsync(string from, string to, long amount)
        {
            var newestBlock = await _wallet.GetNowBlock2Async(new EmptyMessage());

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var transaction = new Transaction();
            var contract = new Transaction.Types.Contract();
            var transferContract = new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(fromAddress),
                ToAddress = ByteString.CopyFrom(toAddress),
                Amount = amount
            };

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return null;
            }
            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            SetReference(transaction, newestBlock);
            return transaction;
        }

        private void SetReference(Transaction transaction, BlockExtention newestBlock)
        {
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = newestBlock.BlockHeader.RawData.ToByteArray().ToSha3Hash();
            var refBlockNum = BitConverter.GetBytes(blockHeight);

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 16));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 8));
        }

        private Transaction SignTransaction2Byte(byte[] transaction, byte[] privateKey, Transaction transactionSigned)
        {
            var ecKey = new ECKey(privateKey, true);
            var transaction1 = Transaction.Parser.ParseFrom(transaction);
            var rawdata = transaction1.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();

            var signedValue = transactionSigned.Signature[0].ToByteArray();

            transaction1.Signature.Add(ByteString.CopyFrom(sign));

            return transaction1;
        }
    }
}
