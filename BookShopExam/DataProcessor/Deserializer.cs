namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(ImportBookDto[]),
                new XmlRootAttribute("Books"));

            var dtos = (ImportBookDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            List<int> validGenres = new List<int>() { 1, 2, 3 };
            List<Book> validBooks = new List<Book>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validGenres.Contains(dto.Genre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(dto.PublishedOn))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Book book = new Book()
                {
                    Name = dto.Name,
                    Genre = (Genre)dto.Genre,
                    Pages = dto.Pages,
                    Price = dto.Price,
                    PublishedOn = DateTime.ParseExact(dto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                };

                validBooks.Add(book);
                sb.AppendLine($"Successfully imported book {book.Name} for {book.Price:f2}.");
            }

            context
                .Books
                .AddRange(validBooks);

            context
                .SaveChanges();



            return sb.ToString().Trim();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorsDtos = JsonConvert.DeserializeObject<ImportAuthorDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            List<string> emails = new List<string>();
            var bookIds = context
                .Books
                .Select(b => b.Id)
                .ToList();

            List<Author> validAuthors = new List<Author>();


            foreach (var dto in authorsDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (emails.Contains(dto.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Author author = new Author()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone
                };

                List<AuthorBook> validAuthorsBooks = new List<AuthorBook>();
                foreach (var book in dto.Books)
                {
                    if (book.Id == null)
                    {
                        continue;
                    }

                    int bookId = (int)book.Id;

                    if (!bookIds.Contains(bookId))
                    {
                        continue;
                    }

                    AuthorBook authorBook = new AuthorBook()
                    {
                        Author = author,
                        BookId = bookId
                    };

                    validAuthorsBooks.Add(authorBook);
                }

                if (validAuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //ToChekIfEmailAlreadyExists
                emails.Add(author.Email);

                //AddingTheValidAuthor
                validAuthors.Add(author);

                //AddingInDbAuthorBook
                context
                    .AuthorsBooks
                    .AddRange(validAuthorsBooks);

                sb.AppendLine($"Successfully imported author - {author.FirstName} {author.LastName} with {validAuthorsBooks.Count} books.");
            }

            context
                .Authors
                .AddRange(validAuthors);

            context
                .SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}