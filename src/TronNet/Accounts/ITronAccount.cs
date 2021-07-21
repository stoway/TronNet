using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Accounts
{
    public interface ITronAccount
    {
        public string PublicKey { get; }
        public string PrivateKey { get; }

        public string Address { get; }

        byte GetAddressPrefix();
    }
}
