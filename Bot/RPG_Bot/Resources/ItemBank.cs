using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;

namespace RPG_Bot.Resources
{
    //NTS, ItemID is 0, other numbers reserved for modifiers like legendary effects down the line :)

    public class GenericItem
    {
        public ulong OwnerID { get; set; }
        public uint ItemID { get; set; }
        public string WebURL { get; set; }
        public string ItemName { get; set; }
        public uint ItemCost { get; set; }
        public string ItemRarity { get; set; }
    }

    public class WearableItem
    {
        public ulong OwnerID { get; set; }
        public uint ItemID { get; set; }
        public string WebURL { get; set; }
        public string ItemName { get; set; }
        public uint ItemCost { get; set; }
        public string ItemRarity { get; set; }
        public uint Armor { get; set; }
        public uint Health { get; set; }
        public uint HealthGainOnDamage { get; set; }
    }

    public class Weapon : GenericItem
    {
        public uint Damage { get; set; }
        public uint LifeSteal { get; set; }

        public Weapon(uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint damage, uint lifeSteal)
        {
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Damage = damage;
            LifeSteal = lifeSteal;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Helmet : WearableItem
    {
        public Helmet(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Accessory : WearableItem
    {
        public Accessory(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Chestplate : WearableItem
    {
        public Chestplate(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Gauntlets : WearableItem
    {
        public Gauntlets(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Belt : WearableItem
    {
        public Belt(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Leggings : WearableItem
    {
        public Leggings(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Boots : WearableItem
    {
        public Boots(ulong ownerID, uint itemID, string url, string itemName, uint itemCost, string itemRarity, uint armor, uint health, uint healthGainOnDamage)
        {
            OwnerID = ownerID;
            ItemID = itemID;
            WebURL = url;
            ItemName = itemName;
            Armor = armor;
            Health = health;
            HealthGainOnDamage = healthGainOnDamage;
            ItemCost = itemCost;
            ItemRarity = itemRarity;
        }
    }

    public class Items
    {
        public const string EmptySlotIcon = "https://cdn.discordapp.com/attachments/542225685695954945/636453041821843486/EmptySlot.png";

        public static List<string> mod_names = new List<string>()
        {
            "Sturdy", "Heavy", "Enchanted", "Infused", "Light", "Iron", "Steel", "Bronze", "Copper", "Cloth", "Magic", "Wizards", "Witchdoctors",
            "Knights", "Kings", "Berserkers", "Kitsunes", "Archers", "Assassins", "Rogues", "Witches", "Wizards", "Tricksters", "Swordsman",
            "Paladins", "Necromancers", "Monks", "Evangels", "Tamers", "Imps", "Wisps", "Misty", "Cloudy", "Shadowed", "Flaming", "Slime Gel",
            "Goblin Cloth", "Skeleton Bone", "Rat Hide", "Living Tree Wood", "Ent Bark", "Hornet Chitin", "Haunted Wood", "Golem Stone", "Shroomling Flesh",
            "Cyclops Chain", "Hob Goblins", "Frost Crystal", "Frost Bear Hide", "Frost Knight Plated", "Frost Wolf Fur", "Enchanted Wood", "Silver Mimic Plated",
            "Silver", "Frost Dragon Scale", "Frost Essence", "Flame Essence", "Dark Essence", "Light Essence", "Earth Essence", "Life Essence", "Living Essence",
            "Royal Pelican Feather", "Harpy Feather", "Cetus Scale", "Megaton Scale", "Fish Man Scale", "Lizard Man Scale", "Salamander Scale", "Salamander Tusk",
            "Leech Scale", "Leech Tooth", "Phoenix Feather", "Phoenix Beak", "Phoenix Talon", "Rex Tooth", "Rex Bone", "Thunder Tooth", "Vasuki Scale", "Flare Shard",
            "Skeletor Bone", "Mighty Duck Feather", "Rourtu Plated", "Bone", "Gem", "Stone", "Wooden", "Cloth", "Fur"
        };

        //NTS Need weapon icons
        public static string[] weapon_icons = new string[]
        {
            
        };

        public static string[] helmet_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636444021102870548/137_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443825501634571/86t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443752260698133/70t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443840949125137/92t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443824305995776/85t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444019987054593/136_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444016002596864/134_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444018154405901/135_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443832723963904/88t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443843155460106/93t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443845055217674/94t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443837602070532/90t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443867993866240/96t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443839585845258/91t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443835320369162/89t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443873400324096/97t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443831104962581/87t.PNG"
        };

        public static string[] accessorie_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636443875648733195/98t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443878374899712/99t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443880040169479/100t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443882954948618/101t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443885211615242/102t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443890060099595/104t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443803280080897/82t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443805653925890/83t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443801048842260/81t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443667007143946/50t.PNG"
        };

        public static string[] chestplate_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636443467261935617/140_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443615945687075/36t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443615471730689/35t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443612229533706/34t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443617258635264/38t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443610832830464/33t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443787144724490/75t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443471745515530/2_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443619573760020/39t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443619225763842/40t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443470625767434/1t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443474291589120/4_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443526527188992/13t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443560861761547/20t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443620500832266/41t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444013586677760/132_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443479056187403/5t.png"
        };

        public static string[] gauntlet_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636444049431068682/139_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444009589637120/130t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636444007458668555/129t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443982154694656/126t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443984184737800/127t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443935383748608/115t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443930627538944/113t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443514443661313/11t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443476862697472/6t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443472643096576/3_t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443654138888224/44t.PNG"
        };

        public static string[] belt_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636443737081511938/63t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443749727338506/68t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443693183664128/54t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443746224832513/66t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443747894296576/67t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443742605279282/64t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443712771325952/61t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443716265181194/62t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443696115482634/52t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443745444954124/65t.png"
        };

        public static string[] legging_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636443709663084554/59t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443705867239424/57t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443571842711552/24t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443480003969034/7b.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443569896292352/22t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443707826241546/58t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443571158908928/23t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443524765712394/12t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443704428724224/56t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443702872637450/55t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443581099540480/30t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443710028120064/60t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443575428579328/25t.png"
        };

        public static string[] boot_icons = new string[]
        {
            "https://cdn.discordapp.com/attachments/542225685695954945/636443937925496832/116t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443920980639761/109t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443960822464512/118t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443980325847051/125t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443966941822991/119t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443969496285204/120t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443975808450570/123t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443972113530881/121t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443977880698890/124t.PNG",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443927700045836/112t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443932804251658/114t.png",
            "https://cdn.discordapp.com/attachments/542225685695954945/636443974130991104/122t.PNG"
        };
    }
}