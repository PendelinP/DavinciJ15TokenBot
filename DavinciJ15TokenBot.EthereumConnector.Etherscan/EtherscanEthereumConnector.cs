using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.EthereumConnector.Etherscan.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.EthereumConnector.Etherscan
{
    public class EtherscanEthereumConnector : IEthereumConnector
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;

        public EtherscanEthereumConnector(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<decimal> GetAccountBalanceAsync(string address, string contractAddress, int decimals)
        {
            var etherscanApiKey = this.configuration["EtherscanApiKey"];

            var requestUrl = $"https://api.etherscan.io/api?module=account&action=tokenbalance&contractaddress={contractAddress}&address={address}&tag=latest&apikey={etherscanApiKey}";

            var client = this.clientFactory.CreateClient();
            var response = await client.GetStringAsync(requestUrl);

            // wait for a little while since the free etherscan api has a rate limit (5 calls/second are documented but it doesn't work - we have to wait longer)
            await Task.Delay(5000);

            var responseObject = JsonConvert.DeserializeObject<AccountBalanceResult>(response);

            var divisor = Math.Pow(10, decimals);

            return responseObject.Result / Convert.ToDecimal(divisor);
        }
    }
}
