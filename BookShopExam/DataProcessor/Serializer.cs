namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var result = context
                .Authors
                .Select(a => new ExportAuthorDto
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                    .OrderByDescending(ab => ab.Book.Price)
                    .Select(ab => new ExportBookDto
                    {
                        BookName = ab.Book.Name,
                        BookPrice = $"{ab.Book.Price:f2}"
                    })
                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(a => a.Books.Length)
                .ThenBy(a => a.AuthorName)
                .ToArray();

            var json = JsonConvert
                .SerializeObject(result, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var result = context
                .Books
                .Where(b => b.PublishedOn < date)
                .Where(b => b.Genre.ToString() == "Science")
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Take(10)
                .Select(b => new ExportBookInfoDto
                {
                    Pages = b.Pages,
                    Date = b.PublishedOn.ToString("d",CultureInfo.InvariantCulture),
                    Name = b.Name
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(
                typeof(ExportBookInfoDto[]),
                new XmlRootAttribute("Books"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            StringBuilder sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, result, namespaces);
            }

            return sb.ToString().Trim();
        }
    }
}