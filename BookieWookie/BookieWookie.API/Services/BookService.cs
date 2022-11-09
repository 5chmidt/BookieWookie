using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;

namespace BookieWookie.API.Services
{
    public interface IBookService
    {
        Entities.Book Create(CreateBookRequest request);
        Entities.Book Update();
        Entities.Book Delete();
        Entities.Book Get();
    }


    public class BookService : IBookService

    {
        private readonly IConfiguration Configuration;

        public BookService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        [AuthorizeOwner]
        public Book Create(CreateBookRequest request)
        {
            var book = new Book()
            {
                Title = request.Title,
                Description = request.Description,
            };

            using (var db = new WookieBookieContext(this.Configuration))
            {
                db.Books.Add(book);
                db.SaveChanges();
            }

            return book;
        }

        public Book Delete()
        {
            throw new NotImplementedException();
        }

        public Book Get()
        {
            throw new NotImplementedException();
        }

        public Book Update()
        {
            throw new NotImplementedException();
        }
    }
}
