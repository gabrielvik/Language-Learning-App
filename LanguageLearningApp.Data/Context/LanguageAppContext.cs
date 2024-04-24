using Microsoft.EntityFrameworkCore;
using LanguageLearningApp.Data.Entities;

namespace LanguageLearningApp.Data.Context
{
    public class LanguageAppContext(DbContextOptions<LanguageAppContext> builder) : DbContext(builder)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>();

            modelBuilder.Entity<User>()
                .HasOne(u => u.RefreshToken)
                .WithOne(rt => rt.User)
                .HasForeignKey<RefreshToken>(rt => rt.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }
    }
}
