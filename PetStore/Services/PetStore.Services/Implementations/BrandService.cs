namespace PetStore.Services.Implementations
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using Data;
    using Data.Models;
    using Services.Models.Brand;
    using Services.Models.Toy;

    public class BrandService : IBrandService
    {
        private readonly PetStoreDbContext data;

        public BrandService(PetStoreDbContext data)
            => this.data = data;

        public int Create(string name)
        {

            if (name.Length > DataValidation.NameMaxLength)
            {
                throw new InvalidOperationException($"Brand name cannot be more than {DataValidation.NameMaxLength} characters");
            }
            var brand = new Brand()
            {
                Name = name
            };

            this.data.Brands.Add(brand);

            this.data.SaveChanges();

            return brand.Id;
        }

        public BrandWithToysServiceModel FindByIdWithToys(int id)
            => this.data
            .Brands
            .Where(b => b.Id == id)
            .Select(b => new BrandWithToysServiceModel
            {
                Name = b.Name,
                Toys = b.Toys.Select(t => new ToyListingServiceModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Price = t.Price,
                    TotalOrders = t.Orders.Count,
                })
            })
            .FirstOrDefault();

        public IEnumerable<BrandListingServiceModel> SearchByName(string name)
            => this.data
            .Brands
            .Where(b => b.Name.ToLower().Contains(name.ToLower()))
            .Select(br => new BrandListingServiceModel
            {
                Id = br.Id,
                Name = br.Name
            })
            .ToList();

    }
}
