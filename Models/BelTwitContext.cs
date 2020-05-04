using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BelTwit_REST_API.Models
{
    public class BelTwitContext : DbContext
    {
        public DbSet<User> Users{ get; set; }

        public BelTwitContext(DbContextOptions<BelTwitContext> options)
            : base (options)
        {            
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //лучше по ID
            modelBuilder.Entity<User>()
                .HasIndex(p => p.Login)
                .IsUnique();
        }
    }
}
