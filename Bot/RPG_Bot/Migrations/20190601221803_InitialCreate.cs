using Microsoft.EntityFrameworkCore.Migrations;

namespace RPG_Bot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    UserID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoldAmount = table.Column<uint>(nullable: false),
                    Age = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Damage = table.Column<uint>(nullable: false),
                    Health = table.Column<uint>(nullable: false),
                    Level = table.Column<uint>(nullable: false),
                    XP = table.Column<uint>(nullable: false),
                    CurrentHealth = table.Column<uint>(nullable: false),
                    EventItem1 = table.Column<uint>(nullable: false),
                    EventItem2 = table.Column<uint>(nullable: false),
                    EventItem3 = table.Column<uint>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false),
                    Second = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    DailyClaimed = table.Column<int>(nullable: false),
                    Class = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.UserID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data");
        }
    }
}
