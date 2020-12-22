using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.DataManager.EF;
using DavinciJ15TokenBot.EthereumConnector.Etherscan;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DavinciJ15TokenBot.Purger.Console
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static async Task Main(string[] args)
        {
            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection()
                .AddLogging()
                .AddHttpClient()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IDataManager, EntityFrameworkDataManager>()
                .AddSingleton<IEthereumConnector, EtherscanEthereumConnector>();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
            dbContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("DavinciJ15Database"));

            var contextFactory = new Func<DataContext>(() => new DataContext(dbContextOptionsBuilder.Options));

            services.AddSingleton(s => contextFactory);

            serviceProvider = services.BuildServiceProvider();

            await DoWork();
        }

        private static async Task DoWork()
        {
            var dataManager = serviceProvider.GetRequiredService<IDataManager>();
            var ethereumConnector = serviceProvider.GetRequiredService<IEthereumConnector>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var holdingsTimeWindow = TimeSpan.FromHours(int.Parse(configuration["HoldingsTimeWindowHours"]));
            var contractAddress = configuration["TokenContractAddress"];
            var decimals = int.Parse(configuration["TokenDecimals"]);

            var minTokenCount = decimal.Parse(configuration["MinTokenCount"]);

            var client = new TelegramBotClient(configuration["TelegramBotToken"]);
            var chatId = configuration["ChannelChatId"];

            var membersToCheck = await dataManager.GetMembersToCheckAsync(holdingsTimeWindow);

            System.Console.WriteLine($"Members to check: {membersToCheck.Count()}");

            foreach (var m in membersToCheck)
            {
                try
                {
                    if (m.Address != null)
                    {
                        var tokenCount = await ethereumConnector.GetAccountBalanceAsync(m.Address, contractAddress, decimals);

                        if (tokenCount >= minTokenCount)
                        {
                            // update the username to enable querying
                            var info = await client.GetChatMemberAsync(chatId, m.TelegramId);
                            m.Name = info.User.Username;

                            m.Amount = tokenCount;
                            m.LastCheckedUtc = DateTime.UtcNow;

                            await dataManager.AddOrUpdateMemberAsync(m);

                            System.Console.WriteLine($"Updated: {m.Name}({m.TelegramId}): {m.Amount}");
                        }
                        else
                        {
                            m.Amount = tokenCount;
                            m.LastCheckedUtc = DateTime.UtcNow;

                            m.KickedAtUtc = DateTime.UtcNow;
                            await dataManager.AddOrUpdateMemberAsync(m);

                            await client.KickChatMemberAsync(chatId, m.TelegramId);

                            System.Console.WriteLine($"Kicked: {m.Name}({m.TelegramId})");

                            await client.SendTextMessageAsync(m.TelegramChatId, string.Format(configuration["SorryForRemovalMessage"], minTokenCount, tokenCount));
                        }
                    }
                    else // it's member without legitimation - kick (we can't send a message since we don't know the chat id)
                    {
                        m.KickedAtUtc = DateTime.UtcNow;
                        await dataManager.AddOrUpdateMemberAsync(m);

                        System.Console.WriteLine($"Kicked: {m.Name}({m.TelegramId})");

                        await client.KickChatMemberAsync(chatId, m.TelegramId);
                    }
                } 
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error while processing: {m.TelegramId}");
                    System.Console.WriteLine(ex);
                }
            }
        }
    }
}
