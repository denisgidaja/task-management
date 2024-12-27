
using TaskManagement.Contracts;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Infrastructure
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Assignment> Assignments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: Configure a composite key
            modelBuilder.Entity<Assignment>()
                .HasKey(a => new { a.ID });

        }
    }
}
