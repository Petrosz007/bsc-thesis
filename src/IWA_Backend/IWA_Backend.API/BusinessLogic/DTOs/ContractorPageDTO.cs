using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.DTOs
{
    public record ContractorPageDTO
    {
        [Required]
        public string Title { get; init; } = null!;
        [Required]
        public string Bio { get; init; } = null!;
    }
}
