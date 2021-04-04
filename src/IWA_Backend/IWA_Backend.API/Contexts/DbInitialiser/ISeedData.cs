using System.Collections.Generic;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Contexts.DbInitialiser
{
    public interface ISeedData
    {
        List<User> Users();
        List<Category> Categories(List<User> users);
        List<Appointment> Appointments(List<Category> categories, List<User> users);
    }
}