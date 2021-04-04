using System.Collections.Generic;
using System.Linq;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface IAppointmentRepository : ICrudRepository<Appointment, int>
    {
        IEnumerable<Appointment> GetContractorsAllAppointments(string contractorUserName);
        IEnumerable<Appointment> GetBookedAppointments(string userName);
    }
}
