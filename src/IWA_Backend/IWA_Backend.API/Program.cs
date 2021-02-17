using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IWA_Backend.API
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                //var context = serviceProvider.GetRequiredService<IWAContext>();

                if (!DbInitialiser.AnyCategories(serviceProvider))
                {
                    await DbInitialiser.SeedDataAsync(serviceProvider);
                }

                //var appointments = context.Appointments.ToList();
                //var categories = context.Categories.Include(c => c.Owner).ToList();
                //var users = context.Users.ToList();
                //var usernames = categories.Select(c => c.Owner.UserName).ToList();
                //var user = context.Entry(categories[0]).Reference(c => c.Owner);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
