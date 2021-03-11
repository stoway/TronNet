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
        public void TestAddress()
        {
            var privateKey = "F43EBCC94E6C257EDBE559183D1A8778B2D5A08040902C0F0A77A3343A1D0EA5";
            var address = "";

            var ecKey = new ECKey(privateKey.HexToByteArray(), true);

        }
    }
}
