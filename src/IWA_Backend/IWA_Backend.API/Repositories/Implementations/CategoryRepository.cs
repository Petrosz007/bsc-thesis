﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Contexts;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IWAContext Context;
        public CategoryRepository(IWAContext context) =>
            Context = context;

        public async Task CreateAsync(Category category)
        {
            Context.Categories.Add(category);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            Context.Appointments.RemoveRange(Context.Appointments
                .Where(a => a.Category.Id == category.Id));
            Context.Categories.Remove(category);
            await Context.SaveChangesAsync();
        }

        public bool Exists(int id) =>
            Context.Categories
                .Any(c => c.Id == id);

        public Category GetById(int id) =>
            Context.Categories
                .FirstOrDefault(category => category.Id == id)
                ?? throw new NotFoundException($"'{id}' azonosítójú kategória nem található.");

        public IEnumerable<Category> GetUsersCategories(string? userName) =>
            Context.Categories
                .Where(category => category.Owner.UserName == userName)
                .ToList();

        public bool IsUserInAnAppointmentOfACategory(int categoryId, string? userName) =>
            Context.Appointments
                .Where(a => a.Category.Id == categoryId)
                .Any(appointment => appointment.Attendees.Any(u => u.UserName == userName));

        public async Task UpdateAsync(Category category)
        {
            Context.Update(category);
            await Context.SaveChangesAsync();
        }
    }
}
