using DavinciJ15TokenBot.Common;
using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DavinciJ15TokenBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IEthereumMessageSigner ethereumMessageSigner;
        private readonly IEthereumConnector ethereumConnector;
        private readonly IDataManager dataManager;
        private readonly TelegramBotClient client;
        private readonly long channelChatId;

        public BotController(IConfiguration configuration, IEthereumMessageSigner ethereumMessageSigner, IEthereumConnector ethereumConnector, IDataManager dataManager)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.ethereumMessageSigner = ethereumMessageSigner ?? throw new ArgumentNullException(nameof(ethereumMessageSigner));
            this.ethereumConnector = ethereumConnector ?? throw new ArgumentNullException(nameof(ethereumConnector));
            this.dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            this.client = new TelegramBotClient(this.configuration["TelegramBotToken"]);

            this.channelChatId = long.Parse(this.configuration["ChannelChatId"]);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var members = await this.dataManager.GetAllMembersAsync();

            return this.Ok(JsonConvert.SerializeObject(members, Formatting.Indented));
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            // the user has to contact the bot via PN - see this
            https://stackoverflow.com/q/49965738/1820522

            // onboard new members
            if (update?.Message?.Type == Telegram.Bot.Types.Enums.MessageType.ChatMembersAdded && update?.Message?.NewChatMembers != null)
            {
                foreach (var m in update.Message.NewChatMembers)
                {
                    await this.dataManager.AddOrUpdateMemberAsync(new Member
                    {
                        TelegramId = m.Id,
                        Name = m.Username,
                        MemberSinceUtc = DateTime.UtcNow
                    });
                }
            }

            // members leaving
            if (update?.Message?.Type == Telegram.Bot.Types.Enums.MessageType.ChatMemberLeft && update?.Message?.LeftChatMember != null)
            {
                var leftMember = update.Message.LeftChatMember;

                await this.dataManager.DeleteMemberByTelegramIdAsync(leftMember.Id);
            }

            // react on private messages
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.Chat.Id != this.channelChatId)
            {
                if (this.SeemsToBeASignedMessage(update.Message.Text))
                {
                    try
                    {
                        var contractAddress = this.configuration["TokenContractAddress"];
                        var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);
                        
                        var result = ParseIncomingBotMessage(update.Message.Text);

                        var balance = await this.ethereumConnector.GetAccountBalanceAsync(result.Address, contractAddress, tokenDecimals);

                        await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Your token account balance is: " + Math.Round(balance, 2));
                    } 
                    catch (SignedMessageParsingError error)
                    {
                        await this.client.SendTextMessageAsync(update.Message.Chat.Id, "Something went wrong.\n\n" + error.Message + "\n\nPlease try again.");
                    }
                }
                else
                {
                    await this.client.SendTextMessageAsync(update.Message.Chat.Id, this.configuration["BotWelcomeMessage"]);
                }
            } 
            else // group join
            {
                await this.client.SendTextMessageAsync(update.Message.Chat.Id, this.configuration["ChannelWelcomeMessage"]);
            }

            return this.Ok();
        }

        private bool SeemsToBeASignedMessage(string message)
        {
            return 
                message.Length >= (BaseDefinitions.EthAddressLength * 2) && 
                message.Contains(BaseDefinitions.EthAddressAndSignatureStartIdentifier);
        }

        private BotMessageToCheck ParseIncomingBotMessage(string message)
        {
            message = message.Trim();

            var messagePartEnd = message.IndexOf("\"", 1);

            if (messagePartEnd < BaseDefinitions.EthAddressLength)
            {
                throw new SignedMessageParsingError("The given message seems to be incorrect. Please note that the message to sign has to be stated within \" symbols (e.g. \"I have DJ15 Tokens at <Your Ethereum Address>\").");
            }

            var messagePart = message.Substring(1, messagePartEnd - 1).Trim();

            if (messagePart.Length < BaseDefinitions.EthAddressLength)
            {
                throw new SignedMessageParsingError("The given message seems to be incorrect. Is your ETH-Address missing?");
            }

            var signaturePart = message.Substring(messagePartEnd + 1).Trim();

            if (signaturePart.Length < BaseDefinitions.EthAddressLength || 
                !signaturePart.StartsWith(BaseDefinitions.EthAddressAndSignatureStartIdentifier))
            {
                throw new SignedMessageParsingError("The given signature seems to be incorrect.");
            }

            var addressIdx = messagePart.IndexOf(BaseDefinitions.EthAddressAndSignatureStartIdentifier);

            if (addressIdx < 0)
            {
                throw new SignedMessageParsingError("The given address within your signed message seems to be incorrect. Please use valid ETH addresses.");
            }

            var address = messagePart.Substring(addressIdx, BaseDefinitions.EthAddressLength);

            return new BotMessageToCheck
            {
                Message = messagePart,
                Address = address,
                Signature = signaturePart
            };
        }
    }
}
