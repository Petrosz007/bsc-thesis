using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IWA_Backend.API.BusinessLogic.Entities
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public virtual List<User> AllowedUsers { get; init; } = new ();
        [Required]
        public bool EveryoneAllowed { get; set; }
        [Required]
        public virtual User Owner { get; set; } = null!;
        [Required]
        public int MaxAttendees { get; set; }
        [Required]
        public int Price { get; set; }

        public virtual List<AllowedUserOnCategories> AllowedUserOnCategoriesJoin { get; }  = null!;
    }
}
