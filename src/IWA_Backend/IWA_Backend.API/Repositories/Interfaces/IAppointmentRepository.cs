using System.Linq;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface IAppointmentRepository : ICrudRepository<Appointment, int>
    {
        IQueryable<Appointment> GetContractorsAllAppointments(string contractorUserName);
        IQueryable<Appointment> GetBookedAppointments(string userName);
    }
}
