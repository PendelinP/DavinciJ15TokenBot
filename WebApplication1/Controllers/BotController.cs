using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication1.Controllers
{
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
            Console.WriteLine(update);
            return this.Ok();
        }
    }
}
