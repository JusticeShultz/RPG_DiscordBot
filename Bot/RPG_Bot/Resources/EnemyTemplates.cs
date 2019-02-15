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

        public Enemy(string url, uint maxHealth, uint minHealth, uint maxDamage, uint minDamage, uint maxLevel, uint minLevel, uint maxGoldDrop, uint minGoldDrop, uint maxXPDrop, uint minXPDrop, string name)
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
            25, 
            1, 
            2, 
            1, 
            "Goblin"
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

        public static Enemy Golem = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140066369863682/Unit_ills_full_30932.png", //URL
            800, //Max Health
            500, //Min Health
            50, //Max Damage
            15, //Min Damage
            20, //Max Level
            15, //Min Level
            45, //Max Gold Drop
            1, //Min Gold Drop
            15, //Max XP Drop
            3, //Min XP Drop
            "Golem" //Unit Name
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
            5, //Max XP Drop
            1, //Min XP Drop
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
            18, //Max XP Drop
            8, //Min XP Drop
            "Hob Goblin" //Unit Name
        );

        public static Enemy TrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140268845694987/giphy_1.gif", //URL
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
        );

        public static Enemy LargeTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140269915373588/Crablops.gif", //URL
            150, //Max Health
            100, //Min Health
            10, //Max Damage
            6, //Min Damage
            10, //Max Level
            5, //Min Level
            35, //Max Gold Drop
            1, //Min Gold Drop
            5, //Max XP Drop
            1, //Min XP Drop
            "Upgraded Training Bot" //Unit Name
        );

        public static Enemy BigTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140266589159428/giphy_2.gif", //URL
            200, //Max Health
            150, //Min Health
            15, //Max Damage
            8, //Min Damage
            10, //Max Level
            7, //Min Level
            35, //Max Gold Drop
            10, //Min Gold Drop
            8, //Max XP Drop
            3, //Min XP Drop
            "Armed Training Bot" //Unit Name
        );

        public static Enemy BossTrainingBot = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140272213852219/r9zecHs.gif", //URL
            2000, //Max Health
            1500, //Min Health
            40, //Max Damage
            15, //Min Damage
            10, //Max Level
            5, //Min Level
            250, //Max Gold Drop
            120, //Min Gold Drop
            25, //Max XP Drop
            10, //Min XP Drop
            "[BOSS] Kerthgore the Mad Scientist" //Unit Name
        );

        public static Enemy HauntedTree = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140199379501077/Unit_ills_full_30082.png", //URL
            400, //Max Health
            300, //Min Health
            25, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            10, //Max XP Drop
            5, //Min XP Drop
            "Haunted Tree" //Unit Name
        );

        public static Enemy GoblinWolf = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140112301817857/Unit_ills_full_40112.png", //URL
            300, //Max Health
            150, //Min Health
            20, //Max Damage
            10, //Min Damage
            15, //Max Level
            10, //Min Level
            25, //Max Gold Drop
            5, //Min Gold Drop
            8, //Max XP Drop
            1, //Min XP Drop
            "Goblin Wolf" //Unit Name
        );

        public static Enemy TreeEnt = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139848987607050/Unit_ills_full_30050.png", //URL
            250, //Max Health
            150, //Min Health
            25, //Max Damage
            15, //Min Damage
            15, //Max Level
            10, //Min Level
            20, //Max Gold Drop
            5, //Min Gold Drop
            6, //Max XP Drop
            1, //Min XP Drop
            "Tree Ent" //Unit Name
        );

        public static Enemy TreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139896827707402/Unit_ills_full_30083.png", //URL
            4500, //Max Health
            3500, //Min Health
            15, //Max Damage
            5, //Min Damage
            15, //Max Level
            10, //Min Level
            250, //Max Gold Drop
            45, //Min Gold Drop
            35, //Max XP Drop
            15, //Min XP Drop
            "[BOSS] Trenthola the Living Tree" //Unit Name
        );

        public static Enemy EventTreeBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139899109277696/image_1.webp", //URL
            25000, //Max Health
            19500, //Min Health
            35, //Max Damage
            15, //Min Damage
            160, //Max Level
            100, //Min Level
            7500, //Max Gold Drop
            4500, //Min Gold Drop
            120, //Max XP Drop
            60, //Min XP Drop
            "[EVENT BOSS] Trelia the Mystic Tree" //Unit Name
        );

        public static Enemy GothkamulBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670992786653193/Gothkamul.png", //URL
            3500000, //Max Health
            1850000, //Min Health
            80, //Max Damage
            15, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            250, //Max XP Drop
            200, //Min XP Drop
            "[WORLD BOSS] Gothkamul the Death Lord" //Unit Name
        );

        public static Enemy RakdoroBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670990957936641/Rakdoro.png", //URL
            2500000, //Max Health
            1850000, //Min Health
            120, //Max Damage
            40, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            250, //Max XP Drop
            200, //Min XP Drop
            "[WORLD BOSS] Rakdoro the Flame Lord" //Unit Name
        );

        public static Enemy ArkdulBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670996356136990/Arkdul.png", //URL
            6000000, //Max Health
            4550000, //Min Health
            50, //Max Damage
            10, //Min Damage
            500, //Max Level
            400, //Min Level
            35000, //Max Gold Drop
            20000, //Min Gold Drop
            250, //Max XP Drop
            200, //Min XP Drop
            "[WORLD BOSS] Arkdul the Earth Lord" //Unit Name
        );

        public static Enemy KenthrosBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/544670999648665600/Kenthros.png", //URL
            2000000, //Max Health
            1550000, //Min Health
            80, //Max Damage
            30, //Min Damage
            500, //Max Level
            400, //Min Level
            25000, //Max Gold Drop
            13000, //Min Gold Drop
            225, //Max XP Drop
            200, //Min XP Drop
            "[WORLD BOSS] Kenthros the Sea Lord" //Unit Name
        );

        public static Enemy FrostWolf = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140199950188564/Unit_ills_full_20892.png", //URL
            4500, //Max Health
            3200, //Min Health
            250, //Max Damage
            40, //Min Damage
            40, //Max Level
            20, //Min Level
            35, //Max Gold Drop
            12, //Min Gold Drop
            30, //Max XP Drop
            5, //Min XP Drop
            "Frost Wolf" //Unit Name
        );

        public static Enemy FrostKnight = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139940150804482/Unit_ills_full_20172.png", //URL
            5000, //Max Health
            3500, //Min Health
            200, //Max Damage
            75, //Min Damage
            40, //Max Level
            20, //Min Level
            55, //Max Gold Drop
            13, //Min Gold Drop
            45, //Max XP Drop
            10, //Min XP Drop
            "Frost Knights" //Unit Name
        );

        public static Enemy BossIceDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139899109277697/image_5.png", //URL
            15000, //Max Health
            12500, //Min Health
            200, //Max Damage
            40, //Min Damage
            40, //Max Level
            20, //Min Level
            350, //Max Gold Drop
            125, //Min Gold Drop
            150, //Max XP Drop
            70, //Min XP Drop
            "[BOSS] Frost Dragon" //Unit Name
        );

        public static Enemy BossFrostWolfPackLeader = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140114549833766/Unit_ills_full_21006.png", //URL
            18000, //Max Health
            14500, //Min Health
            225, //Max Damage
            50, //Min Damage
            40, //Max Level
            20, //Min Level
            400, //Max Gold Drop
            180, //Min Gold Drop
            200, //Max XP Drop
            120, //Min XP Drop
            "[BOSS] Frost Wolf - Pack Leader" //Unit Name
        );

        public static Enemy Megaton = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308191978946610/Megaton.png", //URL
            9000, //Max Health
            7000, //Min Health
            300, //Max Damage
            70, //Min Damage
            60, //Max Level
            40, //Min Level
            75, //Max Gold Drop
            25, //Min Gold Drop
            55, //Max XP Drop
            20, //Min XP Drop
            "Megaton" //Unit Name
        );

        public static Enemy LizardMan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543139848987607051/image_6.png", //URL
            12000, //Max Health
            10000, //Min Health
            320, //Max Damage
            70, //Min Damage
            60, //Max Level
            40, //Min Level
            75, //Max Gold Drop
            25, //Min Gold Drop
            55, //Max XP Drop
            20, //Min XP Drop
            "Lizard Man" //Unit Name
        );

        public static Enemy Leviathan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308182235447305/Leviathan.png", //URL
            40000, //Max Health
            30000, //Min Health
            320, //Max Damage
            70, //Min Damage
            60, //Max Level
            40, //Min Level
            450, //Max Gold Drop
            225, //Min Gold Drop
            120, //Max XP Drop
            60, //Min XP Drop
            "Leviathan" //Unit Name
        );

        public static Enemy CetusBoss = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/543140200507899905/Gear-Cetus_Render.png", //URL
            120000, //Max Health
            85000, //Min Health
            300, //Max Damage
            30, //Min Damage
            60, //Max Level
            40, //Min Level
            650, //Max Gold Drop
            225, //Min Gold Drop
            180, //Max XP Drop
            90, //Min XP Drop
            "[BOSS] Cetus - Lord of Balance" //Unit Name
        );

        public static Enemy Phoenix = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308240682942464/Bennu.webp", //URL
            165000, //Max Health
            145000, //Min Health
            500, //Max Damage
            180, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Phoenix the Reborn" //Unit Name
        );

        public static Enemy Bergelmir = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308295053836301/Bergelmir.png", //URL
            185000, //Max Health
            145000, //Min Health
            400, //Max Damage
            230, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Bergelmir the Great" //Unit Name
        );

        public static Enemy Hastur = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308179408355329/Hastur.png", //URL
            555000, //Max Health
            445000, //Min Health
            325, //Max Damage
            200, //Min Damage
            90, //Max Level
            60, //Min Level
            500, //Max Gold Drop
            65, //Min Gold Drop
            225, //Max XP Drop
            50, //Min XP Drop
            "Hastur, Keeper of Mana" //Unit Name
        );

        public static Enemy Kirin = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308254318886913/Kirin.png", //URL
            100000, //Max Health
            95000, //Min Health
            800, //Max Damage
            400, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Kirin the Storm" //Unit Name
        );

        public static Enemy Haechi = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308258085240863/Haechi.png", //URL
            195000, //Max Health
            145000, //Min Health
            750, //Max Damage
            330, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            50, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Haechi, Son of Fire" //Unit Name
        );

        public static Enemy Orochi = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308244403290132/Eight-Headed_Orochi.png", //URL
            185000, //Max Health
            145000, //Min Health
            500, //Max Damage
            330, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Orochi, Head of Eight" //Unit Name
        );

        public static Enemy Eclipse = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308139965251584/Eclipse.webp", //URL
            165000, //Max Health
            145000, //Min Health
            500, //Max Damage
            180, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Eclipse of the Storm" //Unit Name
        );

        public static Enemy Niflheim = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545321867477516298/Niflheim.webp", //URL
            165000, //Max Health
            145000, //Min Health
            500, //Max Damage
            180, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Niflheim of Hell" //Unit Name
        );

        public static Enemy Muspelheim = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545321830706315274/Muspelheim.webp", //URL
            165000, //Max Health
            145000, //Min Health
            500, //Max Damage
            180, //Min Damage
            90, //Max Level
            60, //Min Level
            250, //Max Gold Drop
            25, //Min Gold Drop
            180, //Max XP Drop
            15, //Min XP Drop
            "Muspelheim, Elemental of Heat" //Unit Name
        );

        public static Enemy AbyssDragon = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545322174001709129/Abyss_Dragon.webp", //URL
            755000, //Max Health
            713000, //Min Health
            900, //Max Damage
            380, //Min Damage
            100, //Max Level
            90, //Min Level
            450, //Max Gold Drop
            45, //Min Gold Drop
            300, //Max XP Drop
            25, //Min XP Drop
            "Abyss Dragon" //Unit Name
        );

        public static Enemy Yamusichea = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323895511711764/Yamusichea.webp", //URL
            855000, //Max Health
            785000, //Min Health
            600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            300, //Max XP Drop
            25, //Min XP Drop
            "Yamusichea, the Almighty" //Unit Name
        );

        public static Enemy Flare = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323841715568640/Mega_Flare.webp", //URL
            1500000, //Max Health
            1000000, //Min Health
            800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            400, //Max XP Drop
            200, //Min XP Drop
            "Flare, Lord of Power" //Unit Name
        );

        public static Enemy Skeletor = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308131677306910/Skeletor_X.webp", //URL
            1500000, //Max Health
            1000000, //Min Health
            800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            400, //Max XP Drop
            200, //Min XP Drop
            "Skeletor, Lord of the Abyss" //Unit Name
        );

        public static Enemy Khan = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323665210867713/Khan.webp", //URL
            1500000, //Max Health
            1000000, //Min Health
            800, //Max Damage
            600, //Min Damage
            100, //Max Level
            98, //Min Level
            750, //Max Gold Drop
            500, //Min Gold Drop
            400, //Max XP Drop
            200, //Min XP Drop
            "Khan, Lord of the Undead" //Unit Name
        );

        public static Enemy Crom = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308301089439754/Crom_Cruach.png", //URL
            955000, //Max Health
            885000, //Min Health
            600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            520, //Max Gold Drop
            85, //Min Gold Drop
            310, //Max XP Drop
            25, //Min XP Drop
            "Crom the Lich King" //Unit Name
        );

        public static Enemy Rex = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545308291014721556/Bone_T._Rex.png", //URL
            1205000, //Max Health
            1000000, //Min Health
            300, //Max Damage
            200, //Min Damage
            100, //Max Level
            98, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            300, //Max XP Drop
            25, //Min XP Drop
            "Rex, Lord of the Underworld" //Unit Name
        );

        public static Enemy Vasuki = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323686866059286/Vasuki.webp", //URL
            1855000, //Max Health
            1785000, //Min Health
            300, //Max Damage
            200, //Min Damage
            100, //Max Level
            99, //Min Level
            650, //Max Gold Drop
            250, //Min Gold Drop
            400, //Max XP Drop
            250, //Min XP Drop
            "Vasuki, Goddess of the Lakes" //Unit Name
        );

        public static Enemy Thunder = new Enemy
        (
            "https://cdn.discordapp.com/attachments/542225685695954945/545323709301260300/image.webp", //URL
            855000, //Max Health
            785000, //Min Health
            600, //Max Damage
            400, //Min Damage
            100, //Max Level
            90, //Min Level
            480, //Max Gold Drop
            45, //Min Gold Drop
            300, //Max XP Drop
            25, //Min XP Drop
            "Thunder, King of Hurricanes" //Unit Name
        );

        public static Enemy Taigo = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545323826112888842/Taigon.webp", //URL
           1000000, //Max Health
           945000, //Min Health
           600, //Max Damage
           400, //Min Damage
           100, //Max Level
           90, //Min Level
           480, //Max Gold Drop
           45, //Min Gold Drop
           300, //Max XP Drop
           25, //Min XP Drop
           "Taigon, Lord of Beasts" //Unit Name
        );

        public static Enemy Masamune = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321849131892736/Masamune.webp", //URL
           2000000, //Max Health
           1945000, //Min Health
           800, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1000, //Max Gold Drop
           800, //Min Gold Drop
           600, //Max XP Drop
           325, //Min XP Drop
           "Masamune the Spirit Lord" //Unit Name
        );

        public static Enemy Rourtu = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321889128644624/Destroyer.webp", //URL
           2500000, //Max Health
           1945000, //Min Health
           1000, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           700, //Max XP Drop
           425, //Min XP Drop
           "Rourtu the Punisher" //Unit Name
        );

        public static Enemy Duck = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/544791450068713483/439083435961745409.gif", //URL
           450000, //Max Health
           125000, //Min Health
           1250, //Max Damage
           100, //Min Damage
           100, //Max Level
           100, //Min Level
           1000, //Max Gold Drop
           500, //Min Gold Drop
           300, //Max XP Drop
           150, //Min XP Drop
           "Mighty Duck" //Unit Name
        );

        public static Enemy Trikento = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545308191462785053/Sea_Dragon.png", //URL
           2500000, //Max Health
           1945000, //Min Health
           1000, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           700, //Max XP Drop
           425, //Min XP Drop
           "Trikento the Sea Dragon" //Unit Name
        );

        public static Enemy Blackout = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321784577097745/Blackout.webp", //URL
           2500000, //Max Health
           1945000, //Min Health
           1000, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           700, //Max XP Drop
           425, //Min XP Drop
           "Blackout, Lord of Light" //Unit Name
        );

        public static Enemy Yggdrasil = new Enemy
        (
           "https://cdn.discordapp.com/attachments/542225685695954945/545321805754269706/Yggdrasil.webp", //URL
           2500000, //Max Health
           1945000, //Min Health
           1000, //Max Damage
           600, //Min Damage
           100, //Max Level
           100, //Min Level
           1450, //Max Gold Drop
           900, //Min Gold Drop
           700, //Max XP Drop
           425, //Min XP Drop
           "Yggdrasil, Queen of Life" //Unit Name
        );

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
}
