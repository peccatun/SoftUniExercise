namespace PetStore
{
    using Data;
    using Services.Implementations;
    using System;

    public class StartUp
    {
        public static void Main()
        {
            using var data = new PetStoreDbContext();

            var userService = new UserSerice(data);
            var breedService = new BreedService(data);

            var categoryService = new CategoryService(data);
            var petService = new PetService(data,breedService,categoryService,userService);

            petService.BuyPet(Data.Models.Gender.Male, DateTime.Now, 0m, null, 1, 1);
            petService.SellPet(1, 1);
        }
    }
}
