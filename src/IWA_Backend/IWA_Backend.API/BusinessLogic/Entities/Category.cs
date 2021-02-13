using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public record Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public List<User> AllowedCustomers { get; set; } = new();
        [Required]
        public bool EveryoneAllowed { get; set; }
        [Required]
        public int MaxAttendees { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
