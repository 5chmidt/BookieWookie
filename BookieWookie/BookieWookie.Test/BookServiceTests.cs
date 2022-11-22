namespace BookieWookie.Test
{
    using BookieWookie.API.Entities;
    using BookieWookie.API.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    public class BookServiceTests
    {
        DbContextOptions<BookieWookieContext> options;
        BookieWookieContext context;
        API.Services.BookService bookService;
        User alice;
        User bob;

        [SetUp]
        public void Setup()
        {
            // setup dbcontext in memory for testing //
            this.options = new DbContextOptionsBuilder<BookieWookieContext>()
                .UseInMemoryDatabase(databaseName: "BookieWookie_Test")
                .Options;
            this.context = new BookieWookieContext(this.options);
            
            // setup services and pass in the testing parameters //
            this.bookService = new API.Services.BookService(this.context);

            // standard user (skip salting and hashing for test speed)//
            var user = new User()
            {
                FirstName = "Bob",
                Pseudonym = "Bob",
                Username = "bob",
                Hash = new byte[0],
                Salt = new byte[0],
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();
            this.bob = this.context.Users.Single(u => u.Username == user.Username);

            // user that cannot publish (skip salting and hashing for test speed)//
            user = new User()
            {
                FirstName = "Anakin",
                LastName = "Skywalker",
                Pseudonym = "Darth_Vader",
                Username = "DarthVader",
                Hash = new byte[0],
                Salt = new byte[0],
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();
            this.alice = this.context.Users.Single(u => u.Username == user.Username);
        }

        /// <summary>
        /// Create a book as user bob.
        /// </summary>
        [Test]
        public void CreateBookTest()
        {
            int bookCount = this.context.Books.Count();
            var createRequest = new CreateBookRequest()
            {
                Title = "Bob's Guide to the Galaxy",
                Description = "Comprehensive guide based on one gallatic travelers experience.",
            };
            var task = this.bookService.Create(createRequest, this.bob.UserId);
            task.Wait();
            Assert.That(this.context.Books.Count(), Is.EqualTo(bookCount + 1));
        }

        /// <summary>
        /// Update book record as author.
        /// </summary>
        [Test]
        public void UpdateOwnBookTest()
        {
            var createRequest = new CreateBookRequest()
            {
                Title = "Update Book Test",
                Description = "Update book test.",
            };
            var task = this.bookService.Create(createRequest, this.bob.UserId);
            task.Wait();

            var book = task.Result;
            string id = Guid.NewGuid().ToString();
            book.Description = id;
            book.Title = id;
            task = this.bookService.Update(book, this.bob.UserId);
            task.Wait();

            int count = this.context.Books
                .Where(b => b.Title == id && b.Description == id)
                .Count();
            Assert.That(count, Is.EqualTo(1));
        }

        /// <summary>
        /// Try to update record with non-author user.
        /// </summary>
        [Test]
        public void UpdateOthersBookTest()
        {
            var createRequest = new CreateBookRequest()
            {
                Title = "Update Book Test",
                Description = "Update book test.",
            };
            var task = this.bookService.Create(createRequest, this.bob.UserId);
            task.Wait();

            var book = task.Result;
            string id = Guid.NewGuid().ToString();
            book.Description = id;
            book.Title = id;
            try
            {
                task = this.bookService.Update(book, this.alice.UserId);
                task.Wait();
            }
            catch(AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    if (exception.GetType() != typeof(UnauthorizedAccessException))
                    {
                        throw exception;
                    }
                }
            }

            int count = this.context.Books
                .Where(b => b.Title == id && b.Description == id)
                .Count();
            Assert.That(count, Is.EqualTo(0));
        }

        /// <summary>
        /// Delete book record.
        /// </summary>
        [Test]
        public void DeleteOwnBookTest()
        {
            var book = new Book()
            {
                CreatedAt = DateTime.Now,
                Title = Guid.NewGuid().ToString(),
                UserId = this.bob.UserId,
            };

            this.context.Books.Add(book);
            this.context.SaveChanges();
            int bookCount = this.context.Books.Count();
            var task = this.bookService.Delete(book.BookId, this.bob.UserId);
            task.Wait();
            Assert.That(this.context.Books.Count(), Is.EqualTo(bookCount - 1));
        }

        /// <summary>
        /// Try to delete a book from another user.
        /// </summary>
        [Test]
        public void DeleteOthersBookTest()
        {
            var book = new Book()
            {
                CreatedAt = DateTime.Now,
                Title = Guid.NewGuid().ToString(),
                UserId = this.bob.UserId,
            };

            this.context.Books.Add(book);
            this.context.SaveChanges();
            int bookCount = this.context.Books.Count();
            try
            {
                var task = this.bookService.Delete(book.BookId, this.alice.UserId);
                task.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    if (exception.GetType() != typeof(UnauthorizedAccessException))
                    {
                        throw exception;
                    }
                }
            }

            Assert.That(this.context.Books.Count(), Is.EqualTo(bookCount));
        }

        /// <summary>
        /// Get all books.
        /// </summary>
        [Test]
        public void GetAllBooksTest()
        {
            int addBooks = 5;
            for (int i = 0; i < addBooks; i++)
            {
                this.context.Books.Add(new Book()
                {
                    Title = Guid.NewGuid().ToString(),
                    UserId = this.bob.UserId,
                    CreatedAt = DateTime.Now,
                });
            }

            this.context.SaveChanges();
            var emptyParams = new BookParameters();
            int bookCount = this.bookService.Get(emptyParams).Result.Count();
            Assert.That(bookCount, Is.EqualTo(this.context.Books.Count()));
        }

        /// <summary>
        /// Get specific book by using parameter filer.
        /// </summary>
        [Test]
        public void GetBookByTitleTest()
        {
            int addBooks = 5;
            string id = string.Empty;
            for (int i = 0; i < addBooks; i++)
            {
                id = Guid.NewGuid().ToString();
                this.context.Books.Add(new Book()
                {
                    Title = id,
                    UserId = this.bob.UserId,
                    CreatedAt = DateTime.Now,
                });
            }

            this.context.SaveChanges();
            var filterParameters = new BookParameters()
            {
                TitleEquals = id,
            };
            int bookCount = this.bookService.Get(filterParameters).Result.Count();
            Assert.That(bookCount, Is.EqualTo(1));
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Remove(this.bob);
            this.context.Remove(this.alice);
            this.context.SaveChanges();
        }
    }
}
