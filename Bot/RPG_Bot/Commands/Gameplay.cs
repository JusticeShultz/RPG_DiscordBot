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

namespace RPG_Bot.Commands
{
    public class Gameplay : ModuleBase<SocketCommandContext> 
    {
        //Handles every server having their own enemies.
        public class ServerIntegratedEnemy
        {
            public Enemy[] currentSpawn { get; set; } = new Enemy[50];
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
                for(int i = 0; i < 100; ++i)
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
            public Discord.Rest.RestUserMessage[] FightMessages = new Discord.Rest.RestUserMessage[100];
            public ulong ServerID { get; set; } = 0;


            public ServerEditMessages() { }
            ~ServerEditMessages() { }
        }

        static ServerIntegratedEnemy[] CurrentSpawn = new ServerIntegratedEnemy[250];
        static BossJoiningSystem[] CurrentBossJoiners = new BossJoiningSystem[250];
        static ServerEditMessages[] ServerMessages = new ServerEditMessages[250];

        static Random rng = new Random();

        //Channel 1-15 reserved for zones
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

        [Command("Spawn"), Alias("spawn", "SpawnEnemy", "spawnenemy", "s", "S"), Summary("Spawn an enemy to fight.")]
        public async Task Spawn(string remaining = null)
        {
            int genType = rng.Next(1, 60);

            //If time replace spawn zones with spawn tables
            if (Context.Channel.Name == "lv1-5")
            {
                await ClearChat();

                if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Goblin, 1);
                else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Imp, 1);
                else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 1);
                else if(genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LogSpider, 1);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TrainingBot, 1);
            }
            else if (Context.Channel.Name == "lv5-10")
            {
                await ClearChat();

                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossTrainingBot, 2);
                else if (genType > 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeleton, 2);
                else if (genType > 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LargeTrainingBot, 2);
                else if (genType < 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BigTrainingBot, 2);
                else if (genType == 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 2);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SkeletonFootSoldier, 2);
            }
            else if (Context.Channel.Name == "lv10-15")
            {
                await ClearChat();

                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeBoss, 3);
                else if (genType > 30)
                {
                    if (genType == 55) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverPot, 3);
                    else if (genType > 50) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantEnt, 3);
                    else if (genType > 43) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeEnt, 3);
                    else if (genType > 40) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GigaEnt, 3);
                    else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GiantRat, 3);
                }
                else
                {
                    if (genType > 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Treant, 3);
                    else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HauntedTree, 3);
                    else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hornet, 3);
                }
            }
            else if (Context.Channel.Name == "lv15-20")
            {
                await ClearChat();

                if (genType == 50) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverPot, 4);
                else if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Mimic, 4);
                else if (genType > 30)
                {

                    if(genType > 40)
                        await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HobGoblin, 4);
                    else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Cyclops, 4);
                }
                else if (genType > 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Golem, 4);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Mushroom, 4);
            }
            else if (Context.Channel.Name == "lv20-30")
            {
                await ClearChat();
                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossFrostWolfPackLeader, 5);
                else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossIceDragon, 5);
                else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostKnight, 5);
                else if (genType > 11) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostWolf, 5);
                else if (genType == 8) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SilverMimic, 5);
                else if (genType == 4) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 5);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostBear, 5);
            }
            else if (Context.Channel.Name == "lv30-40")
            {
                //Lava theme
                await ClearChat();
                if (genType < 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HarpyCaster, 6);
                else if (genType < 20) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.RoyalPelican, 6);
                else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BronzePot, 6);
                else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HarpyShaman, 6);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Harpy, 6);
            }
            else if (Context.Channel.Name == "lv40-50")
            {
                await ClearChat();
                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Leviathan, 7);
                else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.CetusBoss, 7);
                else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Megaton, 7);
                else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 7);
                else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LizardMan, 7);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FishMan, 7);
            }
            else if (Context.Channel.Name == "lv50-60")
            {
                await ClearChat();
                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LavaDragon, 8);
                else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderDragonAdult, 8);
                else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderDragon, 8);
                else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 8);
                else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SalamanderAdult, 8);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Salamander, 8);
            }
            else if (Context.Channel.Name == "lv60-70")
            {
                await ClearChat();
                if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hydra, 9);
                else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.SeaDragon, 9);
                else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Reaver, 9);
                else if (genType == 15) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GoldenPot, 9);
                else if (genType > 10) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.GreaterLeech, 9);
                else await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Leech, 9);
            }
            else if (Context.Channel.Name == "lv70-80")
            {
                await ClearChat();
                if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Phoenix, 10);
                else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Bergelmir, 10);
                else if (genType < 19) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hastur, 10);
                else if (genType < 25) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Kirin, 10);
                else if (genType < 32) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Haechi, 10);
                else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Orochi, 10);
                else if (genType < 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Eclipse, 10);
                else if (genType < 52) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Niflheim, 10);
                else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Muspelheim, 10);
            }
            else if (Context.Channel.Name == "lv80-90")
            {
                await ClearChat();
                if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Phoenix, 11);
                else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Bergelmir, 11);
                else if (genType < 19) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hastur, 11);
                else if (genType < 25) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Kirin, 11);
                else if (genType < 32) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Haechi, 11);
                else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Orochi, 11);
                else if (genType < 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Eclipse, 11);
                else if (genType < 52) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Niflheim, 11);
                else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Muspelheim, 11);
            }
            else if (Context.Channel.Name == "lv90-100")
            {
                await ClearChat();
                if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.AbyssDragon, 12);
                else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yamusichea, 12);
                else if (genType < 18) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Flare, 12);
                else if (genType < 24) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeletor, 12);
                else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Khan, 12);
                else if (genType < 36) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Crom, 12);
                else if (genType < 42) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rex, 12);
                else if (genType < 48) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Vasuki, 12);
                else if (genType < 54) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Thunder, 12);
                else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Taigo, 12);
            }
            else if (Context.Channel.Name == "lv100-150")
            {
                await ClearChat();
                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 14);
            }
            else if (Context.Channel.Name == "lv150-200")
            {
                await ClearChat();
                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 15);
            }
            else if (Context.Channel.Name == "lv200-400")
            {
                await ClearChat();
                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 16);
            }
            else if (Context.Channel.Name == "lv400-800")
            {
                await ClearChat();
                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 17);
            }
            else if (Context.Channel.Name == "lv800-1000")
            {
                await ClearChat();
                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Pepe, 18);
            }
            else if (Context.Channel.Name == "the-aurora")
            {
                await ClearChat();
                if (genType < 4) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Duck, 13);
                else if (genType < 8) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Flare, 13);
                else if (genType < 14) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Khan, 13);
                else if (genType < 22) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeletor, 13);
                else if (genType < 27) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rex, 13);
                else if (genType < 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Vasuki, 13);
                else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Masamune, 13);
                else if (genType < 43) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rourtu, 13);
                else if (genType < 49) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Trikento, 13);
                else if (genType < 54) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Blackout, 13);
                else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yggdrasil, 13);
            }
            else
            {
                SocketGuildUser user = Context.User as SocketGuildUser;

                if (remaining == "eventboss" && user.GuildPermissions.Administrator && Context.Guild.Id == EnemyTemplates.ServerID)
                {
                    await ClearChat();
                    var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                    await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                    await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
                    //await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
                }
                else
                if (remaining == "testboss" && user.GuildPermissions.Administrator && Context.Guild.Id == EnemyTemplates.ServerID)
                {
                    await ClearChat();
                    var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                    await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                    await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.KenthrosBoss, 42);
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("You are not in an area known to contain monsters!");
                    Embed.Color = Color.Red;
                    var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await Context.Channel.DeleteMessageAsync(Context.Message.Id);
                    await Task.Delay(5000);
                    await msg.DeleteAsync();
                    return;
                }
            }

            await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, Data.Data.GetData_Health(Context.User.Id));

            int server = 0;

            if (Context.Channel.Name == "lv1-5") server = 1;
            else if (Context.Channel.Name == "lv5-10") server = 2;
            else if (Context.Channel.Name == "lv10-15") server = 3;
            else if (Context.Channel.Name == "lv15-20") server = 4;
            else if (Context.Channel.Name == "lv20-30") server = 5;
            else if (Context.Channel.Name == "lv30-40") server = 6;
            else if (Context.Channel.Name == "lv40-50") server = 7;
            else if (Context.Channel.Name == "lv50-60") server = 8;
            else if (Context.Channel.Name == "lv60-70") server = 9;
            else if (Context.Channel.Name == "lv70-80") server = 10;
            else if (Context.Channel.Name == "lv80-90") server = 11;
            else if (Context.Channel.Name == "lv90-100") server = 12;
            else if (Context.Channel.Name == "lv100-150") server = 14;
            else if (Context.Channel.Name == "lv150-200") server = 15;
            else if (Context.Channel.Name == "lv200-400") server = 16;
            else if (Context.Channel.Name == "lv400-800") server = 17;
            else if (Context.Channel.Name == "lv800-1000") server = 18;
            else if (Context.Channel.Name == "the-aurora") server = 13;
            else if (Context.Channel.Name == "event-bosses") server = 35;
            else if (Context.Channel.Name == "gothkamul") server = 40;
            else if (Context.Channel.Name == "rakdoro") server = 41;
            else if (Context.Channel.Name == "kenthros") server = 42;
            else if (Context.Channel.Name == "arkdul") server = 43;

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

            await Context.Channel.DeleteMessageAsync(Context.Message.Id);
        }

        public async Task ClearChat()
        {
            //[DEPRECATED] - Discord limiter ruins everything
            //Does what the task says, clears the chat.
            //var messages = await Context.Channel.GetMessagesAsync(5000).FlattenAsync();
            //await(Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

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

            for (int gg = 0; gg < times; ++gg)
            {
                int server = 0;

                //Figure out which server channel to fight in(this makes it 
                //so multiple channels can have a different enemy:
                if (Context.Channel.Name == "lv1-5") server = 1;
                else if (Context.Channel.Name == "lv5-10") server = 2;
                else if (Context.Channel.Name == "lv10-15") server = 3;
                else if (Context.Channel.Name == "lv15-20") server = 4;
                else if (Context.Channel.Name == "lv20-30") server = 5;
                else if (Context.Channel.Name == "lv30-40") server = 6;
                else if (Context.Channel.Name == "lv40-50") server = 7;
                else if (Context.Channel.Name == "lv50-60") server = 8;
                else if (Context.Channel.Name == "lv60-70") server = 9;
                else if (Context.Channel.Name == "lv70-80") server = 10;
                else if (Context.Channel.Name == "lv80-90") server = 11;
                else if (Context.Channel.Name == "lv90-100") server = 12;
                else if (Context.Channel.Name == "lv100-150") server = 14;
                else if (Context.Channel.Name == "lv150-200") server = 15;
                else if (Context.Channel.Name == "lv200-400") server = 16;
                else if (Context.Channel.Name == "lv400-800") server = 17;
                else if (Context.Channel.Name == "lv800-1000") server = 18;
                else if (Context.Channel.Name == "the-aurora") server = 13;
                else if (Context.Channel.Name == "event-bosses") server = 35;
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
                }
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
                }
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
                }
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
                }

                if (CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > 0)
                {
                    uint userdmg = Data.Data.GetData_Damage(Context.User.Id);

                    uint dmg = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinDamage, (int)CurrentSpawn[serverId].currentSpawn[server].MaxDamage);

                    if (Data.Data.GetData_CurrentHealth(Context.User.Id) > dmg)
                    {
                        if (CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > userdmg)
                            CurrentSpawn[serverId].currentSpawn[server].CurrentHealth -= userdmg;
                        else CurrentSpawn[serverId].currentSpawn[server].CurrentHealth = 0;

                        await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, (uint)(-dmg));

                        if (CurrentSpawn[serverId].currentSpawn[server].CurrentHealth > 0)
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor(CurrentSpawn[serverId].currentSpawn[server].Name + " Lv" + CurrentSpawn[serverId].currentSpawn[server].MaxLevel);
                            Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                            Embed.Color = Color.Red;

                            Embed.WithFooter(CurrentSpawn[serverId].currentSpawn[server].Name + "'s Health: " + CurrentSpawn[serverId].currentSpawn[server].CurrentHealth + " / " + CurrentSpawn[serverId].currentSpawn[server].MaxHealth);
                            Embed.WithDescription("You took " + dmg + " damage.\nYou have " + Data.Data.GetData_CurrentHealth(Context.User.Id) + " health remaining.");

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

                            if (gg < 2)
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
                                Embed.Color = Color.Teal;
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
                                Embed2.Color = Color.Gold;
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
                                Embed.Color = Color.Teal;
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
                                Embed2.Color = Color.Gold;
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
                                Embed.Color = Color.Teal;
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
                                Embed2.Color = Color.Gold;
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
                                Embed.Color = Color.Teal;
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
                                Embed2.Color = Color.Gold;
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
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("You defeated " + CurrentSpawn[serverId].currentSpawn[server].Name);
                                Embed.WithImageUrl(CurrentSpawn[serverId].currentSpawn[server].WebURL);
                                Embed.Color = Color.Gold;
                                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

                                uint gold = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinGoldDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxGoldDrop);
                                uint xp = (uint)rng.Next((int)CurrentSpawn[serverId].currentSpawn[server].MinXpDrop, (int)CurrentSpawn[serverId].currentSpawn[server].MaxXpDrop);

                                Embed.WithDescription("You recieved " + gold + " Gold Coins & " + xp + " XP");
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                //Set enemy back to an empty.

                                await Data.Data.SaveData(Context.User.Id, gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Context.User.Id));

                                //Check level up.
                                await CheckLevelUp(Context.User.Id);

                                int EventDrop = rng.Next(1, 10);

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
                        Embed.Color = Color.Red;
                        Embed.WithFooter("You lost: " + (uint)Math.Round(Data.Data.GetData_GoldAmount(Context.User.Id) * 0.15) + "Gold & " + (uint)Math.Round(Data.Data.GetData_XP(Context.User.Id) * 0.25) + "XP");
                        Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                        Embed.WithDescription("You died and lost some Gold and XP. You revive at the guilds holy church to continue your journey...");
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
                    Embed.WithAuthor("There currently are no monsters to fight!");
                    Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                    Embed.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    break;
                }
            }

            await Context.Message.DeleteAsync();
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

        [Command("prune"), Alias("Prune", "clear", "Clear"), Summary("Prune the current channel."), 
            RequireUserPermission(GuildPermission.Administrator), RequireBotPermission(ChannelPermission.ManageMessages)]
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
                Embed1.Color = Color.Red;
                Embed1.WithFooter("You are now level: " + (Data.Data.GetData_Level(UserId) + 1));
                await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                //Level up
                await Data.Data.SaveData(UserId, 0, 0, "", 5 + ((uint)Math.Round(Data.Data.GetData_Level(UserId) * 0.75)), 5 + Data.Data.GetData_Level(UserId), 1, (uint)(Data.Data.GetData_Level(UserId) * Data.Data.GetData_Level(UserId) * -1), 0);
            }
        }

        public async Task SpawnEnemy(Enemy enemy, int channel)
        {
            int serverId = 0;
            bool registered = false;
            
            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                if(guilds.Id == Context.Guild.Id)
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
            Embed.Color = Color.Red;
            Embed.WithFooter("Health: " + health + " / " + health);

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
                    Embed.Color = Color.Red;
                    Embed.WithFooter("Health: " + health + " / " + health);

                    var chnl = RPG_Bot.Program.Client.GetChannel(channelA.Id) as IMessageChannel;
                    await chnl.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        public async Task FindEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You found a piece of Mystic Log!");
            Embed.WithDescription("This item may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
            Embed.Color = Color.Teal;
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithFooter("Special event runs until March 20th");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public async Task FindBossEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You harvest the fallen boss and gather 35 Mystic Logs!");
            Embed.WithDescription("This item may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
            Embed.Color = Color.Teal;
            Embed.WithFooter("Special event runs until March 20th");
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
                    "This command will create a little less than 20 new channels and some new roles, " +
                    "this means that the command " +
                    "is for administrators only. Please talk to the server owner if you need further assistance " +
                    "with setting up the bot and using your servers administration commands!");
                Embed.Color = Color.DarkRed;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            //Send us some details so me may snoop if we like.
            Console.WriteLine("Guild: " + Context.Guild.Name +
                " has initialized the bot! (" + Context.Guild.Id + "-" + Context.Guild.IconUrl + ")");

            //This will handle a first time join of a guild. It will create the necessary channels
            //and provide an instructions pop up.

            bool[] channels = new bool[30];
            for (int q = 0; q < 20; ++q) channels[q] = false;

            foreach (IGuildChannel channel in Context.Guild.Channels)
            {
                if (channel.Name == "lv1-5") channels[0] = true;
                else if (channel.Name == "lv5-10") channels[1] = true;
                else if (channel.Name == "lv10-15") channels[2] = true;
                else if (channel.Name == "lv15-20") channels[3] = true;
                else if (channel.Name == "lv20-30") channels[4] = true;
                else if (channel.Name == "lv30-40") channels[5] = true;
                else if (channel.Name == "lv40-50") channels[6] = true;
                else if (channel.Name == "lv50-60") channels[7] = true;
                else if (channel.Name == "lv60-70") channels[8] = true;
                else if (channel.Name == "lv70-80") channels[9] = true;
                else if (channel.Name == "lv80-90") channels[10] = true;
                else if (channel.Name == "lv90-100") channels[11] = true;
                else if (channel.Name == "lv100-150") channels[12] = true;
                else if (channel.Name == "lv150-200") channels[13] = true;
                else if (channel.Name == "lv200-400") channels[14] = true;
                else if (channel.Name == "lv400-800") channels[15] = true;
                else if (channel.Name == "lv800-1000") channels[16] = true;
                else if (channel.Name == "the-aurora") channels[17] = true;
                else if (channel.Name == "questing") channels[18] = true;
                else if (channel.Name == "daily-blessings") channels[19] = true;
                else if (channel.Name == "guild-shop") channels[20] = true;
                else if (channel.Name == "event-bosses") channels[21] = true;
                else if (channel.Name == "gothkamul") channels[22] = true;
                else if (channel.Name == "rakdoro") channels[23] = true;
                else if (channel.Name == "kenthros") channels[24] = true;
                else if (channel.Name == "arkdul") channels[25] = true;
            }

            if (!channels[0]) await Context.Guild.CreateTextChannelAsync("lv1-5");
            if (!channels[1]) await Context.Guild.CreateTextChannelAsync("lv5-10");
            if (!channels[2]) await Context.Guild.CreateTextChannelAsync("lv10-15");
            if (!channels[3]) await Context.Guild.CreateTextChannelAsync("lv15-20");
            if (!channels[4]) await Context.Guild.CreateTextChannelAsync("lv20-30");
            if (!channels[5]) await Context.Guild.CreateTextChannelAsync("lv30-40");
            if (!channels[6]) await Context.Guild.CreateTextChannelAsync("lv40-50");
            if (!channels[7]) await Context.Guild.CreateTextChannelAsync("lv50-60");
            if (!channels[8]) await Context.Guild.CreateTextChannelAsync("lv60-70");
            if (!channels[9]) await Context.Guild.CreateTextChannelAsync("lv70-80");
            if (!channels[10]) await Context.Guild.CreateTextChannelAsync("lv80-90");
            if (!channels[11]) await Context.Guild.CreateTextChannelAsync("lv90-100");
            if (!channels[12]) await Context.Guild.CreateTextChannelAsync("lv100-150");
            if (!channels[13]) await Context.Guild.CreateTextChannelAsync("lv150-200");
            if (!channels[14]) await Context.Guild.CreateTextChannelAsync("lv200-400");
            if (!channels[15]) await Context.Guild.CreateTextChannelAsync("lv400-800");
            if (!channels[16]) await Context.Guild.CreateTextChannelAsync("lv800-1000");
            if (!channels[17]) await Context.Guild.CreateTextChannelAsync("the-aurora");
            if (!channels[18]) await Context.Guild.CreateTextChannelAsync("questing");
            if (!channels[19]) await Context.Guild.CreateTextChannelAsync("daily-blessings");
            if (!channels[20]) await Context.Guild.CreateTextChannelAsync("guild-shop");
            if (!channels[21]) await Context.Guild.CreateTextChannelAsync("event-bosses");
            if (!channels[22]) await Context.Guild.CreateTextChannelAsync("gothkamul");
            if (!channels[23]) await Context.Guild.CreateTextChannelAsync("rakdoro");
            if (!channels[24]) await Context.Guild.CreateTextChannelAsync("kenthros");
            if (!channels[25]) await Context.Guild.CreateTextChannelAsync("arkdul");

            foreach (IGuildChannel channel in Context.Guild.Channels)
            {
                if (channel.Name == "lv1-5")
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Thanks for inviting RPG Bot!");
                    Embed.WithDescription("As you may have noticed, the bot generated a ton of new channels for the " +
                        "server. Sorry if this is an inconvenience but these channels are reserved for the bot, " +
                        "it is recommended you put them under their own channel category. Please note, if these " +
                        "channels are renamed the bot will not function here correctly!\n\n\n" +
                        "To get started using the bot, type ``-begin [Class] [Name] [Age]``\nUse ``-help`` for general commands\n" +
                        "Type ``-class`` or ``-classes`` to see all available class types!\n" +
                        "\n\nNeed further help? Join the Asteria RPG home server here and we will be more than happy to help!" +
                        "\n" + "https://discord.gg/jnFfqhm");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202701210025993/Zygmunt_Baur_Dreams_and_Illusions_transparent.webp");
                    Embed.Color = Color.Gold;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    break;
                }
            }

            bool[] Ranks = new bool[30];

            for (int a = 0; a < Ranks.Count(); ++a)
                Ranks[a] = false;

            foreach(SocketRole roles in Context.Guild.Roles)
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
                if (roles.Name == "Nechromancer") Ranks[19] = true;
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

            if (!Ranks[1]) await Context.Guild.CreateRoleAsync("Rank - Master I", Discord.GuildPermissions.None, new Discord.Color(255, 0, 0));
            if (!Ranks[2]) await Context.Guild.CreateRoleAsync("Rank - Master II", Discord.GuildPermissions.None, new Discord.Color(255, 79, 0));
            if (!Ranks[3]) await Context.Guild.CreateRoleAsync("Rank - Master III", Discord.GuildPermissions.None, new Discord.Color(255, 150, 32));
            if (!Ranks[4]) await Context.Guild.CreateRoleAsync("Rank - Platinum", Discord.GuildPermissions.None, new Discord.Color(186, 192, 255));
            if (!Ranks[5]) await Context.Guild.CreateRoleAsync("Rank - Orichalcum", Discord.GuildPermissions.None, new Discord.Color(0, 232, 255));
            if (!Ranks[6]) await Context.Guild.CreateRoleAsync("Rank - Quartz", Discord.GuildPermissions.None, new Discord.Color(255, 255, 255));
            if (!Ranks[7]) await Context.Guild.CreateRoleAsync("Rank - Gold", Discord.GuildPermissions.None, new Discord.Color(255, 181, 0));
            if (!Ranks[8]) await Context.Guild.CreateRoleAsync("Rank - Silver", Discord.GuildPermissions.None, new Discord.Color(131, 131, 131));
            if (!Ranks[9]) await Context.Guild.CreateRoleAsync("Rank - Bronze", Discord.GuildPermissions.None, new Discord.Color(156, 71, 0));
            if (!Ranks[10]) await Context.Guild.CreateRoleAsync("Knight", Discord.GuildPermissions.None, new Discord.Color(87, 87, 87));
            if (!Ranks[11]) await Context.Guild.CreateRoleAsync("Assassin", Discord.GuildPermissions.None, new Discord.Color(97, 97, 97));
            if (!Ranks[12]) await Context.Guild.CreateRoleAsync("Wizard", Discord.GuildPermissions.None, new Discord.Color(162, 135, 0));
            if (!Ranks[13]) await Context.Guild.CreateRoleAsync("Rogue", Discord.GuildPermissions.None, new Discord.Color(79, 102, 73));
            if (!Ranks[14]) await Context.Guild.CreateRoleAsync("Archer", Discord.GuildPermissions.None, new Discord.Color(71, 149, 156));
            if (!Ranks[15]) await Context.Guild.CreateRoleAsync("Witch", Discord.GuildPermissions.None, new Discord.Color(184, 65, 65));
            if (!Ranks[16]) await Context.Guild.CreateRoleAsync("Berserker", Discord.GuildPermissions.None, new Discord.Color(145, 4, 4));
            if (!Ranks[17]) await Context.Guild.CreateRoleAsync("Tamer", Discord.GuildPermissions.None, new Discord.Color(152, 168, 74));
            if (!Ranks[18]) await Context.Guild.CreateRoleAsync("Monk", Discord.GuildPermissions.None, new Discord.Color(160, 112, 233));
            if (!Ranks[19]) await Context.Guild.CreateRoleAsync("Nechromancer", Discord.GuildPermissions.None, new Discord.Color(73, 73, 73));
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
            
            Console.WriteLine("Connecting to 1 new Guild\n" +
                "There are 250 available object pool slots.");

            await Gameplay.UpdateUserData();

            for (int q = 0; q < CurrentSpawn.Count(); ++q)
            {
                //Don't need to re-add.
                if (CurrentBossJoiners[q] == null)
                    CurrentBossJoiners[q] = new BossJoiningSystem();

                if (CurrentSpawn[q].ServerID == Context.Guild.Id) break;

                //Find an empty spot and make ourselves that space in memory.
                if(CurrentSpawn[q].ServerID == 0)
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
                    if (role.Name == "Nechromancer") contains[18] = true;
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
                        var Nechromancer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Nechromancer");
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

                        if (Class == "Nechromancer") if (!user.Roles.Contains(Nechromancer))
                        {
                            await RemoveUsersClass(user);
                            await user.AddRoleAsync(Nechromancer);
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
            var Nechromancer = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Nechromancer");
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
    }
}