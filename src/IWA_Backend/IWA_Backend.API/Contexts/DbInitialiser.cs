﻿using IWA_Backend.API.BusinessLogic.Entities;
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
    public class DbInitialiser
    {
        private readonly IWAContext Context;
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<UserRole> RoleManager;

        public DbInitialiser(IWAContext context, UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            Context = context;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public void Initialise()
        {
            if (Context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                Context.Database.EnsureCreated();
            }
            else
            {
                Context.Database.Migrate();
            }

            Context.SaveChanges();
        }

        public bool AnyCategories() =>
            Context.Categories.Any();

        public async Task ReseedDataAsync()
        {
            Context.Database.EnsureDeleted();
            await Context.SaveChangesAsync();
            Initialise();
            await SeedDataAsync();
        }

        public async Task SeedDataAsync()
        {
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
                await UserManager.CreateAsync(user, "kebab");
            }

            var role = new UserRole
            {
                Name = "Contractor",
            };
            await RoleManager.CreateAsync(role);

            await UserManager.AddToRoleAsync(users[0], "Contractor");
            await UserManager.AddToRoleAsync(users[1], "Contractor");


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
            Context.Categories.AddRange(categories);

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[0],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[0],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ users[3] },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
                new Appointment
                {
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
            };
            Context.Appointments.AddRange(appointments);

            await Context.SaveChangesAsync();
        }
    }
}
