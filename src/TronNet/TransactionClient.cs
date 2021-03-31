using Google.Protobuf;
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

        public TransactionClient(IWalletClient walletClient)
        {
            _walletClient = walletClient;
        }

        public async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount)
        {
            var wallet = _walletClient.GetWallet();

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(fromAddress),
                ToAddress = ByteString.CopyFrom(toAddress),
                Amount = amount
            };
            return await wallet.CreateTransaction2Async(transferContract);

            var transaction = new Transaction();

            var contract = new Transaction.Types.Contract();

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return null;
            }
            var newestBlock = await wallet.GetNowBlock2Async(new EmptyMessage());

            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = newestBlock.BlockHeader.RawData.ToByteArray().ToKeccakHash();
            var refBlockNum = BitConverter.GetBytes(blockHeight);

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash);
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum);

            var result = new Return
            {
                Result = true,
                Code = Return.Types.response_code.Success
            };
            var transactionExtension = new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = result,
            };
            return transactionExtension;
        }

        public string GetTransactionHash(Transaction transaction)
        {
            return transaction.RawData.ToByteArray().ToSHA256Hash().ToHex();
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
            var wallet = _walletClient.GetWallet();
            var result = await wallet.BroadcastTransactionAsync(transaction);

            return result;
        }

    }
}
