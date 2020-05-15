using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BelTwit_REST_API.Models
{
    public class BelTwitContext : DbContext
    {
        public DbSet<User> Users{ get; set; }
        public DbSet<SubscriberSubscription> SubscriberSubscriptions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Log> Logs { get; set; }

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





            modelBuilder.Entity<SubscriberSubscription>()
                .HasKey(p => new { p.WhoSubscribeId, p.OnWhomSubscribeId });


            //NO: nothing when deleting!!! (but only that is possible)
            modelBuilder.Entity<SubscriberSubscription>()
                .HasOne(p => p.WhoSubscribe)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(p => p.WhoSubscribeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscriberSubscription>()
                .HasOne(p => p.OnWhomSubscribe)
                .WithMany(p => p.Subscribers)
                .HasForeignKey(p => p.OnWhomSubscribeId);

        }
    }
}
