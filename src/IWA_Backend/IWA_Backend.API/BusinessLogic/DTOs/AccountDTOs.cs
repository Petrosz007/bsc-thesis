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
        [RegularExpression(@"^[a-zA-Z0-9]{3,25}$", ErrorMessage = "Nem megfelelő felhasználónév formátum. 3-25 karakter, a-z, A-Z, 0-9, _")]
        public string UserName { get; init; } = null!;
        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Minimum 5 karakteresnek kell lennie a jelszónak.")]
        public string Password { get; init; } = null!;
    }

    public record RegisterDTO()
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,25}$", ErrorMessage = "Nem megfelelő felhasználónév formátum. 3-25 karakter, a-z, A-Z, 0-9, _")]
        public string UserName { get; init; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; init; } = null!;
        [Required]
        public string Name { get; init; } = null!;
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 5, ErrorMessage = "Minimum 5 karakteresnek kell lennie a jelszónak.")]
        public string Password { get; init; } = null!;
        [Required]
        [Compare("Password")]
        [StringLength(int.MaxValue, MinimumLength = 5, ErrorMessage = "Minimum 5 karakteresnek kell lennie a jelszónak.")]
        public string PasswordConfirmation { get; init; } = null!;
        public ContractorPageDTO? ContractorPage { get; init; } = null;
    }
}
