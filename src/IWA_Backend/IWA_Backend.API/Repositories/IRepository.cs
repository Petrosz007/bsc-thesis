using IWA_Backend.API.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public interface IRepository
    {
        public IQueryable<Appointment> GetAllContractorsAppointments(string contractorUsername);
        public IQueryable<Appointment> GetPublicContractorsAppointments(string contractorUsername);
        public IQueryable<Appointment> GetContractorsAppointmentsVisibleToCustomer(string contractorUsername, string customerUserName);
        public Appointment? GetAppointmentById(int appointmentId);
        public Task CreateAppointment(Appointment appointment);
        public Task UpdateAppointment(Appointment appointment);
        public Task DeleteAppointment(Appointment appointment);
    }
}
