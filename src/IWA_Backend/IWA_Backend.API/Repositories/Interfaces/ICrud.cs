using System.Threading.Tasks;

namespace IWA_Backend.API.Repositories.Interfaces
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
