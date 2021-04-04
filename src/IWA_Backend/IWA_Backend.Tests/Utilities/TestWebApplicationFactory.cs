using IWA_Backend.API;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWA_Backend.API.Contexts.DbInitialiser;

namespace IWA_Backend.Tests.Utilities
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                var descriptor = services.Single(
                d => d.ServiceType ==
                    typeof(DbContextOptions<IWAContext>));

                services.Remove(descriptor);

                var keepAliveConnection = new SqliteConnection("Filename=:memory:");
                keepAliveConnection.Open();

                services.AddDbContext<IWAContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseSqlite(keepAliveConnection));

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var dbInitialiser = scope.ServiceProvider.GetRequiredService<DbInitialiser>();

                dbInitialiser.Initialise();
                await dbInitialiser.SeedDataAsync();
            });
        }

        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                    builder.UseStartup<TStartup>());
    }
}
