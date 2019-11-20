using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {

                Console.WriteLine(GetSalesWithAppliedDiscount(db));
            }
        }

        //problem 9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suplyJson = JsonConvert
                .DeserializeObject<Supplier[]>(inputJson);

             context
                .Suppliers
                .AddRange(suplyJson);

            int saved = context.SaveChanges();

            return $"Successfully imported {saved}.";
        }

        //problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            var suppliersId = context
                .Suppliers
                .Select(s => s.Id)
                .ToList();

            for (int i = 0; i < parts.Length; i++)
            {
                if (suppliersId.Contains(parts[i].SupplierId))
                {
                    context.Parts.Add(parts[i]);
                }
            }

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}.";
        }

        //problem 11
        //public static string ImportCars(CarDealerContext context, string inputJson)
        //{ 
        //    var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

        //    var carsList = new List<Car>();
        //    var carParts = new List<PartCar>();

        //    foreach (var carDto in carsDto)
        //    { 
        //        var car = new Car()
        //        {
        //            Make = carDto.Make,
        //            Model = carDto.Model,
        //            TravelledDistance = carDto.TravelledDistance
        //        };

        //        foreach (var part in carDto.PartsId.Distinct())
        //        {
        //            var partCar = new PartCar()
        //            {
        //                PartId = part,
        //                Car = car
        //            };

        //            carParts.Add(partCar);
        //        }

        //        carsList.Add(car);
        //    }

        //    context.Cars.AddRange(carsList);

        //    context
        //        .PartCars
        //        .AddRange(carParts);

        //     context
        //        .SaveChanges();

        //    return $"Successfully imported {carsList.Count}.";
        //}


        //problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context
                .Customers
                .AddRange(customers);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}.";
        }

        //problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context
                .Sales
                .AddRange(sales);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}.";
        }

        //problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .ToList();

            var json = JsonConvert
                .SerializeObject(customers,Formatting.Indented);

            return json;
        }

        //problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .ToList();

            var json = JsonConvert
                .SerializeObject(cars,Formatting.Indented);

            return json;
        }

        //problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suplies = context
                .Suppliers
                .Where(c => c.IsImporter == false)
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    PartsCount = c.Parts.Count()
                })
                .ToList();

            var json = JsonConvert
                .SerializeObject(suplies, Formatting.Indented);

            return json;
        }

        //problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },

                    parts = c.PartCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = $"{pc.Part.Price:f2}"
                    })
                    .ToList()

                })
                .ToList();


            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        //problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        //problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var result = context
                .Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Models = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },

                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    priceWithDiscount = $@"{(s.Car.PartCars.Sum(p => p.Part.Price) -
                        s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100):F2}"
                })
                .ToList();

            var json = JsonConvert
                .SerializeObject(result, Formatting.Indented);

            return json;
        }
    }
}