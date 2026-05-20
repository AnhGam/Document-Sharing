using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace document_sharing_manager.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<ManagedServer> Servers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // CreatedAt is already set in constructor, but we could enforce it here if needed
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Update();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
