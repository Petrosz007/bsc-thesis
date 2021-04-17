using IWA_Backend.API.BusinessLogic.DTOs;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Logic;
using IWA_Backend.API.BusinessLogic.Mappers;
using IWA_Backend.API.Contexts;
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
using System.Threading.Tasks;
using IWA_Backend.API.Contexts.DbInitialiser;
using IWA_Backend.API.Repositories.Implementations;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        protected virtual ISeedData SeedData => new LiveSeedData();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDb(services);

            services.AddIdentity<User, UserRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IWAContext>();

            services.AddSingleton<ISeedData>(SeedData);
            services.AddTransient<DbInitialiser>();

            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAvatarRepository, AvatarRepository>(_ => new AvatarRepository(Configuration["AvatarDir"] ?? "./Avatars"));

            services.AddTransient<AppointmentLogic>();
            services.AddTransient<CategoryLogic>();
            services.AddTransient<UserLogic>();

            services.AddAutoMapper(typeof(Startup));

            ConfigureControllers(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IWA_Backend.API", Version = "v1" });
            });

            services.AddCors(o => o.AddPolicy("Allow All Policy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddCors(o => o.AddPolicy("Localhost", builder =>
            {
                builder.WithOrigins(Configuration["CorsAllowUrls"]?.Split(',') ?? Array.Empty<string>())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }));
        }

        protected virtual void ConfigureDb(IServiceCollection services)
        {
            services.AddDbContext<IWAContext>(options => options
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging()
                .UseMySql(Configuration.GetConnectionString("MySqlServer"), new MySqlServerVersion(new Version(10, 5, 3))));
        }
        
        protected virtual void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbInitialiser dbInitialiser)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IWA_Backend.API v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }

            //app.UseCors("Allow All Policy");
            app.UseCors("Localhost");


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            dbInitialiser.Initialise();
        }
    }
}
