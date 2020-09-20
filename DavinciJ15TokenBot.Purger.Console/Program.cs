using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.DataManager.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot.Purger.Console
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection()
                .AddLogging()
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
            // DO SOMETHING
        }
    }
}
