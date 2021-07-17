using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet.Accounts
{
    public class TronAccount : ITronAccount
    {
        public string Address { get; protected set; }
        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }

        private TronECKey _key = null;
        public TronAccount(string privateKey, TronNetwork network)
        {
            Initialise(new TronECKey(privateKey, network));
        }

        public TronAccount(TronECKey key)
        {
            Initialise(key);
        }

        public void Initialise(TronECKey key)
        {
            _key = key;
            PrivateKey = key.GetPrivateKey();
            Address = key.GetPublicAddress();
            PublicKey = key.GetPubKey().ToHex();
        }

        public byte GetAddressPrefix()
        {
            return _key.GetPublicAddressPrefix();
        }
    }
}
