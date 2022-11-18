
namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class BookieWookieContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public BookieWookieContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<File> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

    }
}