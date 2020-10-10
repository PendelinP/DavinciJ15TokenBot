using DavinciJ15TokenBot.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEthereumConnector connector;
        private readonly IConfiguration configuration;

        public TestController(IEthereumConnector connector, IConfiguration configuration)
        {
            this.connector = connector;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Test([FromQuery] string address)
        {
            var contractAddress = this.configuration["TokenContractAddress"];
            var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);

            return Ok(await this.connector.GetAccountBalanceAsync(address, contractAddress, tokenDecimals));
        }
    }
}
