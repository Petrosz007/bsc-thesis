using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.Contexts;
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

        public async Task CreateAppointment(Appointment appointment)
        {
            Context.Appointments.Add(appointment);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAppointment(Appointment appointment)
        {
            Context.Appointments.Remove(appointment);
            await Context.SaveChangesAsync();
        }

        public IQueryable<Appointment> GetAllContractorsAppointments(string contractorUsername)
        {
            throw new NotImplementedException();
        }

        public Appointment? GetAppointmentById(int appointmentId) =>
            Context.Appointments
                .FirstOrDefault(appointment => appointment.Id == appointmentId);

        public IQueryable<Appointment> GetContractorsAppointmentsVisibleToCustomer(string contractorUsername, string customerUserName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Appointment> GetPublicContractorsAppointments(string contractorUsername)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAppointment(Appointment appointment)
        {
            Context.Update(appointment);
            await Context.SaveChangesAsync();
        }
    }
}
