using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Reflection;

namespace BookieWookie.API.Services
{
    /// <summary>
    /// Impliments CRUD methods for book model.
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Create a book, users can only publish their own work.
        /// </summary>
        /// <param name="request"><see cref="CreateBookRequest"/></param>
        /// <param name="userId">Unique identifier of current user.</param>
        /// <returns>Newly created <see cref="Book"/>.</returns>
        Task<Book> Create(CreateBookRequest request, int userId);
        
        /// <summary>
        /// Updates a book and returns the revised model.
        /// </summary>
        /// <param name="book"><see cref="Book"/>.</param>
        /// <param name="userId">Unique identifier of current user.</param>
        /// <returns>Updated book. <see cref="Book"/></returns>
        Task<Book> Update(Book book, int userId);

        /// <summary>
        /// Deletes a book from the database (unpublish).
        /// (Users can only delete books they published.)
        /// </summary>
        /// <param name="bookId">Unique identifier of the book to be removed.</param>
        /// <param name="userId">Unique identifier of the current user.</param>
        /// <returns>Deleted object.  <see cref="Book"/></returns>
        Task<Book> Delete(int bookId, int userId);

        /// <summary>
        /// Get books using parameterized query class.
        /// Any null parameters will not be used to filter the returned set.
        /// </summary>
        /// <param name="bookParams"><see cref="BookParameters"/></param>
        /// <returns>Collection of books based on the input filters.</returns>
        Task<IEnumerable<Book>> Get(BookParameters bookParams);
    }

    /// <inheritdoc/>
    public class BookService : IBookService
    {
        private readonly IConfiguration configuration;
        
        private readonly BookieWookieContext context;

        /// <inheritdoc/>
        public BookService(IConfiguration _configuration, BookieWookieContext _context)
        {
            this.configuration = _configuration;
            this.context = _context;
        }
        
        /// <inheritdoc/>
        public async Task<Book> Create(CreateBookRequest request, int userId)
        {
            var book = new Book()
            {
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.Now,
                UserId = userId,
            };

            if (await this.context.Books.Where(b => b.Title == request.Title).AnyAsync())
            {
                string msg = $"Books cannot share titles, '{request.Title}' already exists.";
                throw new InvalidDataException(msg);
            }

            this.context.Books.Add(book);
            await this.context.SaveChangesAsync();
            return book;
        }
        
        /// <inheritdoc/>
        public async Task<Book> Delete(int bookId, int userId)
        {
            Book book = await this.context.Books.SingleAsync(b => b.BookId == bookId);
            if (book.UserId != userId)
            {
                throw new UnauthorizedAccessException($"Authors can only remove their own books.");
            }

            this.context.Books.Remove(book);
            await this.context.SaveChangesAsync();
            return book;
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Book>> Get(BookParameters bookParams)
        {
            var books = this.context.Books.AsQueryable();
            foreach (PropertyInfo property in bookParams.GetType().GetRuntimeProperties())
            {
                object? value = property.GetValue(bookParams, null);
                if (value == null)
                {
                    continue;
                }

                // query mapping //
                if (property.Name == nameof(BookParameters.BookId))
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

            return await books.ToArrayAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Book> Update(Book book, int userId)
        {
            if (book.UserId != userId)
            {
                throw new UnauthorizedAccessException($"Authors can only remove their own books.");
            }

            this.context.Entry<Book>(book).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
            return book;
        }
    }
}
