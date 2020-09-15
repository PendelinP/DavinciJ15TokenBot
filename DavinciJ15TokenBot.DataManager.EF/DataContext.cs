using DavinciJ15TokenBot.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DavinciJ15TokenBot.DataManager.EF
{
    public class DataContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
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
        }
    }
}
