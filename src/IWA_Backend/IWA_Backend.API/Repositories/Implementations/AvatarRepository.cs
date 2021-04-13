﻿using System;
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
        private static string PathFromId(string id) =>
            Path.Combine("Avatars", id);
        
        public bool Exists(string id) =>
            File.Exists(PathFromId(id));

        public async Task<(byte[] Bytes, AvatarFileTypes FileType)> GetByIdAsync(string id)
        {
            if (!Exists(id))
                throw new NotFoundException($"Avatar not found with id: '{id}'");
            
            var filePath = PathFromId(id);
            var extension = AvatarFileTypesExtensions.FromExtension(Path.GetExtension(filePath));
            var bytes = await File.ReadAllBytesAsync(filePath);
            return (bytes, extension);
        }

        public async Task<(byte[] Bytes, AvatarFileTypes FileType)> GetDefaultAsync() =>
            await GetByIdAsync("default.jpg");
        
        public async Task<string> CreateAsync(IFormFile file, AvatarFileTypes extension)
        {
            var id = $"{Guid.NewGuid()}.{extension.GetExtension()}";
            var filePath = PathFromId(id);
            
            using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);

            return id;
        }

        public Task DeleteAsync(string id)
        {
            if (!Exists(id))
            {
                throw new NotFoundException($"Avatar not found with id: '{id}'");
            }
            
            var filePath = PathFromId(id);
            File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}