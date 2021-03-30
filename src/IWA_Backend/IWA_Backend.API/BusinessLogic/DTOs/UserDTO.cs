using IWA_Backend.API.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.DTOs
{
    public record UserUpdateDTO
    {
        [Required]
        public string Email { get; init; } = null!;
        [Required]
        public string Name { get; init; } = null!;
        public ContractorPageDTO? ContractorPage { get; init; } = null;
    }

    public record UserInfoDTO
    {
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ContractorPageDTO? ContractorPage { get; set; } = null;
    }

    public record IsLoggedInDTO(bool IsLoggedIn, string? UserName);
}
