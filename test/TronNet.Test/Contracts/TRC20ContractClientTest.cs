using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TronNet.Contracts;

namespace TronNet.Test.Contracts
{
    public class TRC20ContractClientTest
    {
        private TronTestRecord _record;
        private IWalletClient _wallet;
        private IContractClientFactory _contractClientFactory;

        public TRC20ContractClientTest()
        {
            _record = TronTestServiceExtension.GetTestRecord();
            _wallet = _record.ServiceProvider.GetService<IWalletClient>();
            _contractClientFactory = _record.ServiceProvider.GetService<IContractClientFactory>();
        }
        [Fact]
        public async Task TestTransferAsync()
        {
            var privateKey = "8e812436a0e3323166e1f0e8ba79e19e217b2c4a53c970d4cca0cfb1078979df";
            var account = _wallet.GetAccount(privateKey);

            var contractAddress = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t"; //USDT Contract Address
            var to = "TGehVcNhud84JDCGrNHKVz9jEAVKUpbuiv";
            var amount = 10; //USDT Amount
            var feeAmount = 5 * 1000000L;

            var contractClient = _contractClientFactory.CreateClient(ContractProtocol.TRC20);

            var result = await contractClient.TransferAsync(contractAddress, account, to, amount, string.Empty, feeAmount);

            Assert.NotEmpty(result);
        }
    }
}
