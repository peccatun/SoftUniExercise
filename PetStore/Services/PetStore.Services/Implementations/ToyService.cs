namespace PetStore.Services.Implementations
{
    using Data.Models;
    using PetStore.Data;
    using Services.Models.Toy;
    using System;
    using System.Linq;

    public class ToyService : IToyService
    {
        private readonly PetStoreDbContext data;
        private readonly IUserService userService;

        public ToyService(PetStoreDbContext data, IUserService userService)
        {
            this.data = data;
            this.userService = userService;
        }

        public void BuyFromDestributor(string name, string description, decimal distributorPrice, double profit, int brandId, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or white space");
            }


            if (profit < 0 || profit > 5)
            {
                throw new ArgumentException("Profit must be higher than zero and lower than 500%");
            }

            var toy = new Toy()
            {
                Name = name,
                Description = description,
                DistributorPrice = distributorPrice,
                Price = distributorPrice + (distributorPrice * (decimal)profit),
                BrandId = brandId,
                CategoryId = categoryId,
            };

            this.data.Toys.Add(toy);

            this.data.SaveChanges();
        }

        public void BuyFromDistributor(AddingToyServiceModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Name cannot be null or white space");
            }


            if (model.Profit < 0 || model.Profit > 5)
            {
                throw new ArgumentException("Profit must be higher than zero and lower than 500%");
            }

            var toy = new Toy()
            {
                Name = model.Name,
                Description = model.Description,
                DistributorPrice = model.Price,
                Price = model.Price + (model.Price * (decimal)model.Price),
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
            };

            this.data.Toys.Add(toy);

            this.data.SaveChanges();
        }

        public bool Exists(int toyId)
        {
            return this.data.Toys.Any(t => t.Id == toyId);
        }

        public void SellToyToUser(int toyId, int userId)
        {
            if (!this.data.Toys.Any(t => t.Id == toyId))
            {
                throw new ArgumentException("There is no such toy with given id in the database");
            }

            if (!this.userService.Exists(userId))
            {
                throw new ArgumentException("There is no such user with the given id in the database");
            }

            var order = new Order()
            {
                PurchaseDate = DateTime.Now,
                Status = OrderStatus.Done,
                UserId = userId
            };

            var toyOrder = new ToyOrder()
            {
                ToyId = toyId,
                Order = order
            };

            this.data.Orders.Add(order);
            this.data.ToyOrders.Add(toyOrder);

            this.data.SaveChanges();
        }
    }
}
