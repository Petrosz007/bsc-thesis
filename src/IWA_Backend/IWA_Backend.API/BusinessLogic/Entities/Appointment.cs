using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public Category Category { get; set; } = new();
        [Required]
        public List<User> Attendees { get; set; } = new();
        [Required]
        public User Owner { get; set; } = new();
        [Required]
        public int MaxAttendees { get; set; }
    }
}
