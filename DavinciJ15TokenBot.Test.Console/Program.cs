using DavinciJ15TokenBot.EthereumConnector.Etherscan;
using DavinciJ15TokenBot.EthereumConnector.EthNode;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Test.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 4)
            {
                System.Console.WriteLine("missing args. use with tokenHodlerAddress nodeIp username password");
                return;
            }

            var tokenAddress = args[0];
            var nodeIp = args[1];
            var username = args[2];
            var pw = args[3];

            var connector = new EthNodeEthereumConnector(nodeIp, new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    ASCIIEncoding.ASCII.GetBytes(
                       $"{username}:{pw}"))));
            string tokenContractAddress = "0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9";
            var x = await connector.GetAccountBalanceAsync(tokenAddress, tokenContractAddress, 2);
            System.Console.ReadLine();
        }
    }
}
