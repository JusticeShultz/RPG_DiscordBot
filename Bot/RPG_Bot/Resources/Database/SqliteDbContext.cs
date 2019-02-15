using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace RPG_Bot.Resources.Database
{
    class SqliteDbContext : DbContext
    {
        public DbSet<UserData> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            // string DbLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug\netcoreapp2.1", @"Data\");
            //Console.WriteLine($"{DbLocation}RPG_Bot.dllDatabase.sqlite");
            //Console.WriteLine("SQLITE CONFIGURE] Does: " + $"{DbLocation}RPG_Bot.dllDatabase.sqlite" + " exist? " + File.Exists($"{DbLocation}RPG_Bot.dllDatabase.sqlite"));

            //Options.UseSqlite(@"RPG_Bot.dllDatabase");
            Options.UseSqlite($"Data Source=RPG_Bot.dllDatabase.sqlite");
            //Console.WriteLine("UseSqlite] successful!");
        }
    }
}