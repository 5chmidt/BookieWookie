namespace BookieWookie.Test
{
    using BookieWookie.API.Entities;
    using BookieWookie.API.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using NUnit.Framework.Internal;


    public class UserControllerTests
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

            // standard user (skip salting and hashing for test speed)//
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

        [Test]
        public void AuthenticationLoginTest()
        {
            // test success case //
            var controller = new API.Controllers.UserController(this.userService, this.configuration);
            var result = controller.Authenticate(new AuthenticateRequest()
            {
                Username = this.bob.Username,
                Password = this.bobPassword,
            });

            result.Wait();
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(OkObjectResult)));

            // test for failure //
            result = controller.Authenticate(new AuthenticateRequest()
            {
                Username = this.bob.Username,
                Password = Guid.NewGuid().ToString(),
            });

            result.Wait(); 
            Assert.That(result.Result.GetType(), Is.Not.EqualTo(typeof(OkObjectResult)));

        }
        
        [Test]
        public void CreateUserTest()
        {
            var controller = new API.Controllers.UserController(this.userService, this.configuration);
            var createUser = new CreateUserRequest()
            {
                FirstName = "Alice",
                Username = "alice",
                Password = Guid.NewGuid().ToString(),
            };

            var alice = controller.Create(createUser);
            int aliceCount = this.context.Users.Where(u => u.Username == createUser.Username).Count();

            // check that user was created and okay was returned //
            Assert.That(aliceCount, Is.EqualTo(1));
            Assert.That(alice.GetType(), Is.EqualTo(typeof(OkObjectResult)));
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
