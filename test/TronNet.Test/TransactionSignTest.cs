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

            var transactionExtension = await CreateTransactionAsync(from, to, amount);

            Assert.True(transactionExtension.Result.Result);

            var transaction = transactionExtension.Transaction;

            var transactionSignExtention = await _wallet.GetTransactionSign2Async(new TransactionSign
            {
                PrivateKey = ByteString.CopyFrom(privateStr.HexToByteArray()),
                Transaction = transaction
            });
            Assert.NotNull(transactionSignExtention);

            Assert.True(transactionSignExtention.Result.Result);

            var transactionSigned = transactionSignExtention.Transaction;

            var transactionBytes = transaction.ToByteArray();

            var transaction4 = SignTransaction2Byte(transactionBytes, privateStr.HexToByteArray(), transactionSigned);


            var transaction5 = transactionSigned.ToByteArray();

            Assert.Equal(transaction4, transaction5);

            var result = await _wallet.BroadcastTransactionAsync(transactionSigned);

            Assert.True(result.Result);
        }

        private async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(fromAddress),
                ToAddress = ByteString.CopyFrom(toAddress),
                Amount = amount
            };

            var transaction = new Transaction();

            var contract = new Transaction.Types.Contract();

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return new TransactionExtention
                {
                    Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
                };
            }
            var newestBlock = await _wallet.GetNowBlock2Async(new EmptyMessage());

            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();

            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);

            var refBlockNum = bb.ToArray();

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));

            var transactionExtension = new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };
            return transactionExtension;
        }


        private byte[] SignTransaction2Byte(byte[] transaction, byte[] privateKey, Transaction transactionSigned)
        {
            var ecKey = new ECKey(privateKey, true);
            var transaction1 = Transaction.Parser.ParseFrom(transaction);
            var rawdata = transaction1.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();

            var signedValue = transactionSigned.Signature[0].ToByteArray();

            transaction1.Signature.Add(ByteString.CopyFrom(sign));

            return transaction1.ToByteArray();
        }
    }
}
