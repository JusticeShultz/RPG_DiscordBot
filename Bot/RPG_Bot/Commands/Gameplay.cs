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
        static Random rng = new Random();

        //Object Pooled Enemies.
        static Enemy[] currentSpawn = new Enemy[50];

        //Boss Joiners [Object Pools]
        static ulong[] Boss1Joiner = new ulong[100];
        static ulong[] Boss2Joiner = new ulong[100];
        static ulong[] Boss3Joiner = new ulong[100];
        static ulong[] Boss4Joiner = new ulong[100];
        static ulong[] BossEventJoiner = new ulong[100];

        //Channel 1-15 reserved for zones
        //Channel 35 reserved for event bosses
        //Channel 40 - 45 reserved for world bosses

        [Command("Spawn"), Alias("spawn", "SpawnEnemy", "spawnenemy", "s", "S"), Summary("Spawn an enemy to fight.")]
        public async Task Spawn(string remaining = null)
        {
            int genType = rng.Next(1, 60);

            //If time replace spawn zones with spawn tables in JSON
            if (Context.Channel.Id == EnemyTemplates.Channel1)
            {
                if (genType == 1) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Goblin, 1);
                else  if (genType > 1) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TrainingBot, 1);
            }
            else
            {
                if (Context.Channel.Id == EnemyTemplates.Channel2)
                {
                    if(genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossTrainingBot, 2);
                    else if (genType > 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LargeTrainingBot, 2);
                    else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BigTrainingBot, 2);
                }
                else
                {
                    if (Context.Channel.Id == EnemyTemplates.Channel3)
                    {
                        if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeBoss, 3);
                        else if (genType > 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.TreeEnt, 3);
                        else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HauntedTree, 3);
                    }
                    else
                    {
                        if (Context.Channel.Id == EnemyTemplates.Channel4)
                        {
                            if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Mimic, 4);
                            else if (genType > 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.HobGoblin, 4);
                            else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Golem, 4);
                        }
                        else if (Context.Channel.Id == EnemyTemplates.Channel5)
                        {
                            if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossFrostWolfPackLeader, 5);
                            else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.BossIceDragon, 5);
                            else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostKnight, 5);
                            else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.FrostWolf, 5);
                        }
                        else if (Context.Channel.Id == EnemyTemplates.Channel6)
                        {
                            if (genType == 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Leviathan, 6);
                            else if (genType == 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.CetusBoss, 6);
                            else if (genType > 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Megaton, 6);
                            else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.LizardMan, 6);
                        }
                        else if (Context.Channel.Id == EnemyTemplates.Channel7)
                        {
                            if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Phoenix, 7);
                            else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Bergelmir, 7);
                            else if (genType < 19) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Hastur, 7);
                            else if (genType < 25) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Kirin, 7);
                            else if (genType < 32) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Haechi, 7);
                            else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Orochi, 7);
                            else if (genType < 45) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Eclipse, 7);
                            else if (genType < 52) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Niflheim, 7);
                            else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Muspelheim, 7);
                        }
                        else if (Context.Channel.Id == EnemyTemplates.Channel8)
                        {
                            if (genType < 6) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.AbyssDragon, 8);
                            else if (genType < 12) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yamusichea, 8);
                            else if (genType < 18) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Flare, 8);
                            else if (genType < 24) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeletor, 8);
                            else if (genType < 30) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Khan, 8);
                            else if (genType < 36) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Crom, 8);
                            else if (genType < 42) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rex, 8);
                            else if (genType < 48) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Vasuki, 8);
                            else if (genType < 54) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Thunder, 8);
                            else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Taigo, 8);
                        }
                        else if (Context.Channel.Id == EnemyTemplates.Channel9)
                        {
                            if (genType < 4) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Duck, 9);
                            else if (genType < 8) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Flare, 9);
                            else if (genType < 14) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Khan, 9);
                            else if (genType < 22) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Skeletor, 9);
                            else if (genType < 27) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rex, 9);
                            else if (genType < 31) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Vasuki, 9);
                            else if (genType < 38) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Masamune, 9);
                            else if (genType < 43) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Rourtu, 9);
                            else if (genType < 49) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Trikento, 9);
                            else if (genType < 54) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Blackout, 9);
                            else if (genType < 61) await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.Yggdrasil, 9);
                        }
                        else
                        {
                            SocketGuildUser user = Context.User as SocketGuildUser;

                            if (remaining == "eventboss" && user.GuildPermissions.Administrator)
                            {
                                var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 35);
                            }
                            else
                            if (remaining == "testboss" && user.GuildPermissions.Administrator)
                            {
                                var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                                await SpawnEnemy(RPG_Bot.Resources.EnemyTemplates.KenthrosBoss, 42);
                            }
                            else
                            {
                                EmbedBuilder Embed = new EmbedBuilder();
                                Embed.WithAuthor("You are not in an area known to contain monsters!");
                                Embed.Color = Color.Red;
                                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            }
                        }
                    }
                }
            }

            await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, Data.Data.GetData_Health(Context.User.Id));
        }

        [Command("fight"), Alias("Fight", "F", "f"), Summary("Fight a spawned enemy.")]
        public async Task Fight()
        {
            int server = 0;

            //Figure out which server channel to fight in(this makes it 
            //so multiple channels can have a different enemy:
            if (Context.Channel.Id == EnemyTemplates.Channel1)
                server = 1;
            else if (Context.Channel.Id == EnemyTemplates.Channel2)
                server = 2;
            else if (Context.Channel.Id == EnemyTemplates.Channel3)
                server = 3;
            else if (Context.Channel.Id == EnemyTemplates.Channel4)
                server = 4;
            else if (Context.Channel.Id == EnemyTemplates.Channel5)
                server = 5;
            else if (Context.Channel.Id == EnemyTemplates.Channel6)
                server = 6;
            else if (Context.Channel.Id == EnemyTemplates.Channel7)
                server = 7;
            else if (Context.Channel.Id == EnemyTemplates.Channel8)
                server = 8;
            else if (Context.Channel.Id == EnemyTemplates.Channel9)
                server = 9;
            else if (Context.Channel.Id == EnemyTemplates.ChannelEventSpawn)
                server = 35;
            else if (Context.Channel.Id == EnemyTemplates.gothkamul)
            {
                server = 40;

                for (int i = 0; i < 100; ++i)
                {
                    if (Boss1Joiner[i] == Context.User.Id) break;

                    if (Boss1Joiner[i] == 0)
                    {
                        Boss1Joiner[i] = Context.User.Id;
                        break;
                    }
                }
            }
            else if (Context.Channel.Id == EnemyTemplates.rakdoro)
            {
                server = 41;

                for (int i = 0; i < 100; ++i)
                {
                    if (Boss2Joiner[i] == Context.User.Id) break;

                    if (Boss2Joiner[i] == 0)
                    {
                        Boss2Joiner[i] = Context.User.Id;
                        break;
                    }
                }
            }
            else if (Context.Channel.Id == EnemyTemplates.kenthros)
            {
                server = 42;

                for (int i = 0; i < 100; ++i)
                {
                    if (Boss3Joiner[i] == Context.User.Id) break;

                    if (Boss3Joiner[i] == 0)
                    {
                        Boss3Joiner[i] = Context.User.Id;
                        break;
                    }
                }
            }
            else if (Context.Channel.Id == EnemyTemplates.arkdul)
            {
                server = 43;

                for(int i = 0; i < 100; ++i)
                {
                    if (Boss4Joiner[i] == Context.User.Id) break;

                    if(Boss4Joiner[i] == 0)
                    {
                        Boss4Joiner[i] = Context.User.Id;
                        break;
                    }
                }
            }

            if (currentSpawn[server].CurrentHealth > 0)
            if (currentSpawn[server].CurrentHealth > 0)
            {
                uint userdmg = Data.Data.GetData_Damage(Context.User.Id);

                uint dmg = (uint)rng.Next((int)currentSpawn[server].MinDamage, (int)currentSpawn[server].MaxDamage);

                if (Data.Data.GetData_CurrentHealth(Context.User.Id) > dmg)
                {
                    if (currentSpawn[server].CurrentHealth > userdmg)
                        currentSpawn[server].CurrentHealth -= userdmg;
                    else currentSpawn[server].CurrentHealth = 0;

                    await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 0, 0, 0, (uint)(-dmg));

                    if (currentSpawn[server].CurrentHealth > 0)
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor(currentSpawn[server].Name + " Lv" + currentSpawn[server].MaxLevel);
                        Embed.WithImageUrl(currentSpawn[server].WebURL);
                        Embed.Color = Color.Red;
                        Embed.WithFooter(currentSpawn[server].Name + "'s Health: " + currentSpawn[server].CurrentHealth + " / " + currentSpawn[server].MaxHealth);
                        Embed.WithDescription("You took " + dmg + " damage.\nYou have " + Data.Data.GetData_CurrentHealth(Context.User.Id) + " health remaining.");
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else
                    {
                        if (server == 40)
                        {
                            uint gold = (uint)rng.Next((int)currentSpawn[server].MinGoldDrop, (int)currentSpawn[server].MaxGoldDrop);
                            uint xp = (uint)rng.Next((int)currentSpawn[server].MinXpDrop, (int)currentSpawn[server].MaxXpDrop);
                            
                            for (int i = 0; i < 100; ++i)
                            {
                                if (Boss1Joiner[i] != 0)
                                {
                                    await Data.Data.SaveData(Boss1Joiner[i], gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Boss1Joiner[i]));
                                    await CheckLevelUp(Boss1Joiner[i]);
                                }
                                else break;
                            }

                            for (int z = 0; z < 100; ++z)
                            {
                                Boss1Joiner[z] = 0;
                            }

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("Adventurers who participated defeated " + currentSpawn[server].Name + "!");
                            Embed.WithImageUrl(currentSpawn[server].WebURL);
                            Embed.Color = Color.Teal;
                            Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (server == 41)
                        {
                            uint gold = (uint)rng.Next((int)currentSpawn[server].MinGoldDrop, (int)currentSpawn[server].MaxGoldDrop);
                            uint xp = (uint)rng.Next((int)currentSpawn[server].MinXpDrop, (int)currentSpawn[server].MaxXpDrop);

                            for (int i = 0; i < 100; ++i)
                            {
                                if (Boss2Joiner[i] != 0)
                                {
                                    await Data.Data.SaveData(Boss2Joiner[i], gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Boss2Joiner[i]));
                                    await CheckLevelUp(Boss2Joiner[i]);
                                }
                                else break;
                            }

                            for (int z = 0; z < 100; ++z)
                            {
                                Boss2Joiner[z] = 0;
                            }

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("Adventurers who participated defeated " + currentSpawn[server].Name + "!");
                            Embed.WithImageUrl(currentSpawn[server].WebURL);
                            Embed.Color = Color.Teal;
                            Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (server == 42)
                        {
                            uint gold = (uint)rng.Next((int)currentSpawn[server].MinGoldDrop, (int)currentSpawn[server].MaxGoldDrop);
                            uint xp = (uint)rng.Next((int)currentSpawn[server].MinXpDrop, (int)currentSpawn[server].MaxXpDrop);

                            for (int i = 0; i < 100; ++i)
                            {
                                if (Boss3Joiner[i] != 0)
                                {
                                    await Data.Data.SaveData(Boss3Joiner[i], gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Boss3Joiner[i]));
                                    await CheckLevelUp(Boss3Joiner[i]);
                                }
                                else break;
                            }

                            for (int z = 0; z < 100; ++z)
                            {
                                Boss3Joiner[z] = 0;
                            }

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("Adventurers who participated defeated " + currentSpawn[server].Name + "!");
                            Embed.WithImageUrl(currentSpawn[server].WebURL);
                            Embed.Color = Color.Teal;
                            Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else if (server == 43)
                        {
                            uint gold = (uint)rng.Next((int)currentSpawn[server].MinGoldDrop, (int)currentSpawn[server].MaxGoldDrop);
                            uint xp = (uint)rng.Next((int)currentSpawn[server].MinXpDrop, (int)currentSpawn[server].MaxXpDrop);

                            for (int i = 0; i < 100; ++i)
                            {
                                if (Boss4Joiner[i] != 0)
                                {
                                    await Data.Data.SaveData(Boss4Joiner[i], gold, 0, "", 0, 0, 0, xp, Data.Data.GetData_Health(Boss4Joiner[i]));
                                    await CheckLevelUp(Boss4Joiner[i]);
                                }
                                else break;
                            }

                            for (int z = 0; z < 100; ++z)
                            {
                                Boss4Joiner[z] = 0;
                            }

                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("Adventurers who participated defeated " + currentSpawn[server].Name + "!");
                            Embed.WithImageUrl(currentSpawn[server].WebURL);
                            Embed.Color = Color.Teal;
                            Embed.WithDescription("Adventurers who participated recieved " + gold + " Gold Coins & " + xp + " XP!");
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                        else
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("You defeated " + currentSpawn[server].Name);
                            Embed.WithImageUrl(currentSpawn[server].WebURL);
                            Embed.Color = Color.Gold;

                            uint gold = (uint)rng.Next((int)currentSpawn[server].MinGoldDrop, (int)currentSpawn[server].MaxGoldDrop);
                            uint xp = (uint)rng.Next((int)currentSpawn[server].MinXpDrop, (int)currentSpawn[server].MaxXpDrop);

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
                        }
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
                    Embed.WithDescription("You died and lost some Gold and XP. You revive at the guilds holy church to continue your journey...");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await Data.Data.SaveData(Context.User.Id, ((uint)Math.Round(Data.Data.GetData_GoldAmount(Context.User.Id) * -0.15)), 0, "", 0, 0, 0, (uint)Math.Round((Data.Data.GetData_XP(Context.User.Id) * -0.25)), Data.Data.GetData_Health(Context.User.Id));
                }
            }
            else
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("There currently are no monsters to fight!");
                Embed.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
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

                Embed1.WithImageUrl(url);
                Embed1.Color = Color.Red;
                Embed1.WithFooter("You are now level: " + (Data.Data.GetData_Level(UserId) + 1));
                await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                //Level up
                await Data.Data.SaveData(UserId, 0, 0, "", 3 + ((uint)Math.Round(Data.Data.GetData_Level(UserId) * 0.6)), 2 + Data.Data.GetData_Level(UserId), 1, (uint)(Data.Data.GetData_Level(UserId) * Data.Data.GetData_Level(UserId) * -1), 0);
            }
        }

        public async Task SpawnEnemy(Enemy enemy, int channel)
        {
            uint health = (uint)rng.Next((int)enemy.MinHealth, (int)enemy.MaxHealth);
            uint level = (uint)rng.Next((int)enemy.MinLevel, (int)enemy.MaxLevel);

            currentSpawn[channel] = enemy;
            currentSpawn[channel].MaxHealth = health;
            currentSpawn[channel].CurrentHealth = health;
            currentSpawn[channel].MaxLevel = level;

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("A " + enemy.Name + " lv" + level + " appears. Type -fight to begin battle.");
            Embed.WithImageUrl(enemy.WebURL);
            Embed.Color = Color.Red;
            Embed.WithFooter("Health: " + health + " / " + health);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        //40-44 spawn slot, this spawn takes no context.
        static public async Task SpawnWorldBoss(Enemy enemy, int channel)
        {
            ulong ChannelID = 0;

            if (channel == 40) ChannelID = EnemyTemplates.gothkamul;
            if (channel == 41) ChannelID = EnemyTemplates.rakdoro;
            if (channel == 42) ChannelID = EnemyTemplates.kenthros;
            if (channel == 43) ChannelID = EnemyTemplates.arkdul;
            if (channel == 44 || channel == 35) ChannelID = EnemyTemplates.ChannelEventSpawn;

            int usersOnline = -1;
            
            foreach (SocketGuildUser users in RPG_Bot.Program.Client.GetGuild(RPG_Bot.Resources.EnemyTemplates.ServerID).Users)
                if(users.Status != UserStatus.Offline || users.Status != UserStatus.AFK)
                {
                    ++usersOnline;
                }

            uint health = (uint)rng.Next((int)enemy.MinHealth * (int)Math.Round(usersOnline * 0.5), (int)enemy.MaxHealth * (int)Math.Round(usersOnline * 0.5));
            uint level = (uint)rng.Next((int)enemy.MinLevel, (int)enemy.MaxLevel);

            currentSpawn[channel] = enemy;
            currentSpawn[channel].MaxHealth = health;
            currentSpawn[channel].CurrentHealth = health;
            currentSpawn[channel].MaxLevel = level;;

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("A " + enemy.Name + " lv" + level + " appears. Type -fight to begin battle.");
            Embed.WithImageUrl(enemy.WebURL);
            Embed.Color = Color.Red;
            Embed.WithFooter("Health: " + health + " / " + health);

            var chnl = RPG_Bot.Program.Client.GetChannel(ChannelID) as IMessageChannel;
            await chnl.SendMessageAsync("", false, Embed.Build());
        }

        public async Task FindEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You found a piece of Mystic Log!");
            Embed.WithDescription("This item may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
            Embed.Color = Color.Teal;
            Embed.WithFooter("Special event runs until February 20th");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public async Task FindBossEventItem()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("You harvest the fallen boss and gather 35 Mystic Logs!");
            Embed.WithDescription("This item may be traded in with the shop keeper! Use `-event shop` in the guild store to see what you can recieve!");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
            Embed.Color = Color.Teal;
            Embed.WithFooter("Special event runs until February 20th");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        public static async Task BossChance()
        {
            await Task.Delay(300000);
            BossChance();

            int boss = rng.Next(0, 15);

            if (boss == 10)
            {
                boss = rng.Next(1, 5);

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
                        await SpawnWorldBoss(RPG_Bot.Resources.EnemyTemplates.EventTreeBoss, 44);
                        break;
                    default:
                        Console.WriteLine("Error spawning a boss! Num Error: " + boss);
                        break;
                }
            }
            else Console.WriteLine("Boss tried to spawn but luck says otherwise, trying again" +
                " in 5 minutes!");
        }
    }
}