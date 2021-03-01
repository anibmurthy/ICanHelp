using ICanHelp.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Platform
{
    public class SQLiteDbContext :DbContext
    {
        public DbSet<SQLiteBridge> Boards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ICanHelp.db;");
        }
    }
}
