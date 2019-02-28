﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RPG_Bot.Resources.Database;

namespace RPG_Bot.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    partial class SqliteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("RPG_Bot.Resources.Database.UserData", b =>
                {
                    b.Property<ulong>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("Age");

                    b.Property<string>("Class");

                    b.Property<uint>("CurrentHealth");

                    b.Property<int>("DailyClaimed");

                    b.Property<uint>("Damage");

                    b.Property<int>("Day");

                    b.Property<uint>("EventItem1");

                    b.Property<uint>("EventItem2");

                    b.Property<uint>("EventItem3");

                    b.Property<uint>("GoldAmount");

                    b.Property<uint>("Health");

                    b.Property<int>("Hour");

                    b.Property<uint>("Level");

                    b.Property<int>("Minute");

                    b.Property<string>("Name");

                    b.Property<string>("Rank");

                    b.Property<int>("Second");

                    b.Property<uint>("XP");

                    b.HasKey("UserID");

                    b.ToTable("Data");
                });
#pragma warning restore 612, 618
        }
    }
}
