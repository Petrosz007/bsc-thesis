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
            else if (Context.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
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

        public async Task ReseedDataAsync(bool addId = true)
        {
            Context.Database.EnsureDeleted();
            await Context.SaveChangesAsync();
            Initialise();
            await SeedDataAsync(addId);
        }

        public async Task SeedDataAsync(bool addId = true)
        {
            var users = new List<User>
            {
                new User
                {
                    UserName = "contractor1",
                    Email = "contractor1@example.com",
                    Name = "Kézműves Károly",
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
                    ContractorPage = null
                },
                new User
                {
                    UserName = "customer2",
                    Email = "customer2@example.com",
                    Name = "Konszúmer Konrád",
                    ContractorPage = null
                },
            };

            foreach (var user in users)
            {
                await UserManager.CreateAsync(user, "kebab");
            }
            await Context.SaveChangesAsync();

            var role = new UserRole
            {
                Name = "Contractor",
            };
            await RoleManager.CreateAsync(role);

            await UserManager.AddToRoleAsync(Context.Users.First(u => u.UserName == "contractor1"), "Contractor");
            await UserManager.AddToRoleAsync(Context.Users.First(u => u.UserName == "contractor2"), "Contractor");
            await Context.SaveChangesAsync();


            var categories = new List<Category>
            {
                new Category
                {
                    //Id = 1,
                    Name = "Kézműves Kisegítés",
                    Description = "Karcsi segít mindenféle kézműves dologban!",
                    AllowedUsers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 10,
                    Price = 3000,
                    Owner = users[0],
                },
                new Category
                {
                    //Id = 2,
                    Name = "Korai Kőműves Kajakozás",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User>{ users[2] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[0],
                },
                new Category
                {
                    //Id = 3,
                    Name = "Angol C1 felkészítés",
                    Description = "Felkészítés az Angol C1 nyelvvizsgára",
                    AllowedUsers = new List<User>(),
                    EveryoneAllowed = true,
                    MaxAttendees = 1,
                    Price = 3000,
                    Owner = users[1],
                },
                new Category
                {
                    //Id = 4,
                    Name = "Privát angol Konrádnak",
                    Description = "Karcsi kajakja kajak jó!",
                    AllowedUsers = new List<User>{ users[3] },
                    EveryoneAllowed = false,
                    MaxAttendees = 2,
                    Price = 5000,
                    Owner = users[1],
                },
            };
            int categoryId = 1;
            foreach (var category in categories)
            {
                if (addId)
                    category.Id = categoryId++;
                Context.Categories.Add(category);
            }
            await Context.SaveChangesAsync();

            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    //Id = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[0],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 2,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[0],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[0].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 3,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 4,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[1],
                    Attendees = new List<User>{ users[3] },
                    MaxAttendees = categories[1].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 5,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 6,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[2],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[2].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 7,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
                new Appointment
                {
                    //Id = 8,
                    StartTime = DateTime.Now.AddHours(1),
                    EndTime = DateTime.Now.AddHours(2),
                    Category = categories[3],
                    Attendees = new List<User>{ },
                    MaxAttendees = categories[3].MaxAttendees,
                },
            };
            int appointmentId = 1;
            foreach(var appointment in appointments)
            {
                if (addId)
                    appointment.Id = appointmentId++;

                Context.Appointments.Add(appointment);
            }

            await Context.SaveChangesAsync();
        }
    }
}
