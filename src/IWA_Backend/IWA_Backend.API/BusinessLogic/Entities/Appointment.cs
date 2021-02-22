using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public record Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public virtual Category Category { get; set; } = null!;
        [Required]
        public virtual ICollection<User> Attendees { get; set; } = new List<User>();
        [Required]
        public int MaxAttendees { get; set; }
    }
}
