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
        private readonly TimeSpan holdingsTimeWindow;

        public BotController(IConfiguration configuration, IEthereumMessageSigner ethereumMessageSigner, IEthereumConnector ethereumConnector, IDataManager dataManager)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.ethereumMessageSigner = ethereumMessageSigner ?? throw new ArgumentNullException(nameof(ethereumMessageSigner));
            this.ethereumConnector = ethereumConnector ?? throw new ArgumentNullException(nameof(ethereumConnector));
            this.dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            this.client = new TelegramBotClient(this.configuration["TelegramBotToken"]);

            this.channelChatId = long.Parse(this.configuration["ChannelChatId"]);

            this.holdingsTimeWindow = TimeSpan.FromHours(int.Parse(configuration["HoldingsTimeWindowHours"]));
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
            // don't react of anything else than messages
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                return this.Ok();
            }

            var message = update.Message;

            // the user has to contact the bot via PN - see this
            https://stackoverflow.com/q/49965738/1820522

            try
            {
                var newlyCreatedMembers = false;

                // members joining the group
                if (message.NewChatMembers != null)
                {
                    foreach (var m in message.NewChatMembers)
                    {
                        var member = await this.dataManager.GetMemberByTelegramIdAsync(m.Id);

                        // if the member is new, create in DB
                        if (member == null)
                        {
                            member = new Member
                            {
                                TelegramId = m.Id,
                            };

                            newlyCreatedMembers = true;
                        }
                        // returning member (within the holdings time window it's okay- otherwise check if member has tokens and kick instantly if not)
                        else if (member.MemberSinceUtc < DateTime.UtcNow.Subtract(this.holdingsTimeWindow))
                        {
                            var chatId = configuration["ChannelChatId"];

                            // we know the member and his address - check balance
                            if (member.Address != null)
                            {
                                var contractAddress = this.configuration["TokenContractAddress"];
                                var tokenDecimals = int.Parse(this.configuration["TokenDecimals"]);

                                var tokenCount = await this.ethereumConnector.GetAccountBalanceAsync(member.Address, contractAddress, tokenDecimals);

                                if (tokenCount > 0)
                                {
                                    member.Amount = tokenCount;
                                    member.LastCheckedUtc = DateTime.UtcNow;
                                }
                                else
                                {
                                    await this.client.KickChatMemberAsync(chatId, member.TelegramId);
                                    await this.TrySendMessageAsync(member.TelegramChatId.Value, configuration["SorryForRemovalMessage"]);
                                }
                            }
                            else // it's just a returning member without legitimation - kick (we can't send a message since we don't know the chat id)
                            {
                                await client.KickChatMemberAsync(chatId, member.TelegramId);
                            }
                        }

                        // update telegram username in all cases
                        member.Name = m.Username;
                        member.MemberSinceUtc = DateTime.UtcNow;

                        await this.dataManager.AddOrUpdateMemberAsync(member);
                    }

                    // only send greetings to newly created members - returners already have seen this message and spammes don't need to see it ;)
                    if (newlyCreatedMembers)
                    {
                        await this.TrySendMessageAsync(message.Chat.Id, this.configuration["ChannelWelcomeMessage"]);
                    }

                    return this.Ok();
                }

                // members leaving (not needed)
                //if (message.Type == Telegram.Bot.Types.Enums.MessageType.ChatMemberLeft && message.LeftChatMember != null)
                //{
                //    var leftMember = message.LeftChatMember;

                //    await this.dataManager.DeleteMemberByTelegramIdAsync(leftMember.Id);

                //    return this.Ok();
                //}

                // react on private messages (registration process)
                if (message.Chat.Id != this.channelChatId)
                {
                    // no signed message - show personal welcome message with registration guide
                    if (message.Text == null || !this.SeemsToBeASignedMessage(message.Text))
                    {
                        await this.TrySendMessageAsync(message.Chat.Id, this.configuration["BotWelcomeMessage"]);
                    }
                    else // message contains address and signature information - try to complete registration
                    {
                        var result = ParseIncomingBotMessage(message.Text);

                        var addressUsedForSigning = string.Empty;

                        try
                        {
                            addressUsedForSigning = this.ethereumMessageSigner.GetAddressFromSignedMessage(result.Message, result.Signature);
                        }
                        catch (Exception)
                        {
                            await this.TrySendMessageAsync(message.Chat.Id, $"The given signature seems to be invalid. Please try again.");
                            return this.Ok();
                        }

                        if (result.Address != addressUsedForSigning)
                        {
                            await this.TrySendMessageAsync(message.Chat.Id, $"The given address does not match with your signature address (you signed with {addressUsedForSigning}). Please try again.");
                            return this.Ok();
                        }

                        var memberWithAddress = await this.dataManager.GetMemberByAddressAsync(result.Address);

                        // check if address is already registered
                        if (memberWithAddress != null && memberWithAddress.TelegramId != message.From.Id)
                        {
                            await this.TrySendMessageAsync(message.Chat.Id, $"Nice try but this address is already registered ;)");
                            return this.Ok();
                        }

                        var member = await this.dataManager.GetMemberByTelegramIdAsync(message.From.Id);

                        // if the member is new, create - otherwise update props
                        if (member == null)
                        {
                            member = new Member
                            {
                                TelegramId = message.From.Id
                            };
                        }

                        member.Name = message.From.Username;
                        member.RegistrationValidSinceUtc = DateTime.UtcNow;
                        member.TelegramChatId = message.Chat.Id;
                        member.Address = result.Address;

                        await this.dataManager.AddOrUpdateMemberAsync(member);

                        // var balance = await this.ethereumConnector.GetAccountBalanceAsync(result.Address, contractAddress, tokenDecimals);

                        await this.TrySendMessageAsync(message.Chat.Id, this.configuration["RegistrationSuccessfulMessage"]);

                    }
                }
            }
            catch (SignedMessageParsingError error)
            {
                await this.TrySendMessageAsync(message.Chat.Id, $"Something went wrong.\n\n{error.Message}\n\nPlease try again.");
            }
            catch (Exception)
            {
                await this.TrySendMessageAsync(message.Chat.Id, $"Something went wrong. Please try again.");
            }

            return this.Ok();
        }

        private async Task TrySendMessageAsync(long chatId, string message)
        {
            try
            {
                await this.client.SendTextMessageAsync(chatId, message);
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Unable to deliver message:" + ex.Message);
            }
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
                throw new SignedMessageParsingError($"The given message seems to be incorrect. Please note that the message to sign has to be stated within \" symbols (e.g. {this.configuration["SampleMessageValidationError"]}).");
            }

            var messagePart = message.Substring(1, messagePartEnd - 1).Trim();

            if (messagePart.Length < BaseDefinitions.EthAddressLength)
            {
                throw new SignedMessageParsingError("The given message seems to be incorrect. Is your Ethereum Address missing?");
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
                throw new SignedMessageParsingError("The given address within your signed message seems to be incorrect. Please use a valid Ethereum address.");
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
