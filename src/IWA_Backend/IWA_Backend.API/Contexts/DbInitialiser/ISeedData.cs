using System.Collections.Generic;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Contexts.DbInitialiser
{
    public interface ISeedData
    {
        List<User> Users { get; }
        List<Category> Categories { get; }
        List<Appointment> Appointments { get; }
    }
}