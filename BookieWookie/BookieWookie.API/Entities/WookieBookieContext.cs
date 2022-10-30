
namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class DbContextClass : DbContext
    {
        protected readonly IConfiguration Configuration;
        public DbContextClass(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

    }
}