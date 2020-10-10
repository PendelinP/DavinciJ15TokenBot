using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DavinciJ15TokenBot
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataManager.EF.DataContext>
	{
		public DataManager.EF.DataContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			var builder = new DbContextOptionsBuilder<DataManager.EF.DataContext>();
			var connectionString = configuration.GetConnectionString("DavinciJ15Database");
			builder.UseSqlServer(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(15).TotalSeconds));
			return new DataManager.EF.DataContext(builder.Options);
		}
	}

	public class PostgreSQLDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataManager.PostgreSQL.PGDataContext>
	{
		public DataManager.PostgreSQL.PGDataContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			var builder = new DbContextOptionsBuilder<DataManager.PostgreSQL.PGDataContext>();
			var connectionString = configuration.GetConnectionString("DavinciJ15Database");
			builder.UseNpgsql(connectionString);
			return new DataManager.PostgreSQL.PGDataContext(builder.Options);
		}
	}
}
