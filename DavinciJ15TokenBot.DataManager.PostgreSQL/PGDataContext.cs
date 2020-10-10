using DavinciJ15TokenBot.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.DataManager.PostgreSQL
{
    public class PGDataContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public PGDataContext(DbContextOptions<PGDataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateMembersModel(modelBuilder);
        }

        private static void CreateMembersModel(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Member>();

            entity
                .HasKey(p => p.Id);

            entity
                .HasIndex(p => p.TelegramId)
                .IsUnique();

            entity
                .HasIndex(p => p.TelegramChatId)
                .IsUnique();

            entity
                .HasIndex(p => p.Address)
                .IsUnique();
        }
    }
}
