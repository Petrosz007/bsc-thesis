using IWA_Backend.API.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public interface ICategoryRepository : ICrudRepository<Category, int>
    {
    }
}
