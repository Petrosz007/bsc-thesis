using IWA_Backend.API;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Common;
using System.Linq;
using IWA_Backend.API.Contexts.DbInitialiser;
using Microsoft.Extensions.Logging;

namespace IWA_Backend.Tests.Utilities
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IDisposable
        where TStartup : class
    {
        private readonly DbConnection SqLiteConnection;

        public TestWebApplicationFactory()
        {
            SqLiteConnection = CreateSqLiteDbConnection();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SqLiteConnection.Dispose();
            }

            base.Dispose(disposing);
        }

        private static DbConnection CreateSqLiteDbConnection()
        {
            var keepAliveConnection = new SqliteConnection("Filename=:memory:");
            keepAliveConnection.Open();
            return keepAliveConnection;
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                var descriptor = services.Single(
                d => d.ServiceType ==
                    typeof(DbContextOptions<IWAContext>));

                services.Remove(descriptor);

                services.AddDbContext<IWAContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlite(SqLiteConnection));

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var dbInitialiser = scope.ServiceProvider.GetRequiredService<DbInitialiser>();

                dbInitialiser.Initialise();
                await dbInitialiser.SeedDataAsync();
            });
            builder.ConfigureLogging((context, logging) =>
            {
                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });
        }

        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                    builder.UseStartup<TStartup>());
    }
}
