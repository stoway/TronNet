using Google.Protobuf;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Crypto;
using TronNet.Protocol;

namespace TronNet
{
    class TransactionClient : ITransactionClient
    {
        private readonly IWalletClient _walletClient;
        private readonly IOptions<TronNetOptions> _options;

        public TransactionClient(IWalletClient walletClient, IOptions<TronNetOptions> options)
        {
            _walletClient = walletClient;
            _options = options;
        }

        public async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {
            var wallet = _walletClient.GetProtocol();

            var fromAddress = _walletClient.ParseAddress(from);
            var toAddress = _walletClient.ParseAddress(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = fromAddress,
                ToAddress = toAddress,
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
            var newestBlock = await wallet.GetNowBlock2Async(new EmptyMessage(), headers: _options.Value.GetgRPCHeaders());

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

        public Transaction GetTransactionSign(Transaction transaction, string privateKey)
        {
            var ecKey = new ECKey(privateKey.HexToByteArray(), true);
            var transactionSigned = Transaction.Parser.ParseFrom(transaction.ToByteArray());
            var rawdata = transactionSigned.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();

            transactionSigned.Signature.Add(ByteString.CopyFrom(sign));

            return transactionSigned;
        }

        public async Task<Return> BroadcastTransactionAsync(Transaction transaction)
        {
            var wallet = _walletClient.GetProtocol();
            var result = await wallet.BroadcastTransactionAsync(transaction, headers: _options.Value.GetgRPCHeaders());

            return result;
        }

    }
}
