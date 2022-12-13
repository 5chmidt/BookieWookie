using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieWookie.Test
{
    using BookieWookie.API.Controllers;
    using BookieWookie.API.Entities;
    using BookieWookie.API.Models;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    public class BookControllerTests
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
    }
}
