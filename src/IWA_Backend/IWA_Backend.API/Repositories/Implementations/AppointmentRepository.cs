using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Contexts;
using IWA_Backend.API.Repositories.Interfaces;

namespace IWA_Backend.API.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IWAContext Context;
        public AppointmentRepository(IWAContext context) =>
            Context = context;


        public async Task CreateAsync(Appointment appointment)
        {
            Context.Appointments.Add(appointment);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            Context.Appointments.Remove(appointment);
            await Context.SaveChangesAsync();
        }

        public bool Exists(int id) =>
            Context.Appointments
                .Any(a => a.Id == id);

        public IEnumerable<Appointment> GetBookedAppointments(string userName) =>
            Context.Appointments
                .Where(a => a.Attendees.Any(u => u.UserName == userName))
                .ToList();

        public Appointment GetById(int id) =>
            Context.Appointments
                .FirstOrDefault(appointment => appointment.Id == id)
                ?? throw new NotFoundException($"Appointment with id '{id}' not found.");

        public IEnumerable<Appointment> GetContractorsAllAppointments(string contractorUserName) =>
            Context.Appointments
                .Where(a => a.Category.Owner.UserName == contractorUserName)
                .ToList();


        public async Task UpdateAsync(Appointment appointment)
        {
            Context.Update(appointment);
            await Context.SaveChangesAsync();
        }
    }
}
