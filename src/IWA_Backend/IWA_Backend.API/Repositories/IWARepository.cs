using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public class IWARepository : IRepository
    {
        private readonly IWAContext Context;
        public IWARepository(IWAContext context) =>
            Context = context;

        public bool AppointmentExists(int appointmentId) =>
            Context.Appointments
                .Any(a => a.Id == appointmentId);

        public bool CategoryExists(int categoryId) =>
            Context.Categories
                .Any(c => c.Id == categoryId);
        public async Task CreateAppointment(Appointment appointment)
        {
            Context.Appointments.Add(appointment);
            await Context.SaveChangesAsync();
        }

        public async Task CreateCategory(Category category)
        {
            Context.Categories.Add(category);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAppointment(Appointment appointment)
        {
            Context.Appointments.Remove(appointment);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteCategory(Category category)
        {
            Context.Appointments.RemoveRange(Context.Appointments
                .Where(a => a.Category.Id == category.Id));
            Context.Categories.Remove(category);
            await Context.SaveChangesAsync();
        }

        public IQueryable<Appointment> GetAllContractorsAppointments(string contractorUsername) =>
            Context.Appointments
                .Where(a => a.Category.Owner.UserName == contractorUsername);

        public Appointment GetAppointmentById(int appointmentId) =>
            Context.Appointments
                .FirstOrDefault(appointment => appointment.Id == appointmentId)
                ?? throw new NotFoundException($"Appointment with id '{appointmentId}' not found.");

        public Category GetCategoryById(int categoryId) =>
            Context.Categories
                .FirstOrDefault(category => category.Id == categoryId)
                ?? throw new NotFoundException($"Category with id '{categoryId}' not found.");

        public User GetUserByUserName(string userName) =>
            Context.Users
                .FirstOrDefault(user => user.UserName == userName)
                ?? throw new NotFoundException($"User with user name '{userName}' not found.");

        public async Task UpdateAppointment(Appointment appointment)
        {
            Context.DetachLocal(appointment);
            Context.Update(appointment);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            Context.DetachLocal(category);
            Context.Update(category);
            await Context.SaveChangesAsync();
        }
    }

    public static class IWAContextExtensions
    {
        public static void DetachLocal<T>(this DbContext context, T t)
            where T : class
        {
            static object? GetId(T t) => typeof(T).GetProperty("Id")?.GetValue(t);

            var local = context.Set<T>()?
                .Local?
                .FirstOrDefault(entry => GetId(entry)?.Equals(GetId(t)) ?? false);

            if (local is not null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
        }
    }
}
