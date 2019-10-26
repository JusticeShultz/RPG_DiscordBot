using System.ComponentModel.DataAnnotations;

namespace RPG_Bot.Resources.Database
{
    public class UserData
    {
        [Key]
        public ulong UserID { get; set; }
        public uint GoldAmount { get; set; }
        public uint Age { get; set; }
        public string Name { get; set; }
        public uint Damage { get; set; }
        public uint Health { get; set; }
        public uint Level { get; set; }
        public uint XP { get; set; }
        public uint CurrentHealth { get; set; }
        public uint EventItem1 { get; set; }
        public uint EventItem2 { get; set; }
        public uint EventItem3 { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Day { get; set; }
        public int DailyClaimed { get; set; }
        public string Class { get; set; }
        public string Rank { get; set; }

        // Helmets
        public string Helmet_Name { get; set; }
        public string Helmet_URL { get; set; }
        public string Helmet_Rarity { get; set; }
        public uint Helmet_ModID { get; set; }
        public uint Helmet_Armor { get; set; }
        public uint Helmet_Health { get; set; }
        public uint Helmet_Regen { get; set; }
        public uint Helmet_Cost { get; set; }

        // Chestplates
        public string Chestplate_Name { get; set; }
        public string Chestplate_URL { get; set; }
        public string Chestplate_Rarity { get; set; }
        public uint Chestplate_ModID { get; set; }
        public uint Chestplate_Armor { get; set; }
        public uint Chestplate_Health { get; set; }
        public uint Chestplate_Regen { get; set; }
        public uint Chestplate_Cost { get; set; }

        // Gauntlets
        public string Gauntlet_Name { get; set; }
        public string Gauntlet_URL { get; set; }
        public string Gauntlet_Rarity { get; set; }
        public uint Gauntlet_ModID { get; set; }
        public uint Gauntlet_Armor { get; set; }
        public uint Gauntlet_Health { get; set; }
        public uint Gauntlet_Regen { get; set; }
        public uint Gauntlet_Cost { get; set; }

        // Belts
        public string Belt_Name { get; set; }
        public string Belt_URL { get; set; }
        public string Belt_Rarity { get; set; }
        public uint Belt_ModID { get; set; }
        public uint Belt_Armor { get; set; }
        public uint Belt_Health { get; set; }
        public uint Belt_Regen { get; set; }
        public uint Belt_Cost { get; set; }

        // Leggings
        public string Legging_Name { get; set; }
        public string Legging_URL { get; set; }
        public string Legging_Rarity { get; set; }
        public uint Legging_ModID { get; set; }
        public uint Legging_Armor { get; set; }
        public uint Legging_Health { get; set; }
        public uint Legging_Regen { get; set; }
        public uint Legging_Cost { get; set; }

        // Boots
        public string Boot_Name { get; set; }
        public string Boot_URL { get; set; }
        public string Boot_Rarity { get; set; }
        public uint Boot_ModID { get; set; }
        public uint Boot_Armor { get; set; }
        public uint Boot_Health { get; set; }
        public uint Boot_Regen { get; set; }
        public uint Boot_Cost { get; set; }

        // Stats
        public uint SkillPoints { get; set; }
        public uint Stamina { get; set; }
        public uint Stability { get; set; }
        public uint Dexterity { get; set; }
        public uint Strength { get; set; }
        public uint Luck { get; set; }
        public uint Charisma { get; set; }

        // Potions
        public uint SmallPotionCount { get; set; }
        public uint MediumPotionCount { get; set; }
        public uint LargePotionCount { get; set; }
        public uint DragonPotionCount { get; set; }
        public uint AngelPotionCount { get; set; }

        // Loot Boxes
        public uint CommonBoxCount { get; set; }
        public uint UncommonBoxCount { get; set; }
        public uint RareBoxCount { get; set; }
        public uint VeryRareBoxCount { get; set; }
        public uint EpicBoxCount { get; set; }
        public uint LegendaryBoxCount { get; set; }
        public uint MythicBoxCount { get; set; }
        public uint GodlyBoxCount { get; set; }
        public uint EventBoxCount { get; set; }

        // Donation meta data
        public uint AmountDonated { get; set; }

        // PvP Data
        public uint WinCount { get; set; }
        public uint LoseCount { get; set; }
    }
}
/*public string Weapon_Name { get; set; }
        public int Weapon_ItemID { get; set; }
        public string Weapon_Rarity { get; set; }
        public int Weapon_Damage { get; set; }*/