
namespace BookieWookie.API.Entities
{
    using BookieWookie.API.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class WookieBookieContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        private readonly string _userId;

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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.MarkCreatedItemAsOwnedBy(_userId);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.MarkCreatedItemAsOwnedBy(_userId);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityOwnedBy in modelBuilder.Model.GetEntityTypes().Where(x => x.ClrType.GetInterface(nameof(IOwnedBy)) != null))
            {
                modelBuilder.Entity(entityOwnedBy.ClrType).HasIndex(nameof(IOwnedBy.OwnedBy));
            }

            modelBuilder.Entity<Book>().HasQueryFilter(x => x.OwnedBy == _userId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

    }
}