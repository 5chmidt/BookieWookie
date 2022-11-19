
namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    /// <summary>
    /// Context for entity framework bookie wookie database.
    /// </summary>
    public class BookieWookieContext : DbContext
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialize db context with dependency injection.
        /// </summary>
        /// <param name="configuration"></param>
        public BookieWookieContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// DbSet for user model. <see cref="User"/>
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        /// Dbset for book model <see cref="Book"/>
        /// </summary>
        public virtual DbSet<Book> Books { get; set; }

        /// <summary>
        /// Dbset for file model. <see cref="File"/>
        /// </summary>
        public virtual DbSet<File> Files { get; set; }

        /// <summary>
        /// Override with db context connection string from configuration.
        /// </summary>
        /// <param name="options"><see cref="DbContextOptions"/></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }

    }
}