using IWA_Backend.API.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public interface IAppointmentRepository : ICrudRepository<Appointment, int>
    {
        IQueryable<Appointment> GetContractorsAllAppointments(string contractorUserName);
        IQueryable<Appointment> GetBookedAppointments(string userName);
        Task BookAppointmentAsync(Appointment appointment, string userName);
        Task UnBookAppointmentAsync(Appointment appointment, string userName);
    }
}
