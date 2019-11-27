using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Data.Models
{
    using static DataValidation;

    public class Food
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        //In KG.
        public double Weight { get; set; }

        public decimal DistributorPrice { get; set; }

        public decimal Price { get; set; }

        public DateTime ExpirationDay { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<FoodOrder> Orders { get; set; } = new HashSet<FoodOrder>();
    }
}
