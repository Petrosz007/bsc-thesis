using System;
using System.IO;
using System.Threading.Tasks;
using IWA_Backend.API.BusinessLogic.Entities;
using IWA_Backend.API.BusinessLogic.Exceptions;
using IWA_Backend.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IWA_Backend.API.Repositories.Implementations
{
    public class AvatarRepository : IAvatarRepository
    {
        public async Task<(byte[] Bytes, AvatarFileTypes FileType)> GetByIdAsync(string id)
        {
            var filePath = Path.Combine("Avatars", id);
            if (!File.Exists(filePath))
                throw new NotFoundException($"Avatar not found with id: '{id}'");
            
            var extension = AvatarFileTypesExtensions.FromExtension(Path.GetExtension(filePath));
            var bytes = await File.ReadAllBytesAsync(filePath);
            return (bytes, extension);
        }

        public async Task<(byte[] Bytes, AvatarFileTypes FileType)> GetDefaultAsync() =>
            await GetByIdAsync("default.jpg");
        
        public async Task<string> CreateAsync(IFormFile file, AvatarFileTypes extension)
        {
            var id = $"{Guid.NewGuid()}.{extension.GetExtension()}";
            var filePath = Path.Combine("Avatars", id);
            
            using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);

            return id;
        }

        public Task DeleteAsync(string id)
        {
            var filePath = Path.Combine("Avatars", id);
            if (!File.Exists(filePath))
                throw new NotFoundException($"Avatar not found with id: '{id}'");
            
            File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}