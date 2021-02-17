using IWA_Backend.API;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => 
                    d.ServiceType == typeof(DbContextOptions<IWAContext>));

                services.Remove(descriptor);

                services.AddDbContext<IWAContext>(options =>
                    options.UseInMemoryDatabase("TestInMemoryDb"));

                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;

                //DbInitialiser.SeedDataAsync(serviceProvider).GetAwaiter().GetResult();
            });
        }
    }
}
