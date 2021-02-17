using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Contexts
{
    [ExcludeFromCodeCoverage]
    public static class DbInitialiser
    {
        public static void Initialise(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IWAContext>();

            if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                context.Database.EnsureCreated();
            }
            else
            {
                context.Database.Migrate();
            }

            context.SaveChanges();
        }

        public static bool AnyCategories(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<IWAContext>().Categories.Any() ;

        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IWAContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<UserRole>>();

            var users = new List<User>
            {
                new User
                {
                    UserName = "contractor1",
                    Email = "contractor1@example.com",
                    Name = "Kézműves Károly",
                    Avatar = null,
                    ContractorPage = new ContractorPage
                    {
                        Title = "Kézműves mester",
                        Bio = "Kézműves Károly, 46 éves, nagyon érti a dolgát",
                    },
                },
                new User
                {
                    UserName = "contractor2",
                    Email = "contractor2@example.com",
                    Name = "Angoltanár András",
                    Avatar = null,
                    ContractorPage = new ContractorPage
                    {
                        Title = "Angol tanár",
                        Bio = "Nyelvizsgára felkészítést, magán angol órákat vállalok",
                    },
                },
                new User
                {
                    UserName = "customer1",
                    Email = "customer1@example.com",
                    Name = "Fogyasztó Feri",
                    Avatar = null,
                    ContractorPage = null
                },
                new User
                {
                    UserName = "customer2",
                    Email = "customer2@example.com",
                    Name = "Konszúmer Konrád",
                    Avatar = null,
                    ContractorPage = null
                },
            };

            foreach(var user in users)
            {
                await userManager.CreateAsync(user, "kebab");
            }

            var role = new UserRole
            {
                Name = "Contractor",
            };
            await roleManager.CreateAsync(role);

            await userManager.AddToRoleAsync(users[0], "Contractor");
            await userManager.AddToRoleAsync(users[1], "Contractor");


            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Kézműves Kisegítés",
                    Description = "Karcsi segít mindenféle kézműves dologban!",
                    AllowedCustomers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 10,
                    Price = 3000,
                    Owner = users[0],
                },
                new Category
                {
                    Name = "Korai Kőműves Kajakozás",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedCustomers = new List<User>{ users[2] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[0],
                },
                new Category
                {
                    Name = "Angol C1 felkészítés",
                    Description = "Felkészítés az Angol C1 nyelvvizsgára",
                    AllowedCustomers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 1,
                    Price = 3000,
                    Owner = users[1],
                },
                new Category
                {
                    Name = "Privát angol Konrádnak",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedCustomers = new List<User>{ users[3] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[1],
                },
            };
            context.Categories.AddRange(categories);

            var appointments = new List<Appointment>
            {

            };
            context.Appointments.AddRange(appointments);

            await context.SaveChangesAsync();
        }
    }
}
