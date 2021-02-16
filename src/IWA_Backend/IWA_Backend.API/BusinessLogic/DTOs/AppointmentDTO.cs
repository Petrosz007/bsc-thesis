using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.DTOs
{
    public record AppointmentDTO
    {
        public int Id { get; init; }
        [Required]
        public DateTime StartTime { get; init; }
        [Required]
        public DateTime EndTime { get; init; }
        [Required]
        public int CategoryId { get; init; }
        [Required]
        public IEnumerable<string> AttendeeUserNames { get; init; }  = new List<string>();
        [Required]
        public int MaxAttendees { get; init; }
    }
}
