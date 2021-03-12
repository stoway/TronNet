using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TronNet.Crypto.Test
{
    public class ECKeyTest
    {
        [Fact]
        public void TestPublicKey()
        {
            var privateKey = "F43EBCC94E6C257EDBE559183D1A8778B2D5A08040902C0F0A77A3343A1D0EA5";

            var prvKey = privateKey.HexToByteArray();
            var ecKey = new ECKey(prvKey, true);

            var publicKey0 = ecKey.GetPubKey();
            var publicKey1 = private2PublicDemo(prvKey);

            Assert.Equal(publicKey0, publicKey1);
        }

        private byte[] private2PublicDemo(byte[] privateKey)
        {
            var privKey = new BigInteger(1, privateKey);
            var point = ECKey.CURVE.G.Multiply(privKey);
            return point.GetEncoded();
        }
    }
}
