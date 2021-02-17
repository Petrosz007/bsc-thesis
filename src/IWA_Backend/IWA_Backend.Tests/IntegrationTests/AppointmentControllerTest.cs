using IWA_Backend.API;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class AppointmentControllerTest 
        //: IClassFixture<WebApplicationFactory<Startup>>
    {
        //private readonly WebApplicationFactory<Startup> Factory;
        private readonly TestServer testServer;

        public AppointmentControllerTest()
        {
            //Factory = factory.WithWebHostBuilder(builder =>
            //    builder.ConfigureServices(services =>
            //    {
            //        var descriptor = services.SingleOrDefault(d =>
            //            d.ServiceType == typeof(DbContextOptions<IWAContext>));

            //        services.Remove(descriptor);

            //        services.AddDbContext<IWAContext>(options =>
            //            options.UseInMemoryDatabase("TestInMemoryDb"));

            //        var serviceProvider = services.BuildServiceProvider();

            //        using var scope = serviceProvider.CreateScope();
            //        var scopedServices = scope.ServiceProvider;

            //        //DbInitialiser.SeedDataAsync(serviceProvider).GetAwaiter().GetResult();
            //    })
            //    );
            //Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            //{
            //    AllowAutoRedirect = false,
            //});
            testServer = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup>());
        }

        [Fact]
        public async Task Successful()
        {
            // Arrange
            var client = testServer.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Account/asd");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("10", result);
        }
    }
}
