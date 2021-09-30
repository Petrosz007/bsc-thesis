using IWA_Backend.API;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Contexts.DbInitialiser;
using IWA_Backend.API.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IWA_Backend.Tests.Utilities
{
    public class TestStartup : Startup
    {
        protected override ISeedData SeedData => new IntegrationTestData();

        public TestStartup(IConfiguration configuration) : base(configuration) { }

        protected override void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(AccountController).Assembly)
                .AddApplicationPart(typeof(Appointment).Assembly);
        }
    }
}
