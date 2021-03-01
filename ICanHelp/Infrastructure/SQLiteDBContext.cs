using ICanHelp.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ICanHelp.Infrastructure
{
    public class SQLiteDBContext : DbContext
    {
        //public DbSet<RetroBoard> RetroBoards { get; set; }
        //public DbSet<Comment> Comments { get; set; }
        public DbSet<SQLiteBridge> Board { get; set; }
        public SQLiteDBContext(DbContextOptions<SQLiteDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Add seed data here for internal testing
        }
    }
}
