using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;
using Discord.WebSocket;
using System.Linq;
using RPG_Bot.Resources.Database;
using RPG_Bot.Resources;
using RPG_Bot;
using Microsoft.Extensions.DependencyInjection;
using RPG_Bot.Commands;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Diagnostics;
using static RPG_Bot.Emojis.Emojis;

namespace RPG_Bot.Commands
{
    public class Gameplay : ModuleBase<SocketCommandContext>
    {
        //Handles every server having their own enemies.
        public class ServerIntegratedEnemy
        {
            public Enemy[] currentSpawn { get; set; } = new Enemy[200];
            public ulong ServerID { get; set; } = 0;

            public ServerIntegratedEnemy()
            {
            }

            ~ServerIntegratedEnemy()
            {
            }
        }

        public class Joiner
        {
            public ulong UserID = 0;
            public ulong Damage = 0;
            public Joiner()
            {
                UserID = 0;
                Damage = 0;
            }
            public Joiner(ulong id, ulong damage)
            {
                UserID = id;
                Damage = damage;
            }
            ~Joiner() { }
        }

        //Handles every server having its own world boss participants
        public class BossJoiningSystem
        {
            //Boss Joiners [Object Pools]
            public Joiner[] Boss1Joiner = new Joiner[100];
            public Joiner[] Boss2Joiner = new Joiner[100];
            public Joiner[] Boss3Joiner = new Joiner[100];
            public Joiner[] Boss4Joiner = new Joiner[100];
            public Joiner[] BossEventJoiner = new Joiner[100];
            public ulong ServerID { get; set; } = 0;
            public BossJoiningSystem()
            {
                for (int i = 0; i < 100; ++i)
                {
                    Boss1Joiner[i] = new Joiner();
                    Boss2Joiner[i] = new Joiner();
                    Boss3Joiner[i] = new Joiner();
                    Boss4Joiner[i] = new Joiner();
                    BossEventJoiner[i] = new Joiner();
                }
            }
            ~BossJoiningSystem() { }
        }

        //Handles all editable objects in every single server.
        public class ServerEditMessages
        {
            public Discord.Rest.RestUserMessage[] FightMessages = new Discord.Rest.RestUserMessage[200];
            public ulong ServerID { get; set; } = 0;
            public ServerEditMessages() { }
            ~ServerEditMessages() { }
        }

        static ServerIntegratedEnemy[] CurrentSpawn = new ServerIntegratedEnemy[2500];
        static BossJoiningSystem[] CurrentBossJoiners = new BossJoiningSystem[2500];
        static ServerEditMessages[] ServerMessages = new ServerEditMessages[2500];
        public static List<DesignedEnemy> designedEnemiesIndex = new List<DesignedEnemy>();

        public static Dictionary<string, Spell> spellDatabase = new Dictionary<string, Spell>();

        static Random rng = new Random();

