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
        public DateTimeOffset StartTime { get; init; }
        [Required]
        public DateTimeOffset EndTime { get; init; }
        [Required]
        public int CategoryId { get; init; }
        [Required]
        public IEnumerable<string> AttendeeUserNames { get; init; } = new List<string>();
        [Required]
        public int MaxAttendees { get; init; }

        public bool ValuesEqual(object? obj)
        {
            if(obj is AppointmentDTO other)
            {
                var userNamesEqual = this.AttendeeUserNames.SequenceEqual(other.AttendeeUserNames);

                // The list will be to the same reference, so they are equal
                // We already checked if the sequences are equal
                var list = new List<string>();
                var left = this with { AttendeeUserNames = list };
                var right = other with { AttendeeUserNames = list };

                return userNamesEqual && left.Equals(right);
            }

            return false;
        }
    }
}
