﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DavinciJ15TokenBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DavinciJ15TokenBot.Controllers
{
    // ngrok http 5000 -host-header=localhost
    // 1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U
    // https://api.telegram.org/bot1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U/setWebhook?url=https://f06d16e3afe8.ngrok.io/api/bot

    // https://api.etherscan.io/api?module=account&action=tokenbalance&contractaddress=0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9&address=0x94e9A5A128f7B4af0BEeFe32F411F61d244759cE&tag=latest&apikey=W9BYDVCXB8UJZ7B3YXCNFIQGQC7BYW376M

    // https://etherscan.io/token/0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9#balances
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private TelegramBotClient client;

        public BotController()
        {
            this.client = new TelegramBotClient("1343908176:AAEirQ_PVQscV8rCHDBHLSen2YzY04DAD_U");
        }

         [HttpGet]
         public async Task<IActionResult> Get()
        {
            return this.Ok();
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            var ethAddressLength = 42;

            if(update.Message.Text.Length >= ethAddressLength && update.Message.Text.Contains("0x"))
            {
                // check balance
                var etherscanApiKey = "W9BYDVCXB8UJZ7B3YXCNFIQGQC7BYW376M";

                var addressIdx = update.Message.Text.IndexOf("0x");
                var address = update.Message.Text.Substring(addressIdx, ethAddressLength);


                var requestUrl = $"https://api.etherscan.io/api?module=account&action=tokenbalance&contractaddress=0x5d269fac3B2e0552b0F34cdc253bDB427682A4b9&address={address}&tag=latest&apikey={etherscanApiKey}";

                var client = new HttpClient();
                var response = await client.GetStringAsync(requestUrl);

                var responseObject = JsonConvert.DeserializeObject<AccountBalanceResult>(response);

                await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Your token account balance is: " + Math.Round(responseObject.Result / 1000000000m, 2));
            }
            else
            {
                await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Welcome to the DavinciJ15 token checker bot. Please enter your Ethereum address (e.g. 0x94e9A5A128f7B4af0BEeFe32F411F61d244759cE).");
            }

            return this.Ok();
        }
    }
}