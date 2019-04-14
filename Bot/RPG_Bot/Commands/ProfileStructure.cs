using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using RPG_Bot.Resources.Database;
using System.Collections.Generic;

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
                classEmoji = "<:Archer:543112389579767828>";
            }
            else if (UsersClass == "Knight")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                classType = "Knight";
                classEmoji = "<:Knight:543112385498578967>";
            }
            else if (UsersClass == "Witch")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                classType = "Witch";
                classEmoji = "<:Witch:543112316745416706>";
            }
            else if (UsersClass == "Rogue")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                classType = "Rogue";
                classEmoji = "<:Rogue:543112385406304257>";
            }
            else if (UsersClass == "Wizard")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                classType = "Wizard";
                classEmoji = "<:Wizard:543112388774199297>";
            }
            else if (UsersClass == "Assassin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                classType = "Assassin";
                classEmoji = "<:Assassin:543112389109874719>";
            }
            else if (UsersClass == "Berserker")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904669427859456/Berserker.png");
                classType = "Berserker";
                classEmoji = "<:Berserker:566917750140960768>";
            }
            else if (UsersClass == "Nechromancer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904652009046026/Nechromancer.webp");
                classType = "Nechromancer";
                classEmoji = "<:Nechromancer:566917752640503809>";
            }
            else if (UsersClass == "Trickster")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904660275757066/Trickster.png");
                classType = "Trickster";
                classEmoji = "<:Trickster:566917752875384834>";
            }
            else if (UsersClass == "Kitsune")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904663740252161/Kitsune.webp");
                classType = "Kitsune";
                classEmoji = "<:Kitsune:566917752380719139>";
            }
            else if (UsersClass == "Paladin")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904668891119627/Paladin.png");
                classType = "Paladin";
                classEmoji = "<:Paladin:566917753081036800>";
            }
            else if (UsersClass == "Monk")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904672250626048/Monk.png");
                classType = "Monk";
                classEmoji = "<:Monk:566917753009602570>";
            }
            else if (UsersClass == "Evangel")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904680668725258/Evangel.png");
                classType = "Evangel";
                classEmoji = "<:Evangel:566917753450266645>";
            }
            else if (UsersClass == "Tamer")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904678713917445/Cat_Tamer.png");
                classType = "Tamer";
                classEmoji = "<:Tamer:566918308218273792>";
            }
            else if (UsersClass == "Swordsman")
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904710016139265/Swordsman.png");
                classType = "Swordsman";
                classEmoji = "<:Swordsman:566917753085362186>";
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
            uint guildGems = Data.Data.GetData_Event3(vuser.Id);
            uint neededXP = (Data.Data.GetData_Level(vuser.Id) * Data.Data.GetData_Level(vuser.Id));

            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithAuthor("Profile of: " + user.Username, user.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("XP until level up: " + currentXp + " / " + neededXP);
            Embed.WithDescription("Class: " + classType + " " + classEmoji + "\nName: " + name + " <:freedomdove:543112388996497419>\nAge: " + age + " <:Age:543112389160337408>\n" + "\nGold Coins: " + coins + " <:Coins:543112388493312000>\n" +
                "You have " + guildGems + " <:GuildGem:545341213004529725>\n\n" +
                "Level: " + level + " <:Level:543112387989995521>\n" + "Health: " + health + " <:Health:543112384848461832>\n" + "Damage: " + damage + " <:Damage:543112387763503124>\n\n" +
                "You have " + eventItem + " Mystic Logs (Event Item)<:MysticLog:545341226556325893>");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Group("gold"), Alias("golds, Gold, Golds"), Summary("The base keyword for working with gold.")]
        public class GoldGroup : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("me", "my", "wealth"), Summary("Shows your current gold. You may also do ``-Profile`` for the more data about yourself.")]
            public async Task Me()
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("You have " + Data.Data.GetData_GoldAmount(Context.User.Id) + " Gold Coins");
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
                    Embed.WithDescription("You may not transfer funds to me or any other bot, sorry!");
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
            if(Context.Guild.Id != 542118110774427659)
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("This command may only be activated in the home guild. Join us here for a top leaderboard of users in the home server: https://discord.gg/jnFfqhm");
                Embeder.WithColor(40, 200, 150);
                Embeder.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embeder.Build());
                return;
            }

            if(txt == "" || txt == null)
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("You must use the command as ``-leaderboard [Sort Method]``!" +
                    "\n\nValid sorting methods:" +
                    "\n<:Coins:543112388493312000> - **Gold**" +
                    "\n<:Level:543112387989995521> - **Level**" +
                    "\n<:Damage:543112387763503124> - **Damage**" +
                    "\n<:Health:543112384848461832> - **Health**" +
                    "\n<:GuildGem:545341213004529725> - **Gems**" +
                    "\n<:Age:543112389160337408> - **Age**" +
                    "\n<:MysticLog:545341226556325893> - **Event**");
                Embeder.WithColor(40, 200, 150);
                Embeder.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embeder.Build());
                return;
            }

            List<SimpleDataContainer> list = new List<SimpleDataContainer> { };
            EmbedBuilder Embed = new EmbedBuilder();
            string output = "";

            if (txt == "Gold" || txt == "gold" || txt == "Coins" || txt == "coins" || txt == "<:Coins:543112388493312000>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, Data.Data.GetData_GoldAmount(users.Id), users.Username, 0, 0, 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.GoldValue.CompareTo(s2.GoldValue));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Gold: " + list[i].GoldValue + "<:Coins:543112388493312000>";
                Embed.WithAuthor("Serverwide Leaderboard by Gold Coins");
                Embed.Color = Color.Gold;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Level" || txt == "level" || txt == "Xp" || txt == "XP" || txt == "xp" || txt == "<:Level:543112387989995521>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, Data.Data.GetData_Level(users.Id), 0, 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.Level.CompareTo(s2.Level));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Level: " + list[i].Level + "<:Level:543112387989995521>";
                Embed.WithAuthor("Serverwide Leaderboard by Levels");
                Embed.Color = Color.DarkTeal;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Damage" || txt == "damage" || txt == "Strength" || txt == "strength" || txt == "<:Damage:543112387763503124>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, Data.Data.GetData_Damage(users.Id), 0, 0, 0, 0));
                list.Sort((s2, s1) => s1.Damage.CompareTo(s2.Damage));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Damage: " + list[i].Damage + "<:Damage:543112387763503124>";
                Embed.WithAuthor("Serverwide Leaderboard by Damage");
                Embed.Color = Color.DarkRed;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Health" || txt == "health" || txt == "Life" || txt == "life" || txt == "<:Health:543112384848461832>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, Data.Data.GetData_Health(users.Id), 0, 0, 0));
                list.Sort((s2, s1) => s1.Health.CompareTo(s2.Health));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Health: " + list[i].Health + "<:Health:543112384848461832>";
                Embed.WithAuthor("Serverwide Leaderboard by Health");
                Embed.Color = Color.Green;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Gems" || txt == "gems" || txt == "GuildGems" || txt == "guildgems" || txt == "<:GuildGem:545341213004529725>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, Data.Data.GetData_Event3(users.Id), 0, 0));
                list.Sort((s2, s1) => s1.Gems.CompareTo(s2.Gems));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Guild Gems: " + list[i].Gems + "<:GuildGem:545341213004529725>";
                Embed.WithAuthor("Serverwide Leaderboard by Guild Gems");
                Embed.Color = Color.Purple;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Age" || txt == "age" || txt == "<:Age:543112389160337408>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, 0, Data.Data.GetData_Age(users.Id), 0));
                list.Sort((s2, s1) => s1.Age.CompareTo(s2.Age));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Age: " + list[i].Age + "<:Age:543112389160337408>";
                Embed.WithAuthor("Serverwide Leaderboard by Age");
                Embed.Color = Color.Orange;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else if (txt == "Event" || txt == "event" || txt == "<:MysticLog:545341226556325893>")
            {
                foreach (SocketGuildUser users in Context.Guild.Users) list.Add(new SimpleDataContainer(users.Id, 0, users.Username, 0, 0, 0, 0, 0, Data.Data.GetData_Event1(users.Id)));
                list.Sort((s2, s1) => s1.Event.CompareTo(s2.Event));
                for (int i = 0; i < 5; ++i) output = output + "\n" + (i + 1) + ".) " + list[i].Name + " - Mystic Logs: " + list[i].Event + "<:MysticLog:545341226556325893>";
                Embed.WithAuthor("Serverwide Leaderboard by Mystic Logs");
                Embed.Color = Color.DarkGreen;
                Embed.WithThumbnailUrl(Context.Guild.GetUser(list[0].ID).GetAvatarUrl());
            }
            else
            {
                EmbedBuilder Embeder = new EmbedBuilder();
                Embeder.WithAuthor("Error!");
                Embeder.WithDescription("You must use the command as ``-leaderboard [Sort Method]``!" +
                    "\n\nValid sorting methods:" +
                    "\n<:Coins:543112388493312000> - **Gold**" +
                    "\n<:Level:543112387989995521> - **Level**" +
                    "\n<:Damage:543112387763503124> - **Damage**" +
                    "\n<:Health:543112384848461832> - **Health**" +
                    "\n<:GuildGem:545341213004529725> - **Gems**" +
                    "\n<:Age:543112389160337408> - **Age**" +
                    "\n<:MysticLog:545341226556325893> - **Event**");
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
            Embed.WithTitle("You have " + Data.Data.GetData_Event3(Context.User.Id) + "<:GuildGem:545341213004529725>'s");
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
    }
}
