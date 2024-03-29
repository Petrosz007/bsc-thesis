﻿using AutoMapper;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IWA_Backend.Tests.Utilities
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly TestWebApplicationFactory<TestStartup> Factory = new();
        protected IWAContext Context =>
            Factory.Services.CreateScope().ServiceProvider.GetRequiredService<IWAContext>();

        protected UserManager<User> UserManager =>
            Factory.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();

        protected IMapper Mapper =>
            Factory.Services.CreateScope().ServiceProvider.GetRequiredService<IMapper>();

        public void Dispose()
        {
            Factory.Dispose();
        }
    }
}
