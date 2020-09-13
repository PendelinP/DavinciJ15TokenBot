using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DavinciJ15TokenBot.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nethereum.Signer;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DavinciJ15TokenBot.Controllers
{
    // ngrok http 5000 -host-header=localhost
    // 1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U
    // https://api.telegram.org/bot1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U/setWebhook?url=https://f06d16e3afe8.ngrok.io/api/bot

    // https://api.telegram.org/bot1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U/setWebhook?url=https://davincij15tokenchecker.azurewebsites.net/api/bot

    // https://api.etherscan.io/api?module=account&action=tokenbalance&contractaddress=0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9&address=0x94e9A5A128f7B4af0BEeFe32F411F61d244759cE&tag=latest&apikey=W9BYDVCXB8UJZ7B3YXCNFIQGQC7BYW376M

    // https://etherscan.io/token/0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9#balances
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IEthereumMessageSigner ethereumMessageSigner;
        private readonly IEthereumConnector ethereumConnector;
        private TelegramBotClient client;

        public BotController(IConfiguration configuration, IEthereumMessageSigner ethereumMessageSigner, IEthereumConnector ethereumConnector)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.ethereumMessageSigner = ethereumMessageSigner ?? throw new ArgumentNullException(nameof(ethereumMessageSigner));
            this.ethereumConnector = ethereumConnector ?? throw new ArgumentNullException(nameof(ethereumConnector));
            
            this.client = new TelegramBotClient(this.configuration["TelegramBotToken"]);
        }

         [HttpGet]
         public async Task<IActionResult> Get()
        {
            var msg = "The address 0x94e9A5A128f7B4af0BEeFe32F411F61d244759cE belongs to Davincij15 and Davinci Codes LTD and so does the contract for the token DJ15.";
            var signature = "0x1ad0694fceaeb5a72221e897f159644d0efdb5c5f86c9db90142f27a1d4560225e80eddcbce27991afda726390f8156aadc2971ddaf899a70098f39366f9ff8f1c";

            var address = this.ethereumMessageSigner.GetAddressFromSignedMessage(msg, signature);

            var contractAddress = this.configuration["TokenContractAddress"];
            var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);

            var balance = await this.ethereumConnector.GetAccountBalanceAsync(address, contractAddress, tokenDecimals);

            return this.Ok(address + ":" + balance);
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            var ethAddressLength = 42;

            if (update.Message.Text.Length >= ethAddressLength && update.Message.Text.Contains("0x"))
            {
                var addressIdx = update.Message.Text.IndexOf("0x");
                var address = update.Message.Text.Substring(addressIdx, ethAddressLength);

                var contractAddress = this.configuration["TokenContractAddress"];
                var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);

                var balance = await this.ethereumConnector.GetAccountBalanceAsync(address, contractAddress, tokenDecimals);

                await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Your token account balance is: " + Math.Round(balance, 2));
            }
            else
            {
                await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Welcome to the DavinciJ15 token checker bot. Please enter your Ethereum address (e.g. 0x94e9A5A128f7B4af0BEeFe32F411F61d244759cE).");
            }

            return this.Ok();
        }
    }
}
