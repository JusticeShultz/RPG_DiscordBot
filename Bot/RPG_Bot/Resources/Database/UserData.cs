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
        public uint FlatHealthRegen { get; set; }
        public uint RebornGems { get; set; }

        // Player skills
        public string Learned_Spells { get; set; }
        public string Equipped_Spell_1 { get; set; }
        public string Equipped_Spell_2 { get; set; }
        public string Equipped_Spell_3 { get; set; }
        public string Equipped_Spell_4 { get; set; }

        // Helmets
        public string Helmet_Name { get; set; }
        public string Helmet_URL { get; set; }
        public string Helmet_Rarity { get; set; }
        public uint Helmet_Level { get; set; }
        public uint Helmet_ModID { get; set; }
        public uint Helmet_Armor { get; set; }
        public uint Helmet_Health { get; set; }
        public uint Helmet_Regen { get; set; }
        public uint Helmet_Cost { get; set; }

        // Chestplates
        public string Chestplate_Name { get; set; }
        public string Chestplate_URL { get; set; }
        public string Chestplate_Rarity { get; set; }
        public uint Chestplate_Level { get; set; }
        public uint Chestplate_ModID { get; set; }
        public uint Chestplate_Armor { get; set; }
        public uint Chestplate_Health { get; set; }
        public uint Chestplate_Regen { get; set; }
        public uint Chestplate_Cost { get; set; }

        // Gauntlets
        public string Gauntlet_Name { get; set; }
        public string Gauntlet_URL { get; set; }
        public string Gauntlet_Rarity { get; set; }
        public uint Gauntlet_Level { get; set; }
        public uint Gauntlet_ModID { get; set; }
        public uint Gauntlet_Armor { get; set; }
        public uint Gauntlet_Health { get; set; }
        public uint Gauntlet_Regen { get; set; }
        public uint Gauntlet_Cost { get; set; }

        // Belts
        public string Belt_Name { get; set; }
        public string Belt_URL { get; set; }
        public string Belt_Rarity { get; set; }
        public uint Belt_Level { get; set; }
        public uint Belt_ModID { get; set; }
        public uint Belt_Armor { get; set; }
        public uint Belt_Health { get; set; }
        public uint Belt_Regen { get; set; }
        public uint Belt_Cost { get; set; }

        // Leggings
        public string Legging_Name { get; set; }
        public string Legging_URL { get; set; }
        public string Legging_Rarity { get; set; }
        public uint Legging_Level { get; set; }
        public uint Legging_ModID { get; set; }
        public uint Legging_Armor { get; set; }
        public uint Legging_Health { get; set; }
        public uint Legging_Regen { get; set; }
        public uint Legging_Cost { get; set; }

        // Boots
        public string Boot_Name { get; set; }
        public string Boot_URL { get; set; }
        public string Boot_Rarity { get; set; }
        public uint Boot_Level { get; set; }
        public uint Boot_ModID { get; set; }
        public uint Boot_Armor { get; set; }
        public uint Boot_Health { get; set; }
        public uint Boot_Regen { get; set; }
        public uint Boot_Cost { get; set; }

        // Artifact
        public string Artifact_Name { get; set; }
        public string Artifact_URL { get; set; }
        public string Artifact_Rarity { get; set; }
        public uint Artifact_Level { get; set; }
        public uint Artifact_ModID { get; set; }
        public uint Artifact_ModValue { get; set; }
        public uint Artifact_Armor { get; set; }
        public uint Artifact_Health { get; set; }
        public uint Artifact_Regen { get; set; }
        public uint Artifact_Damage { get; set; }
        public uint Artifact_Cost { get; set; }

        // Weapon
        public string Weapon_Name { get; set; }
        public string Weapon_Efficient_Class { get; set; }
        public string Weapon_URL { get; set; }
        public string Weapon_Rarity { get; set; }
        public uint Weapon_Level { get; set; }
        public uint Weapon_ModID { get; set; }
        public uint Weapon_ModValue { get; set; }
        public uint Weapon_Armor { get; set; }
        public uint Weapon_Health { get; set; }
        public uint Weapon_Regen { get; set; }
        public uint Weapon_Damage { get; set; }
        public uint Weapon_Cost { get; set; }

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
        public uint RebornBoxCount { get; set; }
        public uint EventBoxCount { get; set; }

        // Donation meta data
        public uint AmountDonated { get; set; }

        // PvP Data
        public uint WinCount { get; set; }
        public uint LoseCount { get; set; }

        // God Data
        public string GodChoice { get; set; }

        // Exploration Zones
        public uint Explored_Training_Forts { get; set; }
        public uint Explored_Rayels_Terror_Tower { get; set; }
        public uint Explored_Skeleton_Caves { get; set; }
        public uint Explored_Forest_Path { get; set; }
        public uint Explored_Goblin_Outpost { get; set; }
        public uint Explored_Tomb_of_Heroes { get; set; }
        public uint Explored_Lair_of_Trenthola { get; set; }
        public uint Explored_Snowy_Woods { get; set; }
        public uint Explored_Makkosi_Camp { get; set; }
        public uint Explored_Frigid_Wastes { get; set; }
        public uint Explored_Abandoned_Village { get; set; }
        public uint Explored_Frozen_Peaks { get; set; }
        public uint Explored_Cliffside_Barricade { get; set; }
        public uint Explored_Roosting_Crags { get; set; }
        public uint Explored_Taken_Temple { get; set; }
        public uint Explored_Undying_Storm { get; set; }
        public uint Explored_Crawling_Coastline { get; set; }
        public uint Explored_Megaton_Cove { get; set; }
        public uint Explored_Lunar_Temple { get; set; }
        public uint Explored_Hms_Reliant { get; set; }
        public uint Explored_Logada_Summits { get; set; }
        public uint Explored_Burning_Outcrop { get; set; }
        public uint Explored_Magma_Pits { get; set; }
        public uint Explored_Draconic_Nests { get; set; }
        public uint Explored_Twisted_Riverbed { get; set; }
        public uint Explored_Quarantined_Village { get; set; }
        public uint Explored_The_Breach { get; set; }
        public uint Explored_Within_the_Breach { get; set; }
        public uint Explored_Outer_Blockade { get; set; }
        public uint Explored_Shattered_Streets { get; set; }
        public uint Explored_Catacombs { get; set; }
        public uint Explored_Corrupt_Citadel { get; set; }
        public uint Explored_Ruined_Outpost { get; set; }
        public uint Explored_Sombris_Monument { get; set; }
        public uint Explored_End_of_Asteria { get; set; }
        public uint Explored_Borealis_Gates { get; set; }
        public uint Explored_Stellar_Fields { get; set; }
        public uint Explored_Astral_Reaches { get; set; }
        public uint Explored_Shifted_Wastes { get; set; }
        public uint Explored_Fell_Pantheon { get; set; }

        // Event Data
        public uint EventData1_Int { get; set; }
        public uint EventData2_Int { get; set; }
        public uint EventData3_Int { get; set; }
        public uint EventData4_Int { get; set; }
        public uint EventData5_Int { get; set; }
        public uint EventData6_Int { get; set; }
        public uint EventData7_Int { get; set; }
        public uint EventData8_Int { get; set; }
        public uint EventData9_Int { get; set; }
        public string EventData1_String { get; set; }
        public string EventData2_String { get; set; }
        public string EventData3_String { get; set; }
        public string EventData4_String { get; set; }
        public string EventData5_String { get; set; }
        public string EventData6_String { get; set; }
        public string EventData7_String { get; set; }
        public string EventData8_String { get; set; }
        public string EventData9_String { get; set; }

        // Reusable Special Currency Slots
        public uint ReusableCurrency1 { get; set; }
        public uint ReusableCurrency2 { get; set; }
        public uint ReusableCurrency3 { get; set; }
        public uint ReusableCurrency4 { get; set; }
        public uint ReusableCurrency5 { get; set; }
        public uint ReusableCurrency6 { get; set; }
        public uint ReusableCurrency7 { get; set; }
        public uint ReusableCurrency8 { get; set; }
        public uint ReusableCurrency9 { get; set; }

        //Bank
        public uint StoredGold { get; set; }

        // Achievement Info
        public uint Total_Monsters_Slain { get; set; }
        public uint Total_Gold_Obtained { get; set; }
        public uint Total_Damage_Taken { get; set; }
        public uint Total_Items_Found { get; set; }
        public uint Total_Quests_Completed { get; set; }
        public uint Total_Daily_Blessings_Obtained { get; set; }
        public uint Total_Gold_FromDaily_Blessings { get; set; }
        public uint Total_Gems_FromDaily_Blessings { get; set; }
        public uint Total_Damage_Dealt { get; set; }
        public uint Total_World_Bosses_Slain { get; set; }
        public uint Total_World_Bosses_Participated { get; set; }
        public uint Total_Bosses_Slain { get; set; }
        public uint Total_Bosses_Encountered { get; set; }
        public uint Total_Rare_Creatures_Slain { get; set; }
        public uint Total_Gems_Obtained { get; set; }
        public uint Total_Explorations { get; set; }
        public uint Total_Lore_Found { get; set; }
        public uint Total_Nothings_Found { get; set; }
        public uint Total_Loot_Found_Exploring { get; set; }
        public uint Total_Areas_Discovered { get; set; }
        public uint Total_PvP_Damage_Dealt { get; set; }
        public uint Total_Deaths { get; set; }
        public uint Total_XP_Gained { get; set; }
        public uint Total_XP_Lost { get; set; }
        public uint Total_Reborns { get; set; }
        public uint Total_Gambles_Won { get; set; }
        public uint Total_Gambles_Lost { get; set; }
        public uint Total_Gold_Stolen { get; set; }
    }
}
/*public string Weapon_Name { get; set; }
        public int Weapon_ItemID { get; set; }
        public string Weapon_Rarity { get; set; }
        public int Weapon_Damage { get; set; }*/