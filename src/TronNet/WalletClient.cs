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
    class WalletClient : IWalletClient
    {
        private readonly IGrpcChannelClient _channelClient;

        public WalletClient(IGrpcChannelClient channelClient)
        {
            _channelClient = channelClient;
        }

        public Wallet.WalletClient GetWallet()
        {
            var channel = _channelClient.GetChannel();
            var wallet = new Wallet.WalletClient(channel);
            return wallet;
        }

        public WalletSolidity.WalletSolidityClient WalletSolidity()
        {
            var channel = _channelClient.GetSolidityChannel();
            var wallet = new WalletSolidity.WalletSolidityClient(channel);

            return wallet;
        }

        public ByteString ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentNullException(nameof(address));

            byte[] raw;
            if (address.StartsWith("T"))
            {
                raw = Base58Encoder.DecodeFromBase58Check(address);
            }
            else if (address.StartsWith("41"))
            {
                raw = address.HexToByteArray();
            }
            else if (address.StartsWith("0x"))
            {
                raw = address[2..].HexToByteArray();
            }
            else
            {
                try
                {
                    raw = address.HexToByteArray();
                }
                catch (Exception)
                {
                    throw new ArgumentException($"Invalid address: " + address);
                }
            }
            return ByteString.CopyFrom(raw);
        }
    }
}
