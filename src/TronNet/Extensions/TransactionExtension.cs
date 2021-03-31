using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Crypto;

namespace TronNet
{
    public static class TransactionExtension
    {
        public static string GetTxid(this Protocol.Transaction transaction)
        {
            var txid = transaction.RawData.ToByteArray().ToSHA256Hash().ToHex();

            return txid;
        }

    }
}
