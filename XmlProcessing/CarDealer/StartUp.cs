using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Dtos.Import;
using CarDealer.Dtos.Export;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //var document = File.ReadAllText("./../../../Datasets/sales.xml");
                Console.WriteLine(GetSalesWithAppliedDiscount(db));
            }
        }

        //problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportSupliearDto[]),
                new XmlRootAttribute("Suppliers"));

            var deserializedSupliers = (ImportSupliearDto[])serializer.Deserialize(new StringReader(inputXml));

            List<Supplier> suppliers = new List<Supplier>();
            for (int i = 0; i < deserializedSupliers.Length; i++)
            {
                Supplier supplier = new Supplier()
                {
                    Name = deserializedSupliers[i].Name,
                    IsImporter = deserializedSupliers[i].IsImporter,
                };

                suppliers.Add(supplier);
            }

            context
                .Suppliers
                .AddRange(suppliers);

            int saved = context
                .SaveChanges();


            return $"Successfully imported {saved}";
        }

        //probloem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportPartDto[]),
                new XmlRootAttribute("Parts"));

            var deserializedParts = (ImportPartDto[])serializer.Deserialize(new StringReader(inputXml));

            List<Part> parts = new List<Part>();
            var suppliersIds = context
                .Suppliers
                .Select(s => s.Id)
                .ToList();
            for (int i = 0; i < deserializedParts.Length; i++)
            {
                Part part = new Part()
                {
                    Name = deserializedParts[i].Name,
                    Price = deserializedParts[i].Price,
                    Quantity = deserializedParts[i].Quantity,
                    SupplierId = deserializedParts[i].SupplierId,
                };

                if (suppliersIds.Contains(part.SupplierId))
                {
                    parts.Add(part);
                }
            }

            context
                .Parts
                .AddRange(parts);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}";
        }

        //problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportCarDto[]),
                new XmlRootAttribute("Cars"));

            var deserializedCars = (ImportCarDto[])serializer.Deserialize(new StringReader(inputXml));

            List<Car> cars = new List<Car>();

            List<PartCar> partCars = new List<PartCar>();

            var partsIds = context
                .Parts
                .Select(p => p.Id)
                .ToList();


            foreach (var car in deserializedCars)
            {
                var newCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                };

                cars.Add(newCar);
                var newPartIds = car.Parts
                    .Select(p => p.PartId)
                    .ToArray();

                foreach (var partId in newPartIds.Distinct())
                {
                    if (partsIds.Contains(partId))
                    {
                        var newPartCar = new PartCar()
                        {
                            Car = newCar,
                            PartId = partId,
                        };

                        partCars.Add(newPartCar);
                    }
                }
            }

            context
                .Cars
                .AddRange(cars);

            context
                .PartCars
                .AddRange(partCars);

            context
                .SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportCustomerDto[]),
                new XmlRootAttribute("Customers"));

            ImportCustomerDto[] deserializedCustomers = (ImportCustomerDto[])serializer
                .Deserialize(new StringReader(inputXml));

            Customer[] customers = new Customer[deserializedCustomers.Length];

            for (int i = 0; i < customers.Length; i++)
            {
                customers[i] = new Customer()
                {
                    Name = deserializedCustomers[i].Name,
                    BirthDate = deserializedCustomers[i].BirthDate,
                    IsYoungDriver = deserializedCustomers[i].IsYoungDriver,
                };
            }

            context
                .Customers
                .AddRange(customers);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}";
        }

        //problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportSaleDto[]),
                new XmlRootAttribute("Sales"));

            ImportSaleDto[] deserializedSales = (ImportSaleDto[])serializer.Deserialize(new StringReader(inputXml));

            List<int> carIds = context
                .Cars
                .Select(c => c.Id)
                .ToList();

            List<Sale> sales = new List<Sale>();

            foreach (var sale in deserializedSales)
            {
                if (carIds.Contains(sale.CarId))
                {
                    Sale newSale = new Sale()
                    {
                        CarId = sale.CarId,
                        CustomerId = sale.CustomerId,
                        Discount = sale.Discount,
                    };

                    sales.Add(newSale);
                }
            }

            context
                .Sales
                .AddRange(sales);

            int saved = context
                .SaveChanges();

            return $"Successfully imported {saved}";
        }

        //problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportCarDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, cars, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            ExportCarBmwDto[] cars = context
                .Cars
                .Where(c => c.Make.ToLower() == "bmw")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportCarBmwDto
                {
                     Id = c.Id,
                     Model = c.Model,
                     TravelledDistance = c.TravelledDistance,
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportCarBmwDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, cars, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportSupplierDto[] suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportSupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportSupplierDto[]),
                new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            StringBuilder sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, suppliers, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarWithPartDto[] carsWithParts = context
                .Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Select(c => new ExportCarWithPartDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                    .Select(pc => new ExportPartDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price,
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()

                })
                .Take(5)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportCarWithPartDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, carsWithParts, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportSaleByCustomerDto[] customers = context
                .Sales
                .Where(s => s.Customer != null)
                .Select(s => new ExportSaleByCustomerDto
                {
                    FullName = s.Customer.Name,
                    BoughtCars = s.Customer.Sales.Count,
                    SpentMoney = s.Car.PartCars.Sum(pc => pc.Part.Price),
                                //s.Car.PartCars.Sum(pc => pc.Part.Price
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportSaleByCustomerDto[]),
                new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            StringBuilder sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, customers, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new ExportSaleWithDiscountDto
                {
                    Car = new ExportCarWithAttributesDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) - s.Car.PartCars.Sum(pc => pc.Part.Price) * (s.Discount / 100),
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportSaleWithDiscountDto[]),
                new XmlRootAttribute("sales"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, sales, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

    }
}