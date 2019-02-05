using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RPG_Bot.Resources.Database
{
    class SqliteDbContext : DbContext
    {
        public DbSet<UserData> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            string DbLocation = @"C:\Users\shult\source\repos\RPG_Bot\RPG_Bot\bin\Debug\netcoreapp2.1\";
            Console.WriteLine($"{DbLocation}RPG_Bot.dllDatabase.sqlite");
            Options.UseSqlite($"Data Source={DbLocation}RPG_Bot.dllDatabase.sqlite");
        }
    }
}
