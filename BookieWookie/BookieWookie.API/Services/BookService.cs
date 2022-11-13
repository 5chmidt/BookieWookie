using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Reflection;

namespace BookieWookie.API.Services
{
    public interface IBookService
    {
        Task<Book> Create(CreateBookRequest request, int userId);
        Task<Book> Update(Book book, int userId);
        Task<Book> Delete(int bookId, int userId);
        Task<IEnumerable<Book>> Get(BookParameters bookParams);
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
                UserId = userId,
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
                Book book = await db.Books.SingleAsync(b => b.BookId == bookId);
                if (book.UserId != userId)
                {
                    throw new UnauthorizedAccessException($"Authors can only remove their own books.");
                }

                db.Books.Remove(book);
                await db.SaveChangesAsync();
                return book;
            }
        }

        public async Task<IEnumerable<Book>> Get(BookParameters bookParams)
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                var books = db.Books.AsEnumerable();
                foreach (PropertyInfo property in bookParams.GetType().GetRuntimeProperties())
                {
                    object? value = property.GetValue(bookParams, null);
                    if (value == null)
                    {
                        continue;
                    }

                    // query mapping //
                    if (property.Name == nameof(BookParameters.Id))
                    {
                        books = books.Where(b => b.BookId == (int)value);
                    }
                    else if(property.Name == nameof(BookParameters.AuthorId))
                    {
                        books = books.Where(b => b.UserId == (int)value);
                    }
                    else if (property.Name == nameof(BookParameters.TitleContains))
                    {
                        books = books.Where(b => b.Title.Contains((string)value));
                    }
                    else if(property.Name == nameof(BookParameters.TitleEquals))
                    {
                        books = books.Where(b => b.Title == (string)value);
                    }
                    else if (property.Name == nameof(BookParameters.Description))
                    {
                        books = books.Where(b => b.Description.Contains((string)value));
                    }
                    else if (property.Name == nameof(BookParameters.AuthorFirstName))
                    {
                        books = books.Where(b => b.User.FirstName == (string)value);
                    }
                    else if (property.Name == nameof(BookParameters.AuthorLastName))
                    {
                        books = books.Where(b => b.User.LastName == (string)value);
                    }
                    else if (property.Name == nameof(BookParameters.AuthorPseudonym))
                    {
                        books = books.Where(b => b.User.Pseudonym == (string)value);
                    }
                    else if (property.Name == nameof(BookParameters.CreateBefore))
                    {
                        books = books.Where(b => b.CreatedAt <= (DateTime)value);
                    }
                    else if (property.Name == nameof(BookParameters.CreatedAfter))
                    {
                        books = books.Where(b => b.CreatedAt >= (DateTime)value);
                    }
                    else if (property.Name == nameof(BookParameters.CreatedOn))
                    {
                        books = books.Where(b => b.CreatedAt.Date == (DateTime)value);
                    }
                }

                return await db.Books.ToArrayAsync();
            }
        }

        public async Task<Book> Update(Book book, int userId)
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                if (book.UserId != userId)
                {
                    throw new UnauthorizedAccessException($"Authors can only remove their own books.");
                }

                db.Entry<Book>(book).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return book;
            }
        }
    }
}
