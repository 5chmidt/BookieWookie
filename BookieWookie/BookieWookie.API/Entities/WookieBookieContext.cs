
namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class WookieBookieContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public WookieBookieContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        public WookieBookieContext(DbContextOptions<WookieBookieContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

    }
}