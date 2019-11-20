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
                    Rank = table.Column<string>(nullable: true),
                    Helmet_Name = table.Column<string>(nullable: true),
                    Helmet_URL = table.Column<string>(nullable: true),
                    Helmet_Rarity = table.Column<string>(nullable: true),
                    Helmet_ModID = table.Column<uint>(nullable: false),
                    Helmet_Armor = table.Column<uint>(nullable: false),
                    Helmet_Health = table.Column<uint>(nullable: false),
                    Helmet_Regen = table.Column<uint>(nullable: false),
                    Helmet_Cost = table.Column<uint>(nullable: false),
                    Chestplate_Name = table.Column<string>(nullable: true),
                    Chestplate_URL = table.Column<string>(nullable: true),
                    Chestplate_Rarity = table.Column<string>(nullable: true),
                    Chestplate_ModID = table.Column<uint>(nullable: false),
                    Chestplate_Armor = table.Column<uint>(nullable: false),
                    Chestplate_Health = table.Column<uint>(nullable: false),
                    Chestplate_Regen = table.Column<uint>(nullable: false),
                    Chestplate_Cost = table.Column<uint>(nullable: false),
                    Gauntlet_Name = table.Column<string>(nullable: true),
                    Gauntlet_URL = table.Column<string>(nullable: true),
                    Gauntlet_Rarity = table.Column<string>(nullable: true),
                    Gauntlet_ModID = table.Column<uint>(nullable: false),
                    Gauntlet_Armor = table.Column<uint>(nullable: false),
                    Gauntlet_Health = table.Column<uint>(nullable: false),
                    Gauntlet_Regen = table.Column<uint>(nullable: false),
                    Gauntlet_Cost = table.Column<uint>(nullable: false),
                    Belt_Name = table.Column<string>(nullable: true),
                    Belt_URL = table.Column<string>(nullable: true),
                    Belt_Rarity = table.Column<string>(nullable: true),
                    Belt_ModID = table.Column<uint>(nullable: false),
                    Belt_Armor = table.Column<uint>(nullable: false),
                    Belt_Health = table.Column<uint>(nullable: false),
                    Belt_Regen = table.Column<uint>(nullable: false),
                    Belt_Cost = table.Column<uint>(nullable: false),
                    Legging_Name = table.Column<string>(nullable: true),
                    Legging_URL = table.Column<string>(nullable: true),
                    Legging_Rarity = table.Column<string>(nullable: true),
                    Legging_ModID = table.Column<uint>(nullable: false),
                    Legging_Armor = table.Column<uint>(nullable: false),
                    Legging_Health = table.Column<uint>(nullable: false),
                    Legging_Regen = table.Column<uint>(nullable: false),
                    Legging_Cost = table.Column<uint>(nullable: false),
                    Boot_Name = table.Column<string>(nullable: true),
                    Boot_URL = table.Column<string>(nullable: true),
                    Boot_Rarity = table.Column<string>(nullable: true),
                    Boot_ModID = table.Column<uint>(nullable: false),
                    Boot_Armor = table.Column<uint>(nullable: false),
                    Boot_Health = table.Column<uint>(nullable: false),
                    Boot_Regen = table.Column<uint>(nullable: false),
                    Boot_Cost = table.Column<uint>(nullable: false),
                    SkillPoints = table.Column<uint>(nullable: false),
                    Stamina = table.Column<uint>(nullable: false),
                    Stability = table.Column<uint>(nullable: false),
                    Dexterity = table.Column<uint>(nullable: false),
                    Strength = table.Column<uint>(nullable: false),
                    Luck = table.Column<uint>(nullable: false),
                    Charisma = table.Column<uint>(nullable: false),
                    SmallPotionCount = table.Column<uint>(nullable: false),
                    MediumPotionCount = table.Column<uint>(nullable: false),
                    LargePotionCount = table.Column<uint>(nullable: false),
                    DragonPotionCount = table.Column<uint>(nullable: false),
                    AngelPotionCount = table.Column<uint>(nullable: false),
                    CommonBoxCount = table.Column<uint>(nullable: false),
                    UncommonBoxCount = table.Column<uint>(nullable: false),
                    RareBoxCount = table.Column<uint>(nullable: false),
                    VeryRareBoxCount = table.Column<uint>(nullable: false),
                    EpicBoxCount = table.Column<uint>(nullable: false),
                    LegendaryBoxCount = table.Column<uint>(nullable: false),
                    MythicBoxCount = table.Column<uint>(nullable: false),
                    GodlyBoxCount = table.Column<uint>(nullable: false),
                    EventBoxCount = table.Column<uint>(nullable: false),
                    AmountDonated = table.Column<uint>(nullable: false),
                    WinCount = table.Column<uint>(nullable: false),
                    LoseCount = table.Column<uint>(nullable: false),
                    GodChoice = table.Column<string>(nullable: true)
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
