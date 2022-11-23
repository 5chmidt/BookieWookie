namespace BookieWookie.Test
{
    using BookieWookie.API.Entities;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;

    public class FileServiceTests
    {
        DbContextOptions<BookieWookieContext> options;
        BookieWookieContext context;
        Mock<IWebHostEnvironment> webHostEnvironment;
        API.Services.FileService fileService;
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

            // mock web hosting environment //
            this.webHostEnvironment = new Mock<IWebHostEnvironment>();
            this.webHostEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns(TestContext.CurrentContext.TestDirectory);
            
            // setup file service //
            this.fileService = new API.Services.FileService(this.webHostEnvironment.Object, this.context);

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
        /// Upload a file from resources and check that it does into the correct loction.
        /// </summary>
        [Test]
        public void UploadImageTest ()
        {
            var uploadFolder = new DirectoryInfo(
                Path.Join(this.webHostEnvironment.Object.ContentRootPath, "uploads"));
            int fileCount = uploadFolder.GetFiles().Length;

            int recordCount = this.context.Files.Count();
            Stream stream = new MemoryStream(Resources.wookie_image_1);
            IFormFile file = new FormFile(stream, 0, Resources.wookie_image_1.LongLength, "WookieImage.jpg", "WookieImage.jpg");
            var task = this.fileService.Create(file, this.bob.UserId);
            task.Wait();

            // check that record was added //
            Assert.That(recordCount + 1, Is.EqualTo(this.context.Files.Count()));
            
            // check that file was added //
            Assert.That(fileCount + 1, Is.EqualTo(uploadFolder.GetFiles().Length));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
