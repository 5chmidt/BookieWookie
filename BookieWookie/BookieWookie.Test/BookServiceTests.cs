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
        IConfiguration configuration;

        API.Services.AuthenticationService authenticationService;
        API.Services.UserService userService;
        API.Services.BookService bookService;
        User anakin;
        User bob;


        [SetUp]
        public void Setup()
        {
            // setup dbcontext in memory for testing //
            this.options = new DbContextOptionsBuilder<BookieWookieContext>()
                .UseInMemoryDatabase(databaseName: "BookieWookie_Test")
                .Options;
            this.context = new BookieWookieContext(this.options);
            this.configuration = API.ConfigurationManager.AppSetting;
            
            // setup services and pass in the testing parameters //
            this.authenticationService = new API.Services.AuthenticationService();
            this.userService = new API.Services.UserService(
                this.authenticationService,
                this.configuration,
                this.context);
            this.bookService = new API.Services.BookService(this.context);

            // standard user //
            var userRequest = new CreateUserRequest()
            {
                FirstName = "Bob",
                Pseudonym = "Bob",
                Username = "bob",
                Password = Guid.NewGuid().ToString(),
            };
            this.userService.CreateUser(userRequest);
            this.bob = this.userService.GetById(1);

            // user that cannot publish //
            userRequest = new CreateUserRequest()
            {
                FirstName = "Anakin",
                LastName = "Skywalker",
                Pseudonym = "Darth_Vader",
                Username = "DarthVader",
                Password = Guid.NewGuid().ToString(),
            };
            this.userService.CreateUser(userRequest);
            this.anakin = this.userService.GetById(2);
        }

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

        [TearDown]
        public void TearDown()
        {

        }
    }
}
