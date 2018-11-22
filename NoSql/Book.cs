using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;


namespace NoSql
{
    public class Book
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Count { get; set; }
        public List<string> Genre { get; set; }
        public int Year { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} " +
                   $"\n Author: {Author} " +
                   $"\n Count: {Count} " +
                   $"\n Year: {Year} " +
                   $"\n Genre: {Genre.Aggregate((prev, current) => prev + ", " + current)}";
        }
    }
}
