namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movies = JsonConvert.DeserializeObject<Movie[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            List<string> validMovieTitles = new List<string>();
            List<Movie> validMovies = new List<Movie>();
            List<Genre> genres = new List<Genre>();
            foreach (Genre genre in (Genre[])Enum.GetValues(typeof(Genre)))
            {
                genres.Add(genre);
            }

            for (int i = 0; i < movies.Length; i++)
            {
                if (!(movies[i].Rating >= 1 && movies[i].Rating <= 10))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!(movies[i].Title.Length >= 3 && movies[i].Title.Length <= 20))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!genres.Contains(movies[i].Genre))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!(movies[i].Director.Length >= 3 && movies[i].Director.Length <= 20))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (validMovieTitles.Contains(movies[i].Title))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }
                validMovieTitles.Add(movies[i].Title);
                validMovies.Add(movies[i]);

                sb.AppendLine($"Successfully imported {movies[i].Title} with genre {movies[i].Genre} and rating {movies[i].Rating:f2}!");
            }
            context
                .Movies
                .AddRange(validMovies);

            context.SaveChanges();


            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsSeats = JsonConvert.DeserializeObject <ImportHallSeatDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hallsSeats.Length; i++)
            {
                if (!(hallsSeats[i].Name.Length >= 3 && hallsSeats[i].Name.Length <= 20))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (hallsSeats[i].Seats <= 0)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var hall = new Hall()
                {
                    Name = hallsSeats[i].Name,
                    Is4Dx = hallsSeats[i].Is4Dx,
                    Is3D = hallsSeats[i].Is3D
                };

                context.Halls.Add(hall);

                List<Seat> seats = new List<Seat>();

                for (int j = 0; j < hallsSeats[i].Seats; j++)
                {
                    var seat = new Seat()
                    {
                        Hall = hall,
                    };

                    seats.Add(seat);
                }

                context
                    .Seats.AddRange(seats);

                string projectionType;

                if (hall.Is3D == true)
                {
                    projectionType = "3D";
                }
                else if (hall.Is4Dx == true)
                {
                    projectionType = "4Dx";
                }
                else
                {
                    projectionType = "Normal";
                }

                sb.AppendLine($"Successfully imported {hall.Name}({projectionType}) with {seats.Count} seats!");
            }

            context
                .SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportXmlProjectionDto[]),
                new XmlRootAttribute("Projections")
                );

            var deserializedObjects = (ImportXmlProjectionDto[])serializer.Deserialize(new StringReader(xmlString));

            List<int> movieIds = context
                .Movies
                .Select(m => m.Id)
                .ToList();

            List<int> hallIds = context
                .Halls
                .Select(h => h.Id)
                .ToList();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < deserializedObjects.Length; i++)
            {
                if (!movieIds.Contains(deserializedObjects[i].MovieId))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!hallIds.Contains(deserializedObjects[i].HallId))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var projection = new Projection()
                {
                    HallId = deserializedObjects[i].HallId,
                    MovieId = deserializedObjects[i].MovieId,
                    DateTime = DateTime.Parse(deserializedObjects[i].DateTime)
                };

                context
                    .Projections
                    .Add(projection);

                string importedMovieName = context
                    .Movies
                    .Where(m => m.Id == projection.MovieId)
                    .Select(m => m.Title)
                    .First();

                sb.AppendLine($"Successfully imported projection {importedMovieName} on {projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}!");
            }

            context
                .SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportCustomerDto[]),
                new XmlRootAttribute("Customers")
                );

            var deserializedObjects = (ImportCustomerDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            List<int> projections = context
                .Projections
                .Select(p => p.Id)
                .ToList();

            for (int i = 0; i < deserializedObjects.Length; i++)
            {
                if (!(deserializedObjects[i].FirstName.Length >= 3 &&
                    deserializedObjects[i].FirstName.Length <= 20))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!(deserializedObjects[i].LastName.Length >= 3 &&
                    deserializedObjects[i].LastName.Length <= 20))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!(deserializedObjects[i].Age >= 12 && deserializedObjects[i].Age <= 110))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                if (!(deserializedObjects[i].Balance >= 0.01m && deserializedObjects[i].Balance <= decimal.MaxValue))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var customer = new Customer()
                {
                    FirstName = deserializedObjects[i].FirstName,
                    LastName = deserializedObjects[i].LastName,
                    Age = deserializedObjects[i].Age,
                    Balance = deserializedObjects[i].Balance
                };

                List<Ticket> tickets = new List<Ticket>();

                for (int j = 0; j < deserializedObjects[i].Tickets.Length; j++)
                {
                    if (!(deserializedObjects[i].Tickets[j].Price >= 0.01m &&
                        deserializedObjects[i].Tickets[j].Price <= decimal.MaxValue))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    if (!projections.Contains(deserializedObjects[i].Tickets[j].ProjectionId))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    var ticket = new Ticket()
                    {
                        Customer = customer,
                        ProjectionId = deserializedObjects[i].Tickets[j].ProjectionId,
                        Price = deserializedObjects[i].Tickets[j].Price,
                    };

                    tickets.Add(ticket);
                }

                context
                    .Customers
                    .Add(customer);

                context
                    .Tickets
                    .AddRange(tickets);

                sb.AppendLine($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {tickets.Count}!");

            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
    }
}