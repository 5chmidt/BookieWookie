using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookieWookie.API.Services
{
    public interface IBookService
    {
        Task<Book> Create(CreateBookRequest request, int userId);
        Book Update();
        Task<Book> Delete(int bookId, int userId);
        IEnumerable<Book> Get();
    }


    public class BookService : IBookService

    {
        private readonly IConfiguration Configuration;

        public BookService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task<Book> Create(CreateBookRequest request, int userId)
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
                if (await db.Books.Where(b => b.Title == request.Title).AnyAsync())
                {
                    string msg = $"Books cannot share titles, '{request.Title}' already exists.";
                    throw new InvalidDataException(msg);
                }

                db.Books.Add(book);
                await db.SaveChangesAsync();
            }

            return book;
        }

        public async Task<Book> Delete(int bookId, int userId)
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                Book book = await db.Books.SingleAsync(b => b.Id == bookId);
                if (book.AuthorId != userId)
                {
                    throw new UnauthorizedAccessException($"Authors can only remove their own books.");
                }

                db.Books.Remove(book);
                await db.SaveChangesAsync();
                return book;
            }
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
