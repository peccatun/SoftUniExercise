using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Dtos.Import;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                var document = File.ReadAllText("./../../../Datasets/cars.xml");
                Console.WriteLine(ImportCars(db,document));
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

        //problem11
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
            int carId = 0;
            foreach (var car in deserializedCars)
            {
                carId++;
                var newCar = new Car()
                {
                    //Id = carId,
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
                            //CarId = newCar.Id,
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
    }
}