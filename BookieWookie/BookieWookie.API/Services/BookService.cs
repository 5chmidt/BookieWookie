using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;

namespace BookieWookie.API.Services
{
    public interface IBookService
    {
        Book Create(CreateBookRequest request, int userId);
        Book Update();
        Book Delete();
        IEnumerable<Book> Get();
    }


    public class BookService : IBookService

    {
        private readonly IConfiguration Configuration;

        public BookService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public Book Create(CreateBookRequest request, int userId)
        {
            var book = new Book()
            {
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.Now,
                AuthorId = userId,
            };

            using (var db = new WookieBookieContext(this.Configuration))
            {
                if (db.Books.Where(b => b.Title == request.Title).Any())
                {
                    string msg = $"Books cannot share titles, '{request.Title}' already exists.";
                    throw new InvalidDataException(msg);
                }

                db.Books.Add(book);
                db.SaveChanges();
            }

            return book;
        }

        public Book Delete()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> Get()
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                return db.Books.ToArray();
            }
        }

        public Book Update()
        {
            throw new NotImplementedException();
        }
    }
}
