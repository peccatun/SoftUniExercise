namespace PetStore.Services.Implementations
{
    using System;
    using System.Linq;
    using Data;
    using Data.Models;

    public class BreedService : IBreedService
    {
        private readonly PetStoreDbContext data;

        public BreedService(PetStoreDbContext data)
        {
            this.data = data;
        }

        public void AddBreed(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Breed name cannot be null or whitespace");
            }

            Breed breed = new Breed()
            {
                Name = name
            };

            this.data.Breeds.Add(breed);
            this.data.SaveChanges();
        }

        public bool Exists(int breedId)
        {
            return this.data
                    .Breeds
                    .Any(b => b.Id == breedId);
        }
    }
}
