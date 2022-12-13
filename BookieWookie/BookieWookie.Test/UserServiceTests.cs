namespace BookieWookie.Test
{
    using BookieWookie.API.Entities;
    using BookieWookie.API.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using NUnit.Framework.Internal;


    public class UserServiceTests
    {
        DbContextOptions<BookieWookieContext> options;
        BookieWookieContext context;
        API.Services.AuthenticationService authenticationService;
        API.Services.UserService userService;
        IConfiguration configuration;
        User bob;
        string bobPassword;

        [SetUp]
        public void Setup()
        {
            // setup dbcontext in memory for testing //
            this.options = new DbContextOptionsBuilder<BookieWookieContext>()
                .UseInMemoryDatabase(databaseName: "BookieWookie_Test")
                .Options;
            this.context = new BookieWookieContext(this.options);

            // setup authentication service //
            this.configuration = API.ConfigurationManager.AppSetting;
            this.authenticationService = new API.Services.AuthenticationService();
            this.userService = new API.Services.UserService(this.authenticationService, this.configuration, this.context);

            // standard user //
            var user = new CreateUserRequest()
            {
                FirstName = "Bob",
                Pseudonym = "Bob",
                Username = "bob",
                Password = Guid.NewGuid().ToString(),
            };

            this.bobPassword = user.Password;
            this.userService.CreateUser(user);
            this.bob = this.context.Users.Single(u => u.Username == user.Username);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var user in this.context.Users)
            {
                context.Users.Remove(user);
            }

            this.context.SaveChanges();
        }
    }
}
