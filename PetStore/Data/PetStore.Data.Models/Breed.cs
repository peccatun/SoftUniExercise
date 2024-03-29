﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Data.Models
{
    using static DataValidation;

    public class Breed
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();
    }
}
