using DavinciJ15TokenBot.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEthereumConnector connector;
        private readonly IConfiguration configuration;
        private readonly IDataManager dataManager;

        public TestController(IEthereumConnector connector, IConfiguration configuration, IDataManager dataManager)
        {
            this.connector = connector;
            this.configuration = configuration;
            this.dataManager = dataManager;
        }

        [HttpGet("connector")]
        public async Task<IActionResult> TestConnector([FromQuery] string address)
        {
            var contractAddress = this.configuration["TokenContractAddress"];
            var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);

            return Ok(await this.connector.GetAccountBalanceAsync(address, contractAddress, tokenDecimals));
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDb([FromQuery] string name)
        {
            await this.dataManager.AddOrUpdateMemberAsync(new Common.Models.Member
            {
                Id = default,
                Amount = int.MaxValue,
                LastCheckedUtc = DateTime.UtcNow,
                MemberSinceUtc = DateTime.UtcNow.AddDays(7),
                Name = name,
                TelegramId = new Random().Next(1, int.MaxValue),
                Address = Guid.NewGuid().ToString(),
            });

            var members = await this.dataManager.GetAllMembersAsync();
            return Ok(members);
        }

    }
}
