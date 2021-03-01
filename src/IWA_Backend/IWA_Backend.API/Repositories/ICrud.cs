using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories
{
    public interface ICrudRepository<TEntity, TId>
    {
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        TEntity GetById(TId id);
        bool Exists(TId id);
    }
}
