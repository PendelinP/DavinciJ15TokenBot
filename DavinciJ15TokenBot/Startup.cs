using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DavinciJ15TokenBot.Common.Configuration;
using DavinciJ15TokenBot.Common.Interfaces;
using DavinciJ15TokenBot.DataManager.EF;
using DavinciJ15TokenBot.EthereumConnector.Etherscan;
using DavinciJ15TokenBot.EthereumConnector.EthNode;
using DavinciJ15TokenBot.MessageSigner.Nethereum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavinciJ15TokenBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson();

            services.AddHttpClient();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
            dbContextOptionsBuilder.UseSqlServer(this.Configuration.GetConnectionString("DavinciJ15Database"));

            var contextFactory = new Func<DataContext>(() => new DataContext(dbContextOptionsBuilder.Options));

            services.AddSingleton(s => contextFactory);

            services.AddScoped<IEthereumMessageSigner, NethereumMessageSigner>();
            services.AddScoped<IDataManager, EntityFrameworkDataManager>();

            var connectorConfiguration = this.Configuration.GetSection("ConnectorConfig");
            services.Configure<ConnectorConfiguration>(connectorConfiguration);
            if (connectorConfiguration.GetValue<ConnectorMode>("Mode") == ConnectorMode.Node)
            {
                services.AddScoped<IEthereumConnector, EthNodeEthereumConnector>();
            }
            else
            {
                services.AddScoped<IEthereumConnector, EtherscanEthereumConnector>();
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
