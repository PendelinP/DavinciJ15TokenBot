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
        private readonly string etherscanApiKey;
        private readonly Func<HttpClient> clientFactory2;

        public EtherscanEthereumConnector(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public EtherscanEthereumConnector(string etherscanApiKey, Func<HttpClient> clientFactory2)
        {
            this.etherscanApiKey = etherscanApiKey;
            this.clientFactory2 = clientFactory2;
        }

        public async Task<decimal> GetAccountBalanceAsync(string address, string contractAddress, int decimals)
        {
            var etherscanApiKey = this.etherscanApiKey;

            var requestUrl = $"https://api.etherscan.io/api?module=account&action=tokenbalance&contractaddress={contractAddress}&address={address}&tag=latest&apikey={etherscanApiKey}";

            var client = this.clientFactory2();
            var response = await client.GetStringAsync(requestUrl);

            // wait for a little while since the free etherscan api allows only 5 requests per second
            await Task.Delay(220);

            var responseObject = JsonConvert.DeserializeObject<AccountBalanceResult>(response);

            var divisor = Math.Pow(10, decimals);

            return responseObject.Result / Convert.ToDecimal(divisor);
        }
    }
}
