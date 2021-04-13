using System;
using AutoMapper.Mappers;
using IWA_Backend.API.BusinessLogic.Exceptions;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public enum AvatarFileTypes
    {
        Jpeg, Png
    }

    public static class AvatarFileTypesExtensions
    {
        public static string GetExtension(this AvatarFileTypes type) =>
            type switch
            {
                AvatarFileTypes.Jpeg => "jpg",
                AvatarFileTypes.Png => "png",
                _ => throw new Exception($"Unknown AvatarFileType: {type}"),
            };
        
        public static string GetMIMEType(this AvatarFileTypes type) =>
            type switch
            {
                AvatarFileTypes.Jpeg => "image/jpeg",
                AvatarFileTypes.Png => "image/png",
                _ => throw new Exception($"Unknown AvatarFileType: {type}"),
            };

        public static AvatarFileTypes FromExtension(string extension) =>
            extension switch
            {
                ".jpg" => AvatarFileTypes.Jpeg,
                ".jpeg" => AvatarFileTypes.Jpeg,
                ".png" => AvatarFileTypes.Png,
                _ => throw new InvalidAvatarFileException($"Unsupported extension: {extension}"),
            };
    }
}