namespace BookShop
{
    using BookShop.Models;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            //using (var db = new BookShopContext())
            //{
            //    DbInitializer.ResetDatabase(db);
            //}
            //int input = int.Parse(Console.ReadLine());

            Console.WriteLine(RemoveBooks(new BookShopContext()));

        }

        //problem 01
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var result = context
                .Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in result)
            {
                sb.AppendLine(title.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 02
        public static string GetGoldenBooks(BookShopContext context)
        {
            var result = context
                .Books
                .Where(b => b.EditionType.ToString().ToLower() == "gold")
                .Where(b => b.Copies < 5000)
                .Select(b => new
                {
                    b.Title
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in result)
            {
                sb.AppendLine(title.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 03
        public static string GetBooksByPrice(BookShopContext context)
        {
            var result = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in result)
            {
                sb.AppendLine($"{title.Title} - ${title.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //problem 04
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var result = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var title in result)
            {
                sb.AppendLine(title.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 05
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<Book> books = new List<Book>();

            foreach (var category in categories)
            {
                var result = context
                    .Books
                    .Where(b => b.BookCategories.Select(bc => new
                    {
                        Name = bc.Category.Name
                    }).Any(ca => ca.Name.ToLower() == category.ToLower()))
                    .ToList();

                books.AddRange(result);
            }

            books = books.OrderBy(b => b.Title).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 06

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime compareDate = DateTime.ParseExact(date, "dd-MM-yyyy",CultureInfo.InvariantCulture);

            var result = context
                .Books
                .Where(b => b.ReleaseDate.Value < compareDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate.Value)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in result)
            {
                sb.AppendLine($"{title.Title} - {title.EditionType} - ${title.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //problem 07
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var result = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + ' ' + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in result)
            {
                sb.AppendLine(a.FullName);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 08
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var result = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in result)
            {
                sb.AppendLine(a);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var result = context
                .Books
                .Select(b => new
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .Where(a => a.AuthorLastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in result)
            {
                sb.AppendLine($"{b.Title} ({b.AuthorFirstName} {b.AuthorLastName})");
            }
            return sb.ToString().TrimEnd();
        }

        //problem 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var result =
               context
               .Books
               .Where(b => b.Title.Length > lengthCheck)
               .Select(b => b.Title)
               .ToList();

            return result.Count();
        }

        //problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var result = context
                .Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    Sum = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Sum)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var author in result)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.Sum}");
            }

            return sb.ToString().TrimEnd();
        }

        //problem 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var result = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(b => b.Profit)
                .ThenBy(b => b.Category)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var c in result)
            {
                sb.AppendLine($"{c.Category} ${c.Profit:f2}");
            }


            return sb.ToString().TrimEnd();
        }

        //problem 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var result = context
            .Categories
            .Select(c => new
            {
                Category = c.Name,
                RecentBooks = c.CategoryBooks.Select(cb => new
                {
                    cb.Book.Title,
                    cb.Book.ReleaseDate
                }).OrderByDescending(b => b.ReleaseDate).Take(3).ToList()
            })
            .OrderBy(c => c.Category)
            .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in result)
            {
                sb.AppendLine($"--{b.Category}");

                foreach (var bb in b.RecentBooks)
                {
                    sb.AppendLine($"{bb.Title} ({bb.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //problem 14
        public static void IncreasePrices(BookShopContext context)
        {
            var result = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var b in result)
            {
                b.Price += 5;
                context.Update(b);
            }

            context.SaveChanges();
        }

        //problem 15
        public static int RemoveBooks(BookShopContext context)
        {
            var result = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.RemoveRange(result);

            context.SaveChanges();

            return result.Count;
        }
    }
}
