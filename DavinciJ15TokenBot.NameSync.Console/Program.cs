using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.DataManager.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DavinciJ15TokenBot.NameSync.Console
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
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IDataManager, EntityFrameworkDataManager>();

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
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var client = new TelegramBotClient(configuration["TelegramBotToken"]);
            var chatId = configuration["ChannelChatId"];


            var info = await client.GetChatMemberAsync(chatId, 1401820796);
            var username = info.User.Username;
            System.Console.WriteLine(username);
        }
    }
}
