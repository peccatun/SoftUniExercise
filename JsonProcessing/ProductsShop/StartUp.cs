using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {

            using (var db = new ProductShopContext())
            {
                Console.WriteLine(GetUsersWithProducts(db));
            }
        }

        //problem 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var jsonObjects = JsonConvert.DeserializeObject<User[]>(inputJson);

            context
                .Users.AddRange(jsonObjects);

            context.SaveChanges();

            return $"Successfully imported {jsonObjects.Length}"; 
        }

        //problem 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productObj = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context
                .Products.AddRange(productObj);

            context
                .SaveChanges();


            return $"Successfully imported {productObj.Length}";
        }

        //problem 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
                
            };

            var categoriesObj = JsonConvert.DeserializeObject<Category[]>(inputJson, settings);
            int added = 0;

            for (int i = 0; i < categoriesObj.Length; i++)
            {
                if (!String.IsNullOrEmpty(categoriesObj[i].Name))
                {
                    context.Categories.Add(categoriesObj[i]);
                    added++;
                }
            }

            context
                .SaveChanges();

            return $"Successfully imported {added}";
        }

        //problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categorisProductcObj = JsonConvert
                .DeserializeObject<CategoryProduct[]>(inputJson);

            context
                .AddRange(categorisProductcObj);

            int saved = context
                .SaveChanges();
            return $"Successfully imported {saved}";
        }

        //problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var result = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .ToList();

            var jsonProduct = JsonConvert.SerializeObject(result, Formatting.Indented);

            return jsonProduct;
        }

        //problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {

            var result = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(ps => ps.BuyerId != null)
                    .Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    }).ToList()
                })
                .ToList();

            var jsonObj = JsonConvert.SerializeObject(result, Formatting.Indented); 

            return jsonObj;
        }

        //problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = $"{c.CategoryProducts.Sum(cp => cp.Product.Price) / c.CategoryProducts.Count:f2}",
                    totalRevenue = $"{c.CategoryProducts.Sum(cp => cp.Product.Price):f2}"
                })
                .ToList();

            var jsonObj = JsonConvert.SerializeObject(result, Formatting.Indented);

            return jsonObj;
        }

        //problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(p => p.ProductsSold.Count(ps => ps.Buyer != null))
                .Select(u => new 
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new 
                    {
                        count = u.ProductsSold.Count(p => p.Buyer != null),
                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new 
                        {
                            name = p.Name,
                            price = p.Price
                        })
                        .ToList()
                    }
                })
                .ToList();

            var result = new 
            {
                usersCount = users.Count(),
                users = users
            };

            var json = JsonConvert.SerializeObject(result,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    
}
}