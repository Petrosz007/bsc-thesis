using IWA_Backend.API;
using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Logic;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Contexts;
using IWA_Backend.API.Controllers;
using IWA_Backend.API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IWA_Backend.Tests.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }

        protected override void ConfigureDb(IServiceCollection services)
        {
            services.AddDbContext<IWAContext>(options => options
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase("TestInMemoryDb"));
        }

        protected override void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(AccountController).Assembly)
                .AddApplicationPart(typeof(Appointment).Assembly);
        }
    }
}
