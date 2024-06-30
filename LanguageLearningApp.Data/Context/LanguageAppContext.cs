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
        }
    }
}
