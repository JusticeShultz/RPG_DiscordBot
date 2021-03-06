﻿/////////Completely Deprecated/////////////////////////////////////////////////////////////////////////////////////////////

///////////////////Completely Deprecated///////////////////////////////////////////////////////////////////////////////////

/////////////////////////////Completely Deprecated/////////////////////////////////////////////////////////////////////////

/////////////////////////////////////Completely Deprecated/////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////Completely Deprecated/////////////////////////////////////////////////////

///////////////////////////////////////////////////////////Completely Deprecated///////////////////////////////////////////

/////////////////////////////////////////////////////////////////////Completely Deprecated/////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////Completely Deprecated//////////////////



//This script is just a simple template for things. All that can be found here is the boundings
//for how enemies spawn, such as max health and min health. The actual code behind it all is located
//under Gameplay.cs

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;

//IF TIME CONVERT TO XML OR JSON HERE FOR EASIER READABILITY

namespace RPG_Bot.Resources
{
    public struct Enemy
    {
        public string WebURL { get; set; }
        public uint MaxHealth { get; set; }
        public uint MinHealth { get; set; }
        public uint MaxDamage { get; set; }
        public uint MinDamage { get; set; }
        public uint MaxLevel { get; set; }
        public uint MinLevel { get; set; }
        public uint CurrentHealth { get; set; }
        public uint MaxGoldDrop { get; set; }
        public uint MinGoldDrop { get; set; }
        public uint MaxXpDrop { get; set; }
        public uint MinXpDrop { get; set; }
        public string Name { get; set; }
        public bool IsDead { get; set; }
        public bool IsInCombat { get; set; }

        public Enemy(string url, uint maxHealth, uint minHealth, uint maxDamage, uint minDamage, uint maxLevel, uint minLevel, uint maxGoldDrop, uint minGoldDrop, uint maxXPDrop, uint minXPDrop, string name, bool isDead = false, bool isInCombat = false)
        {
            WebURL = url;
            MaxHealth = maxHealth;
            MinHealth = minHealth;
            CurrentHealth = MaxHealth;
            MaxDamage = maxDamage;
            MinDamage = minDamage;
            MaxLevel = maxLevel;
            MinLevel = minLevel;
            MaxGoldDrop = maxGoldDrop;
            MinGoldDrop = minGoldDrop;
            MaxXpDrop = maxXPDrop;
            MinXpDrop = minXPDrop;
            Name = name;
            IsDead = isDead;
            IsInCombat = isInCombat;
        }
    }

    public static class EnemyTemplates
    {
        //These ulongs are the ID values
        public static ulong Shop = 542136465069441034;
        public static ulong BlackMarket = 542136620892028951;
        public static ulong Channel1 = 543151279984345115; //Zone 1-5
        public static ulong Channel2 = 543495063494328321; //Zone 5-10
        public static ulong Channel3 = 543233752332107779; //Zone 10-15
        public static ulong Channel4 = 543547567045083136; //Zone 15-20
        public static ulong Channel5 = 543547643062648852; //Zone 20-40
        public static ulong Channel6 = 543547674117537833; //Zone 40-60
        public static ulong Channel7 = 543547704928763966; //Zone 60-90
        public static ulong Channel8 = 543547743096799262; //Zone 90-100
        public static ulong Channel9 = 542118918299582474; //Zone 100 - The Aurora

        public static ulong ChannelEventSpawn = 544041750008823829;
        public static ulong Questing = 542118422725656580;
        public static ulong DailyQuesting = 547199675602829323;
        
        //Roles
        public static ulong Bronze = 542123825370890250;
        public static ulong Silver = 542123823936438287;
        public static ulong Gold = 542123823034662934;
        public static ulong Quartz = 542123822086881281;
        public static ulong Orichalcum = 542123822053195798;
        public static ulong Platinum = 542123821398884382;
        public static ulong MasterIII = 542123820790841345;
        public static ulong MasterII = 542123819754717194;
        public static ulong MasterI = 542123818890690603;

        public static ulong gothkamul = 542120236179259424;
        public static ulong rakdoro = 542120271738568714;
        public static ulong kenthros = 542120374456811530;
        public static ulong arkdul = 542120398775648266;
        public static ulong ServerID = 542118110774427659;

        //This unit is not templated.
        public static Enemy Goblin = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139937948532778/Unit_ills_full_10050.png",
            25,
            10,
            25,
            5,
            5,
            1,
            12,
            1,
            3,
            1,
            "Goblin"
        );

