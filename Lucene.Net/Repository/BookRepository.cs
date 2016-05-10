using Lucene.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Net.Repository
{
    public class BookRepository
    {
        private ICollection<Book> _books;

        public BookRepository()
        {
            this._books = new List<Book>();
            _books.Add(new Book

            {
                Title = "Essential C# 6.0, 1st Edition",
                Summary = "Essential C# 6.0 is a well-organized...",
                Isbn = "9780134141046",
                PageNum = 1008
            });
            _books.Add(new Book

            {
                Title = "C# 5.0 in a Nutshell: The Definitive Reference",
                Summary = "When you have a question about C# 5.0...",
                Isbn = "9781449320102",
                PageNum = 1064
            });
            _books.Add(new Book

            {
                Title = "C# in Depth",
                Summary = "C# in Depth, Third Edition updates...",
                Isbn = "9781617291340",
                PageNum = 616
            });
            _books.Add(new Book

            {
                Title = "The Art of Unit Testing: with examples...",
                Isbn = "9781617290893",
                Summary = "Teste",
                PageNum = 296
            });
            _books.Add(new Book

            {
                Title = "Adaptive Code via C#: Agile coding with design patterns and SOLID principles (Developer Reference)",
                Summary = "As every developer knows, requirements...",
                Isbn = "9780735683204",
                PageNum = 448
            });
        }

        public Book Get(string Isbn)
        {
            return _books.FirstOrDefault((x) => x.Isbn.Equals(Isbn));
        }

        public ICollection<Book> GetAll()
        {
            return _books;
        }
    }
}
