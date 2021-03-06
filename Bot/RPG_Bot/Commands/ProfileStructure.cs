﻿using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using RPG_Bot.Resources.Database;
using System.Collections.Generic;
using static RPG_Bot.Emojis.Emojis;

namespace RPG_Bot.Currency
{
    public class ProfileStructure : ModuleBase<SocketCommandContext>
    {
        [Command("profile"), Alias("Profile", "P", "p"), Summary("See a users profile.")]
        public async Task Profile(IUser User = null)
        {
            var vuser = User as SocketGuildUser;

            if (User == null) vuser = Context.User as SocketGuildUser;

            #region Deprecated [rank ID system]
            /*
            //ID system deprecated
            var knight = Context.Guild.GetRole(542217921246658610);
                var witch = Context.Guild.GetRole(542217744805003264);
                var wizard = Context.Guild.GetRole(542217748009451551);
                var archer = Context.Guild.GetRole(542217745690001409);
                var assassin = Context.Guild.GetRole(542217793601536010);
                var rogue = Context.Guild.GetRole(542217746440519681);

                <@&542217921246658610 > -Knight
                <@&542217744805003264 > -Witch
                <@&542217748009451551 > -Wizard
                <@&542217745690001409 > -Archer
                <@&542217793601536010 > -Assassin
                <@&542217746440519681 > -Rogue
            */
            #endregion

            EmbedBuilder Embed = new EmbedBuilder();
            var user = vuser;
            string classType = "";
            string classEmoji = "";
            string UsersClass = Data.Data.GetClass(vuser.Id);

            if (UsersClass == "Archer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                classType = "Archer";
                classEmoji = Archer;
            }
            else if (UsersClass == "Knight")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                classType = "Knight";
                classEmoji = Knight;
            }
            else if (UsersClass == "Witch")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                classType = "Witch";
                classEmoji = Witch;
            }
            else if (UsersClass == "Rogue")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                classType = "Rogue";
                classEmoji = Rogue;
            }
            else if (UsersClass == "Wizard")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                classType = "Wizard";
                classEmoji = Wizard;
            }
            else if (UsersClass == "Assassin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                classType = "Assassin";
                classEmoji = Assassin;
            }
            else if (UsersClass == "Berserker")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904669427859456/Berserker.png");
                classType = "Berserker";
                classEmoji = Berserker;
            }
            else if (UsersClass == "Necromancer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904652009046026/Nechromancer.webp");
                classType = "Necromancer";
                classEmoji = Nechromancer;
            }
            else if (UsersClass == "Trickster")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904660275757066/Trickster.png");
                classType = "Trickster";
                classEmoji = Trickster;
            }
            else if (UsersClass == "Kitsune")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904663740252161/Kitsune.webp");
                classType = "Kitsune";
                classEmoji = Kitsune;
            }
            else if (UsersClass == "Paladin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904668891119627/Paladin.png");
                classType = "Paladin";
                classEmoji = Paladin;
            }
            else if (UsersClass == "Monk")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904672250626048/Monk.png");
                classType = "Monk";
                classEmoji = Monk;
            }
            else if (UsersClass == "Evangel")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904680668725258/Evangel.png");
                classType = "Evangel";
                classEmoji = Evangel;
            }
            else if (UsersClass == "Tamer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904678713917445/Cat_Tamer.png");
                classType = "Tamer";
                classEmoji = Tamer;
            }
            else if (UsersClass == "Swordsman")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904710016139265/Swordsman.png");
                classType = "Swordsman";
                classEmoji = Swordsman;
            }
            else
            {
                Embed.WithAuthor("Error!");
                Embed.WithFooter("Please make an account and try again!");
                Embed.WithDescription("Sorry but you have not registered with the guild yet! To register you must " +
                    "do ``-begin [Class] [Name] [Age]``, type ``-help`` if you need additional guidance!");
                Embed.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            //Console.WriteLine("Context User: " + Context.User.Id + " - User: " + vuser.Id);
            string name = Data.Data.GetData_Name(vuser.Id);
            uint age = Data.Data.GetData_Age(vuser.Id);
            uint coins = Data.Data.GetData_GoldAmount(vuser.Id);
            uint level = Data.Data.GetData_Level(vuser.Id);
            uint health = Data.Data.GetData_Health(vuser.Id);
            uint damage = Data.Data.GetData_Damage(vuser.Id);
            uint currentHealth = Data.Data.GetData_CurrentHealth(vuser.Id);
            uint currentXp = Data.Data.GetData_XP(vuser.Id);
            uint eventItem = Data.Data.GetData_Event1(vuser.Id);
            uint eventItem2 = Data.Data.GetData_Event2(vuser.Id);
            uint guildGems = Data.Data.GetData_Event3(vuser.Id);
            uint neededXP = (Data.Data.GetData_Level(vuser.Id) * Data.Data.GetData_Level(vuser.Id));
            uint currentArmor = 0;
            uint currentRegen = 0;

            if (Data.Data.GetHelmet(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetHelmet(vuser.Id).Armor;
                currentRegen += Data.Data.GetHelmet(vuser.Id).HealthGainOnDamage;
            }
            if (Data.Data.GetChestplate(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetChestplate(vuser.Id).Armor;
                currentRegen += Data.Data.GetChestplate(vuser.Id).HealthGainOnDamage;
            }
            if (Data.Data.GetGauntlet(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetGauntlet(vuser.Id).Armor;
                currentRegen += Data.Data.GetGauntlet(vuser.Id).HealthGainOnDamage;
            }
            if (Data.Data.GetBelt(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetBelt(vuser.Id).Armor;
                currentRegen += Data.Data.GetBelt(vuser.Id).HealthGainOnDamage;
            }
            if (Data.Data.GetLeggings(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetLeggings(vuser.Id).Armor;
                currentRegen += Data.Data.GetLeggings(vuser.Id).HealthGainOnDamage;
            }
            if (Data.Data.GetBoots(vuser.Id) != null)
            {
                currentArmor += Data.Data.GetBoots(vuser.Id).Armor;
                currentRegen += Data.Data.GetBoots(vuser.Id).HealthGainOnDamage;
            }

            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithAuthor("Profile of: " + user.Username, user.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("XP until level up: " + currentXp + " / " + neededXP);
            Embed.WithDescription("Class: " + classType + " " + classEmoji + "\nName: " + name + " " + Dove + "\nAge: " + age + " " + Age + "\n" + "\nGold Coins: " + coins + Coins + "\n" +
                "Guild Gems: " + guildGems + " " + GuildGem + "\nPresents (Event Item): " + eventItem + " " + Present + "\n\n\n" +
                "Level: " + level + " " + Level + "\n" + "Health: " + currentHealth + "/" + health + " " + Health + "\nArmor: " + currentArmor + " " + Armor + "\nHealth Regeneration: " + currentRegen + " " + Regeneration + "\nDamage: " + damage + " " + Damage + "\n\n" +
                Skill + " " + Data.Data.GetData_SkillPoints(vuser.Id) + " Skill Points" + " " + Skill + "\n" +
                "Stamina: " + Data.Data.GetData_Stamina(vuser.Id) +
                "\nStability: " + Data.Data.GetData_Stability(vuser.Id) +
                "\nDexterity: " + Data.Data.GetData_Dexterity(vuser.Id) +
                "\nStrength: " + Data.Data.GetData_Strength(vuser.Id) +
                "\nCharisma: " + Data.Data.GetData_Charisma(vuser.Id) +
                "\nLuck: " + Data.Data.GetData_Luck(vuser.Id));
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Group("gold"), Alias("golds, Gold, Golds"), Summary("The base keyword for working with gold.")]
        public class GoldGroup : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("me", "my", "wealth"), Summary("Shows your current gold. You may also do ``-Profile`` for the more data about yourself.")]
            public async Task Me()
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("You have " + Data.Data.GetData_GoldAmount(Context.User.Id) + " Gold Coins!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }

            [Command("give"), Alias("gift", "Give", "Gift"), Summary("Send a user a certain amount of your currency.")]
            public async Task Give(IUser User = null, uint Amount = 0)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithColor(40, 200, 150);

                if (User == null)
                {
                    Embed.WithAuthor("Error!");
                    Embed.WithFooter("Funds not given.");
                    Embed.WithDescription("This command requires you enter it as ``-Gold Gift @User [Amount]``");
                    Embed.Color = Color.Red;

                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
                else
                if (User.IsBot)
                {
                    Embed.WithAuthor("Error!");
                    Embed.WithFooter("Money is worthless to me you filthy mortal...");
                    Embed.WithDescription("You may not transfer funds to me or any other god, sorry!");
                    Embed.Color = Color.Red;

                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
                else
                if (Amount >= 1)
                {
                    //A user is provided and amount is valid.
                    SocketGuildUser User1 = Context.User as SocketGuildUser;
                    if (Context.User.Id != 228344819422855168 && Context.User.Id != 409566463658033173)
                    {
                        if (User1 == User)
                        {
                            Embed.WithAuthor("Error!");
                            Embed.WithFooter("Funds not given.");
                            Embed.WithDescription("You may not gift yourself money!");
                            Embed.Color = Color.Red;

                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            return;
                        }
                        else
                        {
                            bool exist = true;
                            using (var DbContext = new SqliteDbContext())
                            {
                                var query = DbContext.Data.Where(x => x.UserID == User.Id);
                                if (query == null || query.Count() < 1) exist = false;
                            }

                            if (exist)
                            {
                                if (Data.Data.GetData_GoldAmount(Context.User.Id) >= Amount)
                                {
                                    Embed.WithAuthor("Success!");
                                    Embed.WithFooter("Funds were given.");
                                    Embed.WithDescription($"{User.Mention}, you have recieved **{Amount}** gold coins from {Context.User.Mention}");
                                    Embed.Color = Color.Gold;

                                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                                    await Data.Data.SaveData(Context.User.Id, (uint)(-Amount), 0, "", 0, 0, 0, 0, 0);
                                    await Data.Data.SaveData(User.Id, (uint)(Amount), 0, "", 0, 0, 0, 0, 0);

                                    return;
                                }
                                else
                                {
                                    Embed.WithAuthor("Error!");
                                    Embed.WithFooter("Funds not given.");
                                    Embed.WithDescription("You do not have that much money!");
                                    Embed.Color = Color.Red;

                                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                                    return;
                                }
                            }
                            else
                            {
                                Embed.WithAuthor("Failure!");
                                Embed.WithFooter("Funds were given.");
                                Embed.WithDescription("That user has not set up an account! Users must have an account to recieve funds from other guild members!");
                                Embed.Color = Color.Red;

                                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                                return;
                            }
                        }
                    }
                    else
                    {
                        Embed.WithAuthor("Gift has been sent to " + User.Username);
                        Embed.WithFooter("Bot Admin fund transfer.");
                        Embed.WithDescription("A Bot Administrator sent " + User.Username + " " + Amount + " Gold Coins!");
                        Embed.Color = Color.Gold;
                        await Data.Data.SaveData(User.Id, Amount, 0, "", 0, 0, 0, 0, 0);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                }
                else
                {
                    Embed.WithAuthor("Error!");
                    Embed.WithFooter("Funds not given.");
                    Embed.WithDescription("This command requires you enter it as ``-Gold Gift @User [Amount]``");
                    Embed.Color = Color.Red;

                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
            }

            [Command("take"), Alias("remove", "Remove", "Take"), Summary("Take currency from a user. (Admin only)")]
            public async Task Take(IUser User = null, uint Amount = 0)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithColor(40, 200, 150);

                //A user is provided and amount is valid.
                SocketGuildUser User1 = Context.User as SocketGuildUser;

                if (Context.User.Id != 228344819422855168 && Context.User.Id != 409566463658033173)
                {
                    Embed.WithAuthor("Error!");
                    Embed.WithFooter("Funds not removed.");
                    Embed.WithDescription("You are not a Bot Admin!");
                    Embed.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{User.Mention}, **{Amount}** gold coins have been removed from your account by Bot Admin {Context.User.Mention}");
                    await Data.Data.SaveData(User.Id, (uint)-Amount, 0, "", 0, 0, 0, 0, 0);
                }
            }
        }

        public class SimpleDataContainer
        {
            public ulong ID;
            public uint GoldValue;
            public uint Level;
            public uint Damage;
            public uint Health;
            public uint Gems;
            public uint Age;
            public uint Event;
            public string Name;

            public SimpleDataContainer(ulong iD, uint goldValue, string name, uint level, uint damage, uint health, uint gems, uint age, uint eventD)
            {
                ID = iD;
                GoldValue = goldValue;
                Name = name;
                Level = level;
                Damage = damage;
                Health = health;
                Gems = gems;
                Age = age;
                Event = eventD;
            }

            ~SimpleDataContainer()
            {
                GC.Collect();
            }
        }

        [Command("Leaderboard"), Alias("leaderboard", "Lb", "lb"), Summary("See a list of the richest users.")]
        public async Task Leaderboard(string txt = "")
        {
            if (Context.Guild.Id != 542118110774427659)
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("This command may only be activated in the home guild. Join us here for a top leaderboard of users in the home server: https://discord.gg/jnFfqhm");
                Embeder.WithColor(40, 200, 150);
                Embeder.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embeder.Build());
                return;
            }

            if (txt == "" || txt == null)
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("You must use the command as ``-leaderboard [Sort Method]``!" +
                    "\n\nValid sorting methods:" +
                    "\n" + Coins + " - **Gold**" +
                    "\n" + Level + " - **Level**" +
                    "\n" + Damage + " - **Damage**" +
                    "\n" + Health + " - **Health**" +
                    "\n" + GuildGem + " - **Gems**" +
                    "\n" + Age + " - **Age**" +
                    "\n" + Essence + " - **Event**");
                Embeder.WithColor(40, 200, 150);
                Embeder.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embeder.Build());
                return;
            }

            List<SimpleDataContainer> list = new List<SimpleDataContainer> { };
            EmbedBuilder Embed = new EmbedBuilder();
            string output = "";

            if (txt == "Gold" || txt == "gold" || txt == "Coins" || txt == "coins" || txt == Coins)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, Data.Data.GetData_GoldAmount(users.Id), users.Username, 0, 0, 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.GoldValue.CompareTo(s2.GoldValue));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Gold: " + list[i].GoldValue + Coins;
                Embed.WithAuthor("Serverwide Leaderboard by Gold Coins");
                Embed.Color = Color.Gold;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Level" || txt == "level" || txt == "Xp" || txt == "XP" || txt == "xp" || txt == Level)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, Data.Data.GetData_Level(users.Id), 0, 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.Level.CompareTo(s2.Level));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Level: " + list[i].Level + Level;
                Embed.WithAuthor("Serverwide Leaderboard by Levels");
                Embed.Color = Color.DarkTeal;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Damage" || txt == "damage" || txt == "Strength" || txt == "strength" || txt == Damage)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, Data.Data.GetData_Damage(users.Id), 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Damage: " + list[i].Damage + Damage;
                Embed.WithAuthor("Serverwide Leaderboard by Damage");
                Embed.Color = Color.DarkRed;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Health" || txt == "health" || txt == "Life" || txt == "life" || txt == Health)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, Data.Data.GetData_Health(users.Id), 0, 0, 0));
                list.Sort((s2, s1) => s1.Health.CompareTo(s2.Health));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Health: " + list[i].Health + Health;
                Embed.WithAuthor("Serverwide Leaderboard by Health");
                Embed.Color = Color.Green;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Gems" || txt == "gems" || txt == "GuildGems" || txt == "guildgems" || txt == GuildGem)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, Data.Data.GetData_Event3(users.Id), 0, 0));
                list.Sort((s2, s1) => s1.Gems.CompareTo(s2.Gems));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Guild Gems: " + list[i].Gems + GuildGem;
                Embed.WithAuthor("Serverwide Leaderboard by Guild Gems");
                Embed.Color = Color.Purple;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Age" || txt == "age" || txt == Age)
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, 0, Data.Data.GetData_Age(users.Id), 0));
                list.Sort((s2, s1) => s1.Age.CompareTo(s2.Age));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Age: " + list[i].Age + Age;
                Embed.WithAuthor("Serverwide Leaderboard by Age");
                Embed.Color = Color.Orange;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Event" || txt == "event" || txt == Present) //Essence
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, 0, 0, Data.Data.GetData_Event1(users.Id)));
                list.Sort((s2, s1) => s1.Event.CompareTo(s2.Event));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Present: " + list[i].Event + Essence;
                Embed.WithAuthor("Serverwide Leaderboard by Presents");
                Embed.Color = Color.DarkGreen;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("You must use the command as ``-leaderboard [Sort Method]``!" +
                    "\n\nValid sorting methods:" +
                    "\n" + Coins + " - **Gold**" +
                    "\n" + Level + " - **Level**" +
                    "\n" + Damage + " - **Damage**" +
                    "\n" + Health + " - **Health**" +
                    "\n" + GuildGem + " - **Gems**" +
                    "\n" + Age + " - **Age**" +
                    "\n" + Present + " - **Event**");
                Embeder.WithColor(40, 200, 150);
                Embeder.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embeder.Build());
                return;
            }

            Embed.WithDescription(output);
            Embed.WithFooter("List includes top 5 in the server.");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("Gems"), Alias("gems", "gem", "Gem", "GuildGem", "guildgem", "GuildGems", "guildgems"), Summary("List your guild gems.")]
        public async Task GemCount()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544059986582437891/latest.png");
            Embed.WithTitle("You have " + Data.Data.GetData_Event3(Context.User.Id) + GuildGem + "'s");
            Embed.Color = Color.Purple;
            Embed.WithFooter("Use your gems in the shop to rank up!");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("Pe"), Alias("pe", "PE"), Summary("Make a petition.")]
        public async Task Petition([Remainder]string Words)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("Petition started, react to add to it:");
            Embed.WithDescription(Words + "");
            Embed.Color = Color.Gold;
            Embed.WithFooter("React with an up or down emoji!");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("TestEmbed"), Alias("testem"), Summary("Test embedded message.")]
        public async Task TestEmbed()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("This is a test.", Context.User.GetAvatarUrl());
            Embed.WithDescription("Reee");
            Embed.WithTimestamp(Context.Message.Timestamp);
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithUrl(Context.User.GetAvatarUrl());
            Embed.WithCurrentTimestamp();
            Embed.WithFooter("Test", Context.User.GetAvatarUrl());
            Embed.WithDescription("This embed is a test...");
            Embed.WithTitle("Test embed");
            Embed.Color = Color.Gold;
            Embed.WithFooter("Hi", Context.User.GetAvatarUrl());
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("info"), Alias("Info"), Summary("Info about something...")]
        public async Task Info(string check)
        {
            EmbedBuilder Embed = new EmbedBuilder();

            string classEmoji = "";
            string baseStats = "";

            if (check == "Archer" || check == "archer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                classEmoji = Archer;
                baseStats = "50 Damage, 20 Health";
            }
            else if (check == "Knight" || check == "knight")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                classEmoji = Knight;
                baseStats = "20 Damage, 50 Health";
            }
            else if (check == "Witch" || check == "witch")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                classEmoji = Witch;
                baseStats = "45 Damage, 25 Health";
            }
            else if (check == "Rogue" || check == "rogue")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                classEmoji = Rogue;
                baseStats = "60 Damage, 10 Health";
            }
            else if (check == "Wizard" || check == "wizard")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                classEmoji = Wizard;
                baseStats = "48 Damage, 22 Health";
            }
            else if (check == "Assassin" || check == "assassin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                classEmoji = Assassin;
                baseStats = "65 Damage, 5 Health";
            }
            else if (check == "Berserker" || check == "berserker" || check == "Berzerker" || check == "berzerker")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904669427859456/Berserker.png");
                classEmoji = Berserker;
                baseStats = "40 Damage, 30 Health";
            }
            else if (check == "Necromancer" || check == "Necromancer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904652009046026/Nechromancer.webp");
                classEmoji = Nechromancer;
                baseStats = "65 Damage, 5 Health";
            }
            else if (check == "Trickster" || check == "trickster")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904660275757066/Trickster.png");
                classEmoji = Trickster;
                baseStats = "55 Damage, 15 Health";
            }
            else if (check == "Kitsune" || check == "kitsune")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904663740252161/Kitsune.webp");
                classEmoji = Kitsune;
                baseStats = "65 Damage, 5 Health";
            }
            else if (check == "Paladin" || check == "paladin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904668891119627/Paladin.png");
                classEmoji = Paladin;
                baseStats = "55 Damage, 20 Health";
            }
            else if (check == "Monk" || check == "monk")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904672250626048/Monk.png");
                classEmoji = Monk;
                baseStats = "35 Damage, 35 Health";
            }
            else if (check == "Evangel" || check == "evangel")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904680668725258/Evangel.png");
                classEmoji = Evangel;
                baseStats = "65 Damage, 5 Health";
            }
            else if (check == "Tamer" || check == "tamer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904678713917445/Cat_Tamer.png");
                classEmoji = Tamer;
                baseStats = "65 Damage, 5 Health";
            }
            else if (check == "Swordsman" || check == "swordsman")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904710016139265/Swordsman.png");
                classEmoji = Swordsman;
                baseStats = "40 Damage, 30 Health";
            }
            else if (check == "Aphelion" || check == "aphelion")
            {
                Embed.WithTitle("Aphelion - God of Light, Celebration, and Youth");
                Embed.WithDescription("Aphelion is the God of Light, Celebration and Youth. He blessed Asteria with Sunlight, illuminating the newborn world.\nFollowers of Aphelion are incessant optimists who believe that staying true to one’s self is the key to happiness.They worship Aphelion through life’s bountiful pleasures and joys.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Istara" || check == "istara")
            {
                Embed.WithTitle("Istara - Goddess of Renewal, Nature, and Love");
                Embed.WithDescription("Istara is the Goddess of Renewal, Nature, and Love. She blessed Asteria with all flora and fauna, populating the infant world.\nFollowers of Istara are forgiving altruists who pursue compassion above all else. They revere all life as aspects of Istara, save the monstrous creatures that threaten Asteria.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Val" || check == "val")
            {
                Embed.WithTitle("Val - God of Justice, Science, and Civilization");
                Embed.WithDescription("Val is the god of Balance, Justice, and the Sciences. He blessed Asteria with civilization, providing structure to the sapient creatures of the young world.\nFollowers of Val are level - headed and truth - driven, bearing strong moral compasses and wills to protect others.They revere naught but Justice, and seek it at every turn no matter the cost.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Havi" || check == "havi")
            {
                Embed.WithTitle("Havi - Goddess of Commerce, Artisans, and Art");
                Embed.WithDescription("Havi is the Goddess of Commerce, Artisans, and Wealth. She blessed Asteria with art and luxury, giving meaning to the lives within the maturing world.\nFollowers of Havi are often artists, artisans, or merchants, and they seek not fortune but the betterment of the self.They worship Havi through their crafts, by which they find constant self - improvement.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Ebben" || check == "ebben")
            {
                Embed.WithTitle("Ebben - God of Death, Sleep, and Journeys");
                Embed.WithDescription("Ebben is the God of Death, Sleep, and Journeys. He blessed Asteria with the home and hearth, allowing a place for people to rest in the aging world.\nFollowers of Ebben are gracious and empathetic, and they either travel frequently or make their homes into safe harbors for others.The value of life’s journeys is immense to them, and they worship Ebben through them.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Lune" || check == "lune")
            {
                Embed.WithTitle("Lune - Goddess of Music, Storytelling, and Dreams");
                Embed.WithDescription("Lune is the Goddess of Music, the Moon, and Dreams. She blessed Asteria with storytelling, letting them chronicle the great deeds and legacies of the elderly world.\nFollowers of Lune tend to be ambitious rising stars, heroes, or performers.They worship Lune by carving their own mark in history and leaving magnificent stories to tell at every turn.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Sombri" || check == "sombri")
            {
                Embed.WithTitle("Sombri - Deity of Courage, Meditation, and the Unknown");
                Embed.WithDescription("Sombri is the Deity of Courage, Meditation, and the Unknown. They gave new life to the dying Asteria through a great sacrifice, though the details of the story vary.\nFollowers of Sombri are brave but often troubled, and tend to be adventurers.They worship Sombri through introspection, restraint, and knowing when to throw restraint to the wind.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Crone" || check == "crone" || check == "TheCrone" || check == "thecrone" || check == "The Crone" || check == "the crone" || check == "Ceras" || check == "ceras")
            {
                Embed.WithTitle("The Crone - Deity of Tradition, Knowledge, and Wisdom");
                Embed.WithDescription("The Crone is the Deity of Tradition, Knowledge, and Wisdom. They blessed Asteria with permanence, allowing the blessings of the other Gods to endure evermore.\nFollowers of The Crone vary in practice by regions of Asteria, but they are all focused on uplifting others, though they themselves are forces to be reckoned with.In Silverkeep, the Crone is known as Ceras, and is female.");
                Embed.WithImageUrl("");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else if (check == "Deity" || check == "deity" || check == "Deities" || check == "deities")
            {
                Embed.WithTitle("The deities you may worship are:");
                Embed.WithDescription("**Aphelion** - God of Light, Celebration, and Youth.\n**Istara** - Goddess of Renewal, Nature, and Love.\n**Val** - God of Justice, Science, and Civilization.\n**Havi** - Goddess of Commerce, Artisans, and Art.\n**Ebben** - God of Death, Sleep, and Journeys.\n**Lune** - Goddess of Music, Storytelling, and Dreams.\n**Sombri** - Deity of Courage, Meditation, and the Unknown.\n**The Crone** - Deity of Tradition, Knowledge, and Wisdom.");
                Embed.WithImageUrl("");
                Embed.WithFooter("Use -SwitchDeity [Deity] to switch to one of the following deities.");
                Embed.WithColor(Color.Purple);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());

                return;
            }
            else
            {
                Embed = new EmbedBuilder();
                Embed.WithAuthor("Whoops!");
                Embed.WithDescription("Sorry but that does not have any info currently about it! If you would like info about this or would " +
                    "like to have it included in this command please message Robin#1000 over Discord!");
                return;
            }

            Embed.WithTitle(classEmoji + " " + check + " Info");
            Embed.WithDescription("Class symbol: " + classEmoji + "\nBase stats: " + baseStats);
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        /* Fun times
        [Command("GibRobMuni"), Alias("gibRobMuni"), Summary("uwu")]
        public async Task Gimme()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            foreach (SocketRole roles in Context.Guild.Roles)
            {
                if(roles.Name == "Watermelonium")
                {
                    await user.AddRoleAsync(roles);
                }
            }
        }*/
    }
}

//⟨╲⸜(⧹⸜▾⸝⧸)⸝╱⟩