        public static Enemy LogSpider = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545322112257228820/Wooden_Spider.webp",
            120,
            105,
            12,
            3,
            5,
            3,
            6,
            2,
            3,
            2,
            "Log Spider"
        );

        //This unit is templated.
        public static Enemy Mimic = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139849809559562/Unit_ills_full_60142.png", //URL
            12500, //Max Health
            5000, //Min Health
            45, //Max Damage
            30, //Min Damage
            50, //Max Level
            40, //Min Level
            2000, //Max Gold Drop
            400, //Min Gold Drop
            0, //Max XP Drop
            0, //Min XP Drop
            "[ANOMALY] Mimic" //Unit Name
        );

        public static Enemy SilverMimic = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870700296634368/Unit_ills_full_60224.webp", //URL
            22500, //Max Health
            15000, //Min Health
            45, //Max Damage
            30, //Min Damage
            50, //Max Level
            40, //Min Level
            3000, //Max Gold Drop
            1000, //Min Gold Drop
            0, //Max XP Drop
            0, //Min XP Drop
            "[ANOMALY] Silver Mimic" //Unit Name
        );

        public static Enemy BronzePot = new Enemy
        (
            "https://vignette.wikia.nocookie.net/quiz-rpg-the-world-of-mystic-wiz/images/f/f3/Bronze_Pot_transparent.png/revision/latest?cb=20141025232029", //URL
            22500, //Max Health
            18000, //Min Health
            45, //Max Damage
            30, //Min Damage
            50, //Max Level
            40, //Min Level
            3000, //Max Gold Drop
            2000, //Min Gold Drop
            30, //Max XP Drop
            20, //Min XP Drop
            "[EVENT ANOMALY] Bronze Pot" //Unit Name
        );

        public static Enemy SilverPot = new Enemy
        (
            "https://vignette.wikia.nocookie.net/quiz-rpg-the-world-of-mystic-wiz/images/f/fc/Silver_Pot_transparent.png/revision/latest?cb=20141025232037", //URL
            32500, //Max Health
            25000, //Min Health
            45, //Max Damage
            30, //Min Damage
            50, //Max Level
            40, //Min Level
            4000, //Max Gold Drop
            3000, //Min Gold Drop
            50, //Max XP Drop
            40, //Min XP Drop
            "[EVENT ANOMALY] Silver Pot" //Unit Name
        );

        public static Enemy GoldenPot = new Enemy
        (
            "https://vignette.wikia.nocookie.net/quiz-rpg-the-world-of-mystic-wiz/images/f/f5/The_Golden_Pot_transparent.png/revision/latest?cb=20141025232107", //URL
            42500, //Max Health
            35000, //Min Health
            45, //Max Damage
            30, //Min Damage
            80, //Max Level
            80, //Min Level
            7000, //Max Gold Drop
            5000, //Min Gold Drop
            60, //Max XP Drop
            40, //Min XP Drop
            "[EVENT ANOMALY] Golden Pot" //Unit Name
        );

        public static Enemy Golem = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140066369863682/Unit_ills_full_30932.png", //URL
            800, //Max Health
            500, //Min Health
            80, //Max Damage
            15, //Min Damage
            20, //Max Level
            15, //Min Level
            45, //Max Gold Drop
            1, //Min Gold Drop
            50, //Max XP Drop
            30, //Min XP Drop
            "Golem" //Unit Name
        );

        public static Enemy Mushroom = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546855004120678400/Unit_BigMushroomFire_sprite.webp", //URL
            600, //Max Health
            400, //Min Health
            70, //Max Damage
            15, //Min Damage
            20, //Max Level
            15, //Min Level
            45, //Max Gold Drop
            1, //Min Gold Drop
            60, //Max XP Drop
            22, //Min XP Drop
            "Shroomling" //Unit Name
        );

        public static Enemy Cyclops = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870728184823808/Unit_ills_full_40072.webp", //URL
            900, //Max Health
            125, //Min Health
            70, //Max Damage
            25, //Min Damage
            20, //Max Level
            15, //Min Level
            45, //Max Gold Drop
            1, //Min Gold Drop
            80, //Max XP Drop
            12, //Min XP Drop
            "Cyclops" //Unit Name
        );

        public static Enemy Imp = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139900963160109/Unit_ills_full_60181.png", //URL
            15, //Max Health
            5, //Min Health
            15, //Max Damage
            1, //Min Damage
            5, //Max Level
            1, //Min Level
            8, //Max Gold Drop
            1, //Min Gold Drop
            7, //Max XP Drop
            3, //Min XP Drop
            "Imp" //Unit Name
        );

        public static Enemy HobGoblin = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139937457930300/Unit_ills_full_10052.png", //URL
            1200, //Max Health
            950, //Min Health
            60, //Max Damage
            35, //Min Damage
            20, //Max Level
            15, //Min Level
            25, //Max Gold Drop
            10, //Min Gold Drop
            125, //Max XP Drop
            30, //Min XP Drop
            "Hob Goblin" //Unit Name
        );

        public static Enemy TrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850419725434881/Unit_ills_full_810124.webp", //URL
            50, //Max Health
            50, //Min Health
            1, //Max Damage
            1, //Min Damage
            1, //Max Level
            1, //Min Level
            0, //Max Gold Drop
            0, //Min Gold Drop
            1, //Max XP Drop
            1, //Min XP Drop
            "Training Bot" //Unit Name
        ); //https://cdn.discordapp.com/attachments/542225685695954945/543140268845694987/giphy_1.gif

        public static Enemy LargeTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850062466940939/Unit_ills_full_10922.webp", //URL
            150, //Max Health
            100, //Min Health
            10, //Max Damage
            6, //Min Damage
            10, //Max Level
            5, //Min Level
            35, //Max Gold Drop
            1, //Min Gold Drop
            10, //Max XP Drop
            3, //Min XP Drop
            "Upgraded Training Bot" //Unit Name
        ); //https://cdn.discordapp.com/attachments/542225685695954945/543140269915373588/Crablops.gif

        public static Enemy Skeleton = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870697515810841/Unit_ills_full_60050.webp", //URL
            130, //Max Health
            95, //Min Health
            10, //Max Damage
            6, //Min Damage
            10, //Max Level
            5, //Min Level
            35, //Max Gold Drop
            1, //Min Gold Drop
            13, //Max XP Drop
            1, //Min XP Drop
            "Skeleton" //Unit Name
        );

        public static Enemy BigTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850069341667330/Unit_ills_full_10923.webp", //URL
            200, //Max Health
            150, //Min Health
            15, //Max Damage
            8, //Min Damage
            10, //Max Level
            7, //Min Level
            35, //Max Gold Drop
            10, //Min Gold Drop
            18, //Max XP Drop
            3, //Min XP Drop
            "Armed Training Bot" //Unit Name
        ); //https://cdn.discordapp.com/attachments/542225685695954945/543140266589159428/giphy_2.gif

        public static Enemy SkeletonFootSoldier = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870685839130634/Unit_ills_full_60051.webp", //URL
            190, //Max Health
            150, //Min Health
            15, //Max Damage
            8, //Min Damage
            10, //Max Level
            7, //Min Level
            30, //Max Gold Drop
            10, //Min Gold Drop
            17, //Max XP Drop
            3, //Min XP Drop
            "Skeleton Foot Soldier" //Unit Name
        );

        public static Enemy BossTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140272213852219/r9zecHs.gif", //URL
            2000, //Max Health
            1500, //Min Health
            150, //Max Damage
            15, //Min Damage
            10, //Max Level
            5, //Min Level
            250, //Max Gold Drop
            120, //Min Gold Drop
            225, //Max XP Drop
            10, //Min XP Drop
            "[BOSS] Kerthgore the Mad Scientist" //Unit Name
        );

        public static Enemy HauntedTree = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140199379501077/Unit_ills_full_30082.png", //URL
            400, //Max Health
            300, //Min Health
            150, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            20, //Max XP Drop
            15, //Min XP Drop
            "Haunted Tree" //Unit Name
        );

        public static Enemy GiantEnt = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870671947333652/Unit_ills_full_30051.webp", //URL
            650, //Max Health
            450, //Min Health
            230, //Max Damage
            20, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            40, //Max XP Drop
            15, //Min XP Drop
            "Giant Tree Ent" //Unit Name
        );

        public static Enemy GigaEnt = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850170684309514/Unit_ills_full_30052.webp", //URL
            1500, //Max Health
            950, //Min Health
            210, //Max Damage
            40, //Min Damage
            15, //Max Level
            10, //Min Level
            55, //Max Gold Drop
            32, //Min Gold Drop
            125, //Max XP Drop
            25, //Min XP Drop
            "Giga Tree Ent" //Unit Name
        );

        public static Enemy Hornet = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546855056566255618/detail_monster_03.png", //URL
            350, //Max Health
            300, //Min Health
            110, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            45, //Max XP Drop
            10, //Min XP Drop
            "Giant Hornet" //Unit Name
        );

        public static Enemy GiantRat = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546855051558125589/Unit_RatLight_sprite.webp", //URL
            500, //Max Health
            350, //Min Health
            90, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            40, //Max XP Drop
            13, //Min XP Drop
            "Giant Rat" //Unit Name
        );

        public static Enemy Treant = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546870662715670529/NewTreant.webp", //URL
            500, //Max Health
            350, //Min Health
            70, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            30, //Max XP Drop
            13, //Min XP Drop
            "Treant" //Unit Name
        );

        public static Enemy GoblinWolf = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140112301817857/Unit_ills_full_40112.png", //URL
            300, //Max Health
            150, //Min Health
            400, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            25, //Max Gold Drop
            5, //Min Gold Drop
            16, //Max XP Drop
            1, //Min XP Drop
            "Goblin Wolf" //Unit Name
        );

        public static Enemy TreeEnt = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139848987607050/Unit_ills_full_30050.png", //URL
            250, //Max Health
            150, //Min Health
            120, //Max Damage
            15, //Min Damage
            15, //Max Level
            10, //Min Level
            20, //Max Gold Drop
            5, //Min Gold Drop
            15, //Max XP Drop
            8, //Min XP Drop
            "Tree Ent" //Unit Name
        );

        public static Enemy TreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139896827707402/Unit_ills_full_30083.png", //URL
            4500, //Max Health
            3500, //Min Health
            400, //Max Damage
            205, //Min Damage
            15, //Max Level
            10, //Min Level
            250, //Max Gold Drop
            45, //Min Gold Drop
            160, //Max XP Drop
            100, //Min XP Drop
            "[BOSS] Trenthola the Living Tree" //Unit Name
        );

        /*public static Enemy EventTreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139899109277696/image_1.webp", //URL
            85000, //Max Health
            69500, //Min Health
            155, //Max Damage
            95, //Min Damage
            160, //Max Level
            100, //Min Level
            7500, //Max Gold Drop
            4500, //Min Gold Drop
            180, //Max XP Drop
            110, //Min XP Drop
            "[EVENT BOSS] Trelia the Mystic Tree" //Unit Name
        );*/

        /*
         public static Enemy EventTreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850213533188135/Unit_ills_full_30515.webp", //URL
            185000, //Max Health
            149500, //Min Health
            5285, //Max Damage
            155, //Min Damage
            200, //Max Level
            140, //Min Level
            15000, //Max Gold Drop
            9500, //Min Gold Drop
            554, //Max XP Drop
            280, //Min XP Drop
            "[EVENT BOSS] Essence Dragon Grakuul" //Unit Name
        );
         */

        public static Enemy EventTreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/650264663023550464/Unit_ills_full_20606.webp", //URL
            250000, //Max Health
            209500, //Min Health
            2285, //Max Damage
            155, //Min Damage
            200, //Max Level
            140, //Min Level
            15000, //Max Gold Drop
            9500, //Min Gold Drop
            554, //Max XP Drop
            280, //Min XP Drop
            "[EVENT BOSS] Ice Dragon Thragoo" //Unit Name
        );

        public static Enemy GothkamulBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670992786653193/Gothkamul.png", //URL
            130000, //Max Health
            109000, //Min Health
            2650, //Max Damage
            15, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            1250, //Max XP Drop
            500, //Min XP Drop
            "[WORLD BOSS] Gothkamul the Death Lord" //Unit Name
        );

        public static Enemy RakdoroBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670990957936641/Rakdoro.png", //URL
            145900, //Max Health
            108800, //Min Health
            3525, //Max Damage
            40, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            1250, //Max XP Drop
            300, //Min XP Drop
            "[WORLD BOSS] Rakdoro the Flame Lord" //Unit Name
        );

        public static Enemy ArkdulBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670996356136990/Arkdul.png", //URL
            280000, //Max Health
            230000, //Min Health
            2300, //Max Damage
            10, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            1250, //Max XP Drop
            500, //Min XP Drop
            "[WORLD BOSS] Arkdul the Earth Lord" //Unit Name
        );

        public static Enemy KenthrosBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670999648665600/Kenthros.png", //URL
            189900, //Max Health
            158000, //Min Health
            2450, //Max Damage
            30, //Min Damage
            500, //Max Level
            400, //Min Level
            25000, //Max Gold Drop
            13000, //Min Gold Drop
            1600, //Max XP Drop
            600, //Min XP Drop
            "[WORLD BOSS] Kenthros the Sea Lord" //Unit Name
        );

        public static Enemy MySon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566932053392621579/Unit_ills_full_40324.webp", //URL
            1500000, //Max Health
            1500000, //Min Health
            1250, //Max Damage
            100, //Min Damage
            9999, //Max Level
            9999, //Min Level
            350000, //Max Gold Drop
            100000, //Min Gold Drop
            6500, //Max XP Drop
            4500, //Min XP Drop
            "[EXCLUSIVE] Acolyte of Robin" //Unit Name
        );

        public static Enemy FrostWolf = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140199950188564/Unit_ills_full_20892.png", //URL
            1500, //Max Health
            1000, //Min Health
            380, //Max Damage
            110, //Min Damage
            30, //Max Level
            20, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            185, //Max XP Drop
            5, //Min XP Drop
            "Frost Wolf" //Unit Name
        );

        public static Enemy FrostBear = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546855010802204673/38294-56f49eca3077358022c5214d1a55aa46.png", //URL
            1900, //Max Health
            1400, //Min Health
            550, //Max Damage
            50, //Min Damage
            30, //Max Level
            25, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            150, //Max XP Drop
            35, //Min XP Drop
            "Frost Bear" //Unit Name
        );

        public static Enemy FrostKnight = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139940150804482/Unit_ills_full_20172.png", //URL
            2000, //Max Health
            1800, //Min Health
            365, //Max Damage
            50, //Min Damage
            30, //Max Level
            20, //Min Level
            55, //Max Gold Drop
            13, //Min Gold Drop
            185, //Max XP Drop
            25, //Min XP Drop
            "Frost Knight" //Unit Name
        );

        public static Enemy BossIceDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139899109277697/image_5.png", //URL
            20000, //Max Health
            15500, //Min Health
            450, //Max Damage
            25, //Min Damage
            30, //Max Level
            25, //Min Level
            650, //Max Gold Drop
            285, //Min Gold Drop
            750, //Max XP Drop
            285, //Min XP Drop
            "[BOSS] Frost Dragon" //Unit Name
        );

        public static Enemy BossFrostWolfPackLeader = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140114549833766/Unit_ills_full_21006.png", //URL
            30000, //Max Health
            24500, //Min Health
            1000, //Max Damage
            575, //Min Damage
            30, //Max Level
            20, //Min Level
            500, //Max Gold Drop
            180, //Min Gold Drop
            1200, //Max XP Drop
            250, //Min XP Drop
            "[BOSS] Frost Wolf - Pack Leader" //Unit Name
        );

        public static Enemy Harpy = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850229903556608/Unit_ills_full_40050.webp", //URL
            2200, //Max Health
            1600, //Min Health
            620, //Max Damage
            50, //Min Damage
            40, //Max Level
            30, //Min Level
            45, //Max Gold Drop
            12, //Min Gold Drop
            380, //Max XP Drop
            45, //Min XP Drop
            "Harpy" //Unit Name
        );

        public static Enemy HarpyShaman = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850218801233920/Unit_ills_full_40083.webp", //URL
            3500, //Max Health
            2800, //Min Health
            550, //Max Damage
            50, //Min Damage
            40, //Max Level
            35, //Min Level
            65, //Max Gold Drop
            32, //Min Gold Drop
            395, //Max XP Drop
            55, //Min XP Drop
            "Harpy Shaman" //Unit Name
        );

        public static Enemy HarpyCaster = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850231686266886/Unit_ills_full_40081.webp", //URL
            2300, //Max Health
            1300, //Min Health
            680, //Max Damage
            60, //Min Damage
            38, //Max Level
            33, //Min Level
            60, //Max Gold Drop
            30, //Min Gold Drop
            390, //Max XP Drop
            50, //Min XP Drop
            "Harpy Caster" //Unit Name
        );

        public static Enemy RoyalPelican = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850167874256900/Unit_ills_full_20661.webp", //URL
            3500, //Max Health
            2900, //Min Health
            420, //Max Damage
            20, //Min Damage
            40, //Max Level
            37, //Min Level
            85, //Max Gold Drop
            45, //Min Gold Drop
            300, //Max XP Drop
            65, //Min XP Drop
            "Royal Pelican" //Unit Name
        );

        #region 40-50
        public static Enemy Megaton = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308191978946610/Megaton.png", //URL
            10500, //Max Health
            9550, //Min Health
            550, //Max Damage
            5, //Min Damage
            50, //Max Level
            45, //Min Level
            75, //Max Gold Drop
            25, //Min Gold Drop
            375, //Max XP Drop
            45, //Min XP Drop
            "Megaton" //Unit Name
        );

        public static Enemy LizardMan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139848987607051/image_6.png", //URL
            11000, //Max Health
            8500, //Min Health
            750, //Max Damage
            65, //Min Damage
            48, //Max Level
            40, //Min Level
            75, //Max Gold Drop
            25, //Min Gold Drop
            425, //Max XP Drop
            85, //Min XP Drop
            "Lizard Man" //Unit Name
        );

        public static Enemy FishMan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/546855015180926986/Unit_FishmanFire_sprite.webp", //URL
            11000, //Max Health
            8500, //Min Health
            620, //Max Damage
            65, //Min Damage
            45, //Max Level
            40, //Min Level
            75, //Max Gold Drop
            25, //Min Gold Drop
            480, //Max XP Drop
            40, //Min XP Drop
            "Fish Man" //Unit Name
        );

        public static Enemy Leviathan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308182235447305/Leviathan.png", //URL
            73000, //Max Health
            61500, //Min Health
            520, //Max Damage
            70, //Min Damage
            50, //Max Level
            48, //Min Level
            450, //Max Gold Drop
            225, //Min Gold Drop
            1250, //Max XP Drop
            185, //Min XP Drop
            "Leviathan" //Unit Name
        );

