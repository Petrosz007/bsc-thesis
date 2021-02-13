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
        public string UserName { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }

    public record RegisterDTO()
    {
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; } = "";
    }
}
