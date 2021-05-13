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
        [EmailAddress]
        public string Email { get; init; } = null!;
        [Required]
        public string Name { get; init; } = null!;
        public ContractorPageDTO? ContractorPage { get; init; } = null;
    }

    public record UserInfoDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,25}$", ErrorMessage = "Nem megfelelő felhasználónév formátum. 3-25 karakter, a-z, A-Z, 0-9, _")]
        public string UserName { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public ContractorPageDTO? ContractorPage { get; set; } = null;
    }

    public record UserSelfInfoDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,25}$", ErrorMessage = "Nem megfelelő felhasználónév formátum. 3-25 karakter, a-z, A-Z, 0-9, _")]
        public string UserName { get; init; } = null!;
        [Required]
        public string Name { get; init; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; init; } = null!;
        public ContractorPageDTO? ContractorPage { get; init; } = null;
    }

    public record IsLoggedInDTO(bool IsLoggedIn, string? UserName);
}
