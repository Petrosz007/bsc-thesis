using System.Linq;
using IWA_Backend.API.BusinessLogic.Entities;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface ICategoryRepository : ICrudRepository<Category, int>
    {
        public IQueryable<Category> GetUsersCategories(string? userName);
        public bool IsUserInAnAppointmentOfACategory(int categoryId, string? userName);
    }
}
