﻿using System;
using System.Linq;
using PetStore.Data;
using PetStore.Data.Models;

namespace PetStore.Services.Implementations
{


    public class PetService : IPetService
    {
        private readonly PetStoreDbContext data;
        private readonly IBreedService breedService;
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;

        public PetService(
            PetStoreDbContext data,
            IBreedService breedService,
            ICategoryService categoryService,
            IUserService userService
            )
        {
            this.data = data;
            this.categoryService = categoryService;
            this.breedService = breedService;
            this.userService = userService;
        }
        public void BuyPet(Gender gender, DateTime dateOfBirth, decimal price, string description, int breedId, int categoryId)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price of the pet cannot be less zero");
            }

            if (!this.breedService.Exists(breedId))
            {
                throw new ArgumentException("There is no such breed with that id");
            }

            if (!this.categoryService.Exists(categoryId))
            {
                throw new ArgumentException("There is no such category with breeds");
            }

            var pet = new Pet()
            {
                Gender = gender,
                DateofBirth = dateOfBirth,
                Price = price,
                Description = description,
                BreedId = breedId,
                CategoryId = categoryId,
            };

            this.data.Pets.Add(pet);

            this.data.SaveChanges();

        }

        public bool Exists(int petId)
        {
            return this.data.Pets.Any(p => p.Id == petId);
        }

        public void SellPet(int petId, int userId)
        {
            if (!this.userService.Exists(userId))
            {
                throw new ArgumentException("There is no such user with that id in database");
            }

            if (!this.Exists(petId))
            {
                throw new ArgumentException("There is no such pet in the database");
            }
            var pet = this.data.Pets.First(p => p.Id == petId);

            var order = new Order()
            {
                PurchaseDate = DateTime.Now,
                Status = OrderStatus.Done,
                UserId = userId,
            };

            this.data.Orders.Add(order);
            pet.Order = order;

            this.data.SaveChanges();


        }
    }
}
