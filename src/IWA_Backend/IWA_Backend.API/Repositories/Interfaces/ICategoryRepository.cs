using System.Collections.Generic;
using System.Linq;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface ICategoryRepository : ICrudRepository<Category, int>
    {
        public IEnumerable<Category> GetUsersCategories(string? userName);
        public bool IsUserInAnAppointmentOfACategory(int categoryId, string? userName);
    }
}
