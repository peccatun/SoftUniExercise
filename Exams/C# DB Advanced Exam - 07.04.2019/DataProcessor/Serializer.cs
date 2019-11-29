namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var result = context
                .Projections
                .Where(p => p.Movie.Rating >= rating)
                .Where(p => p.Tickets.Count > 0)
                .Select(p => new ExportMovieDto
                {
                    MovieName = p.Movie.Title,
                    Rating = $"{p.Movie.Rating:f2}",
                    TotalIncomes = $"{p.Tickets.Sum(t => t.Price)}",
                    Customers = p.Tickets.Select(t => new ExportCustomerDto
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = $"{t.Customer.Balance:f2}"
                    })
                    .OrderByDescending(c => c.Balance)
                    .ThenBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .Distinct()
                    .ToArray()
                })
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.TotalIncomes)
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return json.ToString(); ;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var result = context
                .Customers
                .Where(c => c.Age >= age)
                .Select(c => new ExportAllInfoXml
                {

                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = $"{c.Tickets.Sum(t => t.Price):f2}",
                    SpentTime = new TimeSpan(c.Tickets.Sum(t => t.Projection.Movie.Duration.Ticks)).ToString(@"hh\:mm\:ss"),
                    OrderPrice = c.Tickets.Sum(t => t.Price)
                })
                .Take(10)
                .OrderByDescending(c => c.OrderPrice)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportAllInfoXml[]),
                new XmlRootAttribute("Customers")
                );

            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, result, namespaces);
            }

            return sb.ToString().TrimEnd();
        
        }

    }
}