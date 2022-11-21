
namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Context for entity framework bookie wookie database.
    /// </summary>
    public class BookieWookieContext : DbContext
    {
        /// <summary>
        /// Initialize db context with context options.
        /// </summary>
        public BookieWookieContext(DbContextOptions<BookieWookieContext> options) : base(options)
        {

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
    }
}