        //Channel 1-25 reserved for zones
        //Channel 35 reserved for event bosses
        //Channel 40 - 45 reserved for world bosses
        static public async Task ServerConnector()
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Server connection has started");
            int i = 0;
            CurrentSpawn[i] = new ServerIntegratedEnemy();
            //CurrentSpawn[2].ServerID = 55;
            Console.WriteLine("Connecting to " + RPG_Bot.Program.Client.Guilds.Count + " Guilds\n" +
                "There are " + CurrentSpawn.Count() + " available object pool slots.");
            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if (CurrentSpawn[i] == null)
                    CurrentSpawn[i] = new ServerIntegratedEnemy();
                CurrentSpawn[i].ServerID = guilds.Id;
                if (CurrentBossJoiners[i] == null)
                    CurrentBossJoiners[i] = new BossJoiningSystem();
                CurrentBossJoiners[i].ServerID = guilds.Id;
                if (ServerMessages[i] == null)
                    ServerMessages[i] = new ServerEditMessages();
                ServerMessages[i].ServerID = guilds.Id;
                Console.WriteLine("Connected to: " + guilds.Name);
                ++i;
            }
            await Gameplay.UpdateUserData();
            await UpdateUserData();
            Console.WriteLine("Servers have made a full connection");
            Console.WriteLine("---------------------------------------------------------------");
        }

        static public List<Helmet> droppedHelmets = new List<Helmet>();
        static public List<Chestplate> droppedChestplates = new List<Chestplate>();
        static public List<Gauntlets> droppedGauntlets = new List<Gauntlets>();
        static public List<Belt> droppedBelt = new List<Belt>();
        static public List<Leggings> droppedLeggings = new List<Leggings>();
        static public List<Boots> droppedBoots = new List<Boots>();
        static public List<Artifact> droppedArtifacts = new List<Artifact>();

        //Project E
        [Command("Explore"), Alias("explore", "Ex", "ex", "Search", "search", "S", "s"), Summary("Explore the area you typed the message in.")]
        public async Task Explore(string remaining = null)
        {
            int serverId = 0;
            bool registered = false;

            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if (guilds.Id == Context.Guild.Id)
                {
                    registered = true;
                    break;
                }
                else ++serverId;
            }

            #region Zone Explored Check
            //Silverkeep Training Fields
            if (Context.Channel.Name == "training-forts")
            {
                if(!Data.Data.Explored(Context.User.Id, "Training_Forts"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "rayels-terror-tower")
            {
                if (!Data.Data.Explored(Context.User.Id, "Rayels_Terror_Tower"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "skeleton-caves")
            {
                if (!Data.Data.Explored(Context.User.Id, "Skeleton_Caves"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Graveyard Groves
            else if (Context.Channel.Name == "forest-path")
            {
                if (!Data.Data.Explored(Context.User.Id, "Forest_Path"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "goblin-outpost")
            {
                if (!Data.Data.Explored(Context.User.Id, "Goblin_Outpost"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "tomb-of-heroes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Tomb_of_Heroes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "lair-of-trenthola")
            {
                if (!Data.Data.Explored(Context.User.Id, "Lair_of_Trenthola"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Frigid Quarantine
            else if (Context.Channel.Name == "snowy-woods")
            {
                if (!Data.Data.Explored(Context.User.Id, "Snowy_Woods"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "makkosi-camp")
            {
                if (!Data.Data.Explored(Context.User.Id, "Makkosi_Camp"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "frigid-wastes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Frigid_Wastes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "abandoned-village")
            {
                if (!Data.Data.Explored(Context.User.Id, "Abandoned_Village"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "frozen-peaks")
            {
                if (!Data.Data.Explored(Context.User.Id, "Frozen_Peaks"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Harpy's Redoubt
            else if (Context.Channel.Name == "cliffside-barricade")
            {
                if (!Data.Data.Explored(Context.User.Id, "Cliffside_Barricade"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "roosting-crags")
            {
                if (!Data.Data.Explored(Context.User.Id, "Roosting_Crags"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "taken-temple")
            {
                if (!Data.Data.Explored(Context.User.Id, "Taken_Temple"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "undying-storm")
            {
                if (!Data.Data.Explored(Context.User.Id, "Undying_Storm"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Pelean Shores
            else if (Context.Channel.Name == "crawling-coastline")
            {
                if (!Data.Data.Explored(Context.User.Id, "Crawling_Coastline"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "megaton-cove")
            {
                if (!Data.Data.Explored(Context.User.Id, "Megaton_Cove"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "lunar-temple")
            {
                if (!Data.Data.Explored(Context.User.Id, "Lunar_Temple"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "hms-reliant")
            {
                if (!Data.Data.Explored(Context.User.Id, "Hms_Reliant"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Salamander Reach
            else if (Context.Channel.Name == "logada-summits")
            {
                if (!Data.Data.Explored(Context.User.Id, "Logada_Summits"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "burning-outcrop")
            {
                if (!Data.Data.Explored(Context.User.Id, "Burning_Outcrop"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "magma-pits")
            {
                if (!Data.Data.Explored(Context.User.Id, "Magma_Pits"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "draconic-nests")
            {
                if (!Data.Data.Explored(Context.User.Id, "Draconic_Nests"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Riverside Mire
            else if (Context.Channel.Name == "twisted-riverbed")
            {
                if (!Data.Data.Explored(Context.User.Id, "Twisted_Riverbed"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "quarantined-village")
            {
                if (!Data.Data.Explored(Context.User.Id, "Quarantined_Village"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "the-breach")
            {
                if (!Data.Data.Explored(Context.User.Id, "The_Breach"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "within-the-breach")
            {
                if (!Data.Data.Explored(Context.User.Id, "Within_the_Breach"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //City of the Lost
            else if (Context.Channel.Name == "outer-blockade")
            {
                if (!Data.Data.Explored(Context.User.Id, "Outer_Blockade"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "shattered-streets")
            {
                if (!Data.Data.Explored(Context.User.Id, "Shattered_Streets"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "catacombs")
            {
                if (!Data.Data.Explored(Context.User.Id, "Catacombs"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "corrupt-citadel")
            {
                if (!Data.Data.Explored(Context.User.Id, "Corrupt_Citadel"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Northern Wilds
            else if (Context.Channel.Name == "ruined-outpost")
            {
                if (!Data.Data.Explored(Context.User.Id, "Ruined_Outpost"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "sombris-monument")
            {
                if (!Data.Data.Explored(Context.User.Id, "Sombris_Monument"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "end-of-asteria")
            {
                if (!Data.Data.Explored(Context.User.Id, "End_of_Asteria"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "borealis-gates")
            {
                if (!Data.Data.Explored(Context.User.Id, "Borealis_Gates"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Aurora
            else if (Context.Channel.Name == "stellar-fields")
            {
                if (!Data.Data.Explored(Context.User.Id, "Stellar_Fields"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "astral-reaches")
            {
                if (!Data.Data.Explored(Context.User.Id, "Astral_Reaches"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "shifted-wastes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Shifted_Wastes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "fell-pantheon")
            {
                if (!Data.Data.Explored(Context.User.Id, "Fell_Pantheon"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            #endregion

            //Server must be catalogued.
            if (!registered) return;

            //Silverkeep Training Fields
            if (Context.Channel.Name == "entry-exam")
            {
                Console.WriteLine(Data.Data.GetData_Level(Context.User.Id));

                if (Data.Data.GetData_Level(Context.User.Id) == 0 || Data.Data.GetData_Level(Context.User.Id) == 1)
                {
                    await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TrainingBot, 1);
                    return;
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You've already finished your exam.");
                    Embed.WithDescription("You walk up to the entrance of the training grounds.\n\nThe guards standing out front put their pikes together in an X shape and refuse to let you in to destroy another bot. They insist you move on to the next area after explaining how this area is only for brand new adventurers.\n\n\n***You turn around and continue on your way.***");
                    Embed.Color = Discord.Color.DarkRed;
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657016106510319625/400.png");
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
            }
            else
            {
                //Signifies the zones level min/max
                int levelTargetMax = 0;
                int levelTargetMin = 0;

                //Signifies what we are doing here
                #pragma warning disable CS0219 // Variable is assigned but its value is never used
                bool searchingForEnemy = false;
                bool searchingForLoot = false;
                bool searchingForArea = false;
                bool searchingForLore = false;
                bool searchingForBoss = false;
                bool searchingForEvent = false; //Used for things like Snowdown, Spooktober, etc to add events to zones.
                bool searchingForNothing = false;
                #pragma warning restore CS0219 // Variable is assigned but its value is never used

                //Rolls a thousand sided dice.
                Random dice = new Random();
                int roll = dice.Next(0, 1000);

                //Silverkeep Training Fields
                if (Context.Channel.Name == "training-forts")
                {
                    if(CurrentSpawn[serverId].currentSpawn[2].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500)
                        searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Advanced Automaton", 2);
                        else if (roll < 200) await SpawnDesignedEnemy("Armed Automaton", 2);
                        else if (roll < 300) await SpawnDesignedEnemy("Log Spider", 2);
                        else if (roll < 400) await SpawnDesignedEnemy("Pumpkinling", 2);
                        else await SpawnDesignedEnemy("Training Automaton", 2);

                        //if (roll < 100) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BigTrainingBot, 2);
                        //else if (roll < 200) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LargeTrainingBot, 2);
                        //else if (roll < 300) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LogSpider, 2);
                        //else if (roll < 400) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pumpkinling, 2);
                        //else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TrainingBot, 2);
                    }
                    else if (searchingForLoot)
                    {
                        if (roll < 510)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("You find a small chest on the ground!");
                            Embed.WithDescription("Laying on the ground you find a small chest. The chest is beat up seems to have been sitting in the rain untouched for a while, most likely dropped by a merchant while being attacked by rabid Pumpkinlings.\n\n\nYou pick it up and open it.");
                            Embed.Color = Discord.Color.Gold;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());

                            await LootItem(Context, "small chest", 3, 1);
                        }
                        else if (roll < 550)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("You find a small bag of gold on the ground!");
                            Embed.WithDescription("Laying on the ground you find a small bag of gold. The sack the gold is in looks torn and seems to have been sitting in the rain untouched for a while.\n\nYou pick it up and open it to find 55 shiny gold coins!\n\n\nYou add the 55" + Coins + "'s to your bag.\n\n\n***You happily continue on your way...***");
                            Embed.Color = Discord.Color.Gold;
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657033817877381133/bag_1.png");
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            await Data.Data.SaveData(Context.User.Id, 55, 0, "", 0, 0, 0, 0, 0);
                        }
                        else if (roll < 600)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("You find a large chest on the ground!");
                            Embed.WithDescription("Laying on the ground you find a large chest. The chest is lined with gold and seems to have been freshly dropped, most likely dropped by a merchant while being attacked by rabid Pumpkinlings.\n\n\nYou pick it up and open it.");
                            Embed.Color = Discord.Color.Orange;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());

                            await LootItem(Context, "large chest", 3, 2);
                        }
                    }
                    else if (searchingForNothing)
                    {
                        if (roll < 585)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Nothing found...");
                            Embed.WithDescription("You search and search with nothing interesting found for miles...\n\n\n***You start to head in a different direction in hopes to actually find something...***");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 590)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Encouraging Mural");
                            Embed.WithDescription("After one battle too many, you collapse in a half-destroyed wood shelter for a breather. Once your head stops spinning, you open your eyes to see a quaint mural on the wall opposite you, boasting a battered knight with his sword held high and a banner reading ***DON'T GIVE UP, HERO!***.\n\n\n" +
                                "You can’t help but be inspired by the aged, tacky painting. You take a deep breath, get to your feet, and raise your weapon to the stylized knight in brief solidarity. Then, it’s out you go, to face your next opponent.");
                            Embed.Color = Discord.Color.DarkBlue;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 610)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Mad Cackling");
                            Embed.WithDescription("As you wander about, you hear a strange, mad laughter from across the field, followed by a resounding boom. You consider investigating, but if something actually dangerous is in the Training Forts, you’re sure it’s best to let the guards take care of it.\n\n\nAnd this kind of sounds actually dangerous.\n\n\nYou steer clear.");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 640)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Robot Graveyard");
                            Embed.WithDescription("Though Training Automatons are enchanted to rebuild themselves after a period of time, the Training Forts are speckled with broken parts from bots too wrecked to self-salvage. That’s to be expected — what you don’t expect is to find a massive pile of broken parts, all struggling in vain to reclaim their bodies. It’s kind of sad, but mostly creepy, especially when all the parts notice you at once and try to get you.\n\n\n" +
                                "Nope. Someone else can handle this one. You turn around and walk away, just like your mother taught you to.");
                            Embed.Color = Discord.Color.DarkRed;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 660)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Tampered Bot");
                            Embed.WithDescription("You spy a Training Bot idling about and creep closer to attack, only for your blow to bounce off of an unseen barrier. No matter what angle you try, you can’t penetrate its invisible defense — and you get so caught up in trying that you don’t realize until afterwards that it can’t seem to see you." +
                                "\n\n\nThat, or it’s minding its own business quite merrily. It’s even singing a little electronic ditty. You’re stumped, so you decide to do the same, and carry on your way.");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll < 690)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Nothing found...");
                            Embed.WithDescription("The sun shines brightly on you as you search and explore.\n\n\n***You continue to explore.***");
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931633465688086/Dungeon_battle_50300.webp");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 710)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Weird Vandalism");
                            Embed.WithDescription("Someone declaring themself `Forge` has carved their name on a whole cluster of wooden lean-tos. Could it be an adventurer gone by? Or maybe just some bored Silverkeep teenager? No one will ever know. Well, at least, you won’t.");
                            Embed.Color = Discord.Color.DarkGreen;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 755)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("How Is This Still Standing?");
                            Embed.WithDescription("Most of the structures in the field have long-since been hacked, slashed, slammed, and blasted into oblivion, but you manage to find what may be the only intact building in the entirety of the Bastion, aside from the tower. It’s something of a miracle, but you know what has to be done.\n\n\n***Destroying it is incredibly fun.***");
                            Embed.Color = Discord.Color.DarkGrey;
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657281185663156280/NicePng_ruins-png_2282587.png");
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 758)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("A Little Bit Lost");
                            Embed.WithDescription("Getting turned around in the Training Forts is easier than one might think based on its size. While the open field is easily navigable, the forts themselves form a maze of broken planks and muddy trenches. You get lost three times without ever encountering an adversary before you finally find an observant Log Spider giggling at your misfortune." +
                                "\n\n\nYour vengeance is quick! But once it’s done, you realize you haven’t the slightest idea of how to escape the tangle of forts you find yourself in. Doing so ultimately takes at least an hour. This is definitely the Log Spider’s fault.");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 760)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("I Know You Are, But What Am I?");
                            Embed.WithDescription("Just when you’re about to land the finishing blow on a brutal Armed Automaton, a rogue swoops out of nowhere and steals your victory! She lingers for a moment to smirk at you from atop the deactivated bot, then leaps and grapples out of sight. You never find more than a footprint, but you suspect you haven’t seen the last of her…");
                            Embed.Color = Discord.Color.DarkRed;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll == 785)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Just Ignore It");
                            Embed.WithDescription("Nope. You don’t want to look at the ominous tower hanging over the Training Fields, so you’re not going to. That’s it. That’s the end of that story. Something about it just skeeves you off, and the rumors you’ve heard in town about what it might be don’t comfort you at all.");
                            Embed.Color = Discord.Color.DarkPurple;
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll < 800)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Nothing found...");
                            Embed.WithDescription("The sky is blue with little to no clouds to be seen in the sky!\n\n\n***You continue to explore as you enjoy the nice weather.***");
                            Embed.Color = Discord.Color.Red;
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931596857802762/Dungeon_battle_60200.webp");
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                    }
                    else if (searchingForLore)
                    {
                        if (roll < 815)
                        {
                            EmbedBuilder Embed1 = new EmbedBuilder();
                            Embed1.WithTitle("You find a note on the ground...");
                            Embed1.WithDescription("*You open it and read it to yourself*");
                            Embed1.Color = Discord.Color.DarkerGrey;
                            Embed1.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282406608273427/stabbed-note.png");
                            Embed1.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed1.Build());

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Mysterious Note");
                            Embed.WithDescription("Beware the goblin in the iron suit!\n\n\n*The note reads... What does it mean? You think to yourself.*");
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282406608273427/stabbed-note.png");
                            Embed.Color = Discord.Color.DarkBlue;
                            await Context.User.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll < 820)
                        {
                            EmbedBuilder Embed1 = new EmbedBuilder();
                            Embed1.WithTitle("You find a torn page of a book on the ground...");
                            Embed1.WithDescription("You find a strange torn page of a book on the ground and read it to yourself...");
                            Embed1.Color = Discord.Color.Purple;
                            Embed1.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282406608273427/stabbed-note.png");
                            Embed1.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed1.Build());

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("You find a torn page of a book on the ground...");
                            Embed.WithDescription("The torn page is covered with dirt and has a muddy footprint slammed straight down the center. The page reads:\n\n\n" +
                                "Silverkeep simply oozes magic, to such a potent degree that it is plagued by mimics of all sorts, but a certain strain of that magic takes quite nicely to pumpkins. Thus is the Silver City’s infamous pest created: the pumpkinling, a sentient hopping gourd that sings an otherworldly song and dines on any and all meat, including that of humans.\n\n" +
                                "That may sound frightening, but the truth is pumpkinlings pose a minimal threat.Most can be slain in one firm stomp, and while their quiet singing is unsettling, it holds no magic potent enough to register in sapient minds.\n\nThe Asterian Adventurers’ Guild is known to breed a specific variety of pumpkinling that poses a legitimate hazard for the purpose of training adventurers.These pumpkinlings are kept strictly within the confines of the Training Bastion, with considerable fines imposed on any who remove even a small piece of one." +
                                "\n\n\n\n***- Excerpt from the Asterian Bestiary by Professor Lumar Veritas***");
                            Embed.Color = Discord.Color.DarkPurple;
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282406608273427/stabbed-note.png");
                            await Context.User.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (roll < 850)
                        {
                            EmbedBuilder Embed1 = new EmbedBuilder();
                            Embed1.WithTitle("Mysterious Tablet");
                            Embed1.WithDescription("You find a strange tablet on the ground and read it to yourself...");
                            Embed1.Color = Discord.Color.Purple;
                            Embed1.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282636175376384/broken-tablet.png");
                            Embed1.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            await Context.Channel.SendMessageAsync("", false, Embed1.Build());

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithTitle("Mysterious Tablet");
                            Embed.WithDescription("The mysterious tablet is covered with runic symbols. The only text that makes sense on the tablet reads:\n\n\n***Beware the beasts of the Aurora, for if they awaken and remain undefeated, Asteria may fall to ruin and the world as we know it could cease existence in the blink of an eye...***");
                            Embed.Color = Discord.Color.DarkPurple;
                            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/657282636175376384/broken-tablet.png");
                            await Context.User.SendMessageAsync("", false, Embed.Build());
                        }
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossTrainingBot, 2);
                    }
                    else if (searchingForArea)
                    {
                        if (Data.Data.Explored(Context.User.Id, "Rayels_Terror_Tower"))
                        {
                            if (!Data.Data.Explored(Context.User.Id, "Skeleton_Caves"))
                            {
                                SocketTextChannel channel = null;

                                foreach (SocketGuildChannel channelsInServer in Context.Guild.Channels)
                                {
                                    if (channelsInServer.Name == "skeleton-caves")
                                        channel = channelsInServer as SocketTextChannel;
                                }

                                await Data.Data.Explore(Context.User.Id, "Skeleton_Caves");

                                if (channel == null)
                                {
                                    EmbedBuilder Embed = new EmbedBuilder();
                                    Embed.WithTitle("Mysterious Caves");
                                    Embed.WithDescription("A muddy trench you’d dropped into gives way to stone, and then the stone wraps around into a ceiling above you before " +
                                        "you realize you’ve entered a cave. You’re not convinced this is supposed to be here… but if it isn’t, why was it so easy to get to?\n\n" +
                                        "This has to be a test. A test of your resolve. Will you return to the relative safety of the Forts… or face the unknown?\n\n" +
                                        "You have unlocked the area [Missing the channel skeleton-caves, please add it]!");
                                    Embed.WithColor(Discord.Color.Red);
                                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                }
                                else
                                {
                                    EmbedBuilder Embed = new EmbedBuilder();
                                    Embed.WithTitle("Mysterious Caves");
                                    Embed.WithDescription("A muddy trench you’d dropped into gives way to stone, and then the stone wraps around into a ceiling above you before " +
                                        "you realize you’ve entered a cave.You’re not convinced this is supposed to be here… but if it isn’t, why was it so easy to get to?\n\n" +
                                        "This has to be a test. A test of your resolve. Will you return to the relative safety of the Forts… or face the unknown?\n\n" +
                                        "You have unlocked the area  <#" + channel.Id + ">!");
                                    Embed.WithColor(Discord.Color.Green);
                                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                }
                            }
                            else
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithTitle("Nothing found...");
                                Embed.WithDescription("The sun shines brightly on you as you search and explore.\n\n\n***You continue to explore.***");
                                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931633465688086/Dungeon_battle_50300.webp");
                                Embed.Color = Discord.Color.Red;
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                        }
                        else
                        {
                            SocketTextChannel channel = null;

                            foreach (SocketGuildChannel channelsInServer in Context.Guild.Channels)
                            {
                                if (channelsInServer.Name == "rayels-terror-tower")
                                    channel = channelsInServer as SocketTextChannel;
                            }

                            await Data.Data.Explore(Context.User.Id, "Rayels_Terror_Tower");

                            if (channel == null)
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithTitle("Terror Time");
                                Embed.WithDescription("After at least an hour of pointedly not looking at it, you can’t any longer; there’s just no avoiding the tower at the far end " +
                                    "of the Training Forts. The Guild has promised nothing here is truly dangerous, and that has to include the weird tower, right? " +
                                    "Still, you can’t help but be a little creeped out by its dark windows and strange, crooked construction.\n\nBut you’re an Adventurer!" +
                                    "You wouldn’t be here if you were so easily spooked. You make your way up its misshapen front steps, and peek your head inside those rusted " +
                                    "double doors. You have unlocked the area [Missing the channel rayels-terror-tower, please add it]!");
                                Embed.WithColor(Discord.Color.Red);
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                            else
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithTitle("Terror Time");
                                Embed.WithDescription("After at least an hour of pointedly not looking at it, you can’t any longer; there’s just no avoiding the tower at the far end " +
                                    "of the Training Forts. The Guild has promised nothing here is truly dangerous, and that has to include the weird tower, right? " +
                                    "Still, you can’t help but be a little creeped out by its dark windows and strange, crooked construction.\n\nBut you’re an Adventurer!" +
                                    "You wouldn’t be here if you were so easily spooked. You make your way up its misshapen front steps, and peek your head inside those rusted " +
                                    "double doors. You have unlocked the area <#" + channel.Id + ">!");
                                Embed.WithColor(Discord.Color.Green);
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                        }
                    }

                    return;
                }
                else if (Context.Channel.Name == "rayels-terror-tower")
                {
                    if (CurrentSpawn[serverId].currentSpawn[3].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500)
                        searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Giant Tarantula", 3);
                        else if (roll < 150) await SpawnDesignedEnemy("Enchanted Sheetghost", 3);
                        else if (roll < 200) await SpawnDesignedEnemy("Magic Mirror", 3);
                        else if (roll < 300) await SpawnDesignedEnemy("Imp", 3);
                        else if (roll < 400) await SpawnDesignedEnemy("Gripping Novel", 3);
                        else if (roll < 450) await SpawnDesignedEnemy("Unusual Lamp", 3);
                        else await SpawnDesignedEnemy("Enchanted Sheetghost", 3);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Nothing found...");
                        Embed.WithDescription("You walk the dark halls, carefully stepping over the random objects scattered around the ground and " +
                            "poking into the many rooms along the way, looking for things to loot. The place seems pretty empty from the previous adventurers and robbers who've " +
                            "since ransacked the place of all possible treasures.");
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931546698252298/Dungeon_battle_50200.webp");
                        Embed.Color = Discord.Color.Red;
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "skeleton-caves")
                {
                    if (CurrentSpawn[serverId].currentSpawn[4].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500)
                        searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Armed Skeleton", 4);
                        else if (roll < 150) await SpawnDesignedEnemy("Skeleton", 4);
                        else if (roll < 200) await SpawnDesignedEnemy("Imp", 4);
                        else if (roll < 300) await SpawnDesignedEnemy("Darkness Sprite", 4);
                        else if (roll < 400) await SpawnDesignedEnemy("Giant Spider", 4);
                        else if (roll < 450) await SpawnDesignedEnemy("Giant Tarantula", 4);
                        else await SpawnDesignedEnemy("Giant Rat", 4);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        if (!Data.Data.Explored(Context.User.Id, "Forest_Path"))
                        {
                            SocketTextChannel channel = null;

                            foreach (SocketGuildChannel channelsInServer in Context.Guild.Channels)
                            {
                                if (channelsInServer.Name == "forest-path")
                                    channel = channelsInServer as SocketTextChannel;
                            }

                            await Data.Data.Explore(Context.User.Id, "Forest_Path");

                            if (channel == null)
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithTitle("End of the cave");
                                Embed.WithDescription("I'm not creative, you found the forest on the other side of the cave\n\n" +
                                    "You have unlocked the area [Missing the channel forest-path, please add it]!");
                                Embed.WithColor(Discord.Color.Green);
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931613852991502/Dungeon_battle_10400.webp");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                            else
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithTitle("End of the cave");
                                Embed.WithDescription("I'm not creative, you found the forest on the other side of the cave\n\n" +
                                    "You have unlocked the area <#" + channel.Id + ">!");
                                Embed.WithColor(Discord.Color.Green);
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931613852991502/Dungeon_battle_10400.webp");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                        }
                    }
                    else
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Nothing found...");
                        Embed.WithDescription("You walk the caves aimlessly, it feels like you've been here several times already, probably because you have!\n\n***You keep wandering in a cricle.***");
                        Embed.WithColor(Discord.Color.Orange);
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639931672686493718/Dungeon_battle_50400.webp");
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //Graveyard Groves
                else if (Context.Channel.Name == "forest-path")
                {
                    if (CurrentSpawn[serverId].currentSpawn[6].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500)
                        searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Adult Treant", 6);
                        else if (roll < 150) await SpawnDesignedEnemy("Elder Treant", 6);
                        else if (roll < 155) await SpawnDesignedEnemy("Giant Elder Enchanted Tree", 6);
                        else if (roll < 200) await SpawnDesignedEnemy("Haunted Tree", 6);
                        else if (roll < 300) await SpawnDesignedEnemy("Haunted Tree", 6);
                        else if (roll < 400) await SpawnDesignedEnemy("Jackalope", 6);
                        else if (roll < 450) await SpawnDesignedEnemy("Giant Mantis", 6);
                        else if (roll < 475) await SpawnDesignedEnemy("Haunted Tree", 6);
                        else await SpawnDesignedEnemy("Treant", 6);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "goblin-outpost")
                {
                    if (CurrentSpawn[serverId].currentSpawn[7].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Moccus", 7);
                        else if (roll < 150) await SpawnDesignedEnemy("Goblin", 7);
                        else if (roll < 155) await SpawnDesignedEnemy("Goblin War Chief", 7);
                        else if (roll < 200) await SpawnDesignedEnemy("Jackalope", 7);
                        else if (roll < 300) await SpawnDesignedEnemy("Wolf Goblin", 7);
                        else if (roll < 400) await SpawnDesignedEnemy("Hob Goblin", 7);
                        else if (roll < 450) await SpawnDesignedEnemy("Elder Moccus", 7);
                        else if (roll < 475) await SpawnDesignedEnemy("Moccus", 7);
                        else await SpawnDesignedEnemy("Goblin", 7);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "tomb-of-heroes")
                {
                    if (CurrentSpawn[serverId].currentSpawn[8].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Elder Moccus", 8);
                        else if (roll < 150) await SpawnDesignedEnemy("Goblin", 8);
                        else if (roll < 155) await SpawnDesignedEnemy("Magic Infused Golem", 8);
                        else if (roll < 200) await SpawnDesignedEnemy("Hob Goblin", 8);
                        else if (roll < 300) await SpawnDesignedEnemy("Goblin War Chief", 8);
                        else if (roll < 400) await SpawnDesignedEnemy("Hob Goblin", 8);
                        else if (roll < 450) await SpawnDesignedEnemy("Elder Moccus", 8);
                        else if (roll < 475) await SpawnDesignedEnemy("Magic Infused Golemite", 8);
                        else await SpawnDesignedEnemy("Overgrown Golem", 8);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "lair-of-trenthola")
                {
                    if (CurrentSpawn[serverId].currentSpawn[9].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500)
                        searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Adult Treant", 9);
                        else if (roll < 150) await SpawnDesignedEnemy("Elder Treant", 9);
                        else if (roll < 155) await SpawnDesignedEnemy("Giant Elder Enchanted Tree", 9);
                        else if (roll < 200) await SpawnDesignedEnemy("Haunted Tree", 9);
                        else if (roll < 300) await SpawnDesignedEnemy("Elder Enchanted Tree", 9);
                        else if (roll < 400) await SpawnDesignedEnemy("Jackalope", 9);
                        else if (roll < 450) await SpawnDesignedEnemy("Giant Mantis", 9);
                        else if (roll < 475) await SpawnDesignedEnemy("Goblin", 9);
                        else await SpawnDesignedEnemy("Treant", 9);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Trenthola, Mother of the Forest", 9);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //The Frigid Quarantine
                else if (Context.Channel.Name == "snowy-woods")
                {
                    if (CurrentSpawn[serverId].currentSpawn[10].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Frost Bear", 10);
                        else if (roll < 150) await SpawnDesignedEnemy("Adult Alpha Frost Wolf", 10);
                        else if (roll < 155) await SpawnDesignedEnemy("Frost Knight", 10);
                        else if (roll < 200) await SpawnDesignedEnemy("Frozen Golem", 10);
                        else if (roll < 300) await SpawnDesignedEnemy("Frost Wolf", 10);
                        else if (roll < 400) await SpawnDesignedEnemy("Elder Jackalope", 10);
                        else if (roll < 450) await SpawnDesignedEnemy("Jackalope", 10);
                        else if (roll < 475) await SpawnDesignedEnemy("Frost Yeti", 10);
                        else await SpawnDesignedEnemy("Snowling", 10);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "makkosi-camp")
                {
                    if (CurrentSpawn[serverId].currentSpawn[11].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Makkosi Dummy", 11);
                        else if (roll < 150) await SpawnDesignedEnemy("Adult Alpha Frost Wolf", 11);
                        else if (roll < 155) await SpawnDesignedEnemy("Frost Knight", 11);
                        else if (roll < 200) await SpawnDesignedEnemy("Frozen Golem", 11);
                        else if (roll < 300) await SpawnDesignedEnemy("Frost Wolf", 11);
                        else if (roll < 400) await SpawnDesignedEnemy("Makkosi Sergeant", 11);
                        else if (roll < 450) await SpawnDesignedEnemy("Makkosi Dummy", 11);
                        else if (roll < 475) await SpawnDesignedEnemy("Frost Yeti", 11);
                        else await SpawnDesignedEnemy("Frost Bot", 11);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "frigid-wastes")
                {
                    if (CurrentSpawn[serverId].currentSpawn[12].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Frost Bear", 12);
                        else if (roll < 150) await SpawnDesignedEnemy("Adult Alpha Frost Wolf", 12);
                        else if (roll < 155) await SpawnDesignedEnemy("Frost Knight", 12);
                        else if (roll < 200) await SpawnDesignedEnemy("Frozen Golem", 12);
                        else if (roll < 300) await SpawnDesignedEnemy("Frost Wolf", 12);
                        else if (roll < 400) await SpawnDesignedEnemy("Elder Jackalope", 12);
                        else if (roll < 450) await SpawnDesignedEnemy("Jackalope", 12);
                        else if (roll < 475) await SpawnDesignedEnemy("Frost Yeti", 12);
                        else await SpawnDesignedEnemy("Snowling", 12);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Grathmeer, Dragon of the North", 12);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "abandoned-village")
                {
                    if (CurrentSpawn[serverId].currentSpawn[14].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Frost Bear", 14);
                        else if (roll < 150) await SpawnDesignedEnemy("Goblin", 14);
                        else if (roll < 155) await SpawnDesignedEnemy("Frost Knight", 14);
                        else if (roll < 200) await SpawnDesignedEnemy("Frozen Golem", 14);
                        else if (roll < 300) await SpawnDesignedEnemy("Frost Wolf", 14);
                        else if (roll < 400) await SpawnDesignedEnemy("Elder Jackalope", 14);
                        else if (roll < 450) await SpawnDesignedEnemy("Jackalope", 14);
                        else if (roll < 475) await SpawnDesignedEnemy("Frost Yeti", 14);
                        else await SpawnDesignedEnemy("Snowling", 14);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "frozen-peaks")
                {
                    if (CurrentSpawn[serverId].currentSpawn[15].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Frost Bear", 15);
                        else if (roll < 150) await SpawnDesignedEnemy("Frost Wolf", 15);
                        else if (roll < 155) await SpawnDesignedEnemy("Frost Knight", 15);
                        else if (roll < 200) await SpawnDesignedEnemy("Frozen Golem", 15);
                        else if (roll < 300) await SpawnDesignedEnemy("Frost Wolf", 15);
                        else if (roll < 400) await SpawnDesignedEnemy("Elder Jackalope", 15);
                        else if (roll < 450) await SpawnDesignedEnemy("Jackalope", 15);
                        else if (roll < 475) await SpawnDesignedEnemy("Frost Yeti", 15);
                        else await SpawnDesignedEnemy("Snowling", 15);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Krandar, Lord of the Frozen Peaks", 15);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //Harpy's Redoubt
                else if (Context.Channel.Name == "cliffside-barricade")
                {
                    if (CurrentSpawn[serverId].currentSpawn[16].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Mechanical Harpy", 16);
                        else if (roll < 200) await SpawnDesignedEnemy("Cockatrice", 16);
                        else if (roll < 300) await SpawnDesignedEnemy("Harpy", 16);
                        else if (roll < 450) await SpawnDesignedEnemy("Upgraded Mechanical Harpy", 16);
                        else await SpawnDesignedEnemy("Harpy", 16);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "roosting-crags")
                {
                    if (CurrentSpawn[serverId].currentSpawn[17].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Elder Harpy", 17);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Harpy", 17);
                        else if (roll < 200) await SpawnDesignedEnemy("Elder Harpy", 17);
                        else if (roll < 300) await SpawnDesignedEnemy("Harpy", 17);
                        else if (roll < 400) await SpawnDesignedEnemy("Elder Jackalope", 17);
                        else if (roll < 450) await SpawnDesignedEnemy("Cockatrice", 17);
                        else if (roll < 475) await SpawnDesignedEnemy("Upgraded Mechanical Harpy", 17);
                        else await SpawnDesignedEnemy("Mechanical Harpy", 17);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "taken-temple")
                {
                    if (CurrentSpawn[serverId].currentSpawn[18].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Possessed Artifact", 18);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Harpy", 18);
                        else if (roll < 200) await SpawnDesignedEnemy("Elder Harpy", 18);
                        else if (roll < 300) await SpawnDesignedEnemy("Harpy", 18);
                        else if (roll < 400) await SpawnDesignedEnemy("Minotaur", 18);
                        else if (roll < 450) await SpawnDesignedEnemy("Mechanical Harpy", 18);
                        else if (roll < 475) await SpawnDesignedEnemy("Upgraded Mechanical Harpy", 18);
                        else await SpawnDesignedEnemy("Harpy Sorceress", 18);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "undying-storm")
                {
                    if (CurrentSpawn[serverId].currentSpawn[13].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Mechanical Harpy", 13);
                        else if (roll < 200) await SpawnDesignedEnemy("Cockatrice", 13);
                        else if (roll < 300) await SpawnDesignedEnemy("Harpy", 13);
                        else if (roll < 450) await SpawnDesignedEnemy("Upgraded Mechanical Harpy", 13);
                        else await SpawnDesignedEnemy("Harpy", 13);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Korthuur, Lord of the Storm", 13);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //Pelean Shores
                else if (Context.Channel.Name == "crawling-coastline")
                {
                    if (CurrentSpawn[serverId].currentSpawn[19].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Royal Pelican", 19);
                        else if (roll < 150) await SpawnDesignedEnemy("Merfolk", 19);
                        else if (roll < 155) await SpawnDesignedEnemy("Greater Royal Pelican", 19);
                        else if (roll < 200) await SpawnDesignedEnemy("Merfolk", 19);
                        else if (roll < 300) await SpawnDesignedEnemy("Serpant Man", 19);
                        else if (roll < 400) await SpawnDesignedEnemy("Royal Pelican", 19);
                        else if (roll < 450) await SpawnDesignedEnemy("Merfolk", 19);
                        else if (roll < 475) await SpawnDesignedEnemy("Greater Royal Pelican", 19);
                        else await SpawnDesignedEnemy("Serpant Man", 19);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "megaton-cove")
                {
                    if (CurrentSpawn[serverId].currentSpawn[20].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Megaton", 20);
                        else if (roll < 150) await SpawnDesignedEnemy("Megaton", 20);
                        else if (roll < 155) await SpawnDesignedEnemy("Mecha Manta", 20);
                        else if (roll < 200) await SpawnDesignedEnemy("Mecha Manta", 20);
                        else if (roll < 300) await SpawnDesignedEnemy("Upgraded Mecha Manta", 20);
                        else if (roll < 400) await SpawnDesignedEnemy("Upgraded Mecha Manta", 20);
                        else if (roll < 450) await SpawnDesignedEnemy("Mecha Manta", 20);
                        else if (roll < 475) await SpawnDesignedEnemy("Mecha Manta", 20);
                        else await SpawnDesignedEnemy("Megaton", 20);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "lunar-temple")
                {
                    if (CurrentSpawn[serverId].currentSpawn[21].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Mecha Manta", 21);
                        else if (roll < 150) await SpawnDesignedEnemy("Reaver", 21);
                        else if (roll < 155) await SpawnDesignedEnemy("Reaver", 21);
                        else if (roll < 200) await SpawnDesignedEnemy("Reaver", 21);
                        else if (roll < 300) await SpawnDesignedEnemy("Leech", 21);
                        else if (roll < 400) await SpawnDesignedEnemy("Upgraded Mecha Manta", 21);
                        else if (roll < 450) await SpawnDesignedEnemy("Leech", 21);
                        else if (roll < 475) await SpawnDesignedEnemy("Leech", 21);
                        else await SpawnDesignedEnemy("Megaton", 21);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "hms-reliant")
                {
                    if (CurrentSpawn[serverId].currentSpawn[22].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Mecha Manta", 22);
                        else if (roll < 150) await SpawnDesignedEnemy("Reaver", 22);
                        else if (roll < 155) await SpawnDesignedEnemy("Reaver", 22);
                        else if (roll < 200) await SpawnDesignedEnemy("Reaver", 22);
                        else if (roll < 300) await SpawnDesignedEnemy("Leech", 22);
                        else if (roll < 400) await SpawnDesignedEnemy("Upgraded Mecha Manta", 22);
                        else if (roll < 450) await SpawnDesignedEnemy("Leech", 22);
                        else if (roll < 475) await SpawnDesignedEnemy("Leech", 22);
                        else await SpawnDesignedEnemy("Megaton", 22);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //Salamander Reach
                else if (Context.Channel.Name == "logada-summits")
                {
                    if (CurrentSpawn[serverId].currentSpawn[23].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Flame Salamander", 23);
                        else if (roll < 150) await SpawnDesignedEnemy("Flame Salamander", 23);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Salamander", 23);
                        else if (roll < 200) await SpawnDesignedEnemy("Flame Sprite", 23);
                        else if (roll < 300) await SpawnDesignedEnemy("Flame Salamander", 23);
                        else if (roll < 400) await SpawnDesignedEnemy("Greater Flame Spirit", 23);
                        else if (roll < 450) await SpawnDesignedEnemy("Flame Salamander", 23);
                        else if (roll < 475) await SpawnDesignedEnemy("Flame Sprite", 23);
                        else await SpawnDesignedEnemy("Flame Sprite", 23);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        //await Context.Channel.SendMessageAsync("Boss template");
                        await SpawnDesignedEnemy("[BOSS] Cerberus, Hound of Hades", 23);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "burning-outcrop")
                {
                    if (CurrentSpawn[serverId].currentSpawn[24].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Flame Salamander", 24);
                        else if (roll < 150) await SpawnDesignedEnemy("Flame Golem", 24);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Salamander", 24);
                        else if (roll < 200) await SpawnDesignedEnemy("Flame Sprite", 24);
                        else if (roll < 300) await SpawnDesignedEnemy("Flame Golem", 24);
                        else if (roll < 400) await SpawnDesignedEnemy("Greater Flame Spirit", 24);
                        else if (roll < 450) await SpawnDesignedEnemy("Flame Slime", 24);
                        else if (roll < 475) await SpawnDesignedEnemy("Flame Slime", 24);
                        else await SpawnDesignedEnemy("Flame Slime", 24);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "magma-pits")
                {
                    if (CurrentSpawn[serverId].currentSpawn[25].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Flame Salamander", 25);
                        else if (roll < 150) await SpawnDesignedEnemy("Flame Salamander", 25);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Salamander", 25);
                        else if (roll < 200) await SpawnDesignedEnemy("Flaming Lotus", 25);
                        else if (roll < 300) await SpawnDesignedEnemy("Flame Salamander", 25);
                        else if (roll < 400) await SpawnDesignedEnemy("Greater Flame Spirit", 25);
                        else if (roll < 450) await SpawnDesignedEnemy("Flame Salamander", 25);
                        else if (roll < 475) await SpawnDesignedEnemy("Flaming Lotus", 25);
                        else await SpawnDesignedEnemy("Flaming Lotus", 25);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Rashul, Lord of Lava", 25);
                        //await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "draconic-nests")
                {
                    if (CurrentSpawn[serverId].currentSpawn[26].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Flame Salamander", 26);
                        else if (roll < 150) await SpawnDesignedEnemy("Adult Flame Salamander", 26);
                        else if (roll < 155) await SpawnDesignedEnemy("Flame Salamander", 26);
                        else if (roll < 200) await SpawnDesignedEnemy("Greater Adult Flame Salamander", 26);
                        else if (roll < 300) await SpawnDesignedEnemy("Greater Adult Flame Salamander", 26);
                        else if (roll < 400) await SpawnDesignedEnemy("Flame Salamander", 26);
                        else if (roll < 450) await SpawnDesignedEnemy("Greater Adult Flame Salamander", 26);
                        else if (roll < 475) await SpawnDesignedEnemy("Flame Salamander", 26);
                        else await SpawnDesignedEnemy("Adult Flame Salamander", 26);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //Riverside Mire
                else if (Context.Channel.Name == "twisted-riverbed")
                {
                    if (CurrentSpawn[serverId].currentSpawn[27].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Dire Leech", 27);
                        else if (roll < 150) await SpawnDesignedEnemy("Greater Dire Leech", 27);
                        else if (roll < 155) await SpawnDesignedEnemy("Greater Dire Leech", 27);
                        else if (roll < 200) await SpawnDesignedEnemy("Dire Leech", 27);
                        else if (roll < 300) await SpawnDesignedEnemy("Dire Leech", 27);
                        else if (roll < 400) await SpawnDesignedEnemy("Leech", 27);
                        else if (roll < 450) await SpawnDesignedEnemy("Leech", 27);
                        else if (roll < 475) await SpawnDesignedEnemy("Leech", 27);
                        else await SpawnDesignedEnemy("Dire Leech", 27);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "quarantined-village")
                {
                    if (CurrentSpawn[serverId].currentSpawn[28].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 150) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 155) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 200) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 300) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 400) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 450) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else if (roll < 475) await SpawnDesignedEnemy("Shifting Villager", 28);
                        else await SpawnDesignedEnemy("Shifting Villager", 28);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "the-breach")
                {
                    if (CurrentSpawn[serverId].currentSpawn[29].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 150) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 155) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 200) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 300) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 400) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 450) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else if (roll < 475) await SpawnDesignedEnemy("Mana Reaver", 29);
                        else await SpawnDesignedEnemy("Mana Reaver", 29);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "within-the-breach")
                {
                    if (CurrentSpawn[serverId].currentSpawn[30].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Greater Reaver", 30);
                        else if (roll < 150) await SpawnDesignedEnemy("Greater Reaver", 30);
                        else if (roll < 155) await SpawnDesignedEnemy("Greater Reaver", 30);
                        else if (roll < 200) await SpawnDesignedEnemy("Mind Eater", 30);
                        else if (roll < 300) await SpawnDesignedEnemy("Mind Eater", 30);
                        else if (roll < 400) await SpawnDesignedEnemy("Mind Eater", 30);
                        else if (roll < 450) await SpawnDesignedEnemy("Mana Reaver", 30);
                        else if (roll < 475) await SpawnDesignedEnemy("Mana Reaver", 30);
                        else await SpawnDesignedEnemy("Reaver", 30);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //City of the Lost
                else if (Context.Channel.Name == "outer-blockade")
                {
                    if (CurrentSpawn[serverId].currentSpawn[31].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Lucent Lieutenant", 31);
                        else if (roll < 150) await SpawnDesignedEnemy("Lucent Lieutenant", 31);
                        else if (roll < 155) await SpawnDesignedEnemy("Lucent Lieutenant", 31);
                        else if (roll < 200) await SpawnDesignedEnemy("Greater Reaver", 31);
                        else if (roll < 300) await SpawnDesignedEnemy("Lucent Dummy", 31);
                        else if (roll < 400) await SpawnDesignedEnemy("Mana Reaver", 31);
                        else if (roll < 450) await SpawnDesignedEnemy("Mind Eater", 31);
                        else if (roll < 475) await SpawnDesignedEnemy("Lucent Dummy", 31);
                        else await SpawnDesignedEnemy("Lucent Dummy", 31);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "shattered-streets")
                {
                    if (CurrentSpawn[serverId].currentSpawn[32].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Hellish Chimera", 32);
                        else if (roll < 150) await SpawnDesignedEnemy("Hellish Chimera", 32);
                        else if (roll < 155) await SpawnDesignedEnemy("Mana Keeper", 32);
                        else if (roll < 200) await SpawnDesignedEnemy("Mana Keeper", 32);
                        else if (roll < 300) await SpawnDesignedEnemy("Mana Keeper", 32);
                        else if (roll < 400) await SpawnDesignedEnemy("Mana Reaver", 32);
                        else if (roll < 450) await SpawnDesignedEnemy("Greater Reaver", 32);
                        else if (roll < 475) await SpawnDesignedEnemy("Greater Reaver", 32);
                        else await SpawnDesignedEnemy("Mind Eater", 32);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "catacombs")
                {
                    if (CurrentSpawn[serverId].currentSpawn[33].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Bone Fiend", 33);
                        else if (roll < 150) await SpawnDesignedEnemy("Hellish Chimera", 33);
                        else if (roll < 155) await SpawnDesignedEnemy("Death Keeper", 33);
                        else if (roll < 200) await SpawnDesignedEnemy("Mana Keeper", 33);
                        else if (roll < 300) await SpawnDesignedEnemy("Death Keeper", 33);
                        else if (roll < 400) await SpawnDesignedEnemy("Death Keeper", 33);
                        else if (roll < 450) await SpawnDesignedEnemy("Death Keeper", 33);
                        else if (roll < 475) await SpawnDesignedEnemy("Bone Fiend", 33);
                        else await SpawnDesignedEnemy("Bone Fiend", 33);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "corrupt-citadel")
                {
                    if (CurrentSpawn[serverId].currentSpawn[34].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Storm Kirin", 34);
                        else if (roll < 150) await SpawnDesignedEnemy("Storm Kirin", 34);
                        else if (roll < 155) await SpawnDesignedEnemy("Storm Kirin", 34);
                        else if (roll < 200) await SpawnDesignedEnemy("Storm Kirin", 34);
                        else if (roll < 300) await SpawnDesignedEnemy("Storm Kirin", 34);
                        else if (roll < 400) await SpawnDesignedEnemy("Death Keeper", 34);
                        else if (roll < 450) await SpawnDesignedEnemy("Death Keeper", 34);
                        else if (roll < 460) await SpawnDesignedEnemy("[BOSS] Hurricane Lord", 34);
                        else await SpawnDesignedEnemy("Bone Fiend", 34);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //The Northern Wilds
                else if (Context.Channel.Name == "ruined-outpost")
                {
                    if (CurrentSpawn[serverId].currentSpawn[36].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Spirit Lord", 36);
                        else if (roll < 150) await SpawnDesignedEnemy("Spirit Lord", 36);
                        else if (roll < 155) await SpawnDesignedEnemy("Spirit Lord", 36);
                        else if (roll < 200) await SpawnDesignedEnemy("Spirit Lord", 36);
                        else if (roll < 300) await SpawnDesignedEnemy("Lost Spectre", 36);
                        else if (roll < 400) await SpawnDesignedEnemy("Lost Spectre", 36);
                        else if (roll < 450) await SpawnDesignedEnemy("Lost Spectre", 36);
                        else if (roll < 475) await SpawnDesignedEnemy("Lost Spectre", 36);
                        else await SpawnDesignedEnemy("Lost Spectre", 36);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "sombris-monument")
                {
                    if (CurrentSpawn[serverId].currentSpawn[37].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Spirit Lord", 37);
                        else if (roll < 150) await SpawnDesignedEnemy("Darkness Dragon", 37);
                        else if (roll < 155) await SpawnDesignedEnemy("Spirit Lord", 37);
                        else if (roll < 200) await SpawnDesignedEnemy("Darkness Dragon", 37);
                        else if (roll < 300) await SpawnDesignedEnemy("Lost Spectre", 37);
                        else if (roll < 400) await SpawnDesignedEnemy("Lost Spectre", 37);
                        else if (roll < 450) await SpawnDesignedEnemy("Darkness Dragon", 37);
                        else if (roll < 475) await SpawnDesignedEnemy("Lost Spectre", 37);
                        else await SpawnDesignedEnemy("Lost Spectre", 37);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "end-of-asteria")
                {
                    if (CurrentSpawn[serverId].currentSpawn[38].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Sea Dragon", 38);
                        else if (roll < 150) await SpawnDesignedEnemy("Sea Dragon", 38);
                        else if (roll < 155) await SpawnDesignedEnemy("Sea Dragon", 38);
                        else if (roll < 200) await SpawnDesignedEnemy("Sea Dragon", 38);
                        else if (roll < 300) await SpawnDesignedEnemy("Atlantan Naga", 38);
                        else if (roll < 400) await SpawnDesignedEnemy("Atlantan Naga", 38);
                        else if (roll < 450) await SpawnDesignedEnemy("Atlantan Naga", 38);
                        else if (roll < 475) await SpawnDesignedEnemy("Atlantan Naga", 38);
                        else await SpawnDesignedEnemy("Atlantan Naga", 38);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "borealis-gates")
                {
                    if (CurrentSpawn[serverId].currentSpawn[39].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 150) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 155) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 200) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 300) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 400) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 450) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else if (roll < 475) await SpawnDesignedEnemy("Gatekeeper", 39);
                        else await SpawnDesignedEnemy("Gatekeeper", 39);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                //The Aurora
                else if (Context.Channel.Name == "stellar-fields")
                {
                    if (CurrentSpawn[serverId].currentSpawn[44].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Abyss Dragon", 44);
                        else if (roll < 150) await SpawnDesignedEnemy("Abyss Dragon", 44);
                        else if (roll < 155) await SpawnDesignedEnemy("Abyss Dragon", 44);
                        else if (roll < 200) await SpawnDesignedEnemy("Abyss Dragon", 44);
                        else if (roll < 300) await SpawnDesignedEnemy("Scorching Manticore", 44);
                        else if (roll < 400) await SpawnDesignedEnemy("Scorching Manticore", 44);
                        else if (roll < 450) await SpawnDesignedEnemy("Scorching Manticore", 44);
                        else if (roll < 475) await SpawnDesignedEnemy("Scorching Manticore", 44);
                        else await SpawnDesignedEnemy("Scorching Manticore", 44);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "astral-reaches")
                {
                    if (CurrentSpawn[serverId].currentSpawn[45].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Runic Warrior", 45);
                        else if (roll < 150) await SpawnDesignedEnemy("Runic Warrior", 45);
                        else if (roll < 155) await SpawnDesignedEnemy("Runic Warrior", 45);
                        else if (roll < 200) await SpawnDesignedEnemy("Runic Warrior", 45);
                        else if (roll < 300) await SpawnDesignedEnemy("Shifted Golem", 45);
                        else if (roll < 400) await SpawnDesignedEnemy("Shifted Golem", 45);
                        else if (roll < 450) await SpawnDesignedEnemy("Shifted Golem", 45);
                        else if (roll < 475) await SpawnDesignedEnemy("Shifted Golem", 45);
                        else await SpawnDesignedEnemy("Shifted Golem", 45);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "shifted-wastes")
                {
                    if (CurrentSpawn[serverId].currentSpawn[46].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Queen of Night", 46);
                        else if (roll < 150) await SpawnDesignedEnemy("Queen of Night", 46);
                        else if (roll < 155) await SpawnDesignedEnemy("Heart Horror", 46);
                        else if (roll < 200) await SpawnDesignedEnemy("Heart Horror", 46);
                        else if (roll < 300) await SpawnDesignedEnemy("Heart Horror", 46);
                        else if (roll < 400) await SpawnDesignedEnemy("Heart Horror", 46);
                        else if (roll < 450) await SpawnDesignedEnemy("Heart Horror", 46);
                        else if (roll < 475) await SpawnDesignedEnemy("Heart Horror", 46);
                        else await SpawnDesignedEnemy("Heart Horror", 46);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await Context.Channel.SendMessageAsync("Boss template");
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else if (Context.Channel.Name == "fell-pantheon")
                {
                    if (CurrentSpawn[serverId].currentSpawn[47].CurrentHealth > 0)
                    {
                        EmbedBuilder Embedaaa = new EmbedBuilder();
                        Embedaaa.WithTitle("There is an enemy in your path!");
                        Embedaaa.WithDescription("All enemies must be slain in order to continue forward!");
                        Embedaaa.Color = Discord.Color.Red;
                        Embedaaa.WithThumbnailUrl(Context.User.GetAvatarUrl());

                        var msg = await Context.Channel.SendMessageAsync("", false, Embedaaa.Build());
                        await Task.Delay(6500);
                        await msg.DeleteAsync();
                        await Context.Message.DeleteAsync();
                        return;
                    }

                    if (roll < 500) searchingForEnemy = true;
                    else if (roll < 558) searchingForLoot = true;
                    else if (roll < 800) searchingForNothing = true;
                    else if (roll < 830) searchingForLore = true;
                    else if (roll < 832) searchingForBoss = true;
                    else if (roll < 875) searchingForArea = true;
                    else if (roll < 1000) searchingForEnemy = true;

                    if (searchingForEnemy)
                    {
                        if (roll < 100) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 150) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 155) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 200) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 300) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 400) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 450) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else if (roll < 475) await SpawnDesignedEnemy("Shifted Angel", 47);
                        else await SpawnDesignedEnemy("Shifted Angel", 47);
                    }
                    else if (searchingForLoot)
                    {
                        await Context.Channel.SendMessageAsync("Loot template");
                    }
                    else if (searchingForNothing)
                    {
                        await Context.Channel.SendMessageAsync("Nothing template");
                    }
                    else if (searchingForLore)
                    {
                        await Context.Channel.SendMessageAsync("Lore template");
                    }
                    else if (searchingForBoss)
                    {
                        await SpawnDesignedEnemy("[BOSS] Warlock of the Shift", 47);
                    }
                    else if (searchingForArea)
                    {
                        //Needs implementation
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Found area template");
                        Embed.WithDescription("Needs to be implemented...");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }

                    return;
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Nothing to find here...");
                    Embed.WithDescription("It seems this place has already been fully explored by others, but don't give up, Asteria is vast and there is still much to explore past the walls of Silverkeep!");
                    Embed.Color = Discord.Color.DarkRed;
                    var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Task.Delay(8500);
                    await Context.Channel.DeleteMessageAsync(Context.Message.Id);
                    await msg.DeleteAsync();
                    return;
                }

                //await Context.Channel.SendMessageAsync("Sorry, but this zone is not available at current time.");
            }
        } //𝕯𝖚𝖓𝖌𝖊𝖔𝖓

        //Deprecated by explore (above)
        //[Command("Spawn"), Alias("spawn", "SpawnEnemy", "spawnenemy", "s", "S"), Summary("Spawn an enemy to fight.")]
        //public async Task Spawn(string remaining = null)
        //{
        //    int genType = rng.Next(1, 60);
        //    //If time replace spawn zones with spawn tables
        //    #region Spooktober event [Disabled]
        //    //{
        //    //    if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Lodaga, 125);
        //    //    else if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantArachne, 125);
        //    //    else if (genType < 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Wraith, 125);
        //    //    else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GreaterWraith, 125);
        //    //    else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantSpider, 125);
        //    //    else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pumpkinling, 125);
        //    //}
        //    //else
        //    #endregion
        //    if (Context.Channel.Name == "snowy-woods")
        //    {
        //        if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.IceIdol, 125);
        //        else if (genType < 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TabletKeeper, 125);
        //        else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GreaterWolf, 125);
        //        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.RedGoblin, 125);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Snowling, 125);
        //    }
        //    else if (Context.Channel.Name == "lv1-5")
        //    {
        //        //await ClearChat();
        //        if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Goblin, 1);
        //        else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Imp, 1);
        //        else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 1);
        //        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LogSpider, 1);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TrainingBot, 1);
        //    }
        //    else if (Context.Channel.Name == "lv5-10")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossTrainingBot, 2);
        //        else if (genType > 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeleton, 2);
        //        else if (genType > 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LargeTrainingBot, 2);
        //        else if (genType < 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BigTrainingBot, 2);
        //        else if (genType == 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 2);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SkeletonFootSoldier, 2);
        //    }
        //    else if (Context.Channel.Name == "lv10-15")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeBoss, 3);
        //        else if (genType > 30)
        //        {
        //            if (genType == 55) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverPot, 3);
        //            else if (genType > 50) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantEnt, 3);
        //            else if (genType > 43) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeEnt, 3);
        //            else if (genType > 36) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoblinWolf, 3);
        //            else if (genType > 34) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GigaEnt, 3);
        //            else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantRat, 3);
        //        }
        //        else
        //        {
        //            if (genType > 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Treant, 3);
        //            else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HauntedTree, 3);
        //            else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hornet, 3);
        //        }
        //    }
        //    else if (Context.Channel.Name == "lv15-20")
        //    {
        //        //await ClearChat();
        //        if (genType == 50) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverPot, 4);
        //        else if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Mimic, 4);
        //        else if (genType > 30)
        //        {
        //            if (genType > 40)
        //                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HobGoblin, 4);
        //            else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Cyclops, 4);
        //        }
        //        else if (genType > 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Golem, 4);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Mushroom, 4);
        //    }
        //    else if (Context.Channel.Name == "lv20-30")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossFrostWolfPackLeader, 5);
        //        else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossIceDragon, 5);
        //        else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostKnight, 5);
        //        else if (genType > 11) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostWolf, 5);
        //        else if (genType == 8) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverMimic, 5);
        //        else if (genType == 4) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 5);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostBear, 5);
        //    }
        //    else if (Context.Channel.Name == "lv30-40")
        //    {
        //        //Lava theme
        //        //await ClearChat();
        //        if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HarpyCaster, 6);
        //        else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.RoyalPelican, 6);
        //        else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 6);
        //        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HarpyShaman, 6);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Harpy, 6);
        //    }
        //    else if (Context.Channel.Name == "lv40-50")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Leviathan, 7);
        //        else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.CetusBoss, 7);
        //        else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Megaton, 7);
        //        else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 7);
        //        else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LizardMan, 7);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FishMan, 7);
        //    }
        //    else if (Context.Channel.Name == "lv50-60")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LavaDragon, 8);
        //        else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderDragonAdult, 8);
        //        else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderDragon, 8);
        //        else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 8);
        //        else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderAdult, 8);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Salamander, 8);
        //    }
        //    else if (Context.Channel.Name == "lv60-70")
        //    {
        //        //await ClearChat();
        //        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hydra, 9);
        //        else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SeaDragon, 9);
        //        else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Reaver, 9);
        //        else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 9);
        //        else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GreaterLeech, 9);
        //        else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Leech, 9);
        //    }
        //    else if (Context.Channel.Name == "lv70-80")
        //    {
        //        //await ClearChat();
        //        if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Phoenix, 10);
        //        else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Bergelmir, 10);
        //        else if (genType < 19) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hastur, 10);
        //        else if (genType < 25) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Kirin, 10);
        //        else if (genType < 32) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Haechi, 10);
        //        else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Orochi, 10);
        //        else if (genType < 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Eclipse, 10);
        //        else if (genType < 52) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Niflheim, 10);
        //        else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Muspelheim, 10);
        //    }
        //    else if (Context.Channel.Name == "lv80-90")
        //    {
        //        //await ClearChat();
        //        if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Phoenix, 11);
        //        else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Bergelmir, 11);
        //        else if (genType < 19) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hastur, 11);
        //        else if (genType < 25) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Kirin, 11);
        //        else if (genType < 32) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Haechi, 11);
        //        else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Orochi, 11);
        //        else if (genType < 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Eclipse, 11);
        //        else if (genType < 52) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Niflheim, 11);
        //        else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Muspelheim, 11);
        //    }
        //    else if (Context.Channel.Name == "lv90-100")
        //    {
        //        //await ClearChat();
        //        if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.AbyssDragon, 12);
        //        else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yamusichea, 12);
        //        else if (genType < 18) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Flare, 12);
        //        else if (genType < 24) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeletor, 12);
        //        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Khan, 12);
        //        else if (genType < 36) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Crom, 12);
        //        else if (genType < 42) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rex, 12);
        //        else if (genType < 48) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Vasuki, 12);
        //        else if (genType < 54) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Thunder, 12);
        //        else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Taigo, 12);
        //    }
        //    else if (Context.Channel.Name == "lv100-150")
        //    {
        //        //await ClearChat();
        //        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 14);
        //    }
        //    else if (Context.Channel.Name == "lv150-200")
        //    {
        //        //await ClearChat();
        //        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 15);
        //    }
        //    else if (Context.Channel.Name == "lv200-400")
        //    {
        //        //await ClearChat();
        //        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 16);
        //    }
        //    else if (Context.Channel.Name == "lv400-800")
        //    {
        //        //await ClearChat();
        //        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 17);
        //    }
        //    else if (Context.Channel.Name == "lv800-1000")
        //    {
        //        //await ClearChat();
        //        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 18);
        //    }
        //    else if (Context.Channel.Name == "the-aurora")
        //    {
        //        //await ClearChat();
        //        if (genType < 3) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Duck, 13);
        //        else if (genType < 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Masamune, 13);
        //        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rourtu, 13);
        //        else if (genType < 40) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Trikento, 13);
        //        else if (genType < 50) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Blackout, 13);
        //        else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yggdrasil, 13);
        //    }
        //    else
        //    {
        //        SocketGuildUser user = Context.User as SocketGuildUser;
        //        if (remaining == "eventboss" && (Context.User.Id == 228344819422855168 || Context.User.Id != 409566463658033173))
        //        {
        //            //await ClearChat();
        //            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
        //            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        //            await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
        //            //await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
        //        }
        //        else
        //        if (remaining == "testboss" && (Context.User.Id == 228344819422855168 || Context.User.Id != 409566463658033173))
        //        {
        //            //await ClearChat();
        //            var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
        //            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        //            await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.MySon, 35);
        //        }
        //        else
        //        {
        //            EmbedBuilder Embed = new EmbedBuilder();
        //            Embed.WithAuthor("You are not in an area known to contain monsters!");
        //            Embed.Color = Discord.Color.Red;
        //            var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //            await Context.Channel.DeleteMessageAsync(Context.Message.Id);
        //            await Task.Delay(5000);
        //            await msg.DeleteAsync();
        //            return;
        //        }
        //    }

        //    await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, Data.Data.GetData_Health(Context.User.Id));
        //    int server = 0;

        //    if (Context.Channel.Name == "snowy-woods") server = 125;
        //    else if (Context.Channel.Name == "lv1-5") server = 1;
        //    else if (Context.Channel.Name == "lv5-10") server = 2;
        //    else if (Context.Channel.Name == "lv10-15") server = 3;
        //    else if (Context.Channel.Name == "lv15-20") server = 4;
        //    else if (Context.Channel.Name == "lv20-30") server = 5;
        //    else if (Context.Channel.Name == "lv30-40") server = 6;
        //    else if (Context.Channel.Name == "lv40-50") server = 7;
        //    else if (Context.Channel.Name == "lv50-60") server = 8;
        //    else if (Context.Channel.Name == "lv60-70") server = 9;
        //    else if (Context.Channel.Name == "lv70-80") server = 10;
        //    else if (Context.Channel.Name == "lv80-90") server = 11;
        //    else if (Context.Channel.Name == "lv90-100") server = 12;
        //    else if (Context.Channel.Name == "lv100-150") server = 14;
        //    else if (Context.Channel.Name == "lv150-200") server = 15;
        //    else if (Context.Channel.Name == "lv200-400") server = 16;
        //    else if (Context.Channel.Name == "lv400-800") server = 17;
        //    else if (Context.Channel.Name == "lv800-1000") server = 18;
        //    else if (Context.Channel.Name == "the-aurora") server = 13;
        //    else if (Context.Channel.Name == "event-bosses") server = 35;
        //    else if (Context.Channel.Name == "gothkamul") server = 40;
        //    else if (Context.Channel.Name == "rakdoro") server = 41;
        //    else if (Context.Channel.Name == "kenthros") server = 42;
        //    else if (Context.Channel.Name == "arkdul") server = 43;

        //    int spot = -1;

        //    for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
        //    {
        //        if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
        //        {
        //            spot = i;
        //            break;
        //        }
        //    }

        //    if (spot == -1) return;

        //    ServerMessages[spot].FightMessages[server] = null;

        //    await Context.Channel.DeleteMessageAsync(Context.Message.Id);
        //}

        //public async Task ClearChat()
        //{
        //    //[DEPRECATED] - Discord limiter ruins everything
        //    //Does what the task says, clears the chat.
        //    //var messages = await Context.Channel.GetMessagesAsync(5000).FlattenAsync();
        //    //await(Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        //}

        [Command("fight"), Alias("Fight", "F", "f"), Summary("Fight a spawned enemy.")]
        public async Task Fight(int times = 1)
        {
            int serverId = 0;
            bool registered = false;

            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if (guilds.Id == Context.Guild.Id)
                {
                    registered = true;
                    break;
                }
                else ++serverId;
            }
            
            //Server must be catalogued.
            if (!registered) return;
            
            #region Zone Explored Check
            //Silverkeep Training Fields
            if (Context.Channel.Name == "training-forts")
            {
                if (!Data.Data.Explored(Context.User.Id, "Training_Forts"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "rayels-terror-tower")
            {
                if (!Data.Data.Explored(Context.User.Id, "Rayels_Terror_Tower"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "skeleton-caves")
            {
                if (!Data.Data.Explored(Context.User.Id, "Skeleton_Caves"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Graveyard Groves
            else if (Context.Channel.Name == "forest-path")
            {
                if (!Data.Data.Explored(Context.User.Id, "Forest_Path"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "goblin-outpost")
            {
                if (!Data.Data.Explored(Context.User.Id, "Goblin_Outpost"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "tomb-of-heroes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Tomb_of_Heroes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "lair-of-trenthola")
            {
                if (!Data.Data.Explored(Context.User.Id, "Lair_of_Trenthola"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Frigid Quarantine
            else if (Context.Channel.Name == "snowy-woods")
            {
                if (!Data.Data.Explored(Context.User.Id, "Snowy_Woods"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "makkosi-camp")
            {
                if (!Data.Data.Explored(Context.User.Id, "Makkosi_Camp"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "frigid-wastes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Frigid_Wastes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "abandoned-village")
            {
                if (!Data.Data.Explored(Context.User.Id, "Abandoned_Village"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "frozen-peaks")
            {
                if (!Data.Data.Explored(Context.User.Id, "Frozen_Peaks"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Harpy's Redoubt
            else if (Context.Channel.Name == "cliffside-barricade")
            {
                if (!Data.Data.Explored(Context.User.Id, "Cliffside_Barricade"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "roosting-crags")
            {
                if (!Data.Data.Explored(Context.User.Id, "Roosting_Crags"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "taken-temple")
            {
                if (!Data.Data.Explored(Context.User.Id, "Taken_Temple"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "undying-storm")
            {
                if (!Data.Data.Explored(Context.User.Id, "Undying_Storm"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Pelean Shores
            else if (Context.Channel.Name == "crawling-coastline")
            {
                if (!Data.Data.Explored(Context.User.Id, "Crawling_Coastline"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "megaton-cove")
            {
                if (!Data.Data.Explored(Context.User.Id, "Megaton_Cove"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "lunar-temple")
            {
                if (!Data.Data.Explored(Context.User.Id, "Lunar_Temple"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "hms-reliant")
            {
                if (!Data.Data.Explored(Context.User.Id, "Hms_Reliant"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Salamander Reach
            else if (Context.Channel.Name == "logada-summits")
            {
                if (!Data.Data.Explored(Context.User.Id, "Logada_Summits"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "burning-outcrop")
            {
                if (!Data.Data.Explored(Context.User.Id, "Burning_Outcrop"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "magma-pits")
            {
                if (!Data.Data.Explored(Context.User.Id, "Magma_Pits"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "draconic-nests")
            {
                if (!Data.Data.Explored(Context.User.Id, "Draconic_Nests"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //Riverside Mire
            else if (Context.Channel.Name == "twisted-riverbed")
            {
                if (!Data.Data.Explored(Context.User.Id, "Twisted_Riverbed"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "quarantined-village")
            {
                if (!Data.Data.Explored(Context.User.Id, "Quarantined_Village"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "the-breach")
            {
                if (!Data.Data.Explored(Context.User.Id, "The_Breach"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "within-the-breach")
            {
                if (!Data.Data.Explored(Context.User.Id, "Within_the_Breach"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //City of the Lost
            else if (Context.Channel.Name == "outer-blockade")
            {
                if (!Data.Data.Explored(Context.User.Id, "Outer_Blockade"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "shattered-streets")
            {
                if (!Data.Data.Explored(Context.User.Id, "Shattered_Streets"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "catacombs")
            {
                if (!Data.Data.Explored(Context.User.Id, "Catacombs"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "corrupt-citadel")
            {
                if (!Data.Data.Explored(Context.User.Id, "Corrupt_Citadel"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Northern Wilds
            else if (Context.Channel.Name == "ruined-outpost")
            {
                if (!Data.Data.Explored(Context.User.Id, "Ruined_Outpost"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "sombris-monument")
            {
                if (!Data.Data.Explored(Context.User.Id, "Sombris_Monument"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "end-of-asteria")
            {
                if (!Data.Data.Explored(Context.User.Id, "End_of_Asteria"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "borealis-gates")
            {
                if (!Data.Data.Explored(Context.User.Id, "Borealis_Gates"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            //The Aurora
            else if (Context.Channel.Name == "stellar-fields")
            {
                if (!Data.Data.Explored(Context.User.Id, "Stellar_Fields"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "astral-reaches")
            {
                if (!Data.Data.Explored(Context.User.Id, "Astral_Reaches"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "shifted-wastes")
            {
                if (!Data.Data.Explored(Context.User.Id, "Shifted_Wastes"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            else if (Context.Channel.Name == "fell-pantheon")
            {
                if (!Data.Data.Explored(Context.User.Id, "Fell_Pantheon"))
                {
                    await Context.Channel.SendMessageAsync("You have not discovered this area yet!");
                    return;
                }
            }
            #endregion

            for (int gg = 0; gg < times; ++gg)
            {
                int server = 0;

                //Figure out which server channel to fight in(this makes it 
                //so multiple channels can have a different enemy:
                if (Context.Channel.Name == "entry-exam") server = 1;
                else if (Context.Channel.Name == "training-forts") server = 2;
                else if (Context.Channel.Name == "rayels-terror-tower") server = 3;
                else if (Context.Channel.Name == "skeleton-caves") server = 4;
                else if (Context.Channel.Name == "𝕯𝖚𝖓𝖌𝖊𝖔𝖓-skeleton-crypts") server = 5;
                else if (Context.Channel.Name == "forest-path") server = 6;
                else if (Context.Channel.Name == "goblin-outpost") server = 7;
                else if (Context.Channel.Name == "tomb-of-heroes") server = 8;
                else if (Context.Channel.Name == "lair-of-trenthola") server = 9;
                else if (Context.Channel.Name == "snowy-woods") server = 10;
                else if (Context.Channel.Name == "makkosi-camp") server = 11;
                else if (Context.Channel.Name == "frigid-wastes") server = 12;
                else if (Context.Channel.Name == "abandoned-village") server = 14;
                else if (Context.Channel.Name == "frozen-peaks") server = 15;
                else if (Context.Channel.Name == "cliffside-barricade") server = 16;
                else if (Context.Channel.Name == "roosting-crags") server = 17;
                else if (Context.Channel.Name == "taken-temple") server = 18;
                else if (Context.Channel.Name == "undying-storm") server = 13;
                else if (Context.Channel.Name == "crawling-coastline") server = 19;
                else if (Context.Channel.Name == "megaton-cove") server = 20;
                else if (Context.Channel.Name == "lunar-temple") server = 21;
                else if (Context.Channel.Name == "hms-reliant") server = 22;
                else if (Context.Channel.Name == "logada-summits") server = 23;
                else if (Context.Channel.Name == "burning-outcrop") server = 24;
                else if (Context.Channel.Name == "magma-pits") server = 25;
                else if (Context.Channel.Name == "draconic-nests") server = 26;
                else if (Context.Channel.Name == "twisted-riverbed") server = 27;
                else if (Context.Channel.Name == "quarantined-village") server = 28;
                else if (Context.Channel.Name == "the-breach") server = 29;
                else if (Context.Channel.Name == "within-the-breach") server = 30;
                else if (Context.Channel.Name == "outer-blockade") server = 31;
                else if (Context.Channel.Name == "shattered-streets") server = 32;
                else if (Context.Channel.Name == "catacombs") server = 33;
                else if (Context.Channel.Name == "corrupt-citadel") server = 34;
                else if (Context.Channel.Name == "event-bosses") server = 35;
                else if (Context.Channel.Name == "ruined-outpost") server = 36;
                else if (Context.Channel.Name == "sombris-monument") server = 37;
                else if (Context.Channel.Name == "end-of-asteria") server = 38;
                else if (Context.Channel.Name == "borealis-gates") server = 39;
                else if (Context.Channel.Name == "gothkamul")
                {
                    server = 40;
                    int spot = -1;
                    for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                    {
                        if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                        {
                            spot = i;
                            break;
                        }
                    }
                    if (spot == -1) return;
                    for (int i = 0; i < 100; ++i)
                    {
                        if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID == Context.User.Id) break;
                        if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID == 0)
                        {
                            CurrentBossJoiners[spot].Boss1Joiner[i].UserID = Context.User.Id;
                            break;
                        }
                    }
                } //40
                else if (Context.Channel.Name == "rakdoro")
                {
                    server = 41;
                    int spot = -1;
                    for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                    {
                        if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                        {
                            spot = i;
                            break;
                        }
                    }
                    if (spot == -1) return;
                    for (int i = 0; i < 100; ++i)
                    {
                        if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID == Context.User.Id) break;
                        if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID == 0)
                        {
                            CurrentBossJoiners[spot].Boss2Joiner[i].UserID = Context.User.Id;
                            break;
                        }
                    }
                } //41
                else if (Context.Channel.Name == "kenthros")
                {
                    server = 42;
                    int spot = -1;
                    for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                    {
                        if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                        {
                            spot = i;
                            break;
                        }
                    }
                    if (spot == -1) return;
                    for (int i = 0; i < 100; ++i)
                    {
                        if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID == Context.User.Id) break;
                        if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID == 0)
                        {
                            CurrentBossJoiners[spot].Boss3Joiner[i].UserID = Context.User.Id;
                            break;
                        }
                    }
                } //42
                else if (Context.Channel.Name == "arkdul")
                {
                    server = 43;
                    int spot = -1;
                    for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                    {
                        if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                        {
                            spot = i;
                            break;
                        }
                    }
                    if (spot == -1) return;
                    for (int i = 0; i < 100; ++i)
                    {
                        if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID == Context.User.Id) break;
                        if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID == 0)
                        {
                            CurrentBossJoiners[spot].Boss4Joiner[i].UserID = Context.User.Id;
                            break;
                        }
                    }
                } //43
                else if (Context.Channel.Name == "stellar-fields") server = 44;
                else if (Context.Channel.Name == "astral-reaches") server = 45;
                else if (Context.Channel.Name == "shifted-wastes") server = 46;
                else if (Context.Channel.Name == "fell-pantheon") server = 47;
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("This area has no monsters!");
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    Embed.Color = Discord.Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }

                if (CurrentSpawn[serverId].currentSpawn[server].IsInCombat) return;

                CurrentSpawn[serverId].currentSpawn[server].IsInCombat = true;

                if (CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > 0)
                {
                    if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                    uint userdmg = Data.Data.GetData_Damage(Context.User.Id);
                    uint currentArmor = 0;
                    uint currentRegen = 0;
                    if (Data.Data.GetHelmet(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetHelmet(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetHelmet(Context.User.Id).HealthGainOnDamage;
                    }
                    if (Data.Data.GetChestplate(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetChestplate(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetChestplate(Context.User.Id).HealthGainOnDamage;
                    }
                    if (Data.Data.GetGauntlet(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetGauntlet(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetGauntlet(Context.User.Id).HealthGainOnDamage;
                    }
                    if (Data.Data.GetBelt(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetBelt(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetBelt(Context.User.Id).HealthGainOnDamage;
                    }
                    if (Data.Data.GetLeggings(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetLeggings(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetLeggings(Context.User.Id).HealthGainOnDamage;
                    }
                    if (Data.Data.GetBoots(Context.User.Id) != null)
                    {
                        currentArmor += Data.Data.GetBoots(Context.User.Id).Armor;
                        currentRegen += Data.Data.GetBoots(Context.User.Id).HealthGainOnDamage;
                    }
                    await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, (currentRegen / 10) + (Data.Data.GetData_Stability(Context.User.Id) * 10));
                    uint dmg = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinDamage, (int)CurrentSpawn[serverId].currentSpawn[server].MaxDamage);
                    bool blocked = false;
                    bool critical = false;
                    bool regenerate = false;
                    if ((int)dmg - (int)currentArmor / 10 <= 0)
                        dmg = 0;
                    else
                        dmg -= currentArmor / 10;
                    Random ran = new Random();
                    int dodge = ran.Next(0, 1000 / ((int)Data.Data.GetData_Dexterity(Context.User.Id) + 1));
                    int crit = ran.Next(0, 1000 / ((int)Data.Data.GetData_Strength(Context.User.Id) + 1));
                    int staminaregen = ran.Next(0, 1000 / ((int)Data.Data.GetData_Stamina(Context.User.Id) + 1));
                    if (dodge == 0 || dodge == 1) blocked = true;
                    if (crit == 0 || crit == 1) critical = true;
                    if (staminaregen == 0 || staminaregen == 1) regenerate = true;
                    if (regenerate)
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, Data.Data.GetData_Health(Context.User.Id) / 4);
                    if (Data.Data.GetData_CurrentHealth(Context.User.Id) > dmg)
                    {
                        if (!critical && CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > userdmg)
                            CurrentSpawn[serverId].currentSpawn[server].CurrentHealth -= userdmg;
                        else if (critical && CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > userdmg * 2)
                            CurrentSpawn[serverId].currentSpawn[server].CurrentHealth -= userdmg * 2;
                        else CurrentSpawn[serverId].currentSpawn[server].CurrentHealth = 0;
                        if (!blocked)
                            await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, (uint)(-dmg));
                        if (CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > 0)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor(CurrentSpawn[serverId].currentSpawn[server].Name + " Lv" + CurrentSpawn[serverId].currentSpawn[server].MaxLevel);
                            Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            Embed.Color = Discord.Color.Red;
                            Embed.WithFooter(CurrentSpawn[serverId].currentSpawn[server].Name + "'s Health: " + CurrentSpawn[serverId].currentSpawn[server].CurrentHealth + " / " + CurrentSpawn[serverId].currentSpawn[server].MaxHealth);
                            string emplace = blocked == true ? "You dodged the enemies attack!\n" : "You took " + dmg + " damage.\n";
                            Embed.WithDescription(emplace + "You have " + Data.Data.GetData_CurrentHealth(Context.User.Id) + " health remaining." + (regenerate == true ? "\nYou regenerated 25% health from your Stamina skill!" : "")
                                + (critical == true ? "\nYou critical striked for an additional " + dmg + " damage" : ""));
                            int spot = -1;
                            for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                            {
                                if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                {
                                    spot = i;
                                    break;
                                }
                            }
                            if (spot == -1) return;
                            if (server == 40) for (int i = 0; i < 100; ++i)
                                    if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID == Context.User.Id)
                                        CurrentBossJoiners[spot].Boss1Joiner[i].Damage += userdmg;
                            if (server == 41) for (int i = 0; i < 100; ++i)
                                    if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID == Context.User.Id)
                                        CurrentBossJoiners[spot].Boss2Joiner[i].Damage += userdmg;
                            if (server == 42) for (int i = 0; i < 100; ++i)
                                    if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID == Context.User.Id)
                                        CurrentBossJoiners[spot].Boss3Joiner[i].Damage += userdmg;
                            if (server == 43) for (int i = 0; i < 100; ++i)
                                    if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID == Context.User.Id)
                                        CurrentBossJoiners[spot].Boss4Joiner[i].Damage += userdmg;
                            if (gg >= times - 1 || gg <= 1)
                            {
                                if (ServerMessages[spot].FightMessages[server] != null)
                                {
                                    await ServerMessages[spot].FightMessages[server].ModifyAsync(x =>
                                    {
                                        x.Content = "";
                                        x.Embed = Embed.Build();
                                    });
                                }
                                else
                                {
                                    ServerMessages[spot].FightMessages[server] = await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                }
                            }
                            CurrentSpawn[serverId].currentSpawn[server].IsInCombat = false;
                        }
                        else
                        {
                            if (server == 40)
                            {
                                int spot = -1;
                                for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                                {
                                    if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                    {
                                        spot = i;
                                        break;
                                    }
                                }
                                if (spot == -1) return;
                                if (server == 40) for (int i = 0; i < 100; ++i)
                                        if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID == Context.User.Id)
                                            CurrentBossJoiners[spot].Boss1Joiner[i].Damage += userdmg;
                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID != 0)
                                    {
                                        await Data.Data.SaveData(CurrentBossJoiners[spot].Boss1Joiner[i].UserID, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(CurrentBossJoiners[spot].Boss1Joiner[i].UserID));
                                        await CheckLevelUp(CurrentBossJoiners[spot].Boss1Joiner[i].UserID);
                                    }
                                    else break;
                                }
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("Adventurers who participated defeated " + CurrentSpawn[serverId].currentSpawn[server].Name + "!");
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Discord.Color.Teal;
                                Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                string User1 = "N/A";
                                string User2 = "N/A";
                                string User3 = "N/A";
                                string User1DMG = "N/A";
                                string User2DMG = "N/A";
                                string User3DMG = "N/A";
                                List<Joiner> list = new List<Joiner> { };
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss1Joiner[i].UserID != 0)
                                    {
                                        list.Add(new Joiner(CurrentBossJoiners[spot].Boss1Joiner[i].UserID, CurrentBossJoiners[spot].Boss1Joiner[i].Damage));
                                    }
                                }
                                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                                if (list.Count > 0)
                                {
                                    User1DMG = "" + list[0].Damage;
                                    User1 = Program.Client.GetUser(list[0].UserID).Username;
                                }
                                if (list.Count > 1)
                                {
                                    User2DMG = "" + list[1].Damage;
                                    User2 = Program.Client.GetUser(list[1].UserID).Username;
                                }
                                if (list.Count > 2)
                                {
                                    User3DMG = "" + list[2].Damage;
                                    User3 = Program.Client.GetUser(list[2].UserID).Username;
                                }
                                EmbedBuilder Embed2 = new EmbedBuilder();
                                Embed2.WithTitle("The servers top 3 damage dealers were:");
                                Embed2.Color = Discord.Color.Gold;
                                Embed2.WithDescription("1.) **" + User1 + "** *-* **" + User1DMG + "** *-* **x2 Rewards**" +
                                                     "\n2.) **" + User2 + "** *-* **" + User2DMG + "** *-* **x1.5 Rewards**" +
                                                     "\n3.) **" + User3 + "** *-* **" + User3DMG + "** *-* **x1.25 Rewards**");
                                Embed2.WithFooter("These participants will earn a reward multiplier by their placings.");
                                await Context.Channel.SendMessageAsync("", false, Embed2.Build());
                                if (list.Count > 0)
                                {
                                    await Data.Data.SaveData(list[0].UserID, gold, 0, "", 0, 0, 0, xp, 0);
                                    await CheckLevelUp(list[0].UserID);
                                }
                                if (list.Count > 1)
                                {
                                    await Data.Data.SaveData(list[1].UserID, (uint)Math.Floor(gold * 0.5), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.5), 0);
                                    await CheckLevelUp(list[1].UserID);
                                }
                                if (list.Count > 2)
                                {
                                    await Data.Data.SaveData(list[2].UserID, (uint)Math.Floor(gold * 0.25), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.25), 0);
                                    await CheckLevelUp(list[2].UserID);
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss1Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss1Joiner[z].Damage = 0;
                                }
                                break;
                            }
                            else if (server == 41)
                            {
                                int spot = -1;
                                for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                                {
                                    if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                    {
                                        spot = i;
                                        break;
                                    }
                                }
                                if (spot == -1) return;
                                if (server == 41) for (int i = 0; i < 100; ++i)
                                        if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID == Context.User.Id)
                                            CurrentBossJoiners[spot].Boss2Joiner[i].Damage += userdmg;
                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID != 0)
                                    {
                                        await Data.Data.SaveData(CurrentBossJoiners[spot].Boss2Joiner[i].UserID, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(CurrentBossJoiners[spot].Boss2Joiner[i].UserID));
                                        await CheckLevelUp(CurrentBossJoiners[spot].Boss2Joiner[i].UserID);
                                    }
                                    else break;
                                }
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("Adventurers who participated defeated " + CurrentSpawn[serverId].currentSpawn[server].Name + "!");
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Discord.Color.Teal;
                                Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                string User1 = "N/A";
                                string User2 = "N/A";
                                string User3 = "N/A";
                                string User1DMG = "N/A";
                                string User2DMG = "N/A";
                                string User3DMG = "N/A";
                                List<Joiner> list = new List<Joiner> { };
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss2Joiner[i].UserID != 0)
                                    {
                                        list.Add(new Joiner(CurrentBossJoiners[spot].Boss2Joiner[i].UserID, CurrentBossJoiners[spot].Boss2Joiner[i].Damage));
                                    }
                                }
                                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                                if (list.Count > 0)
                                {
                                    User1DMG = "" + list[0].Damage;
                                    User1 = Program.Client.GetUser(list[0].UserID).Username;
                                }
                                if (list.Count > 1)
                                {
                                    User2DMG = "" + list[1].Damage;
                                    User2 = Program.Client.GetUser(list[1].UserID).Username;
                                }
                                if (list.Count > 2)
                                {
                                    User3DMG = "" + list[2].Damage;
                                    User3 = Program.Client.GetUser(list[2].UserID).Username;
                                }
                                EmbedBuilder Embed2 = new EmbedBuilder();
                                Embed2.WithTitle("The servers top 3 damage dealers were:");
                                Embed2.Color = Discord.Color.Gold;
                                Embed2.WithDescription("1.) **" + User1 + "** *-* **" + User1DMG + "** *-* **x2 Rewards**" +
                                                     "\n2.) **" + User2 + "** *-* **" + User2DMG + "** *-* **x1.5 Rewards**" +
                                                     "\n3.) **" + User3 + "** *-* **" + User3DMG + "** *-* **x1.25 Rewards**");
                                Embed2.WithFooter("These participants will earn a reward multiplier by their placings.");
                                await Context.Channel.SendMessageAsync("", false, Embed2.Build());
                                if (list.Count > 0)
                                {
                                    await Data.Data.SaveData(list[0].UserID, gold, 0, "", 0, 0, 0, xp, 0);
                                    await CheckLevelUp(list[0].UserID);
                                }
                                if (list.Count > 1)
                                {
                                    await Data.Data.SaveData(list[1].UserID, (uint)Math.Floor(gold * 0.5), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.5), 0);
                                    await CheckLevelUp(list[1].UserID);
                                }
                                if (list.Count > 2)
                                {
                                    await Data.Data.SaveData(list[2].UserID, (uint)Math.Floor(gold * 0.25), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.25), 0);
                                    await CheckLevelUp(list[2].UserID);
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss2Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss2Joiner[z].Damage = 0;
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss2Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss2Joiner[z].Damage = 0;
                                }
                                break;
                            }
                            else if (server == 42)
                            {
                                int spot = -1;
                                for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                                {
                                    if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                    {
                                        spot = i;
                                        break;
                                    }
                                }
                                if (spot == -1) return;
                                if (server == 42) for (int i = 0; i < 100; ++i)
                                        if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID == Context.User.Id)
                                            CurrentBossJoiners[spot].Boss3Joiner[i].Damage += userdmg;
                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID != 0)
                                    {
                                        await Data.Data.SaveData(CurrentBossJoiners[spot].Boss3Joiner[i].UserID, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(CurrentBossJoiners[spot].Boss3Joiner[i].UserID));
                                        await CheckLevelUp(CurrentBossJoiners[spot].Boss3Joiner[i].UserID);
                                    }
                                    else break;
                                }
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("Adventurers who participated defeated " + CurrentSpawn[serverId].currentSpawn[server].Name + "!");
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Discord.Color.Teal;
                                Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                string User1 = "N/A";
                                string User2 = "N/A";
                                string User3 = "N/A";
                                string User1DMG = "N/A";
                                string User2DMG = "N/A";
                                string User3DMG = "N/A";
                                List<Joiner> list = new List<Joiner> { };
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss3Joiner[i].UserID != 0)
                                    {
                                        list.Add(new Joiner(CurrentBossJoiners[spot].Boss3Joiner[i].UserID, CurrentBossJoiners[spot].Boss3Joiner[i].Damage));
                                    }
                                }
                                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                                if (list.Count > 0)
                                {
                                    User1DMG = "" + list[0].Damage;
                                    User1 = Program.Client.GetUser(list[0].UserID).Username;
                                }
                                if (list.Count > 1)
                                {
                                    User2DMG = "" + list[1].Damage;
                                    User2 = Program.Client.GetUser(list[1].UserID).Username;
                                }
                                if (list.Count > 2)
                                {
                                    User3DMG = "" + list[2].Damage;
                                    User3 = Program.Client.GetUser(list[2].UserID).Username;
                                }
                                EmbedBuilder Embed2 = new EmbedBuilder();
                                Embed2.WithTitle("The servers top 3 damage dealers were:");
                                Embed2.Color = Discord.Color.Gold;
                                Embed2.WithDescription("1.) **" + User1 + "** *-* **" + User1DMG + "** *-* **x2 Rewards**" +
                                                     "\n2.) **" + User2 + "** *-* **" + User2DMG + "** *-* **x1.5 Rewards**" +
                                                     "\n3.) **" + User3 + "** *-* **" + User3DMG + "** *-* **x1.25 Rewards**");
                                Embed2.WithFooter("These participants will earn a reward multiplier by their placings.");
                                await Context.Channel.SendMessageAsync("", false, Embed2.Build());
                                if (list.Count > 0)
                                {
                                    await Data.Data.SaveData(list[0].UserID, gold, 0, "", 0, 0, 0, xp, 0);
                                    await CheckLevelUp(list[0].UserID);
                                }
                                if (list.Count > 1)
                                {
                                    await Data.Data.SaveData(list[1].UserID, (uint)Math.Floor(gold * 0.5), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.5), 0);
                                    await CheckLevelUp(list[1].UserID);
                                }
                                if (list.Count > 2)
                                {
                                    await Data.Data.SaveData(list[2].UserID, (uint)Math.Floor(gold * 0.25), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.25), 0);
                                    await CheckLevelUp(list[2].UserID);
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss3Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss3Joiner[z].Damage = 0;
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss3Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss3Joiner[z].Damage = 0;
                                }
                                break;
                            }
                            else if (server == 43)
                            {
                                int spot = -1;
                                for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                                {
                                    if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                    {
                                        spot = i;
                                        break;
                                    }
                                }
                                if (spot == -1) return;
                                if (server == 43) for (int i = 0; i < 100; ++i)
                                        if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID == Context.User.Id)
                                            CurrentBossJoiners[spot].Boss4Joiner[i].Damage += userdmg;
                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID != 0)
                                    {
                                        await Data.Data.SaveData(CurrentBossJoiners[spot].Boss4Joiner[i].UserID, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(CurrentBossJoiners[spot].Boss4Joiner[i].UserID));
                                        await CheckLevelUp(CurrentBossJoiners[spot].Boss4Joiner[i].UserID);
                                    }
                                    else break;
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss4Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss4Joiner[z].Damage = 0;
                                }
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("Adventurers who participated defeated " + CurrentSpawn[serverId].currentSpawn[server].Name + "!");
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Discord.Color.Teal;
                                Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                ServerMessages[spot].FightMessages[server] = null;
                                string User1 = "N/A";
                                string User2 = "N/A";
                                string User3 = "N/A";
                                string User1DMG = "N/A";
                                string User2DMG = "N/A";
                                string User3DMG = "N/A";
                                List<Joiner> list = new List<Joiner> { };
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (CurrentBossJoiners[spot].Boss4Joiner[i].UserID != 0)
                                    {
                                        list.Add(new Joiner(CurrentBossJoiners[spot].Boss4Joiner[i].UserID, CurrentBossJoiners[spot].Boss4Joiner[i].Damage));
                                    }
                                }
                                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                                if (list.Count > 0)
                                {
                                    User1DMG = "" + list[0].Damage;
                                    User1 = Program.Client.GetUser(list[0].UserID).Username;
                                }
                                if (list.Count > 1)
                                {
                                    User2DMG = "" + list[1].Damage;
                                    User2 = Program.Client.GetUser(list[1].UserID).Username;
                                }
                                if (list.Count > 2)
                                {
                                    User3DMG = "" + list[2].Damage;
                                    User3 = Program.Client.GetUser(list[2].UserID).Username;
                                }
                                EmbedBuilder Embed2 = new EmbedBuilder();
                                Embed2.WithTitle("The servers top 3 damage dealers were:");
                                Embed2.Color = Discord.Color.Gold;
                                Embed2.WithDescription("1.) **" + User1 + "** *-* **" + User1DMG + "** *-* **x2 Rewards**" +
                                                     "\n2.) **" + User2 + "** *-* **" + User2DMG + "** *-* **x1.5 Rewards**" +
                                                     "\n3.) **" + User3 + "** *-* **" + User3DMG + "** *-* **x1.25 Rewards**");
                                Embed2.WithFooter("These participants will earn a reward multiplier by their placings.");
                                await Context.Channel.SendMessageAsync("", false, Embed2.Build());
                                if (list.Count > 0)
                                {
                                    await Data.Data.SaveData(list[0].UserID, gold, 0, "", 0, 0, 0, xp, 0);
                                    await CheckLevelUp(list[0].UserID);
                                }
                                if (list.Count > 1)
                                {
                                    await Data.Data.SaveData(list[1].UserID, (uint)Math.Floor(gold * 0.5), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.5), 0);
                                    await CheckLevelUp(list[1].UserID);
                                }
                                if (list.Count > 2)
                                {
                                    await Data.Data.SaveData(list[2].UserID, (uint)Math.Floor(gold * 0.25), 0, "", 0, 0, 0, (uint)Math.Floor(xp * 0.25), 0);
                                    await CheckLevelUp(list[2].UserID);
                                }
                                for (int z = 0; z < 100; ++z)
                                {
                                    CurrentBossJoiners[spot].Boss4Joiner[z].UserID = 0;
                                    CurrentBossJoiners[spot].Boss4Joiner[z].Damage = 0;
                                }
                                break;
                            }
                            else
                            {
                                if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("You defeated " + CurrentSpawn[serverId].currentSpawn[server].Name);
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Discord.Color.Gold;
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);
                                Embed.WithDescription("You recieved " + gold + " Gold Coins & " + xp + " XP");
                                if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                //Set enemy back to an empty.
                                if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                                await Data.Data.SaveData(Context.User.Id, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Context.User.Id));
                                //Check level up.
                                await CheckLevelUp(Context.User.Id);
                                bool LuckLoot = false;
                                if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                                int looted = ran.Next(0, 500 / ((int)Data.Data.GetData_Luck(Context.User.Id) + 1));
                                if (looted == 0 || looted == 1) LuckLoot = true;
                                int EventDrop = rng.Next(1, 25);
                                int ItemDrop = rng.Next(0, 10);
                                if (server == 125)
                                {
                                    int _ItemDrop = rng.Next(0, 5);
                                    int _CandyDrop = rng.Next(0, 12);
                                    //if(CurrentSpawn[serverId].currentSpawn[server].Name == "[EVENT BOSS] Lodaga - Lord of Halloween")
                                    //{
                                    //    EmbedBuilder Embeda = new EmbedBuilder();
                                    //    Embeda.WithAuthor("You defeated Lodaga, Lord of Halloween!");
                                    //    Embeda.WithDescription("You were awarded 10 candies for your valiant efforts by the guild!");
                                    //    Embeda.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                    //    Embeda.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/637578400730578954/Item_thum_800103.webp");
                                    //    Embeda.Color = Discord.Color.Orange;
                                    //    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                                    //    await Data.Data.SaveEventData(Context.User.Id, 0, 10, 0);
                                    //}
                                    //else 
                                    if (_CandyDrop == 7 || LuckLoot)
                                    {
                                        EmbedBuilder Embeda = new EmbedBuilder();
                                        Embeda.WithAuthor("You found a stolen present!");
                                        Embeda.WithThumbnailUrl(Context.User.GetAvatarUrl());
                                        Embeda.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/650246026749542440/gift_PNG5945.webp");
                                        Embeda.Color = Discord.Color.DarkGreen;
                                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                                        await Data.Data.SaveEventData(Context.User.Id, 1, 0, 0);
                                    }
                                    if (_ItemDrop == 2 || LuckLoot)
                                        await LootItem(Context, CurrentSpawn[serverId].currentSpawn[server].Name, rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinLevel, (int)CurrentSpawn[serverId].currentSpawn[server].MaxLevel));
                                }
                                else
                                if (server == 35)
                                {
                                    await FindBossEventItem();
                                    await Data.Data.SaveEventData(Context.User.Id, 35, 0, 0);
                                }
                                else if (EventDrop == 5)
                                {
                                    await FindEventItem();
                                    await Data.Data.SaveEventData(Context.User.Id, 1, 0, 0);
                                }
                                else if (ItemDrop == 5 || LuckLoot)
                                {
                                    await LootItem(Context, CurrentSpawn[serverId].currentSpawn[server].Name, rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinLevel, (int)CurrentSpawn[serverId].currentSpawn[server].MaxLevel));
                                }
                                int spot = -1;
                                for (int i = 0; i < CurrentBossJoiners.Count(); ++i)
                                {
                                    if (CurrentBossJoiners[i].ServerID == Context.Guild.Id)
                                    {
                                        spot = i;
                                        break;
                                    }
                                }
                                if (spot == -1) return;
                                ServerMessages[spot].FightMessages[server] = null;
                                if (CurrentSpawn[serverId].currentSpawn[server].IsDead) return;
                                CurrentSpawn[serverId].currentSpawn[server].IsDead = true;
                                break;
                            } //Double Xpppppppppppppppppppppp
                        }
                    }
                    else
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor(Context.User.Username + " Died...");
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543343901705109504/tumblr_of0k0pssmz1uysue2o3_540.gif");
                        //"https://cdn.discordapp.com/attachments/542225685695954945/543240460060196874/Grave.png");
                        Embed.Color = Discord.Color.Red;
                        Embed.WithFooter("You lost: " + (uint)Math.Round(Data.Data.GetData_GoldAmount(Context.User.Id) * 0.15) + "Gold & " + (uint)Math.Round(Data.Data.GetData_XP(Context.User.Id) * 0.25) + "XP");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithDescription("You died and lost some Gold and XP. You revive at the guilds holy church to continue your journey...");
                        CurrentSpawn[serverId].currentSpawn[server].IsInCombat = false;
                        var deathMSG = await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, ((uint)Math.Round(Data.Data.GetData_GoldAmount(Context.User.Id) * -0.15)), 0, "", 0, 0, 0, (uint)Math.Round((Data.Data.GetData_XP(Context.User.Id) * -0.25)), Data.Data.GetData_Health(Context.User.Id));
                        await Task.Delay(5000);
                        await deathMSG.DeleteAsync();
                        break;
                    }
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("There are currently no monsters to fight!");
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    Embed.Color = Discord.Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    break;
                }
            }

            await Context.Message.DeleteAsync();
        }

        //Project S
        [Command("Cast"), Alias("cast", "c", "C", "CastSpell", "castspell", "Castspell"), Summary("Fight a spawned enemy.")]
        public async Task CastSpell([Remainder] string spell)
        {
            //Check if this is spell 1, 2, 3 or 4

            //Check if this spell even exists

            //Check if the user has learned this spell

            //Check there is a target

            //Check the players stamina and mana and make sure it's more or equal to the spells cost

            //Check effect

            //Resolve

            //Update combat state

            //Finished
            await Task.Delay(-1);
        }

        //Project S
        [Command("SpellBind"), Alias("BindSpell", "bindspell", "spellbind", "Bind", "SpellBinder", "bind"), Summary("Bind a spell to a number.")]
        public async Task SpellBind(string slot, [Remainder] string spell)
        {
            if (slot == "1")
            {
                //Check spell is learned by player

                //Bind it
            }
            else if (slot == "2")
            {
                //Check spell is learned by player

                //Bind it
            }
            else if (slot == "3")
            {
                //Check spell is learned by player

                //Bind it
            }
            else if (slot == "4")
            {
                //Check spell is learned by player

                //Bind it
            }
            else
            {
                //That slots no goods
            }

            await Task.Delay(-1);
        }

        [Command("ping1"), Alias("Ping1", "p1", "P1"), Summary("Ping bot to make sure enemies can spawn.")]
        public async Task PingMe()
        {
            await Context.Channel.SendMessageAsync("Pong");
        }

        [Command("Online"), Alias("online"), Summary("Check users online/active.")]
        public async Task UsersOnline()
        {
            int usersOnline = -1;
            foreach (SocketGuildUser users in RPG_Bot.Program.Client.GetGuild(RPG_Bot.Resources.EnemyTemplates.ServerID).Users)
                if (users.Status != UserStatus.Offline || users.Status != UserStatus.AFK)
                {
                    ++usersOnline;
                }
            await Context.Channel.SendMessageAsync("There are " + usersOnline + " active users.");
        }

        [Command("prune"), Alias("Prune", "clear", "Clear"), Summary("Prune the current channel."), RequireUserPermission(GuildPermission.Administrator), RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat(uint amount)
        {
            var messages = await Context.Channel.GetMessagesAsync((int)amount).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        public async Task CheckLevelUp(ulong? ID = null)
        {
            ulong UserId = 0;
            if (ID == null)
                UserId = Context.User.Id;
            else
                UserId = (ulong)ID;
            string Username = Context.User.Username;
            while (Data.Data.GetData_XP(UserId) >= (Data.Data.GetData_Level(UserId) * Data.Data.GetData_Level(UserId)))
            {
                EmbedBuilder Embed1 = new EmbedBuilder();
                Embed1.WithAuthor(Username + " leveled up!");
                int genType = rng.Next(1, 6);
                string url = "";
                if (genType == 1)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543248244277510154/LevelUp.gif";
                if (genType == 2)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543343927382507540/tumblr_makguvDV5K1qbvovho1_500.gif";
                if (genType == 3)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543343936177963008/tumblr_inline_micbonUKFV1qz4rgp.gif";
                if (genType == 4)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543343922869567488/23a.gif";
                if (genType == 5)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543343921980243969/8nFidTY.gif";
                if (genType == 6)
                    url = "https://cdn.discordapp.com/attachments/542225685695954945/543343922869567488/23a.gif";
                Embed1.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed1.WithImageUrl(url);
                Embed1.Color = Discord.Color.Red;
                Embed1.WithFooter("You are now level: " + (Data.Data.GetData_Level(UserId) + 1));
                await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                //Level up
                await Data.Data.SaveData(UserId, 0, 0, "", 5 + ((uint)Math.Round(Data.Data.GetData_Level(UserId) * 0.75)), 5 + Data.Data.GetData_Level(UserId), 1, (uint)(Data.Data.GetData_Level(UserId) * Data.Data.GetData_Level(UserId) * -1), 0);
                await Data.Data.AddSkillPoints(UserId, 1);

                if (Data.Data.GetData_Level(UserId) == 2)
                {
                    SocketTextChannel channel = null;

                    foreach (SocketGuildChannel channelsInServer in Context.Guild.Channels)
                    {
                        if (channelsInServer.Name == "training-forts")
                            channel = channelsInServer as SocketTextChannel;
                    }

                    if (channel == null)
                    {
                        await Data.Data.Explore(Context.User.Id, "Training_Forts");

                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.Title = "Congratulations";
                        Embeda.Description = "The guild has fully enlisted you, you are now free to continue on to the Training Forts.\nUse `-explore` in [Missing channel. Please add the channel `training-forts`] to continue your journey!";
                        Embeda.Color = Discord.Color.Gold;

                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    }
                    else
                    {
                        await Data.Data.Explore(Context.User.Id, "Training_Forts");

                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.Title = "Congratulations";
                        Embeda.Description = "The guild has fully enlisted you, you are now free to continue on to the Training Forts.\nUse `-explore` in <#" + channel.Id + "> to continue your journey!";
                        Embeda.Color = Discord.Color.Gold;

                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    }
                }
            }
        }

        public async Task SpawnEnemy(Enemy enemy, int channel)
        {
            int serverId = 0;
            bool registered = false;

            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if (guilds.Id == Context.Guild.Id)
                {
                    registered = true;
                    break;
                }
                else ++serverId;
            }

            //Server must be catalogued.
            if (!registered) return;
            uint health = (uint)rng.Next((int)enemy.MinHealth, (int)enemy.MaxHealth);
            uint level = (uint)rng.Next((int)enemy.MinLevel, (int)enemy.MaxLevel);

            CurrentSpawn[serverId].currentSpawn[channel] = enemy;
            CurrentSpawn[serverId].currentSpawn[channel].MaxHealth = health;
            CurrentSpawn[serverId].currentSpawn[channel].CurrentHealth = health;
            CurrentSpawn[serverId].currentSpawn[channel].MaxLevel = level;

            EmbedBuilder Embed = new EmbedBuilder();

            Embed.WithAuthor("A " + enemy.Name + " lv" + level + " appears. Type -fight to begin battle.");
            Embed.WithImageUrl(enemy.WebURL);
            Embed.Color = Discord.Color.Red;
            Embed.WithFooter("Health: " + health + " / " + health);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public async Task SpawnDesignedEnemy(string EnemyName, int channel)
        {
            int serverId = 0;
            bool registered = false;

            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if (guilds.Id == Context.Guild.Id)
                {
                    registered = true;
                    break;
                }
                else ++serverId;
            }

            //Server must be catalogued.
            if (!registered) return;

            Enemy? generatedEnemy = null;

            int z = -1;

            for (int i = 0; i < designedEnemiesIndex.Count; i++)
            {
                if (designedEnemiesIndex[i].name == EnemyName)
                {
                    z = i;
                    break;
                }
            }

            if (z == -1) return;

            int level = (int)rng.Next((int)designedEnemiesIndex[z].minLevel, (int)designedEnemiesIndex[z].maxLevel);
            int ilevel = (int)Math.Round(Math.Pow(level, (designedEnemiesIndex[z].difficulty / 100) + 1));

            uint health = (uint)rng.Next((int)Math.Round
            (
                (designedEnemiesIndex[z].tankiness * 5 * (21 + ilevel * (5 + .75f * ilevel))) * 0.45f),
                (int)Math.Round((designedEnemiesIndex[z].tankiness * 5 * (21 + ilevel * (5 + .75f * ilevel))) * 1.1f)
            );

            //public Enemy(string url, uint maxHealth, uint minHealth, uint maxDamage, uint minDamage, uint maxLevel, uint minLevel, uint maxGoldDrop, uint minGoldDrop, uint maxXPDrop, uint minXPDrop, string name, bool isDead = false, bool isInCombat = false)

            CurrentSpawn[serverId].currentSpawn[channel] = new Enemy
            (
                designedEnemiesIndex[z].iconURL, //url
                health, //max health
                health, //min health
                (uint)MathF.Round((((ilevel - 1) * 5 + 48) / (designedEnemiesIndex[z].tankiness * 5)) * 1.25f), //max damage
                (uint)MathF.Round((((ilevel - 1) * 5 + 48) / (designedEnemiesIndex[z].tankiness * 5)) * 0.1f), //min damage
                (uint)designedEnemiesIndex[z].maxLevel, //max level
                (uint)designedEnemiesIndex[z].minLevel, //min level
                (uint)level * (uint)designedEnemiesIndex[z].difficulty, //max gold drop
                1, // min gold drop
                (uint)level * (uint)designedEnemiesIndex[z].difficulty, //max xp drop
                1, //min xp drop
                designedEnemiesIndex[z].name //name
            );

            CurrentSpawn[serverId].currentSpawn[channel].MaxHealth = health;
            CurrentSpawn[serverId].currentSpawn[channel].CurrentHealth = health;
            CurrentSpawn[serverId].currentSpawn[channel].MaxLevel = (uint)level;

            EmbedBuilder Embed = new EmbedBuilder();

            Embed.WithTitle(designedEnemiesIndex[z].flavor + "\n***Level " + level + "***");
            Embed.WithImageUrl(designedEnemiesIndex[z].iconURL);
            Embed.Color = Discord.Color.Red;
            Embed.WithFooter("Health: " + health + " / " + health);
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            builder.Name = "Minimum Damage";
            builder.Value = "" + CurrentSpawn[serverId].currentSpawn[channel].MinDamage;
            
            Embed.AddField("Minimum Damage", "" + CurrentSpawn[serverId].currentSpawn[channel].MinDamage, true);
            Embed.AddField("Max Damage", "" + CurrentSpawn[serverId].currentSpawn[channel].MaxDamage, true);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        static public async Task SpawnWorldBoss(Enemy enemy, int channel)
        {
            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                /*################################## - Handling dependency channels - #######################################################*/
                bool[] registered = new bool[5];
                for (int i = 0; i < registered.Count(); ++i) registered[i] = false;
                foreach (SocketGuildChannel channelA in guilds.Channels)
                {
                    if (channelA.Name == "gothkamul") registered[0] = true;
                    if (channelA.Name == "rakdoro") registered[1] = true;
                    if (channelA.Name == "kenthros") registered[2] = true;
                    if (channelA.Name == "arkdul") registered[3] = true;
                    if (channelA.Name == "event-bosses") registered[4] = true;
                }
                //Server must be catalogued - meaning it's been "started" / has the channels we depend on. Otherwise go to the next one and search.
                if (!registered[0] || !registered[1] || !registered[2] || !registered[3] || !registered[4]) continue;
                /*################################## - Handling dependency channels - #######################################################*/
                //If we are signed up search and do the spawning.
                //int usersOnline = 0;
                //foreach (SocketGuildUser users in guilds.Users) ++usersOnline;
                var guildA = guilds as SocketGuild;
                await guildA.DownloadUsersAsync();
                var usersOnline = guildA.Users.Where((x) => x.Status == UserStatus.Online).Count();
                //Find our place in the stack.
                int spot = -1;
                for (int i = 0; i < CurrentSpawn.Count(); ++i) if (CurrentSpawn[i].ServerID == guilds.Id) { spot = i; break; }
                if (spot == -1) return;
                foreach (SocketGuildChannel channelA in guilds.Channels)
                {
                    bool channelFound = false;
                    if (channel == 40 && channelA.Name == "gothkamul")
                        channelFound = true;
                    if (channel == 41 && channelA.Name == "rakdoro")
                        channelFound = true;
                    if (channel == 42 && channelA.Name == "kenthros")
                        channelFound = true;
                    if (channel == 43 && channelA.Name == "arkdul")
                        channelFound = true;
                    if ((channel == 44 || channel == 35) && channelA.Name == "event-bosses")
                        channelFound = true;
                    //Not in the right channel.
                    if (!channelFound) continue;
                    uint health = 0;
                    if (channel != 44 && channel != 35)
                        health = (uint)rng.Next((int)enemy.MinHealth * (int)Math.Round(usersOnline * 0.5), (int)enemy.MaxHealth * (int)Math.Round(usersOnline * 0.5));
                    else health = (uint)rng.Next((int)enemy.MinHealth, (int)enemy.MaxHealth);
                    uint level = (uint)rng.Next((int)enemy.MinLevel, (int)enemy.MaxLevel);
                    CurrentSpawn[spot].currentSpawn[channel] = enemy;
                    CurrentSpawn[spot].currentSpawn[channel].MaxHealth = health;
                    CurrentSpawn[spot].currentSpawn[channel].CurrentHealth = health;
                    CurrentSpawn[spot].currentSpawn[channel].MaxLevel = level;
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("A " + enemy.Name + " lv" + level + " appears. Type -fight to begin battle.");
                    Embed.WithDescription("Rally the guild, the plunder is split to everyone!");
                    Embed.WithImageUrl(enemy.WebURL);
                    Embed.Color = Discord.Color.Red;
                    Embed.WithFooter("Health: " + health + " / " + health);
                    var chnl = RPG_Bot.Program.Client.GetChannel(channelA.Id) as IMessageChannel;
                    await chnl.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        public async Task FindEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You found a Present!");
            Embed.WithDescription("This currency may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/650246026749542440/gift_PNG5945.webp");
            Embed.Color = Discord.Color.Teal;
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithFooter("Holiday event runs until January 31st");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public async Task FindBossEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You enter the bosses lair and gather 35 Presents!");
            Embed.WithDescription("This currency may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/650246026749542440/gift_PNG5945.webp");
            Embed.Color = Discord.Color.Teal;
            Embed.WithFooter("Holiday event runs until January 31st");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public static async Task BossChance()
        {
            await Task.Delay(300000);
            BossChance();
            int boss = rng.Next(0, 15);
            if (boss == 10)
            {
                boss = rng.Next(1, 6);
                switch (boss)
                {
                    case 1:
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.GothkamulBoss, 40);
                        break;
                    case 2:
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.RakdoroBoss, 41);
                        break;
                    case 3:
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.KenthrosBoss, 42);
                        break;
                    case 4:
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.ArkdulBoss, 43);
                        break;
                    case 5:
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
                        break;
                    default:
                        Console.WriteLine("Error spawning a boss! Num Error: " + boss);
                        break;
                }
            }
            else Console.WriteLine("Boss tried to spawn but luck says otherwise, trying again" +
                " in 5 minutes!");
        }

        [Command("StartServer"), Alias("startserver", "SS", "ss"), Summary("Generate channels for the server.")]
        public async Task StartServer()
        {
            IGuildUser user = Context.User as IGuildUser;
            if (!user.GuildPermissions.Administrator && user.Id != 228344819422855168)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Thanks for inviting RPG Bot!");
                Embed.WithDescription("Hey there, I noticed you tried to start the servers bot initializer!\n" +
                    "This command will create a 50+ new channels and some new roles, " +
                    "this means that the command " +
                    "is for administrators only. Please talk to the server owner if you need further assistance " +
                    "with setting up the bot and using your servers administration commands!");
                Embed.Color = Discord.Color.DarkRed;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }
            //Send us some details so me may snoop if we like.
            Console.WriteLine("Guild: " + Context.Guild.Name +
                " has initialized the bot! (" + Context.Guild.Id + "-" + Context.Guild.IconUrl + ")");
            //This will handle a first time join of a guild. It will create the necessary channels
            //and provide an instructions pop up.

            bool[] channels = new bool[50];

            for (int q = 0; q < 50; ++q) channels[q] = false;

            foreach (IGuildChannel channel in Context.Guild.Channels)
            {
                if (channel.Name == "entry-exam") channels[0] = true;
                else if (channel.Name == "training-forts") channels[1] = true;
                else if (channel.Name == "rayels-terror-tower") channels[2] = true;
                else if (channel.Name == "skeleton-caves") channels[3] = true;
                else if (channel.Name == "dungeon-skeleton-crypts") channels[4] = true;
                else if (channel.Name == "forest-path") channels[5] = true;
                else if (channel.Name == "goblin-outpost") channels[6] = true;
                else if (channel.Name == "tomb-of-heroes") channels[7] = true;
                else if (channel.Name == "lair-of-trenthola") channels[8] = true;
                else if (channel.Name == "snowy-woods") channels[9] = true;
                else if (channel.Name == "makkosi-camp") channels[10] = true;
                else if (channel.Name == "frigid-wastes") channels[11] = true;
                else if (channel.Name == "abandoned-village") channels[12] = true;
                else if (channel.Name == "frozen-peaks") channels[13] = true;
                else if (channel.Name == "cliffside-barricade") channels[14] = true;
                else if (channel.Name == "roosting-crags") channels[15] = true;
                else if (channel.Name == "taken-temple") channels[16] = true;
                else if (channel.Name == "undying-storm") channels[17] = true;
                else if (channel.Name == "questing") channels[18] = true;
                else if (channel.Name == "daily-blessings") channels[19] = true;
                else if (channel.Name == "guild-shop") channels[20] = true;
                else if (channel.Name == "event-bosses") channels[21] = true;
                else if (channel.Name == "gothkamul") channels[22] = true;
                else if (channel.Name == "rakdoro") channels[23] = true;
                else if (channel.Name == "kenthros") channels[24] = true;
                else if (channel.Name == "megaton-cove") channels[25] = true;
                else if (channel.Name == "lunar-temple") channels[26] = true;
                else if (channel.Name == "hms-reliant") channels[27] = true;
                else if (channel.Name == "logada-summits") channels[28] = true;
                else if (channel.Name == "burning-outcrop") channels[29] = true;
                else if (channel.Name == "magma-pits") channels[30] = true;
                else if (channel.Name == "draconic-nests") channels[31] = true;
                else if (channel.Name == "twisted-riverbed") channels[32] = true;
                else if (channel.Name == "quarantined-village") channels[33] = true;
                else if (channel.Name == "the-breach") channels[34] = true;
                else if (channel.Name == "within-the-breach") channels[35] = true;
                else if (channel.Name == "outer-blockade") channels[36] = true;
                else if (channel.Name == "shattered-streets") channels[37] = true;
                else if (channel.Name == "catacombs") channels[38] = true;
                else if (channel.Name == "corrupt-citadel") channels[39] = true;
                else if (channel.Name == "ruined-outpost") channels[40] = true;
                else if (channel.Name == "sombris-monument") channels[41] = true;
                else if (channel.Name == "end-of-asteria") channels[42] = true;
                else if (channel.Name == "borealis-gates") channels[43] = true;
                else if (channel.Name == "stellar-fields") channels[44] = true;
                else if (channel.Name == "astral-reaches") channels[45] = true;
                else if (channel.Name == "shifted-wastes") channels[46] = true;
                else if (channel.Name == "fell-pantheon") channels[47] = true;
                else if (channel.Name == "arkdul") channels[48] = true;
            }

            var a1 = await Context.Guild.CreateCategoryChannelAsync("Silverkeep");
            var a2 = await Context.Guild.CreateCategoryChannelAsync("Raid Bosses - Spawns");
            var a3 = await Context.Guild.CreateCategoryChannelAsync("Silverkeep Training Fields");
            var b = await Context.Guild.CreateCategoryChannelAsync("Graveyard Groves");
            var d = await Context.Guild.CreateCategoryChannelAsync("The Frigid Quarantine");
            var e = await Context.Guild.CreateCategoryChannelAsync("Harpy's Redoubt");
            var f = await Context.Guild.CreateCategoryChannelAsync("Pelean Shores");
            var g = await Context.Guild.CreateCategoryChannelAsync("Salamander Reach");
            var h = await Context.Guild.CreateCategoryChannelAsync("Riverside Mire");
            var i = await Context.Guild.CreateCategoryChannelAsync("City of the Lost");
            var j = await Context.Guild.CreateCategoryChannelAsync("The Northern Wilds");
            var k = await Context.Guild.CreateCategoryChannelAsync("The Aurora");

            //await (await Context.Guild.CreateTextChannelAsync("entry-exam") as Discord.Rest.RestTextChannel).ModifyAsync(c => c.CategoryId = a.Id);

            if (!channels[0]) await Context.Guild.CreateTextChannelAsync("entry-exam", c => c.CategoryId = a3.Id);
            if (!channels[1]) await Context.Guild.CreateTextChannelAsync("training-forts", c => c.CategoryId = a3.Id);
            if (!channels[2]) await Context.Guild.CreateTextChannelAsync("rayels-terror-tower", c => c.CategoryId = a3.Id);
            if (!channels[3]) await Context.Guild.CreateTextChannelAsync("skeleton-caves", c => c.CategoryId = a3.Id);
            if (!channels[4]) await Context.Guild.CreateTextChannelAsync("dungeon-skeleton-crypts", c => c.CategoryId = a3.Id);
            if (!channels[5]) await Context.Guild.CreateTextChannelAsync("forest-path", c => c.CategoryId = b.Id);
            if (!channels[6]) await Context.Guild.CreateTextChannelAsync("goblin-outpost", c => c.CategoryId = b.Id);
            if (!channels[7]) await Context.Guild.CreateTextChannelAsync("tomb-of-heroes", c => c.CategoryId = b.Id);
            if (!channels[8]) await Context.Guild.CreateTextChannelAsync("lair-of-trenthola", c => c.CategoryId = b.Id);
            if (!channels[9]) await Context.Guild.CreateTextChannelAsync("snowy-woods", c => c.CategoryId = d.Id);
            if (!channels[10]) await Context.Guild.CreateTextChannelAsync("makkosi-camp", c => c.CategoryId = d.Id);
            if (!channels[11]) await Context.Guild.CreateTextChannelAsync("frigid-wastes", c => c.CategoryId = d.Id);
            if (!channels[12]) await Context.Guild.CreateTextChannelAsync("abandoned-village", c => c.CategoryId = d.Id);
            if (!channels[13]) await Context.Guild.CreateTextChannelAsync("frozen-peaks", c => c.CategoryId = d.Id);
            if (!channels[14]) await Context.Guild.CreateTextChannelAsync("cliffside-barricade", c => c.CategoryId = e.Id);
            if (!channels[15]) await Context.Guild.CreateTextChannelAsync("roosting-crags", c => c.CategoryId = e.Id);
            if (!channels[16]) await Context.Guild.CreateTextChannelAsync("taken-temple", c => c.CategoryId = e.Id);
            if (!channels[17]) await Context.Guild.CreateTextChannelAsync("undying-storm", c => c.CategoryId = e.Id);
            if (!channels[18]) await Context.Guild.CreateTextChannelAsync("questing", c => c.CategoryId = a1.Id);
            if (!channels[19]) await Context.Guild.CreateTextChannelAsync("daily-blessings", c => c.CategoryId = a1.Id);
            if (!channels[20]) await Context.Guild.CreateTextChannelAsync("guild-shop", c => c.CategoryId = a1.Id);
            if (!channels[21]) await Context.Guild.CreateTextChannelAsync("event-bosses", c => c.CategoryId = a2.Id);
            if (!channels[22]) await Context.Guild.CreateTextChannelAsync("gothkamul", c => c.CategoryId = a2.Id);
            if (!channels[23]) await Context.Guild.CreateTextChannelAsync("rakdoro", c => c.CategoryId = a2.Id);
            if (!channels[24]) await Context.Guild.CreateTextChannelAsync("kenthros", c => c.CategoryId = a2.Id);
            if (!channels[25]) await Context.Guild.CreateTextChannelAsync("megaton-cove", c => c.CategoryId = f.Id);
            if (!channels[26]) await Context.Guild.CreateTextChannelAsync("lunar-temple", c => c.CategoryId = f.Id);
            if (!channels[27]) await Context.Guild.CreateTextChannelAsync("hms-reliant", c => c.CategoryId = f.Id);
            if (!channels[28]) await Context.Guild.CreateTextChannelAsync("logada-summits", c => c.CategoryId = g.Id);
            if (!channels[29]) await Context.Guild.CreateTextChannelAsync("burning-outcrop", c => c.CategoryId = g.Id);
            if (!channels[30]) await Context.Guild.CreateTextChannelAsync("magma-pits", c => c.CategoryId = g.Id);
            if (!channels[31]) await Context.Guild.CreateTextChannelAsync("draconic-nests", c => c.CategoryId = g.Id);
            if (!channels[32]) await Context.Guild.CreateTextChannelAsync("twisted-riverbed", c => c.CategoryId = h.Id);
            if (!channels[33]) await Context.Guild.CreateTextChannelAsync("quarantined-village", c => c.CategoryId = h.Id);
            if (!channels[34]) await Context.Guild.CreateTextChannelAsync("the-breach", c => c.CategoryId = h.Id);
            if (!channels[35]) await Context.Guild.CreateTextChannelAsync("within-the-breach", c => c.CategoryId = h.Id);
            if (!channels[36]) await Context.Guild.CreateTextChannelAsync("outer-blockade", c => c.CategoryId = i.Id);
            if (!channels[37]) await Context.Guild.CreateTextChannelAsync("shattered-streets", c => c.CategoryId = i.Id);
            if (!channels[38]) await Context.Guild.CreateTextChannelAsync("catacombs", c => c.CategoryId = i.Id);
            if (!channels[39]) await Context.Guild.CreateTextChannelAsync("corrupt-citadel", c => c.CategoryId = i.Id);
            if (!channels[40]) await Context.Guild.CreateTextChannelAsync("ruined-outpost", c => c.CategoryId = j.Id);
            if (!channels[41]) await Context.Guild.CreateTextChannelAsync("sombris-monument", c => c.CategoryId = j.Id);
            if (!channels[42]) await Context.Guild.CreateTextChannelAsync("end-of-asteria", c => c.CategoryId = j.Id);
            if (!channels[43]) await Context.Guild.CreateTextChannelAsync("borealis-gates", c => c.CategoryId = j.Id);
            if (!channels[44]) await Context.Guild.CreateTextChannelAsync("stellar-fields", c => c.CategoryId = k.Id);
            if (!channels[45]) await Context.Guild.CreateTextChannelAsync("astral-reaches", c => c.CategoryId = k.Id);
            if (!channels[46]) await Context.Guild.CreateTextChannelAsync("shifted-wastes", c => c.CategoryId = k.Id);
            if (!channels[47]) await Context.Guild.CreateTextChannelAsync("fell-pantheon", c => c.CategoryId = k.Id);
            if (!channels[48]) await Context.Guild.CreateTextChannelAsync("arkdul", c => c.CategoryId = a2.Id);
            
            foreach (IGuildChannel channel in Context.Guild.Channels)
            {
                if (channel.Name == "entry-exam")
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Thanks for inviting RPG Bot!");
                    Embed.WithDescription("As you may have noticed, the bot generated a ton of new channels and roles for the " +
                        "server. Sorry if this is an inconvenience but these channels are reserved for the bot, " +
                        "it is recommended you put them under their own channel category (You can do -cleanup to remove all the roles and channels if you're an admin though!). Please note, if these " +
                        "channels are renamed the bot will not function here correctly!\n\n\n" +
                        "To get started using the bot, type ``-begin [Class] [Name] [Age]``\nUse ``-help`` for general commands\n" +
                        "Type ``-class`` or ``-classes`` to see all available class types!\n" +
                        "\n\nNeed further help? Join the Asteria RPG home server here and we will be more than happy to help!" +
                        "\n" + "https://discord.gg/jnFfqhm");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202701210025993/Zygmunt_Baur_Dreams_and_Illusions_transparent.webp");
                    Embed.Color = Discord.Color.Gold;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    break;
                }
            }

            bool[] Ranks = new bool[30];

            for (int a = 0; a < Ranks.Count(); ++a)
                Ranks[a] = false;

            foreach (SocketRole roles in Context.Guild.Roles)
            {
                if (roles.Name == "Rank - Master I") Ranks[1] = true;
                if (roles.Name == "Rank - Master II") Ranks[2] = true;
                if (roles.Name == "Rank - Master III") Ranks[3] = true;
                if (roles.Name == "Rank - Platinum") Ranks[4] = true;
                if (roles.Name == "Rank - Orichalcum") Ranks[5] = true;
                if (roles.Name == "Rank - Quartz") Ranks[6] = true;
                if (roles.Name == "Rank - Gold") Ranks[7] = true;
                if (roles.Name == "Rank - Silver") Ranks[8] = true;
                if (roles.Name == "Rank - Bronze") Ranks[9] = true;
                if (roles.Name == "Knight") Ranks[10] = true;
                if (roles.Name == "Assassin") Ranks[11] = true;
                if (roles.Name == "Wizard") Ranks[12] = true;
                if (roles.Name == "Rogue") Ranks[13] = true;
                if (roles.Name == "Archer") Ranks[14] = true;
                if (roles.Name == "Witch") Ranks[15] = true;
                if (roles.Name == "Berserker") Ranks[16] = true;
                if (roles.Name == "Tamer") Ranks[17] = true;
                if (roles.Name == "Monk") Ranks[18] = true;
                if (roles.Name == "Necromancer") Ranks[19] = true;
                if (roles.Name == "Paladin") Ranks[20] = true;
                if (roles.Name == "Swordsman") Ranks[21] = true;
                if (roles.Name == "Trickster") Ranks[22] = true;
                if (roles.Name == "Evangel") Ranks[23] = true;
                if (roles.Name == "Kitsune") Ranks[24] = true;
                if (roles.Name == "Slayer of Arkdul") Ranks[25] = true;
                if (roles.Name == "Slayer of Kenthros") Ranks[26] = true;
                if (roles.Name == "Slayer of Rakdoro") Ranks[27] = true;
                if (roles.Name == "Slayer of Gothkamul") Ranks[28] = true;
            }

            if (!Ranks[1] ) await Context.Guild.CreateRoleAsync("Rank - Master I", Discord.GuildPermissions.None, new Discord.Color(255, 0, 0));
            if (!Ranks[2] ) await Context.Guild.CreateRoleAsync("Rank - Master II", Discord.GuildPermissions.None, new Discord.Color(255, 79, 0));
            if (!Ranks[3] ) await Context.Guild.CreateRoleAsync("Rank - Master III", Discord.GuildPermissions.None, new Discord.Color(255, 150, 32));
            if (!Ranks[4] ) await Context.Guild.CreateRoleAsync("Rank - Platinum", Discord.GuildPermissions.None, new Discord.Color(186, 192, 255));
            if (!Ranks[5] ) await Context.Guild.CreateRoleAsync("Rank - Orichalcum", Discord.GuildPermissions.None, new Discord.Color(0, 232, 255));
            if (!Ranks[6] ) await Context.Guild.CreateRoleAsync("Rank - Quartz", Discord.GuildPermissions.None, new Discord.Color(255, 255, 255));
            if (!Ranks[7] ) await Context.Guild.CreateRoleAsync("Rank - Gold", Discord.GuildPermissions.None, new Discord.Color(255, 181, 0));
            if (!Ranks[8] ) await Context.Guild.CreateRoleAsync("Rank - Silver", Discord.GuildPermissions.None, new Discord.Color(131, 131, 131));
            if (!Ranks[9] ) await Context.Guild.CreateRoleAsync("Rank - Bronze", Discord.GuildPermissions.None, new Discord.Color(156, 71, 0));
            if (!Ranks[10]) await Context.Guild.CreateRoleAsync("Knight", Discord.GuildPermissions.None, new Discord.Color(87, 87, 87));
            if (!Ranks[11]) await Context.Guild.CreateRoleAsync("Assassin", Discord.GuildPermissions.None, new Discord.Color(97, 97, 97));
            if (!Ranks[12]) await Context.Guild.CreateRoleAsync("Wizard", Discord.GuildPermissions.None, new Discord.Color(162, 135, 0));
            if (!Ranks[13]) await Context.Guild.CreateRoleAsync("Rogue", Discord.GuildPermissions.None, new Discord.Color(79, 102, 73));
            if (!Ranks[14]) await Context.Guild.CreateRoleAsync("Archer", Discord.GuildPermissions.None, new Discord.Color(71, 149, 156));
            if (!Ranks[15]) await Context.Guild.CreateRoleAsync("Witch", Discord.GuildPermissions.None, new Discord.Color(184, 65, 65));
            if (!Ranks[16]) await Context.Guild.CreateRoleAsync("Berserker", Discord.GuildPermissions.None, new Discord.Color(145, 4, 4));
            if (!Ranks[17]) await Context.Guild.CreateRoleAsync("Tamer", Discord.GuildPermissions.None, new Discord.Color(152, 168, 74));
            if (!Ranks[18]) await Context.Guild.CreateRoleAsync("Monk", Discord.GuildPermissions.None, new Discord.Color(160, 112, 233));
            if (!Ranks[19]) await Context.Guild.CreateRoleAsync("Necromancer", Discord.GuildPermissions.None, new Discord.Color(73, 73, 73));
            if (!Ranks[20]) await Context.Guild.CreateRoleAsync("Paladin", Discord.GuildPermissions.None, new Discord.Color(10, 146, 153));
            if (!Ranks[21]) await Context.Guild.CreateRoleAsync("Swordsman", Discord.GuildPermissions.None, new Discord.Color(206, 31, 31));
            if (!Ranks[22]) await Context.Guild.CreateRoleAsync("Trickster", Discord.GuildPermissions.None, new Discord.Color(5, 141, 9));
            if (!Ranks[23]) await Context.Guild.CreateRoleAsync("Evangel", Discord.GuildPermissions.None, new Discord.Color(45, 103, 179));
            if (!Ranks[24]) await Context.Guild.CreateRoleAsync("Kitsune", Discord.GuildPermissions.None, new Discord.Color(255, 148, 225));
            if (!Ranks[25]) await Context.Guild.CreateRoleAsync("Slayer of Arkdul", Discord.GuildPermissions.None, new Discord.Color(213, 105, 255));
            if (!Ranks[26]) await Context.Guild.CreateRoleAsync("Slayer of Kenthros", Discord.GuildPermissions.None, new Discord.Color(0, 103, 231));
            if (!Ranks[27]) await Context.Guild.CreateRoleAsync("Slayer of Rakdoro", Discord.GuildPermissions.None, new Discord.Color(189, 75, 75));
            if (!Ranks[28]) await Context.Guild.CreateRoleAsync("Slayer of Gothkamul", Discord.GuildPermissions.None, new Discord.Color(41, 255, 197));

            //new Discord.Color(0xff0000);

            Console.WriteLine("Server was initialized!");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Server connection has started - Adding to the object pool");
            Console.WriteLine("Connecting to 1 new Guild\nThere are 250 available object pool slots.");

            await Gameplay.UpdateUserData();

            for (int q = 0; q < CurrentSpawn.Count(); ++q)
            {
                //Don't need to re-add.
                if (CurrentBossJoiners[q] == null)
                    CurrentBossJoiners[q] = new BossJoiningSystem();
                if (CurrentSpawn[q].ServerID == Context.Guild.Id) break;
                //Find an empty spot and make ourselves that space in memory.
                if (CurrentSpawn[q].ServerID == 0)
                {
                    if (CurrentSpawn[q] == null)
                        CurrentSpawn[q] = new ServerIntegratedEnemy();
                    CurrentSpawn[q].ServerID = Context.Guild.Id;
                    CurrentBossJoiners[q].ServerID = Context.Guild.Id;
                    Console.WriteLine("Connected to: " + Context.Guild.Name);
                    break;
                }
            }

            Console.WriteLine("Servers have made a full connection");
            Console.WriteLine("---------------------------------------------------------------");
        }

        public static async Task UpdateUserData()
        {
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("User roles are being updated!");
            foreach (SocketGuild guilds in Program.Client.Guilds)
            {
                //Check to make sure the guild has been started up and has the valid roles.
                bool[] contains = new bool[30];
                foreach (SocketRole role in guilds.Roles)
                {
                    if (role.Name == "Rank - Bronze") contains[0] = true;
                    if (role.Name == "Rank - Silver") contains[1] = true;
                    if (role.Name == "Rank - Gold") contains[2] = true;
                    if (role.Name == "Rank - Quartz") contains[3] = true;
                    if (role.Name == "Rank - Orichalcum") contains[4] = true;
                    if (role.Name == "Rank - Platinum") contains[5] = true;
                    if (role.Name == "Rank - Master III") contains[6] = true;
                    if (role.Name == "Rank - Master II") contains[7] = true;
                    if (role.Name == "Rank - Master I") contains[8] = true;
                    if (role.Name == "Knight") contains[9] = true;
                    if (role.Name == "Assassin") contains[10] = true;
                    if (role.Name == "Wizard") contains[11] = true;
                    if (role.Name == "Rogue") contains[12] = true;
                    if (role.Name == "Archer") contains[13] = true;
                    if (role.Name == "Witch") contains[14] = true;
                    if (role.Name == "Berserker") contains[15] = true;
                    if (role.Name == "Tamer") contains[16] = true;
                    if (role.Name == "Monk") contains[17] = true;
                    if (role.Name == "Necromancer") contains[18] = true;
                    if (role.Name == "Paladin") contains[19] = true;
                    if (role.Name == "Swordsman") contains[20] = true;
                    if (role.Name == "Evangel") contains[21] = true;
                    if (role.Name == "Kitsune") contains[22] = true;
                    if (role.Name == "Slayer of Arkdul") contains[23] = true;
                    if (role.Name == "Slayer of Kenthros") contains[24] = true;
                    if (role.Name == "Slayer of Rakdoro") contains[25] = true;
                }
                if (!contains[0]) continue;
                else if (!contains[1]) continue;
                else if (!contains[2]) continue;
                else if (!contains[3]) continue;
                else if (!contains[4]) continue;
                else if (!contains[5]) continue;
                else if (!contains[6]) continue;
                else if (!contains[7]) continue;
                else if (!contains[8]) continue;
                else if (!contains[9]) continue;
                else if (!contains[10]) continue;
                else if (!contains[11]) continue;
                else if (!contains[12]) continue;
                else if (!contains[13]) continue;
                else if (!contains[14]) continue;
                else if (!contains[15]) continue;
                else if (!contains[16]) continue;
                else if (!contains[17]) continue;
                else if (!contains[18]) continue;
                else if (!contains[19]) continue;
                else if (!contains[20]) continue;
                else if (!contains[21]) continue;
                else if (!contains[22]) continue;
                else if (!contains[23]) continue;
                else if (!contains[24]) continue;
                else if (!contains[25]) continue;
                else
                {
                    Console.WriteLine("Guild: " + guilds.Name);
                    foreach (SocketGuildUser user in guilds.Users)
                    {
                        Console.WriteLine("User: " + user.Username);
                        var Knight = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Knight");
                        var Assassin = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Assassin");
                        var Wizard = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Wizard");
                        var Rogue = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rogue");
                        var Archer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Archer");
                        var Witch = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Witch");
                        var Berserker = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Berserker");
                        var Tamer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Tamer");
                        var Monk = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Monk");
                        var Necromancer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Necromancer");
                        var Paladin = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Paladin");
                        var Swordsman = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Swordsman");
                        var Evangel = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Evangel");
                        var Kitsune = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Kitsune");
                        var Slayer_Arkdul = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Slayer of Arkdul");
                        var Slayer_Kenthros = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Slayer of Kenthros");
                        var Slayer_Rakdoro = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Slayer of Rakdoro");
                        var Slayer_Gothkamul = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Slayer of Gothkamul");
                        var Rank_Bronze = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Bronze");
                        var Rank_Silver = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Silver");
                        var Rank_Gold = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Gold");
                        var Rank_Quartz = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Quartz");
                        var Rank_Orichalcum = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Orichalcum");
                        var Rank_Platinum = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Platinum");
                        var Rank_Master3 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master III");
                        var Rank_Master2 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master II");
                        var Rank_Master1 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master I");
                        string Class = Data.Data.GetClass(user.Id);
                        string Rank = Data.Data.GetRank(user.Id);
                        if (Rank == "Bronze") if (!user.Roles.Contains(Rank_Bronze))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Bronze);
                            }
                        if (Rank == "Silver") if (!user.Roles.Contains(Rank_Silver))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Silver);
                            }
                        if (Rank == "Gold") if (!user.Roles.Contains(Rank_Gold))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Gold);
                            }
                        if (Rank == "Quartz") if (!user.Roles.Contains(Rank_Quartz))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Quartz);
                            }
                        if (Rank == "Orichalcum") if (!user.Roles.Contains(Rank_Orichalcum))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Orichalcum);
                            }
                        if (Rank == "Platinum") if (!user.Roles.Contains(Rank_Platinum))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Platinum);
                            }
                        if (Rank == "Master3") if (!user.Roles.Contains(Rank_Master3))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Master3);
                            }
                        if (Rank == "Master2") if (!user.Roles.Contains(Rank_Master2))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Master2);
                            }
                        if (Rank == "Master1") if (!user.Roles.Contains(Rank_Master1))
                            {
                                await RemoveUsersRank(user);
                                await user.AddRoleAsync(Rank_Master1);
                            }
                        if (Class == "Knight") if (!user.Roles.Contains(Knight))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Knight);
                            }
                        if (Class == "Assassin") if (!user.Roles.Contains(Assassin))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Assassin);
                            }
                        if (Class == "Wizard") if (!user.Roles.Contains(Wizard))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Wizard);
                            }
                        if (Class == "Rogue") if (!user.Roles.Contains(Rogue))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Rogue);
                            }
                        if (Class == "Archer") if (!user.Roles.Contains(Archer))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Archer);
                            }
                        if (Class == "Witch") if (!user.Roles.Contains(Witch))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Witch);
                            }
                        if (Class == "Berserker") if (!user.Roles.Contains(Berserker))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Berserker);
                            }
                        if (Class == "Tamer") if (!user.Roles.Contains(Tamer))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Tamer);
                            }
                        if (Class == "Monk") if (!user.Roles.Contains(Monk))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Monk);
                            }
                        if (Class == "Necromancer") if (!user.Roles.Contains(Necromancer))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Necromancer);
                            }
                        if (Class == "Paladin") if (!user.Roles.Contains(Paladin))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Paladin);
                            }
                        if (Class == "Swordsman") if (!user.Roles.Contains(Swordsman))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Swordsman);
                            }
                        if (Class == "Evangel") if (!user.Roles.Contains(Evangel))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Evangel);
                            }
                        if (Class == "Kitsune") if (!user.Roles.Contains(Kitsune))
                            {
                                await RemoveUsersClass(user);
                                await user.AddRoleAsync(Kitsune);
                            }
                    }
                }
            }
        }

        public static async Task RemoveUsersClass(SocketGuildUser user)
        {
            Console.WriteLine("Removing " + user.Username + "'s class");
            var Knight = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Knight");
            var Assassin = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Assassin");
            var Wizard = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Wizard");
            var Rogue = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rogue");
            var Archer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Archer");
            var Witch = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Witch");
            var Berserker = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Berserker");
            var Tamer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Tamer");
            var Monk = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Monk");
            var Necromancer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Necromancer");
            var Paladin = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Paladin");
            var Swordsman = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Swordsman");
            var Evangel = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Evangel");
            var Kitsune = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Kitsune");
            if (user.Roles.Contains(Knight)) await user.RemoveRoleAsync(Knight);
            if (user.Roles.Contains(Assassin)) await user.RemoveRoleAsync(Assassin);
            if (user.Roles.Contains(Wizard)) await user.RemoveRoleAsync(Wizard);
            if (user.Roles.Contains(Rogue)) await user.RemoveRoleAsync(Rogue);
            if (user.Roles.Contains(Archer)) await user.RemoveRoleAsync(Archer);
            if (user.Roles.Contains(Witch)) await user.RemoveRoleAsync(Witch);
            if (user.Roles.Contains(Berserker)) await user.RemoveRoleAsync(Berserker);
            if (user.Roles.Contains(Tamer)) await user.RemoveRoleAsync(Tamer);
            if (user.Roles.Contains(Monk)) await user.RemoveRoleAsync(Monk);
            if (user.Roles.Contains(Necromancer)) await user.RemoveRoleAsync(Necromancer);
            if (user.Roles.Contains(Paladin)) await user.RemoveRoleAsync(Paladin);
            if (user.Roles.Contains(Swordsman)) await user.RemoveRoleAsync(Swordsman);
            if (user.Roles.Contains(Evangel)) await user.RemoveRoleAsync(Evangel);
            if (user.Roles.Contains(Kitsune)) await user.RemoveRoleAsync(Kitsune);
            Console.WriteLine("Removed " + user.Username + "'s class");
            return;
        }

        public static async Task RemoveUsersRank(SocketGuildUser user)
        {
            Console.WriteLine("Removing " + user.Username + "'s rank");
            var Rank_Bronze = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Bronze");
            var Rank_Silver = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Silver");
            var Rank_Gold = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Gold");
            var Rank_Quartz = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Quartz");
            var Rank_Orichalcum = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Orichalcum");
            var Rank_Platinum = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Platinum");
            var Rank_Master3 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master III");
            var Rank_Master2 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master II");
            var Rank_Master1 = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Rank - Master I");
            if (user.Roles.Contains(Rank_Bronze)) await user.RemoveRoleAsync(Rank_Bronze);
            if (user.Roles.Contains(Rank_Silver)) await user.RemoveRoleAsync(Rank_Silver);
            if (user.Roles.Contains(Rank_Gold)) await user.RemoveRoleAsync(Rank_Gold);
            if (user.Roles.Contains(Rank_Quartz)) await user.RemoveRoleAsync(Rank_Quartz);
            if (user.Roles.Contains(Rank_Orichalcum)) await user.RemoveRoleAsync(Rank_Orichalcum);
            if (user.Roles.Contains(Rank_Platinum)) await user.RemoveRoleAsync(Rank_Platinum);
            if (user.Roles.Contains(Rank_Master3)) await user.RemoveRoleAsync(Rank_Master3);
            if (user.Roles.Contains(Rank_Master2)) await user.RemoveRoleAsync(Rank_Master2);
            if (user.Roles.Contains(Rank_Master1)) await user.RemoveRoleAsync(Rank_Master1);
            Console.WriteLine("Removed " + user.Username + "'s rank");
            return;
        }

        [Command("list")]
        public async Task GetUserListAsync()
        {
            var guild = Context.Guild as SocketGuild;
            await guild.DownloadUsersAsync();
            var online = guild.Users.Where((x) => x.Status == UserStatus.Online).Count();
            var offline = guild.Users.Where((x) => x.Status == UserStatus.Offline).Count();
            await ReplyAsync($"User count: {guild.Users.Count()}      Online: {online}, Offline: {offline}");
        }

        [Command("Cleanup"), Alias("cleanup", "Clean", "clean"), Summary("Remove every channel and rank made by the bot.")]
        public async Task CleanServer()
        {
            IGuildUser user = Context.User as IGuildUser;
            if (!user.GuildPermissions.Administrator && user.Id != 228344819422855168)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Sorry about that :(");
                Embed.WithDescription("Hey there, I noticed you tried to use the ``-cleanup`` command but have invalid " +
                                      "permissions... Only administrators may use this command as it will clear the " +
                                      "entire servers channels and ranks that are associated with the bot. If you are " +
                                      "having issues and need help, please DM Robin#1000 and help will be granted.");
                Embed.Color = Discord.Color.DarkRed;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }
            //Send us some details so me may snoop if we like.
            Console.WriteLine("Guild: " + Context.Guild.Name +
                " has killed the bot :( (" + Context.Guild.Id + "-" + Context.Guild.IconUrl + ")");

            //Delete every category
            foreach (ICategoryChannel category in Context.Guild.CategoryChannels)
            {
                if (category.Name == "Silverkeep") await category.DeleteAsync();
                if (category.Name == "Raid Bosses - Spawns") await category.DeleteAsync();
                if (category.Name == "Silverkeep Training Fields") await category.DeleteAsync();
                if (category.Name == "Graveyard Groves") await category.DeleteAsync();
                if (category.Name == "The Frigid Quarantine") await category.DeleteAsync();
                if (category.Name == "Harpy's Redoubt") await category.DeleteAsync();
                if (category.Name == "Pelean Shores") await category.DeleteAsync();
                if (category.Name == "Salamander Reach") await category.DeleteAsync();
                if (category.Name == "Riverside Mire") await category.DeleteAsync();
                if (category.Name == "City of the Lost") await category.DeleteAsync();
                if (category.Name == "The Northern Wilds") await category.DeleteAsync();
                if (category.Name == "The Aurora") await category.DeleteAsync();
            }

            //Delete every channel.
            foreach (IGuildChannel channel in Context.Guild.Channels)
            {
                if (channel.Name == "entry-exam") await channel.DeleteAsync();
                if (channel.Name == "training-forts") await channel.DeleteAsync();
                if (channel.Name == "rayels-terror-tower") await channel.DeleteAsync();
                if (channel.Name == "skeleton-caves") await channel.DeleteAsync();
                if (channel.Name == "dungeon-skeleton-crypts") await channel.DeleteAsync();
                if (channel.Name == "forest-path") await channel.DeleteAsync();
                if (channel.Name == "goblin-outpost") await channel.DeleteAsync();
                if (channel.Name == "tomb-of-heroes") await channel.DeleteAsync();
                if (channel.Name == "lair-of-trenthola") await channel.DeleteAsync();
                if (channel.Name == "snowy-woods") await channel.DeleteAsync();
                if (channel.Name == "makkosi-camp") await channel.DeleteAsync();
                if (channel.Name == "frigid-wastes") await channel.DeleteAsync();
                if (channel.Name == "abandoned-village") await channel.DeleteAsync();
                if (channel.Name == "frozen-peaks") await channel.DeleteAsync();
                if (channel.Name == "cliffside-barricade") await channel.DeleteAsync();
                if (channel.Name == "roosting-crags") await channel.DeleteAsync();
                if (channel.Name == "taken-temple") await channel.DeleteAsync();
                if (channel.Name == "undying-storm") await channel.DeleteAsync();
                if (channel.Name == "questing") await channel.DeleteAsync();
                if (channel.Name == "daily-blessings") await channel.DeleteAsync();
                if (channel.Name == "guild-shop") await channel.DeleteAsync();
                if (channel.Name == "event-bosses") await channel.DeleteAsync();
                if (channel.Name == "gothkamul") await channel.DeleteAsync();
                if (channel.Name == "rakdoro") await channel.DeleteAsync();
                if (channel.Name == "kenthros") await channel.DeleteAsync();
                if (channel.Name == "arkdul") await channel.DeleteAsync();
                if (channel.Name == "megaton-cove") await channel.DeleteAsync();
                if (channel.Name == "lunar-temple") await channel.DeleteAsync();
                if (channel.Name == "hms-reliant") await channel.DeleteAsync();
                if (channel.Name == "logada-summits") await channel.DeleteAsync();
                if (channel.Name == "burning-outcrop") await channel.DeleteAsync();
                if (channel.Name == "magma-pits") await channel.DeleteAsync();
                if (channel.Name == "draconic-nests") await channel.DeleteAsync();
                if (channel.Name == "twisted-riverbed") await channel.DeleteAsync();
                if (channel.Name == "quarantined-village") await channel.DeleteAsync();
                if (channel.Name == "the-breach") await channel.DeleteAsync();
                if (channel.Name == "within-the-breach") await channel.DeleteAsync();
                if (channel.Name == "outer-blockade") await channel.DeleteAsync();
                if (channel.Name == "shattered-streets") await channel.DeleteAsync();
                if (channel.Name == "catacombs") await channel.DeleteAsync();
                if (channel.Name == "corrupt-citadel") await channel.DeleteAsync();
                if (channel.Name == "ruined-outpost") await channel.DeleteAsync();
                if (channel.Name == "sombris-monument") await channel.DeleteAsync();
                if (channel.Name == "end-of-asteria") await channel.DeleteAsync();
                if (channel.Name == "borealis-gates") await channel.DeleteAsync();
                if (channel.Name == "stellar-fields") await channel.DeleteAsync();
                if (channel.Name == "astral-reaches") await channel.DeleteAsync();
                if (channel.Name == "shifted-wastes") await channel.DeleteAsync();
                if (channel.Name == "fell-pantheon") await channel.DeleteAsync();
            }
            foreach (SocketRole roles in Context.Guild.Roles)
            {
                if (roles.Name == "Rank - Master I") await roles.DeleteAsync();
                if (roles.Name == "Rank - Master II") await roles.DeleteAsync();
                if (roles.Name == "Rank - Master III") await roles.DeleteAsync();
                if (roles.Name == "Rank - Platinum") await roles.DeleteAsync();
                if (roles.Name == "Rank - Orichalcum") await roles.DeleteAsync();
                if (roles.Name == "Rank - Quartz") await roles.DeleteAsync();
                if (roles.Name == "Rank - Gold") await roles.DeleteAsync();
                if (roles.Name == "Rank - Silver") await roles.DeleteAsync();
                if (roles.Name == "Rank - Bronze") await roles.DeleteAsync();
                if (roles.Name == "Knight") await roles.DeleteAsync();
                if (roles.Name == "Assassin") await roles.DeleteAsync();
                if (roles.Name == "Wizard") await roles.DeleteAsync();
                if (roles.Name == "Rogue") await roles.DeleteAsync();
                if (roles.Name == "Archer") await roles.DeleteAsync();
                if (roles.Name == "Witch") await roles.DeleteAsync();
                if (roles.Name == "Berserker") await roles.DeleteAsync();
                if (roles.Name == "Tamer") await roles.DeleteAsync();
                if (roles.Name == "Monk") await roles.DeleteAsync();
                if (roles.Name == "Necromancer") await roles.DeleteAsync();
                if (roles.Name == "Paladin") await roles.DeleteAsync();
                if (roles.Name == "Swordsman") await roles.DeleteAsync();
                if (roles.Name == "Trickster") await roles.DeleteAsync();
                if (roles.Name == "Evangel") await roles.DeleteAsync();
                if (roles.Name == "Kitsune") await roles.DeleteAsync();
                if (roles.Name == "Slayer of Arkdul") await roles.DeleteAsync();
                if (roles.Name == "Slayer of Kenthros") await roles.DeleteAsync();
                if (roles.Name == "Slayer of Rakdoro") await roles.DeleteAsync();
                if (roles.Name == "Slayer of Gothkamul") await roles.DeleteAsync();
            }
            Console.WriteLine("Server was cleaned up");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Server connection has ended");
            Console.WriteLine("Server disconnected...");
            Console.WriteLine("---------------------------------------------------------------");
        }

        //[Command("Debug_RollHelmet"), Alias(), Summary("RIG system for helmets.")]
        //public async Task RollHelmet(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Helmet";
        //    string imageURL = Items.helmet_icons[rando.Next(0, Items.helmet_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    } else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    } else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    } else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    } else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    } else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    } else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    } else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}
        //[Command("Debug_RollChestplate"), Alias(), Summary("RIG system for chestplates.")]
        //public async Task RollChestplate(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Chestplate";
        //    string imageURL = Items.chestplate_icons[rando.Next(0, Items.chestplate_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    }
        //    else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    }
        //    else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    }
        //    else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    }
        //    else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    }
        //    else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    }
        //    else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    }
        //    else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}
        //[Command("Debug_RollGauntlets"), Alias(), Summary("RIG system for gauntlets.")]
        //public async Task RollGauntlet(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Gauntlets";
        //    string imageURL = Items.gauntlet_icons[rando.Next(0, Items.gauntlet_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    }
        //    else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    }
        //    else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    }
        //    else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    }
        //    else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    }
        //    else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    }
        //    else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    }
        //    else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}
        //[Command("Debug_RollBelt"), Alias(), Summary("RIG system for belts.")]
        //public async Task RollBelt(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Belt";
        //    string imageURL = Items.belt_icons[rando.Next(0, Items.belt_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    }
        //    else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    }
        //    else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    }
        //    else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    }
        //    else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    }
        //    else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    }
        //    else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    }
        //    else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}
        //[Command("Debug_RollLeggings"), Alias(), Summary("RIG system for leggings.")]
        //public async Task RollLeggings(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Leggings";
        //    string imageURL = Items.legging_icons[rando.Next(0, Items.legging_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    }
        //    else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    }
        //    else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    }
        //    else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    }
        //    else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    }
        //    else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    }
        //    else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    }
        //    else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}
        //[Command("Debug_RollBoots"), Alias(), Summary("RIG system for boots.")]
        //public async Task RollBoots(IUser User = null)
        //{
        //    var vuser = User as SocketGuildUser;
        //    if (User == null) vuser = Context.User as SocketGuildUser;
        //    EmbedBuilder Embed = new EmbedBuilder();
        //    Random rando = new Random();
        //    string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Boots";
        //    string imageURL = Items.boot_icons[rando.Next(0, Items.boot_icons.Length - 1)];
        //    string Rarity = "Common";
        //    int roll = rando.Next(0, 1000);
        //    if (roll == 747)
        //        Rarity = "Godly";
        //    else if (roll < 650)
        //        Rarity = "Common";
        //    else if (roll < 850)
        //        Rarity = "Uncommon";
        //    else if (roll < 930)
        //        Rarity = "Rare";
        //    else if (roll < 950)
        //        Rarity = "Ultra Rare";
        //    else if (roll < 992)
        //        Rarity = "Epic";
        //    else if (roll < 998)
        //        Rarity = "Legendary";
        //    else if (roll < 1000)
        //        Rarity = "Godly";
        //    uint Armor = 0;
        //    uint Health = 0;
        //    uint HealthRegen = 0;
        //    if (Rarity == "Common")
        //    {
        //        int _roll = rando.Next(1, 8);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 3));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 10) * rando.Next(1, _roll) * rando.Next(0, 2));
        //        Embed.WithColor(Discord.Color.LightGrey);
        //    }
        //    else if (Rarity == "Uncommon")
        //    {
        //        int _roll = rando.Next(2, 15);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 6));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 8) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Embed.WithColor(Discord.Color.Green);
        //    }
        //    else if (Rarity == "Rare")
        //    {
        //        int _roll = rando.Next(4, 35);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 4));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 8));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 6) * rando.Next(1, _roll) * rando.Next(0, 5));
        //        Embed.WithColor(Discord.Color.Blue);
        //    }
        //    else if (Rarity == "Ultra Rare")
        //    {
        //        int _roll = rando.Next(8, 60);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 8));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(1, 17));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 4) * rando.Next(1, _roll) * rando.Next(0, 10));
        //        Embed.WithColor(Discord.Color.Magenta);
        //    }
        //    else if (Rarity == "Epic")
        //    {
        //        int _roll = rando.Next(10, 100);
        //        Armor = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(4, 12));
        //        Health = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(3, 18));
        //        HealthRegen = (uint)rando.Next(0, ((int)Data.Data.GetData_Level(vuser.Id) / 2) * rando.Next(1, _roll) * rando.Next(1, 15));
        //        Embed.WithColor(Discord.Color.Purple);
        //    }
        //    else if (Rarity == "Legendary")
        //    {
        //        int _roll = rando.Next(40, 180);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 20));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(5, 22));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(6, 18));
        //        Embed.WithColor(Discord.Color.Orange);
        //    }
        //    else if (Rarity == "Mythical")
        //    {
        //        int _roll = rando.Next(50, 400);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(10, 35));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(15, 50));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(7, 50));
        //        Embed.WithColor(Discord.Color.Red);
        //    }
        //    else if (Rarity == "Godly")
        //    {
        //        int _roll = rando.Next(300, 8000);
        //        Armor = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 85));
        //        Health = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        HealthRegen = (uint)rando.Next(0, (int)Data.Data.GetData_Level(vuser.Id) * rando.Next(1, _roll) * rando.Next(35, 90));
        //        Embed.WithColor(Discord.Color.Gold);
        //    }
        //    Embed.WithAuthor("Rolled a " + itemName);
        //    Embed.WithImageUrl(imageURL);
        //    Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor + "\n" + "Health: " + Health + "\n" + "Health Regeneration: " + HealthRegen);
        //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
        //}

        public async Task LootItem(SocketCommandContext Context, string monsterName, int monsterLevel, int forceLoot = 0)
        {
            var vuser = Context.User as SocketGuildUser;
            Random ran = new Random();
            int get = ran.Next(0, 6);
            if (get == 0)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();

                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Helmet";
                string imageURL = Items.helmet_icons[rando.Next(0, Items.helmet_icons.Length - 1)];

                string Rarity = "Common";

                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";

                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;

                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);

                }

                Embed.WithAuthor(monsterName + " dropped a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");

                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }

                droppedHelmets.Add(new Helmet(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (get == 1)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();
                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Chestplate";
                string imageURL = Items.chestplate_icons[rando.Next(0, Items.chestplate_icons.Length - 1)];
                string Rarity = "Common";
                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";
                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;
                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);
                }
                Embed.WithAuthor(monsterName + " dropped a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
                droppedChestplates.Add(new Chestplate(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (get == 2)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();
                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Gauntlets";
                string imageURL = Items.gauntlet_icons[rando.Next(0, Items.gauntlet_icons.Length - 1)];
                string Rarity = "Common";
                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";
                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;
                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);
                }
                Embed.WithAuthor(monsterName + " dropped a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
                droppedGauntlets.Add(new Gauntlets(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (get == 3)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();
                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Belt";
                string imageURL = Items.belt_icons[rando.Next(0, Items.belt_icons.Length - 1)];
                string Rarity = "Common";
                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";
                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;
                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);
                }
                Embed.WithAuthor("Rolled a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
                droppedBelt.Add(new Belt(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (get == 4)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();
                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Leggings";
                string imageURL = Items.legging_icons[rando.Next(0, Items.legging_icons.Length - 1)];
                string Rarity = "Common";
                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";
                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;
                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);
                }
                Embed.WithAuthor(monsterName + " dropped a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
                droppedLeggings.Add(new Leggings(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (get == 5)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Random rando = new Random();
                string itemName = Items.mod_names[rando.Next(0, Items.mod_names.Count - 1)] + " Boots";
                string imageURL = Items.boot_icons[rando.Next(0, Items.boot_icons.Length - 1)];
                string Rarity = "Common";
                if (forceLoot == 0)
                {
                    int roll = rando.Next(0, 10000);
                    if (roll == 7470)
                        Rarity = "Godly";
                    else if (roll < 6000)
                        Rarity = "Common";
                    else if (roll < 9000)
                        Rarity = "Uncommon";
                    else if (roll < 9300)
                        Rarity = "Rare";
                    else if (roll < 9500)
                        Rarity = "Ultra Rare";
                    else if (roll < 9920)
                        Rarity = "Epic";
                    else if (roll < 9995)
                        Rarity = "Legendary";
                    else if (roll < 10000)
                        Rarity = "Mythical";
                }
                else if (forceLoot == 1) Rarity = "Common";
                else if (forceLoot == 2) Rarity = "Uncommon";
                else if (forceLoot == 3) Rarity = "Rare";
                else if (forceLoot == 4) Rarity = "Ultra Rare";
                else if (forceLoot == 5) Rarity = "Epic";
                else if (forceLoot == 6) Rarity = "Legendary";
                else if (forceLoot == 7) Rarity = "Mythical";
                else if (forceLoot == 8) Rarity = "Godly";
                uint Armor = 0;
                uint Health = 0;
                uint HealthRegen = 0;
                uint SellPrice = 0;
                if (Rarity == "Common")
                {
                    int _roll = rando.Next(1, 8);
                    Armor = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(8, 9 + (monsterLevel / 7) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(1, 2 + (monsterLevel / 7) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(0, 50) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.LightGrey);
                }
                else if (Rarity == "Uncommon")
                {
                    int _roll = rando.Next(2, 15);
                    Armor = (uint)rando.Next(4, 5 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(3, 4 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(30, 150) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Green);
                }
                else if (Rarity == "Rare")
                {
                    int _roll = rando.Next(4, 30);
                    Armor = (uint)rando.Next(5, 6 + (monsterLevel / 3) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(13, 14 + (monsterLevel / 3) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(8, 9 + (monsterLevel / 3) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(140, 200) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Blue);
                }
                else if (Rarity == "Ultra Rare")
                {
                    int _roll = rando.Next(8, 48);
                    Armor = (uint)rando.Next(7, 8 + (monsterLevel / 4) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(16, 17 + (monsterLevel / 4) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + (monsterLevel / 4) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(125, 350) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Magenta);
                }
                else if (Rarity == "Epic")
                {
                    int _roll = rando.Next(10, 70);
                    Armor = (uint)rando.Next(6, 7 + (monsterLevel / 2) * rando.Next(1, _roll));
                    Health = (uint)rando.Next(14, 15 + (monsterLevel / 2) * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(9, 10 + (monsterLevel / 2) * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(500, 850) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Purple);
                }
                else if (Rarity == "Legendary")
                {
                    int _roll = rando.Next(40, 180);
                    Armor = (uint)rando.Next(8, 9 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(2000, 2500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Orange);
                }
                else if (Rarity == "Mythical")
                {
                    int _roll = rando.Next(50, 350);
                    Armor = (uint)rando.Next(10, 11 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(20, 21 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(8000, 12500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Red);
                }
                else if (Rarity == "Godly")
                {
                    int _roll = rando.Next(200, 1250);
                    Armor = (uint)rando.Next(15, 16 + monsterLevel * rando.Next(1, _roll));
                    Health = (uint)rando.Next(25, 26 + monsterLevel * rando.Next(1, _roll));
                    HealthRegen = (uint)rando.Next(28, 29 + monsterLevel * rando.Next(1, _roll));
                    SellPrice = (uint)rando.Next(100000, 112500) * (1 + Data.Data.GetData_Charisma(Context.User.Id));
                    Embed.WithColor(Discord.Color.Gold);
                }
                Embed.WithAuthor(monsterName + " dropped a " + itemName, Context.User.GetAvatarUrl());
                Embed.WithImageUrl(imageURL);
                Embed.WithDescription("Rarity: " + Rarity + "\n\n" + "Armor: " + Armor / 2 + "\n" + "Health: " + Health / 2 + "\n" + "Health Regeneration: " + HealthRegen / 2 + "\nSell Price: " + SellPrice);
                Embed.WithFooter("Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor slot to make room for this item. Do -Sell Drop to sell this item.");
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
                droppedBoots.Add(new Boots(Context.User.Id, 0, imageURL, itemName, SellPrice, Rarity, Armor / 2, Health / 2, HealthRegen / 2));
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }

        [Command("Equip"), Alias("equip", "EquipDrop", "equipdrop"), Summary("Equip the last dropped item that is under your Id.")]
        public async Task EquipDrop([Remainder]string option = "")
        {
            Helmet helmet = null;
            Chestplate chestplate = null;
            Gauntlets gauntlets = null;
            Belt belt = null;
            Leggings leggings = null;
            Boots boots = null;
            for (int i = 0; i < droppedHelmets.Count; i++)
            {
                if (droppedHelmets[i].OwnerID == Context.User.Id)
                {
                    helmet = droppedHelmets[i];
                    break;
                }
            }
            for (int i = 0; i < droppedChestplates.Count; i++)
            {
                if (droppedChestplates[i].OwnerID == Context.User.Id)
                {
                    chestplate = droppedChestplates[i];
                    break;
                }
            }
            for (int i = 0; i < droppedGauntlets.Count; i++)
            {
                if (droppedGauntlets[i].OwnerID == Context.User.Id)
                {
                    gauntlets = droppedGauntlets[i];
                    break;
                }
            }
            for (int i = 0; i < droppedBelt.Count; i++)
            {
                if (droppedBelt[i].OwnerID == Context.User.Id)
                {
                    belt = droppedBelt[i];
                    break;
                }
            }
            for (int i = 0; i < droppedLeggings.Count; i++)
            {
                if (droppedLeggings[i].OwnerID == Context.User.Id)
                {
                    leggings = droppedLeggings[i];
                    break;
                }
            }
            for (int i = 0; i < droppedBoots.Count; i++)
            {
                if (droppedBoots[i].OwnerID == Context.User.Id)
                {
                    boots = droppedBoots[i];
                    break;
                }
            }
            if (helmet == null && chestplate == null && gauntlets == null && belt == null && leggings == null && boots == null)
            {
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Nothing to loot!");
                Embeda.WithColor(Discord.Color.Red);
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            if (option == string.Empty || option == "")
            {
                if ((helmet != null && Data.Data.GetHelmet(Context.User.Id) != null) ||
                   (chestplate != null && Data.Data.GetChestplate(Context.User.Id) != null) ||
                   (gauntlets != null && Data.Data.GetGauntlet(Context.User.Id) != null) ||
                   (leggings != null && Data.Data.GetLeggings(Context.User.Id) != null) ||
                   (boots != null && Data.Data.GetBoots(Context.User.Id) != null))
                {
                    EmbedBuilder Embedq = new EmbedBuilder();
                    Embedq.WithAuthor("But wait... You already have an item equipped in that slot!");
                    Embedq.WithDescription("If you want to replace the current slot, type -Equip Replace\nIf you want to sell the armor piece" +
                        " you have equipped, type -Sell [Armor Piece (Helmet, Chestplate, Gauntlets, Leggings, Boots)] and then try equipping this" +
                        " item again!\n\nNote: If you loot a new item during this time, this item will be lost!");
                    await Context.Channel.SendMessageAsync("", false, Embedq.Build());
                    return;
                }
                else
                {
                    if (helmet != null)
                    {
                        await Data.Data.SetHelmet(Context.User.Id, helmet);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Helmet Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, helmet.Health, 0, 0, helmet.Health);
                        for (int i = 0; i < droppedHelmets.Count; i++)
                        {
                            if (droppedHelmets[i].OwnerID == Context.User.Id)
                            {
                                droppedHelmets.Remove(droppedHelmets[i]);
                                break;
                            }
                        }
                    }
                    else if (chestplate != null)
                    {
                        await Data.Data.SetChestplate(Context.User.Id, chestplate);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Chestplate Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, chestplate.Health, 0, 0, chestplate.Health);
                        for (int i = 0; i < droppedChestplates.Count; i++)
                        {
                            if (droppedChestplates[i].OwnerID == Context.User.Id)
                            {
                                droppedChestplates.Remove(droppedChestplates[i]);
                                break;
                            }
                        }
                    }
                    if (gauntlets != null)
                    {
                        await Data.Data.SetGauntlets(Context.User.Id, gauntlets);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Gauntlets Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, gauntlets.Health, 0, 0, gauntlets.Health);
                        for (int i = 0; i < droppedGauntlets.Count; i++)
                        {
                            if (droppedGauntlets[i].OwnerID == Context.User.Id)
                            {
                                droppedGauntlets.Remove(droppedGauntlets[i]);
                                break;
                            }
                        }
                    }
                    if (belt != null)
                    {
                        await Data.Data.SetBelt(Context.User.Id, belt);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Belt Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, belt.Health, 0, 0, belt.Health);
                        for (int i = 0; i < droppedBelt.Count; i++)
                        {
                            if (droppedBelt[i].OwnerID == Context.User.Id)
                            {
                                droppedBelt.Remove(droppedBelt[i]);
                                break;
                            }
                        }
                    }
                    if (leggings != null)
                    {
                        await Data.Data.SetLeggings(Context.User.Id, leggings);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Leggings Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, leggings.Health, 0, 0, leggings.Health);
                        for (int i = 0; i < droppedLeggings.Count; i++)
                        {
                            if (droppedLeggings[i].OwnerID == Context.User.Id)
                            {
                                droppedLeggings.Remove(droppedLeggings[i]);
                                break;
                            }
                        }
                    }
                    if (boots != null)
                    {
                        await Data.Data.SeBoots(Context.User.Id, boots);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Boots Equipped!");
                        Embed.WithColor(Discord.Color.Teal);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, boots.Health, 0, 0, boots.Health);
                        for (int i = 0; i < droppedBoots.Count; i++)
                        {
                            if (droppedBoots[i].OwnerID == Context.User.Id)
                            {
                                droppedBoots.Remove(droppedBoots[i]);
                                break;
                            }
                        }
                    }
                }
            }
            else if (option == "Replace" || option == "replace")
            {
                if (helmet != null)
                {
                    if (Data.Data.GetHelmet(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetHelmet(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SetHelmet(Context.User.Id, helmet);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Helmet Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedHelmets.Count; i++)
                    {
                        if (droppedHelmets[i].OwnerID == Context.User.Id)
                        {
                            droppedHelmets.Remove(droppedHelmets[i]);
                            break;
                        }
                    }
                }
                else if (chestplate != null)
                {
                    if (Data.Data.GetChestplate(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetChestplate(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SetChestplate(Context.User.Id, chestplate);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Chestplate Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedChestplates.Count; i++)
                    {
                        if (droppedChestplates[i].OwnerID == Context.User.Id)
                        {
                            droppedChestplates.Remove(droppedChestplates[i]);
                            break;
                        }
                    }
                }
                if (gauntlets != null)
                {
                    if (Data.Data.GetGauntlet(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetGauntlet(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SetGauntlets(Context.User.Id, gauntlets);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Gauntlets Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedGauntlets.Count; i++)
                    {
                        if (droppedGauntlets[i].OwnerID == Context.User.Id)
                        {
                            droppedGauntlets.Remove(droppedGauntlets[i]);
                            break;
                        }
                    }
                }
                if (belt != null)
                {
                    if (Data.Data.GetBelt(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetBelt(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SetBelt(Context.User.Id, belt);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Belt Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedBelt.Count; i++)
                    {
                        if (droppedBelt[i].OwnerID == Context.User.Id)
                        {
                            droppedBelt.Remove(droppedBelt[i]);
                            break;
                        }
                    }
                }
                if (leggings != null)
                {
                    if (Data.Data.GetLeggings(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetLeggings(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SetLeggings(Context.User.Id, leggings);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Leggings Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedLeggings.Count; i++)
                    {
                        if (droppedLeggings[i].OwnerID == Context.User.Id)
                        {
                            droppedLeggings.Remove(droppedLeggings[i]);
                            break;
                        }
                    }
                }
                if (boots != null)
                {
                    if (Data.Data.GetBoots(Context.User.Id) != null)
                        await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, Data.Data.GetBoots(Context.User.Id).Health, 0, 0, 0);
                    await Data.Data.SeBoots(Context.User.Id, boots);
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Boots Equipped!");
                    Embed.WithColor(Discord.Color.Teal);
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    for (int i = 0; i < droppedBoots.Count; i++)
                    {
                        if (droppedBoots[i].OwnerID == Context.User.Id)
                        {
                            droppedBoots.Remove(droppedBoots[i]);
                            break;
                        }
                    }
                }
            }
            else
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Invalid Subcommand");
                Embed.WithDescription("The only valid subcommand for -Equip is Replace. Use the command like: -Equip Replace");
                Embed.WithColor(Discord.Color.Teal);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            //Type -Equip to wear this armor piece, type -Sell [Armor Piece] to sell your current armor 
            //slot to make room for this item. Do -Sell Drop to sell this item.
        }

        [Command("Sell"), Alias("sell", "SellItem", "sellitem", "se"), Summary("Sell a current piece of equipment.")]
        public async Task SellEquipment([Remainder]string option = "")
        {
            option = option.ToLower();
            if (option == "helmet" || option == "helmets")
            {
                if (Data.Data.GetHelmet(Context.User.Id) != null)
                {
                    Helmet item = Data.Data.GetHelmet(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteHelmet(Context.User.Id);
                }
            }
            else if (option == "chestplate" || option == "chestplates")
            {
                if (Data.Data.GetChestplate(Context.User.Id) != null)
                {
                    Chestplate item = Data.Data.GetChestplate(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteChestplate(Context.User.Id);
                }
            }
            else if (option == "gauntlet" || option == "gauntlets")
            {
                if (Data.Data.GetGauntlet(Context.User.Id) != null)
                {
                    Gauntlets item = Data.Data.GetGauntlet(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteGauntlets(Context.User.Id);
                }
            }
            else if (option == "belt" || option == "belts")
            {
                if (Data.Data.GetBelt(Context.User.Id) != null)
                {
                    Belt item = Data.Data.GetBelt(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteBelt(Context.User.Id);
                }
            }
            else if (option == "legging" || option == "leggings")
            {
                if (Data.Data.GetLeggings(Context.User.Id) != null)
                {
                    Leggings item = Data.Data.GetLeggings(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteLeggings(Context.User.Id);
                }
            }
            else if (option == "boot" || option == "boots")
            {
                if (Data.Data.GetBoots(Context.User.Id) != null)
                {
                    Boots item = Data.Data.GetBoots(Context.User.Id);
                    await Data.Data.SaveData(Context.User.Id, item.ItemCost, 0, "", 0, 0, 0, 0, 0);
                    await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, item.Health, 0, 0, 0);
                    EmbedBuilder Embeda = new EmbedBuilder();
                    Embeda.WithAuthor("Sold " + item.ItemName + " for " + item.ItemCost + " Gold!");
                    Embeda.WithColor(Discord.Color.Gold);
                    Embeda.WithUrl(item.WebURL);
                    await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                    await Data.Data.DeleteBoots(Context.User.Id);
                }
            }
            else if (option == "drop" || option == "itemdrop")
            {
                for (int i = 0; i < droppedHelmets.Count; i++)
                {
                    if (droppedHelmets[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedHelmets[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedHelmets[i].ItemName + " for " + droppedHelmets[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedHelmets[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedHelmets.Remove(droppedHelmets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedChestplates.Count; i++)
                {
                    if (droppedChestplates[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedChestplates[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedChestplates[i].ItemName + " for " + droppedChestplates[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedChestplates[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedChestplates.Remove(droppedChestplates[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedGauntlets.Count; i++)
                {
                    if (droppedGauntlets[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedGauntlets[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedGauntlets[i].ItemName + " for " + droppedGauntlets[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedGauntlets[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedGauntlets.Remove(droppedGauntlets[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBelt.Count; i++)
                {
                    if (droppedBelt[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedBelt[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedBelt[i].ItemName + " for " + droppedBelt[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedBelt[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedBelt.Remove(droppedBelt[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedLeggings.Count; i++)
                {
                    if (droppedLeggings[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedLeggings[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedLeggings[i].ItemName + " for " + droppedLeggings[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedLeggings[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedLeggings.Remove(droppedLeggings[i]);
                        break;
                    }
                }
                for (int i = 0; i < droppedBoots.Count; i++)
                {
                    if (droppedBoots[i].OwnerID == Context.User.Id)
                    {
                        await Data.Data.SaveData(Context.User.Id, droppedBoots[i].ItemCost, 0, "", 0, 0, 0, 0, 0);
                        EmbedBuilder Embeda = new EmbedBuilder();
                        Embeda.WithAuthor("Sold " + droppedBoots[i].ItemName + " for " + droppedBoots[i].ItemCost + " Gold!");
                        Embeda.WithColor(Discord.Color.Gold);
                        Embeda.WithUrl(droppedBoots[i].WebURL);
                        await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                        droppedBoots.Remove(droppedBoots[i]);
                        break;
                    }
                }
            }
            else
            {
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Invalid item to sell!");
                Embeda.WithDescription("To sell a piece of equipment you must use the command like:\n``-Sell Helmet``\n\nThe available slots to sell " +
                    "are:\nHelmet, Chestplate, Gauntlets, Belt, Leggings & Boots");
                Embeda.WithColor(Discord.Color.Red);
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
        }

        [Command("Equipment"), Alias("equipment", "EquipDrop", "equipdrop", "e"), Summary("Equip the last dropped item that is under your Id.")]
        public async Task Equipment([Remainder]string option = "")
        {
            option = option.ToLower();
            if (option == "helmet" || option == "helmets")
            {
                Helmet item = Data.Data.GetHelmet(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Helmet");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No helmet equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            else if (option == "gauntlet" || option == "gauntlets")
            {
                Gauntlets item = Data.Data.GetGauntlet(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Gauntlets");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No gauntlets equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            else if (option == "chestplate" || option == "chestplates")
            {
                Chestplate item = Data.Data.GetChestplate(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Chestplate");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No chestplate equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            else if (option == "belt" || option == "belts")
            {
                Belt item = Data.Data.GetBelt(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Belt");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No belt equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            else if (option == "legging" || option == "leggings")
            {
                Leggings item = Data.Data.GetLeggings(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Leggings");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No leggings equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            else if (option == "boot" || option == "boots")
            {
                Boots item = Data.Data.GetBoots(Context.User.Id);
                EmbedBuilder Embeda = new EmbedBuilder();
                Embeda.WithAuthor("Currently Equipped Boots");
                if (item != null)
                {
                    Embeda.WithDescription(item.ItemName + " - " + item.ItemRarity + "\nArmor: " + item.Armor + "\nHealth: " + item.Health + "\nHealth Regeneration: " + item.HealthGainOnDamage + "\nSell Price: " + item.ItemCost);
                    Embeda.WithImageUrl(item.WebURL);
                    Embeda.WithColor(GetRarityColor(item.ItemRarity));
                }
                else
                {
                    Embeda.WithDescription("No boots equipped...");
                    Embeda.WithColor(Discord.Color.Red);
                }
                await Context.Channel.SendMessageAsync("", false, Embeda.Build());
                return;
            }
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Current Equipment:");
            Helmet helm = Data.Data.GetHelmet(Context.User.Id);
            Gauntlets gaun = Data.Data.GetGauntlet(Context.User.Id);
            Chestplate ches = Data.Data.GetChestplate(Context.User.Id);
            Belt belt = Data.Data.GetBelt(Context.User.Id);
            Leggings legg = Data.Data.GetLeggings(Context.User.Id);
            Boots boot = Data.Data.GetBoots(Context.User.Id);
            string helmets;
            string gauntlets;
            string chestplate;
            string belts;
            string leggings;
            string boots;
            if (helm != null)
                helmets = helm.ItemName + " - " + helm.ItemRarity + "\nArmor: " + helm.Armor + "\nHealth: " + helm.Health + "\nHealth Regeneration: " + helm.HealthGainOnDamage + "\nSell Price: " + helm.ItemCost;
            else helmets = "No helmet equipped...";
            if (gaun != null)
                gauntlets = gaun.ItemName + " - " + gaun.ItemRarity + "\nArmor: " + gaun.Armor + "\nHealth: " + gaun.Health + "\nHealth Regeneration: " + gaun.HealthGainOnDamage + "\nSell Price: " + gaun.ItemCost;
            else gauntlets = "No gauntlets equipped...";
            if (ches != null)
                chestplate = ches.ItemName + " - " + ches.ItemRarity + "\nArmor: " + ches.Armor + "\nHealth: " + ches.Health + "\nHealth Regeneration: " + ches.HealthGainOnDamage + "\nSell Price: " + ches.ItemCost;
            else chestplate = "No chestplate equipped...";
            if (belt != null)
                belts = belt.ItemName + " - " + belt.ItemRarity + "\nArmor: " + belt.Armor + "\nHealth: " + belt.Health + "\nHealth Regeneration: " + belt.HealthGainOnDamage + "\nSell Price: " + belt.ItemCost;
            else belts = "No belt equipped...";
            if (legg != null)
                leggings = legg.ItemName + " - " + legg.ItemRarity + "\nArmor: " + legg.Armor + "\nHealth: " + legg.Health + "\nHealth Regeneration: " + legg.HealthGainOnDamage + "\nSell Price: " + legg.ItemCost;
            else leggings = "No leggings equipped...";
            if (boot != null)
                boots = boot.ItemName + " - " + boot.ItemRarity + "\nArmor: " + boot.Armor + "\nHealth: " + boot.Health + "\nHealth Regeneration: " + boot.HealthGainOnDamage + "\nSell Price: " + boot.ItemCost;
            else boots = "No boots equipped...";
            Embed.WithDescription("═════════════════════\n" + helmets + "\n═════════════════════\n" + gauntlets + "\n═════════════════════\n" + chestplate + "\n═════════════════════\n" + belts + "\n═════════════════════\n" + leggings + "\n═════════════════════\n" + boots + "\n═════════════════════");
            Embed.WithFooter("To view the icons of your item, do -Equipment [EquipmentType]");
            Embed.WithColor(Discord.Color.LightOrange);
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
            return;
        }

        public Discord.Color GetRarityColor(string ItemRarity)
        {
            if (ItemRarity == "Common")
                return Discord.Color.LightGrey;
            else if (ItemRarity == "Uncommon")
                return Discord.Color.Green;
            else if (ItemRarity == "Rare")
                return Discord.Color.Blue;
            else if (ItemRarity == "Ultra Rare")
                return Discord.Color.Magenta;
            else if (ItemRarity == "Epic")
                return Discord.Color.Purple;
            else if (ItemRarity == "Legendary")
                return Discord.Color.Orange;
            else if (ItemRarity == "Mythical")
                return Discord.Color.Red;
            else if (ItemRarity == "Godly")
                return Discord.Color.Gold;
            else return Discord.Color.DarkMagenta;
        }

        [Command("Skill"), Alias("Skills", "skill", "skills", "SkillPoint", "skillpoint", "sk"), Summary("Display and up skills.")]
        public async Task LevelUpSkill(string option = "", uint amountOfPoints = 1)
        {
            option = option.ToLower();
            if (option == "" || option.Length == 0)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle(Skill + "Skills");
                Embed.WithDescription("Current Skill Points: " + Data.Data.GetData_SkillPoints(Context.User.Id) +
                                      " " + Skill + "\nTo upgrade a skill do -Skill [Stat].\n\n" +
                                      "Stamina: " + Data.Data.GetData_Stamina(Context.User.Id) +
                                      "\nStability: " + Data.Data.GetData_Stability(Context.User.Id) +
                                      "\nDexterity: " + Data.Data.GetData_Dexterity(Context.User.Id) +
                                      "\nStrength: " + Data.Data.GetData_Strength(Context.User.Id) +
                                      "\nCharisma: " + Data.Data.GetData_Charisma(Context.User.Id) +
                                      "\nLuck: " + Data.Data.GetData_Luck(Context.User.Id));
                Embed.Color = Discord.Color.Teal;
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithFooter("Do -Skill Info for a list of what each skill does");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (option == "info")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle(Skill + "Skill Info");
                Embed.WithDescription("**Stamina**: Gain a random chance to regain 25% health each turn.\n\n" +
                                      "**Stability**: Increases how much health you will regain upon it becoming your turn to fight again.\n\n" +
                                      "**Dexterity**: Increases the chance you will completely dodge an enemy attack.\n\n" +
                                      "**Strength**: Increases how likely you are to deal a critical blow upon attacking doing 200% extra damage.\n\n" +
                                      "**Luck**: Increases the chance an item will drop off an enemy.\n\n" +
                                      "**Charisma**: Increases how much a found item will sell for.\n\n");
                Embed.Color = Discord.Color.Teal;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (Data.Data.GetData_SkillPoints(Context.User.Id) >= amountOfPoints)
            {
                if (option == "stamina" || option == "stability" || option == "dexterity" || option == "strength" || option == "luck" || option == "charisma")
                {
                    if (option == "stamina")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddStaminaPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Stamina increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else if (option == "stability")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddStabilityPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Stability increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else if (option == "dexterity")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddDexterityPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Dexterity increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else if (option == "strength")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddStrengthPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Strength increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else if (option == "luck")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddLuckPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Luck increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else if (option == "charisma")
                    {
                        await Data.Data.SetSkillPoints(Context.User.Id, Data.Data.GetData_SkillPoints(Context.User.Id) - amountOfPoints);
                        await Data.Data.AddCharismaPoints(Context.User.Id, amountOfPoints);
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithTitle("Charisma increased by " + amountOfPoints + "!");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithFooter("Remaining skill points: " + Data.Data.GetData_SkillPoints(Context.User.Id));
                        Embed.Color = Discord.Color.Green;
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Invalid sub command!");
                    Embed.WithDescription("The available sub commands are: Stamina, Stability, Dexterity, Strength, Luck & Charisma. You may additionally leave it blank or @ somone.");
                    Embed.Color = Discord.Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else
            {
                if (option == "stamina" || option == "stability" || option == "dexterity" || option == "strength" || option == "luck" || option == "charisma")
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle(Skill + "Whoops!" + Skill);
                    Embed.WithDescription("You do not have enough skill points to do that!");
                    Embed.Color = Discord.Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Invalid sub command!");
                    Embed.WithDescription("The available sub commands are: Stamina, Stability, Dexterity, Strength, Luck & Charisma. You may additionally leave it blank or @ somone.");
                    Embed.Color = Discord.Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        [Command("Skill"), Alias("Skills", "skill", "skills", "SkillPoint", "skillpoint", "sk"), Summary("Display and up skills.")]
        public async Task LevelUpSkill(IUser User)
        {
            if (User == null) return;
            var vuser = User as SocketGuildUser;
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle(Skill + "Skills");
            Embed.WithDescription("Current Skill Points: " + Data.Data.GetData_SkillPoints(vuser.Id) +
                " " + Skill + "\n\nStamina: " + Data.Data.GetData_Stamina(vuser.Id) +
                "\nStability: " + Data.Data.GetData_Stability(vuser.Id) +
                "\nDexterity: " + Data.Data.GetData_Dexterity(vuser.Id) +
                "\nStrength: " + Data.Data.GetData_Strength(vuser.Id) +
                "\nCharisma: " + Data.Data.GetData_Charisma(vuser.Id) +
                "\nLuck: " + Data.Data.GetData_Luck(vuser.Id));
            Embed.Color = Discord.Color.Teal;
            Embed.WithThumbnailUrl(vuser.GetAvatarUrl());
            Embed.WithFooter("Do -Skill Info for a list of what each skill does");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("lootbox"), Alias("Lootbox", "lootboxes", "Lootboxes", "Lootchest", "Lootchests", "l", "lb"), Summary("Open and look at loot boxes you own.")]
        public async Task Lootboxes([Remainder]string option = "")
        {
            option = option.ToLower();
            if (option == "")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Current Loot Chests");
                Embed.WithDescription("Common Chests: " + Data.Data.GetData_CommonBoxCount(Context.User.Id) +
                    "\nUncommon Chests: " + Data.Data.GetData_UncommonBoxCount(Context.User.Id) +
                    "\nRare Chests: " + Data.Data.GetData_RareBoxCount(Context.User.Id) +
                    "\nVery Rare Chests: " + Data.Data.GetData_VeryRareBoxCount(Context.User.Id) +
                    "\nEpic Chests: " + Data.Data.GetData_EpicBoxCount(Context.User.Id) +
                    "\nLegendary Chests: " + Data.Data.GetData_LegendaryBoxCount(Context.User.Id) +
                    "\nMythic Chests: " + Data.Data.GetData_MythicBoxCount(Context.User.Id) +
                    "\nGodly Chests: " + Data.Data.GetData_GodlyBoxCount(Context.User.Id));
                Embed.Color = Discord.Color.Gold;
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithFooter("Do -Lootbox [Type] to open them!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (option == "common")
            {
                if (Data.Data.GetData_CommonBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Common Loot Box Opened!");
                    Embed.Color = Discord.Color.LightGrey;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetCommonBoxCount(Context.User.Id, Data.Data.GetData_CommonBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Common Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 1);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "uncommon")
            {
                if (Data.Data.GetData_UncommonBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Uncommon Loot Box Opened!");
                    Embed.Color = Discord.Color.Green;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetUncommonBoxCount(Context.User.Id, Data.Data.GetData_UncommonBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Uncommon Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 2);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "rare")
            {
                if (Data.Data.GetData_RareBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Rare Loot Box Opened!");
                    Embed.Color = Discord.Color.Blue;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetRareBoxCount(Context.User.Id, Data.Data.GetData_RareBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Rare Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 3);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "veryrare")
            {
                if (Data.Data.GetData_VeryRareBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Very Rare Loot Box Opened!");
                    Embed.Color = Discord.Color.Magenta;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetVeryRareBoxCount(Context.User.Id, Data.Data.GetData_VeryRareBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Very Rare Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 4);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "epic")
            {
                if (Data.Data.GetData_EpicBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Epic Loot Box Opened!");
                    Embed.Color = Discord.Color.Purple;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetEpicBoxCount(Context.User.Id, Data.Data.GetData_EpicBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Epic Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 5);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "legendary")
            {
                if (Data.Data.GetData_LegendaryBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Legendary Loot Box Opened!");
                    Embed.Color = Discord.Color.Orange;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetLegendaryBoxCount(Context.User.Id, Data.Data.GetData_LegendaryBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Legendary Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 6);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "mythic")
            {
                if (Data.Data.GetData_MythicBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Mythic Loot Box Opened!");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetMythicBoxCount(Context.User.Id, Data.Data.GetData_MythicBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Mythic Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 7);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            else if (option == "godly")
            {
                if (Data.Data.GetData_GodlyBoxCount(Context.User.Id) >= 1)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Godly Loot Box Opened!");
                    Embed.Color = Discord.Color.Teal;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Data.Data.SetGodlyBoxCount(Context.User.Id, Data.Data.GetData_GodlyBoxCount(Context.User.Id) - 1);
                    await LootItem(Context, "Godly Loot Box", (int)Data.Data.GetData_Level(Context.User.Id), 8);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("You do not have any loot boxes of this type...");
                    Embed.Color = Discord.Color.Red;
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
            /*
                else if(forceLoot == 1) Rarity = "Common";
                else if(forceLoot == 2) Rarity = "Uncommon";
                else if(forceLoot == 3) Rarity = "Rare";
                else if(forceLoot == 4) Rarity = "Ultra Rare";
                else if(forceLoot == 5) Rarity = "Epic";
                else if(forceLoot == 6) Rarity = "Legendary";
                else if(forceLoot == 7) Rarity = "Mythical";
                else if(forceLoot == 8) Rarity = "Godly"; 
             */
        }

        [Command("DeleteAccount"), Alias("deleteaccount", "Deleteaccount"), Summary("Deletes the users account.")]
        public async Task DeleteAccount([Remainder]string option = "")
        {
            if(option == Data.Data.GetData_Name(Context.User.Id) || option == "(OverwriteNameCommand)")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You have left the guild...");
                Embed.WithDescription("***THIS ACTION IS NOT REVERSABLE***\n\n\nFarewell ***" + Data.Data.GetData_Name(Context.User.Id) + "***.");
                Embed.Color = Discord.Color.Red;
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                Data.Data.DeleteSave(Context.User.Id);
            }
            else
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Wait! You're about to delete your current save!");
                Embed.WithDescription("***THIS ACTION IS NOT REVERSABLE***\n\n\nIf you do this, you will lose all of your progress, and to continue, you will need to re-inroll with the guild...\n\n" +
                    "To confirm this action, you will need to retype this command, but enter your profile name ***" + Data.Data.GetData_Name(Context.User.Id) + "*** after it.\n\n\nOnce your account is gone, there is no way to recover your data, so be positive you want to restart.");
                Embed.Color = Discord.Color.Red;
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }

        [Command("Reload"), Alias("reload"), Summary("Reloads a specified module.")]
        public async Task ReloadModule([Remainder]string option = "")
        {
            if (RPG_Bot.Resources.BotAdmins.HasPermission(Context.User.Id))
            {
                option = option.ToLower();

                if (option == "spell" || option == "spells")
                {
                    await Context.Channel.SendMessageAsync("Spells are being reloaded.");

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    spellDatabase = new Dictionary<string, Spell>();

                    UserCredential credential;

                    using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        // The file token.json stores the user's access and refresh tokens, and is created
                        // automatically when the authorization flow completes for the first time.
                        string credPath = "token.json";
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Program.Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result;
                        Console.WriteLine("Credential file saved to: " + credPath);
                    }

                    // Create Google Sheets API service.
                    var service = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = Program.ApplicationName,
                    });

                    // Define request parameters.
                    string spreadsheetId = "1EFRAzE64e8bZlDs1WWZiG5e1hJgOWlQ3uIezJ_6ZZCQ";//"1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
                    string range = "Runtime Spells!A3:T";

                    SpreadsheetsResource.ValuesResource.GetRequest request =
                            service.Spreadsheets.Values.Get(spreadsheetId, range);

                    // Prints the names and majors of students in a sample spreadsheet:
                    // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
                    // https://docs.google.com/spreadsheets/d/1zBzUJ8TE6jpQRF-RubmZ-p8PmVXqm_o-cjUb4kwnh1U/edit?usp=sharing
                    // https://docs.google.com/spreadsheets/d/1EFRAzE64e8bZlDs1WWZiG5e1hJgOWlQ3uIezJ_6ZZCQ/edit?usp=sharing
                    ValueRange response = request.Execute();

                    IList<IList<object>> values = response.Values;

                    int loaded = 0;

                    if (values != null && values.Count > 0)
                    {
                        /*Console.WriteLine("Spell Name, Spell Rarity/How to Obtain, Type, Class Exclusive, Spell Damage Min, Spell Damage Max, Crit Chance, " +
                                            "Modifier ID (See Spell Index), Spell Heal Min (% max health), Spell Heal Max (% max health), Mana Cost, Stamina Cost	Cast Chance (%), " +
                                            "Spell Type, Element, Spell Turns, Spell Description, Spell Icon, Allowed in PvP, Allowed on World Bosses");*/

                        foreach (var row in values)
                        {
                            //try
                            //{
                            // Print columns A and E, which correspond to indices 0 and 4.
                            Console.WriteLine("Adding new spell to the database: {0}", row[0] as string);

                            loaded++;

                            float damage_min = 0;
                            float damage_max = 0;
                            float crit_chance = 0;
                            float modID = 0;
                            float spell_heal_min = 0;
                            float spell_heal_max = 0;
                            float mana_cost = 0;
                            float stamina_cost = 0;
                            float cast_chance = 0;
                            float spell_turns = 0;
                            bool allowed_in_pvp = false;
                            bool allowed_on_world_bosses = false;

                            try { damage_min = Convert.ToSingle((row[4] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set damage_min of " + row[0] as string); }
                            try { damage_max = Convert.ToSingle((row[5] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set damage_max of " + row[0] as string); }
                            try { crit_chance = Convert.ToSingle((row[6] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set crit_chance of " + row[0] as string); }
                            try { modID = Convert.ToSingle((row[7] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set modID of " + row[0] as string); }
                            try { spell_heal_min = Convert.ToSingle((row[8] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set spell_heal_min of " + row[0] as string); }
                            try { spell_heal_max = Convert.ToSingle((row[9] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set spell_heal_max of " + row[0] as string); }
                            try { mana_cost = Convert.ToSingle((row[10] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set mana_cost of " + row[0] as string); }
                            try { stamina_cost = Convert.ToSingle((row[11] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set stamina_cost of " + row[0] as string); }
                            try { cast_chance = Convert.ToSingle((row[12] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set cast_chance of " + row[0] as string); }
                            try { spell_turns = Convert.ToSingle((row[15] as string).Replace("%", "").Replace("x", "")); }
                            catch { Console.WriteLine("Failed to set spell_turns of " + row[0] as string); }
                            try { allowed_in_pvp = (row[17] as string) == "Yes" ? true : false; }
                            catch { Console.WriteLine("Failed to set allowed_in_pvp of " + row[0] as string); }
                            try { allowed_on_world_bosses = (row[18] as string) == "Yes" ? true : false; }
                            catch { Console.WriteLine("Failed to set allowed_on_world_bosses of " + row[0] as string); }

                            Resources.Spell spell = new Resources.Spell
                            (
                                row[0] as string,
                                row[1] as string,
                                row[2] as string,
                                row[3] as string,
                                damage_min,
                                damage_max,
                                crit_chance,
                                modID,
                                spell_heal_min,
                                spell_heal_max,
                                mana_cost,
                                stamina_cost,
                                cast_chance,
                                row[13] as string,
                                row[14] as string,
                                spell_turns,
                                row[16] as string,
                                row[17] as string,
                                allowed_in_pvp,
                                allowed_on_world_bosses
                            );

                            Gameplay.spellDatabase.Add(row[0] as string, spell);
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine("Failed to parse {0}, because {1}", row[0] as string, ex.ToString());
                            //}
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }

                    stopWatch.Stop();

                    TimeSpan ts = stopWatch.Elapsed;

                    Console.WriteLine("Loaded {0} spells into the spell database in {1}ms", loaded, ts.Milliseconds);
                    await Context.Channel.SendMessageAsync("Loaded " + loaded + " spells into the spell database in " + ts.Milliseconds + "ms");
                }
                else if (option == "monster" || option == "monsters" || option == "creature" || option == "creatures")
                {
                    await Context.Channel.SendMessageAsync("Monsters are being reloaded.");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You lack the permission to use that command. You must be a certified Bot Admin.");
            }
        }
    }
}