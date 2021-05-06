using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Accounts
{
    public class TronAccount : ITronAccount
    {
        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public TronAccount(string privateKey, TronNetwork network)
        {
            Initialise(new TronECKey(privateKey, network));
        }

        public void Initialise(TronECKey key)
        {
            PrivateKey = key.GetPrivateKey();
            Address = key.GetPublicAddress();
            PublicKey = key.GetPubKey().ToHex();
        }
    }
}