#endregion

        #region LV 50-60
        public static Enemy Salamander = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566849954492973099/Unit_ills_full_10081.webp", //URL
            63500, //Max Health
            44500, //Min Health
            1460, //Max Damage
            120, //Min Damage
            60, //Max Level
            50, //Min Level
            112, //Max Gold Drop
            75, //Min Gold Drop
            650, //Max XP Drop
            100, //Min XP Drop
            "Salamander" //Unit Name
        );

        public static Enemy SalamanderAdult = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566849956623810560/Unit_ills_full_10082.webp", //URL
            78500, //Max Health
            55500, //Min Health
            1400, //Max Damage
            230, //Min Damage
            60, //Max Level
            56, //Min Level
            160, //Max Gold Drop
            105, //Min Gold Drop
            650, //Max XP Drop
            165, //Min XP Drop
            "Adult Salamander" //Unit Name
        );

        public static Enemy SalamanderDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566849960126185504/Unit_ills_full_10092.webp", //URL
            155500, //Max Health
            93500, //Min Health
            660, //Max Damage
            120, //Min Damage
            60, //Max Level
            50, //Min Level
            112, //Max Gold Drop
            75, //Min Gold Drop
            700, //Max XP Drop
            120, //Min XP Drop
            "Salamander Dragon" //Unit Name
        );

        public static Enemy SalamanderDragonAdult = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566849963288428555/Unit_ills_full_10094.webp", //URL
            323500, //Max Health
            285500, //Min Health
            1480, //Max Damage
            40, //Min Damage
            60, //Max Level
            58, //Min Level
            225, //Max Gold Drop
            175, //Min Gold Drop
            3500, //Max XP Drop
            2000, //Min XP Drop
            "[BOSS] Adult Salamander Dragon" //Unit Name
        );

        public static Enemy LavaDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566849967055044611/Unit_ills_full_10095.webp", //URL
            233500, //Max Health
            180500, //Min Health
            1980, //Max Damage
            30, //Min Damage
            60, //Max Level
            59, //Min Level
            450, //Max Gold Drop
            375, //Min Gold Drop
            4500, //Max XP Drop
            3200, //Min XP Drop
            "[BOSS] Lava Dragon" //Unit Name
        );
        #endregion

        #region LV 60-70
        public static Enemy Leech = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850121745039371/Unit_ills_full_20261.webp", //URL
            63500, //Max Health
            47500, //Min Health
            2485, //Max Damage
            200, //Min Damage
            70, //Max Level
            60, //Min Level
            200, //Max Gold Drop
            135, //Min Gold Drop
            950, //Max XP Drop
            450, //Min XP Drop
            "Leech" //Unit Name
        );

        public static Enemy GreaterLeech = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850122890215425/Unit_ills_full_20262.webp", //URL
            99850, //Max Health
            57800, //Min Health
            2300, //Max Damage
            200, //Min Damage
            70, //Max Level
            60, //Min Level
            220, //Max Gold Drop
            145, //Min Gold Drop
            1050, //Max XP Drop
            400, //Min XP Drop
            "Greater Leech" //Unit Name
        );

        public static Enemy Reaver = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850075100446721/Unit_ills_full_20081.webp", //URL
            89850, //Max Health
            57800, //Min Health
            2300, //Max Damage
            200, //Min Damage
            70, //Max Level
            60, //Min Level
            220, //Max Gold Drop
            145, //Min Gold Drop
            950, //Max XP Drop
            400, //Min XP Drop
            "Reaver" //Unit Name
        );

        public static Enemy SeaDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850058394271754/Unit_ills_full_20112.webp", //URL
            423500, //Max Health
            380500, //Min Health
            2440, //Max Damage
            180, //Min Damage
            70, //Max Level
            65, //Min Level
            5500, //Max Gold Drop
            3750, //Min Gold Drop
            4000, //Max XP Drop
            1680, //Min XP Drop
            "[BOSS] Sea Dragon" //Unit Name
        );

        public static Enemy Hydra = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566850073279856650/Unit_ills_full_20072.webp", //URL
            425000, //Max Health
            305500, //Min Health
            2320, //Max Damage
            240, //Min Damage
            70, //Max Level
            68, //Min Level
            5500, //Max Gold Drop
            3750, //Min Gold Drop
            3700, //Max XP Drop
            2500, //Min XP Drop
            "[BOSS] Hydra" //Unit Name
        );

        public static Enemy CetusBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140200507899905/Gear-Cetus_Render.png", //URL
            245999, //Max Health
            178111, //Min Health
            2085, //Max Damage
            30, //Min Damage
            80, //Max Level
            70, //Min Level
            650, //Max Gold Drop
            225, //Min Gold Drop
            1650, //Max XP Drop
            350, //Min XP Drop
            "[BOSS] Cetus - Lord of Balance" //Unit Name
        );

        public static Enemy Pepe = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/566788410816069632/20170424-4O1GHDF3vyAAcQfG8B1r.webp", //URL
            1, //Max Health
            1, //Min Health
            1, //Max Damage
            1, //Min Damage
            1, //Max Level
            1, //Min Level
            0, //Max Gold Drop
            0, //Min Gold Drop
            0, //Max XP Drop
            0, //Min XP Drop
            "Pepe Himself [Placeholder]" //Unit Name
        );
        #endregion

        #region LV 80-90
        public static Enemy Phoenix = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308240682942464/Bennu.webp", //URL
            165000, //Max Health
            50000, //Min Health
            5580, //Max Damage
            100, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2000, //Max XP Drop
            200, //Min XP Drop
            "Phoenix the Reborn" //Unit Name
        );

        public static Enemy Bergelmir = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308295053836301/Bergelmir.png", //URL
            186500, //Max Health
            63500, //Min Health
            6200, //Max Damage
            330, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2200, //Max XP Drop
            120, //Min XP Drop
            "Bergelmir the Great" //Unit Name
        );

        public static Enemy Hastur = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308179408355329/Hastur.png", //URL
            198500, //Max Health
            66500, //Min Health
            5225, //Max Damage
            100, //Min Damage
            80, //Max Level
            70, //Min Level
            500, //Max Gold Drop
            65, //Min Gold Drop
            2200, //Max XP Drop
            100, //Min XP Drop
            "Hastur, Keeper of Mana" //Unit Name
        );

        public static Enemy Kirin = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308254318886913/Kirin.png", //URL
            174000, //Max Health
            51000, //Min Health
            6850, //Max Damage
            10, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2800, //Max XP Drop
            650, //Min XP Drop
            "Kirin the Storm" //Unit Name
        );

        public static Enemy Haechi = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308258085240863/Haechi.png", //URL
            195500, //Max Health
            79450, //Min Health
            5150, //Max Damage
            100, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            50, //Min Gold Drop
            2950, //Max XP Drop
            300, //Min XP Drop
            "Haechi, Son of Fire" //Unit Name
        );

        public static Enemy Orochi = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308244403290132/Eight-Headed_Orochi.png", //URL
            120000, //Max Health
            77777, //Min Health
            6500, //Max Damage
            330, //Min Damage
            80, //Max Level
            90, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2200, //Max XP Drop
            600, //Min XP Drop
            "Orochi, Head of Eight" //Unit Name
        );

        public static Enemy Eclipse = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308139965251584/Eclipse.webp", //URL
            113500, //Max Health
            91150, //Min Health
            5500, //Max Damage
            180, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2250, //Max XP Drop
            250, //Min XP Drop
            "Eclipse of the Storm" //Unit Name
        );

        public static Enemy Niflheim = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545321867477516298/Niflheim.webp", //URL
            170000, //Max Health
            74500, //Min Health
            6500, //Max Damage
            180, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2360, //Max XP Drop
            100, //Min XP Drop
            "Niflheim of Hell" //Unit Name
        );

        public static Enemy Muspelheim = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545321830706315274/Muspelheim.webp", //URL
            100500, //Max Health
            80500, //Min Health
            5500, //Max Damage
            180, //Min Damage
            80, //Max Level
            70, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            2500, //Max XP Drop
            150, //Min XP Drop
            "Muspelheim, Elemental of Heat" //Unit Name
        );
        #endregion

        #region LV 90-100
        public static Enemy AbyssDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545322174001709129/Abyss_Dragon.webp", //URL
            200500, //Max Health
            180500, //Min Health
            7900, //Max Damage
            380, //Min Damage
            100, //Max Level
            90, //Min Level
            450, //Max Gold Drop
            45, //Min Gold Drop
            4000, //Max XP Drop
            160, //Min XP Drop
            "Abyss Dragon" //Unit Name
        );

        public static Enemy Yamusichea = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323895511711764/Yamusichea.webp", //URL
            100550, //Max Health
            78050, //Min Health
            7600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            4120, //Max XP Drop
            25, //Min XP Drop
            "Yamusichea, the Almighty" //Unit Name
        );

        public static Enemy Flare = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323841715568640/Mega_Flare.webp", //URL
            180000, //Max Health
            100000, //Min Health
            8800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            4650, //Max XP Drop
            200, //Min XP Drop
            "Flare, Lord of Power" //Unit Name
        );

        public static Enemy Skeletor = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308131677306910/Skeletor_X.webp", //URL
            190000, //Max Health
            100000, //Min Health
            9800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            4805, //Max XP Drop
            1200, //Min XP Drop
            "Skeletor, Lord of the Abyss" //Unit Name
        );

        public static Enemy Khan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323665210867713/Khan.webp", //URL
            200000, //Max Health
            100000, //Min Health
            10800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            4800, //Max XP Drop
            200, //Min XP Drop
            "Khan, Lord of the Undead" //Unit Name
        );

        public static Enemy Crom = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308301089439754/Crom_Cruach.png", //URL
            195500, //Max Health
            88500, //Min Health
            10600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            520, //Max Gold Drop
            85, //Min Gold Drop
            4780, //Max XP Drop
            25, //Min XP Drop
            "Crom the Lich King" //Unit Name
        );

        public static Enemy Rex = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308291014721556/Bone_T._Rex.png", //URL
            312050, //Max Health
            250000, //Min Health
            12300, //Max Damage
            200, //Min Damage
            100, //Max Level
            98, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            4800, //Max XP Drop
            125, //Min XP Drop
            "Rex, Lord of the Underworld" //Unit Name
        );

        public static Enemy Vasuki = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323686866059286/Vasuki.webp", //URL
            318550, //Max Health
            27850, //Min Health
            14300, //Max Damage
            200, //Min Damage
            100, //Max Level
            99, //Min Level
            650, //Max Gold Drop
            250, //Min Gold Drop
            4650, //Max XP Drop
            1250, //Min XP Drop
            "Vasuki, Goddess of the Lakes" //Unit Name
        );

        public static Enemy Thunder = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323709301260300/image.webp", //URL
            308550, //Max Health
            207850, //Min Health
            16600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            4500, //Max XP Drop
            125, //Min XP Drop
            "Thunder, King of Hurricanes" //Unit Name
        );

        public static Enemy Taigo = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545323826112888842/Taigon.webp", //URL
           210000, //Max Health
           94500, //Min Health
           17600, //Max Damage
           400, //Min Damage
           100, //Max Level
           90, //Min Level
           480, //Max Gold Drop
           45, //Min Gold Drop
           4300, //Max XP Drop
           125, //Min XP Drop
           "Taigon, Lord of Beasts" //Unit Name
        );
        #endregion

        #region Aurora
        public static Enemy Duck = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/544791450068713483/439083435961745409.gif", //URL
           1145000, //Max Health
           1112500, //Min Health
           9250, //Max Damage
           100, //Min Damage
           100, //Max Level
           100, //Min Level
           1000, //Max Gold Drop
           500, //Min Gold Drop
           9000, //Max XP Drop
           3500, //Min XP Drop
           "Mighty Duck" //Unit Name
        );

        public static Enemy Masamune = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321849131892736/Masamune.webp", //URL
           3200000, //Max Health
           1940050, //Min Health
           7900, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1000, //Max Gold Drop
           800, //Min Gold Drop
           9000, //Max XP Drop
           3205, //Min XP Drop
           "Masamune the Spirit Lord" //Unit Name
        );

        public static Enemy Rourtu = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321889128644624/Destroyer.webp", //URL
           2250000, //Max Health
           1945000, //Min Health
           8800, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           9000, //Max XP Drop
           4205, //Min XP Drop
           "Rourtu the Punisher" //Unit Name
        );

        public static Enemy Trikento = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545308191462785053/Sea_Dragon.png", //URL
           2500000, //Max Health
           1945000, //Min Health
           7890, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           9000, //Max XP Drop
           4250, //Min XP Drop
           "Trikento the Sea Dragon" //Unit Name
        );

        public static Enemy Blackout = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321784577097745/Blackout.webp", //URL
           2500000, //Max Health
           1945000, //Min Health
           11000, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           9000, //Max XP Drop
           4250, //Min XP Drop
           "Blackout, Lord of Light" //Unit Name
        );

        public static Enemy Yggdrasil = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321805754269706/Yggdrasil.webp", //URL
           3000000, //Max Health
           1900450, //Min Health
           10000, //Max Damage
           10, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           9000, //Max XP Drop
           4250, //Min XP Drop
           "Yggdrasil, Queen of Life" //Unit Name
        );
        #endregion

        #region Halloween Spooktober Enemies
        public static Enemy Pumpkinling = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578405860081666/Unit_ills_full_18830.webp", //URL
           60, //Max Health
           20, //Min Health
           10, //Max Damage
           4, //Min Damage
           5, //Max Level
           1, //Min Level
           3, //Max Gold Drop
           1, //Min Gold Drop
           13, //Max XP Drop
           5, //Min XP Drop
           "Pumpkinling" //Unit Name
        );

        public static Enemy GiantSpider = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578395064205334/Unit_ills_full_60261.webp", //URL
           1895, //Max Health
           1500, //Min Health
           165, //Max Damage
           50, //Min Damage
           15, //Max Level
           10, //Min Level
           200, //Max Gold Drop
           50, //Min Gold Drop
           60, //Max XP Drop
           2, //Min XP Drop
           "Giant Spider" //Unit Name
        );

        public static Enemy GiantArachne = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578405344182282/Unit_ills_full_60262.webp", //URL
           3895, //Max Health
           2500, //Min Health
           235, //Max Damage
           100, //Min Damage
           30, //Max Level
           25, //Min Level
           300, //Max Gold Drop
           250, //Min Gold Drop
           130, //Max XP Drop
           10, //Min XP Drop
           "Giant Arachne" //Unit Name
        );

        public static Enemy Wraith = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578405033934864/Unit_ills_full_60103.webp", //URL
           4895, //Max Health
           3500, //Min Health
           535, //Max Damage
           300, //Min Damage
           40, //Max Level
           35, //Min Level
           400, //Max Gold Drop
           350, //Min Gold Drop
           280, //Max XP Drop
           45, //Min XP Drop
           "Wraith" //Unit Name
        );

        public static Enemy GreaterWraith = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578400445235260/Unit_ills_full_60104.webp", //URL
           14895, //Max Health
           13500, //Min Health
           1535, //Max Damage
           1300, //Min Damage
           70, //Max Level
           65, //Min Level
           1400, //Max Gold Drop
           1350, //Min Gold Drop
           480, //Max XP Drop
           245, //Min XP Drop
           "Greater Wraith" //Unit Name
        );

        public static Enemy Lodaga = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/637578404539006987/Unit_ills_full_60105.webp", //URL
           226895, //Max Health
           143500, //Min Health
           3535, //Max Damage
           500, //Min Damage
           111, //Max Level
           105, //Min Level
           27400, //Max Gold Drop
           18350, //Min Gold Drop
           6000, //Max XP Drop
           245, //Min XP Drop
           "[EVENT BOSS] Lodaga - Lord of Halloween" //Unit Name
        );
        #endregion

        #region Frost Legion Event Enemies
        public static Enemy Snowling = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/650402581264334883/Unit_ills_full_820212.webp", //URL
           60, //Max Health
           20, //Min Health
           10, //Max Damage
           4, //Min Damage
           5, //Max Level
           1, //Min Level
           3, //Max Gold Drop
           1, //Min Gold Drop
           13, //Max XP Drop
           5, //Min XP Drop
           "Snowling" //Unit Name
        );

        public static Enemy RedGoblin = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/650402574960295977/Unit_ills_full_10051.webp", //URL
           1895, //Max Health
           1500, //Min Health
           165, //Max Damage
           50, //Min Damage
           15, //Max Level
           10, //Min Level
           200, //Max Gold Drop
           50, //Min Gold Drop
           60, //Max XP Drop
           2, //Min XP Drop
           "Red Legion Goblin" //Unit Name
        );

        public static Enemy TabletKeeper = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/650402580375011338/Unit_ills_full_50792.webp", //URL
           3895, //Max Health
           2500, //Min Health
           235, //Max Damage
           100, //Min Damage
           30, //Max Level
           25, //Min Level
           300, //Max Gold Drop
           250, //Min Gold Drop
           130, //Max XP Drop
           10, //Min XP Drop
           "Frost Tablet Elgif" //Unit Name
        );

        public static Enemy IceIdol = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/650402578215075880/Unit_ills_full_20132.webp", //URL
           4895, //Max Health
           3500, //Min Health
           535, //Max Damage
           300, //Min Damage
           40, //Max Level
           35, //Min Level
           400, //Max Gold Drop
           350, //Min Gold Drop
           280, //Max XP Drop
           45, //Min XP Drop
           "Possessed Ice Idol" //Unit Name
        );

        public static Enemy GreaterWolf = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/650402577762091038/Unit_ills_full_20893.webp", //URL
           14895, //Max Health
           13500, //Min Health
           1535, //Max Damage
           1300, //Min Damage
           70, //Max Level
           65, //Min Level
           1400, //Max Gold Drop
           1350, //Min Gold Drop
           480, //Max XP Drop
           245, //Min XP Drop
           "Greater Dire Frost Wolf" //Unit Name
        );
        #endregion

        /*public static Enemy KenthrosBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670999648665600/Kenthros.png", //URL
            500, //Max Health
            500, //Min Health
            0, //Max Damage
            0, //Min Damage
            1, //Max Level
            1, //Min Level
            500, //Max Gold Drop
            500, //Min Gold Drop
            1, //Max XP Drop
            1, //Min XP Drop
            "[WORLD BOSS] Kenthros the Sea Lord - Nerfed Testing Edition" //Unit Name
        );*/
    }

    public class DesignedEnemy
    {
        public string name = "";
        public string flavor = "";
        public string iconURL = "";
        public int minLevel = 0;
        public int maxLevel = 0;
        public float difficulty = 0f;
        public float tankiness = 0f;
        public bool valid = false;

        public DesignedEnemy(string _name, string _flavor, string _iconURL, string _minLevel, string _maxLevel, string _difficulty, string _tankiness)
        {
            name = _name;

            while (_flavor.Contains("[MonsterName]"))
            {
                _flavor = _flavor.Replace("[MonsterName]", _name);
            }

            flavor = _flavor;
            iconURL = _iconURL;
            //Console.WriteLine("_minLevel: " + _minLevel + " for " + _name);
            //Console.WriteLine("_maxLevel: " + _maxLevel + " for " + _name);
            //Console.WriteLine("_difficulty: " + _difficulty + " for " + _name);
            //Console.WriteLine("_tankiness: " + _tankiness + " for " + _name);

            bool parsedMinLevel = int.TryParse(_minLevel, out minLevel);
            bool parsedMaxLevel = int.TryParse(_maxLevel, out maxLevel);
            bool parsedDifficulty = float.TryParse(_difficulty, out difficulty);
            bool parsedTankiness = float.TryParse(_tankiness, out tankiness);

            if (!parsedMinLevel)
                Console.WriteLine("[Designed Enemy] Error parsing min level for " + _name);
            if (!parsedMaxLevel)
                Console.WriteLine("[Designed Enemy] Error parsing max level for " + _name);
            if (!parsedDifficulty)
                Console.WriteLine("[Designed Enemy] Error parsing difficulty for " + _name);
            if (!parsedTankiness)
                Console.WriteLine("[Designed Enemy] Error parsing tankiness for " + _name);

            if (parsedMinLevel && parsedMaxLevel)
            {
                if (minLevel > maxLevel)
                {
                    Console.WriteLine("[Designed Enemy] Error, min level is greater than max level for " + _name + ". Min level: " + minLevel + ", Max Level: " + maxLevel);
                }
                else if (parsedMinLevel && parsedMaxLevel && parsedDifficulty && parsedTankiness)
                {
                    Console.WriteLine("[Designed Enemy] Successfully loaded " + _name);
                    valid = true;
                }
            }
        }
    }
}
