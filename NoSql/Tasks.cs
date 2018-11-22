using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NoSql;

namespace MongoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "mongodb://localhost";

            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("Library");
            // standart MongoDb driver documents collection
            var bookCollection = db.GetCollection<Book>("Book");
            bookCollection.InsertMany(Books);
            // queryably collection for LINQ
            var queryableBookCollection = bookCollection.AsQueryable();

            // 2
            Console.WriteLine("2.Найдите книги с количеством экземпляров больше единицы. \r\n a.Покажите в результате только название книги." +
                              "\r\n b.Отсортируйте книги по названию. \r\n c.Ограничьте количество возвращаемых книг тремя. \r\n d.Подсчитайте количество таких книг.");
            var sortedBooks = bookCollection.Find(b => b.Count > 1).Project(book => book.Name)
                .Sort(new JsonSortDefinition<Book>("{Name: 1}")).Limit(3).ToList();
            foreach (var book in sortedBooks)
                Console.WriteLine(book);
            Console.WriteLine($"Sorted books count: {sortedBooks.Count}");

            // 3
            Console.WriteLine("\n\nНайдите книгу с макимальным/минимальным количеством (count).");
            var maxResult = queryableBookCollection.OrderByDescending(b => b.Count).First();
            var minResult = queryableBookCollection.OrderBy(b => b.Count).First();
            Console.WriteLine($"Максимальное кол-во: {maxResult}");
            Console.WriteLine($"Минимальное кол-во: {minResult}");

            // 4
            Console.WriteLine("\n\nНайдите список авторов (каждый автор должен быть в списке один раз).");
            var authors = queryableBookCollection.Where(b => b.Author != null).Select(b => b.Author).Distinct();
            foreach (var name in authors)
                Console.WriteLine(name);

            // 5
            Console.WriteLine("\n\nВыберите книги без авторов.");
            var unnamedBooks = queryableBookCollection.Where(b => b.Author == null).Distinct();
            foreach (var name in unnamedBooks)
                Console.WriteLine(name);

            // 6
            Console.WriteLine("\n\nУвеличьте количество экземпляров каждой книги на единицу.");
            bookCollection.UpdateManyAsync("{}", new JsonUpdateDefinition<Book>("{ $inc: { Count:1 } }"));
            Console.WriteLine(queryableBookCollection.First(b => b.Name == "Repka"));

            // 7
            Console.WriteLine("\n\nДобавьте дополнительный жанр “favority” всем книгам с жанром “fantasy”" +
                              " (последующие запуски запроса не должны дублировать жанр “favority”)");
            bookCollection.UpdateManyAsync(b => b.Genre.Contains("fantasy"),
                new JsonUpdateDefinition<Book>("{$addToSet: {Genre: \"favority\"} }"));
            Console.WriteLine(queryableBookCollection.First(b => b.Name == "Hobbit"));

            // 8
            Console.WriteLine("\n\nУдалите книги с количеством экземпляров меньше трех.");
            bookCollection.DeleteMany(book => book.Count < 3);
            // returns false if nothing found
            Console.WriteLine(queryableBookCollection.Any(b => b.Count < 3));

            // 9
            Console.WriteLine("\n\nУдалите все книги.");
            bookCollection.DeleteMany("{}");
            // returns false if nothing found
            Console.WriteLine(queryableBookCollection.Any());
            Console.ReadLine();
        }

        static void ShowResults(List<Book> books)
        {
            foreach (var book in books)
                Console.WriteLine(book);
        }

        static Book[] Books => new Book[]
        {
            new Book
            {
                Name = "Hobbit",
                Author = "Tolkien",
                Count = 5,
                Genre = new List<string> { "fantasy" },
                Year = 2014
            },
            new Book
            {
                Name = "Lord of the rings",
                Author = "Tolkien",
                Count = 3,
                Genre = new List<string> { "fantasy" },
                Year = 2015
            },
            new Book
            {
                Name = "Kolobok",
                Count = 10,
                Genre = new List<string> { "kids" },
                Year = 2000
            },
            new Book
            {
                Name = "Repka",
                Count = 11,
                Genre = new List<string> { "kids" },
                Year = 2000
            },
            new Book
            {
                Name = "Dyadya Stiopa",
                Author = "Mihalkov",
                Count = 1,
                Genre = new List<string> { "kids" },
                Year = 2001
            }
        };
    }
}