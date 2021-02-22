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

        public bool CategoryExists(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAppointment(Appointment appointment)
        {
            Context.Appointments.Add(appointment);
            await Context.SaveChangesAsync();
        }

        public Task CreateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAppointment(Appointment appointment)
        {
            Context.Appointments.Remove(appointment);
            await Context.SaveChangesAsync();
        }

        public Task DeleteCategory(Category category)
        {
            throw new NotImplementedException();
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
            Context.Update(appointment);
            await Context.SaveChangesAsync();
        }

        public Task UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
