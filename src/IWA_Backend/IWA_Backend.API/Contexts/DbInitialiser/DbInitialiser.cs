using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IWA_Backend.API.Contexts.DbInitialiser
{
    public class DbInitialiser
    {
        private readonly IWAContext Context;
        private readonly ISeedData SeedData;
        private readonly UserManager<User> UserManager;
        private readonly RoleManager<UserRole> RoleManager;

        public DbInitialiser(IWAContext context, UserManager<User> userManager, RoleManager<UserRole> roleManager, ISeedData seedData)
        {
            Context = context;
            UserManager = userManager;
            RoleManager = roleManager;
            SeedData = seedData;
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
            await Context.Database.EnsureDeletedAsync();
            Initialise();
            await SeedDataAsync(addId);
        }

        public async Task SeedDataAsync(bool addId = true)
        {
            var users = SeedData.Users();
            var categories = SeedData.Categories(users);
            var appointments = SeedData.Appointments(categories, users);
            
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


            int categoryId = 1;
            foreach (var category in categories)
            {
                if (addId)
                    category.Id = categoryId++;
                Context.Categories.Add(category);
            }
            await Context.SaveChangesAsync();

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
