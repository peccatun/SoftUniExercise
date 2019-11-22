using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new ProductShopContext())
            {
                //var document = File.ReadAllText("./../../../Datasets/categories-products.xml");
                Console.WriteLine(GetSoldProducts(db));
            }
        }

        //problem 01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);
            List<User> users = new List<User>();

            foreach (var user in doc.Root.Elements())
            {
                var newUser = new User()
                {
                    FirstName = user.Element("firstName").Value,
                    LastName = user.Element("lastName").Value,
                    Age = int.Parse(user.Element("age").Value),
                };

                users.Add(newUser);
            }

            context
                .Users
                .AddRange(users);

            int saved = context
                .SaveChanges();


            return $"Successfully imported {saved}";
        }

        //proble 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(
                typeof(ProductDto[]),
                new XmlRootAttribute("Products"));

            var deserializedProducts = (ProductDto[])serializer.Deserialize(new StringReader(inputXml));

            List<Product> productsList = new List<Product>();
            foreach (var product in deserializedProducts)
            {
                var newProduct = new Product()
                {
                    Name = product.Name,
                    Price = product.Price,
                    SellerId = product.SellerId,
                    BuyerId = product.BuyerId,
                };

                if (newProduct.BuyerId == 0)
                {
                    newProduct.BuyerId = null;
                }

                productsList.Add(newProduct);
            }

            context
                .Products
                .AddRange(productsList);

            int saved = context
                .SaveChanges();


            return $"Successfully imported {saved}";
        }

        //problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(
                typeof(CategoryDto[]),
                new XmlRootAttribute("Categories"));

            CategoryDto[] categoryDtos;
            using (var stream = new StringReader(inputXml))
            {
                categoryDtos = (CategoryDto[])serializer.Deserialize(stream);
            }

            List<Category> result = new List<Category>();

            for (int i = 0; i < categoryDtos.Length; i++)
            {
                if (!string.IsNullOrEmpty(categoryDtos[i].Name))
                {
                    Category category = new Category()
                    {
                        Name = categoryDtos[i].Name,
                    };

                    result.Add(category);
                }
            }

            context
                .Categories
                .AddRange(result);

            int saved = context
                .SaveChanges();


            return $"Successfully imported {saved}";
        }

        //problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(
                typeof(CategoryProductDto[]),
                new XmlRootAttribute("CategoryProducts"));

            CategoryProductDto[] deserializedCategoryProducts;
            using (var sr = new StringReader(inputXml))
            {
                deserializedCategoryProducts = (CategoryProductDto[])serializer.Deserialize(sr);
            }

            var productIds = context
                .Products
                .Select(p => p.Id)
                .ToList();

            var categoryIds = context
                .Categories
                .Select(c => c.Id)
                .ToList();

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            for (int i = 0; i < deserializedCategoryProducts.Length; i++)
            {
                if (productIds.Contains(deserializedCategoryProducts[i].ProductId) && 
                    categoryIds.Contains(deserializedCategoryProducts[i].CategoryId))
                {
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = deserializedCategoryProducts[i].CategoryId,
                        ProductId = deserializedCategoryProducts[i].ProductId,
                    };

                    categoryProducts.Add(categoryProduct);
                }
            }

            context
                .CategoryProducts
                .AddRange(categoryProducts);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}";
        }

        //prolem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var result = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductsOverDto()
                {
                    Name = p.Name,
                    Price = p.Price.ToString("G29"),
                    Buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .Take(10)
                .ToArray();

            for (int i = 0; i < result.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(result[i].Buyer))
                {
                    result[i].Buyer = null;
                }
            }

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportProductsOverDto[]),
                new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer
                    .Serialize(writer, result,namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var products = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new UserSoldDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                    .Select(ps => new SoldProductDto
                    {
                        Name = ps.Name,
                        Price = ps.Price,
                    })
                    .ToArray()
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(UserSoldDto[]),
                new XmlRootAttribute("Users"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, products,namespaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}