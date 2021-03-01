using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.DTOs
{
    public record LoginDTO()
    {
        [Required]
        public string UserName { get; init; } = null!;
        [Required]
        public string Password { get; init; } = null!;
    }

    public record RegisterDTO()
    {
        [Required]
        public string UserName { get; init; } = null!;
        [Required]
        public string Email { get; init; } = null!;
        [Required]
        public string Name { get; init; } = null!;
        [Required]
        public string Password { get; init; } = null!;
        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; init; } = null!;
        public ContractorPageDTO? ContractorPage { get; init; } = null;
    }
}
