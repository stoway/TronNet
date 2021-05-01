using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Protocol;

namespace TronNet
{
    public interface ITransactionClient
    {
        Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount);

        Transaction GetTransactionSign(Transaction transaction, string privateKey);

        Task<Return> BroadcastTransactionAsync(Transaction transaction);
    }
}
