using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IWA_Backend.API.BusinessLogic.DTOs
{
    public record CategoryDTO
    {
        // Not required, always set to the update id, or created by the db
        public int Id { get; init; }
        [Required]
        public string Name { get; init; } = null!;
        [Required]
        public string Description { get; init; } = null!;
        [Required]
        public IEnumerable<string> AllowedUserNames { get; init; } = new List<string>();
        [Required]
        public bool EveryoneAllowed { get; init; }
        // Not required, always set to the current user on create, update
        public string OwnerUserName { get; init; } = null!;
        [Required]
        [Range(1, int.MaxValue)]
        public int MaxAttendees { get; init; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Price { get; init; }

        public bool ValuesEqual(object? obj)
        {
            if (obj is CategoryDTO other)
            {
                var userNamesEqual = this.AllowedUserNames.SequenceEqual(other.AllowedUserNames);

                // The list will be to the same reference, so they are equal
                // We already checked if the sequences are equal
                var list = new List<string>();
                var left = this with { AllowedUserNames = list };
                var right = other with { AllowedUserNames = list };

                return userNamesEqual && left.Equals(right);
            }

            return false;
        }
    }
}
