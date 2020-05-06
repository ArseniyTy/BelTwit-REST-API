using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BelTwit_REST_API.Models
{
    public class BelTwitContext : DbContext
    {
        public DbSet<User> Users{ get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public BelTwitContext(DbContextOptions<BelTwitContext> options)
            : base (options)
        {            
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(p => p.Login)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasKey(p => p.TokenValue);

            modelBuilder.Entity<User>()
                .HasMany(p => p.RefreshTokens)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
