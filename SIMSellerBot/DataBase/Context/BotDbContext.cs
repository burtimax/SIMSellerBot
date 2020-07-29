using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerTelegramBot.DataBase.Models;
using SIMSellerBot.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace SIMSellerTelegramBot.DataBase.Context
{
    public class BotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatState> ChatStates { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NumberRequest> NumberRequests { get; set; }
        public DbSet<Question> Questions { get; set; }

        

        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region AutoDatetime

            modelBuilder.Entity<User>()
                .Property(p => p.CreateTime)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<NumberRequest>()
                .Property(p => p.CreateTime)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Question>()
                .Property(p => p.CreateTime)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Message>()
                .Property(p => p.CreateTime)
                .HasDefaultValueSql("GETDATE()");
            #endregion



        }
    }
}
