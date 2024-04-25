﻿using Microsoft.EntityFrameworkCore;
using LanguageLearningApp.Data.Entities;

namespace LanguageLearningApp.Data.Context
{
    public class LanguageAppContext(DbContextOptions<LanguageAppContext> builder) : DbContext(builder)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.RefreshToken)
                .WithOne()
                .HasForeignKey<RefreshToken>(rt => rt.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }
    }
}
