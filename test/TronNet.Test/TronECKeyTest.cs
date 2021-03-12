using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TronNet.Test
{
    public class TronECKeyTest
    {
        [Fact]
        public void TestPublicAddress()
        {
            var privateKey = "F43EBCC94E6C257EDBE559183D1A8778B2D5A08040902C0F0A77A3343A1D0EA5";

            var mainKey = new TronECKey(privateKey, TronNetwork.MainNet);

            Assert.Equal("TWVRXXN5tsggjUCDmqbJ4KxPdJKQiynaG6", mainKey.GetPublicAddress());
        }

    }
}
