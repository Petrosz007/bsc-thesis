using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using Microsoft.AspNetCore.Http;

namespace IWA_Backend.API.Repositories.Interfaces
{
    public interface IAvatarRepository
    {
        bool Exists(string id);
        Task<(byte[] Bytes, AvatarFileTypes FileType)> GetByIdAsync(string id);
        Task<(byte[] Bytes, AvatarFileTypes FileType)> GetDefaultAsync();
        Task<string> CreateAsync(IFormFile file, AvatarFileTypes extension);
        Task DeleteAsync(string id);
    }
}