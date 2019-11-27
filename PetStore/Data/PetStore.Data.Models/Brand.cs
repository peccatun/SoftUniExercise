using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Data.Models
{
    using static DataValidation;

    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();

        public ICollection<Food> Foods { get; set; } = new HashSet<Food>();
    }
}
