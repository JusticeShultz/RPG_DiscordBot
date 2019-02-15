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

            var knight = Context.Guild.GetRole(542217921246658610);
            var witch = Context.Guild.GetRole(542217744805003264);
            var wizard = Context.Guild.GetRole(542217748009451551);
            var archer = Context.Guild.GetRole(542217745690001409);
            var assassin = Context.Guild.GetRole(542217793601536010);
            var rogue = Context.Guild.GetRole(542217746440519681);

            /*
                <@&542217921246658610 > -Knight
                <@&542217744805003264 > -Witch
                <@&542217748009451551 > -Wizard
                <@&542217745690001409 > -Archer
                <@&542217793601536010 > -Assassin
                <@&542217746440519681 > -Rogue
            */

            EmbedBuilder Embed = new EmbedBuilder();
            var user = vuser;
            string classType = "";
            string classEmoji = "";

            if (vuser.Roles.Contains(archer))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                classType = "Archer";
                classEmoji = "<:Archer:543112389579767828>";
            }
            else if (vuser.Roles.Contains(knight))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                classType = "Knight";
                classEmoji = "<:Knight:543112385498578967>";
            }
            else if (vuser.Roles.Contains(witch))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                classType = "Witch";
                classEmoji = "<:Witch:543112316745416706>";
            }
            else if (vuser.Roles.Contains(rogue))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                classType = "Rogue";
                classEmoji = "<:Rogue:543112385406304257>";
            }
            else if (vuser.Roles.Contains(wizard))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                classType = "Wizard";
                classEmoji = "<:Wizard:543112388774199297>";
            }
            else if (vuser.Roles.Contains(assassin))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                classType = "Assassin";
                classEmoji = "<:Assassin:543112389109874719>";
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
                    if (!User1.GuildPermissions.Administrator)
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
                        Embed.WithFooter("Admin fund transfer.");
                        Embed.WithDescription("An Administrator sent " + User.Username + " " + Amount + " Gold Coins!");
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

                if (!User1.GuildPermissions.Administrator)
                {
                    Embed.WithAuthor("Error!");
                    Embed.WithFooter("Funds not removed.");
                    Embed.WithDescription("You are not an admin!");
                    Embed.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{User.Mention}, **{Amount}** gold coins have been removed from your account by admin {Context.User.Mention}");
                    await Data.Data.SaveData(User.Id, (uint)-Amount, 0, "", 0, 0, 0, 0, 0);
                }
            }
        }

        public class SimpleDataContainer
        {
            public ulong ID;
            public uint GoldValue;
            public string Name;

            public SimpleDataContainer(ulong iD, uint goldValue, string name)
            {
                ID = iD;
                GoldValue = goldValue;
                Name = name;
            }

            ~SimpleDataContainer()
            {
                GC.Collect();
            }

        }

        [Command("Leaderboard"), Alias("leaderboard", "Lb", "lb"), Summary("See a list of the richest users.")]
        public async Task Leaderboard()
        {
            List<SimpleDataContainer> list = new List<SimpleDataContainer> { };

            foreach(SocketGuildUser users in Context.Guild.Users)
                list.Add(new SimpleDataContainer(users.Id, Data.Data.GetData_GoldAmount(users.Id), users.Username));

            list.Sort((s2, s1) => s1.GoldValue.CompareTo(s2.GoldValue));

            string output = "";
            for(int i = 0; i < 5; ++i)
            {
                output = output + "\n" + (i+1) + ".) " + list[i].Name + " - " + list[i].GoldValue + " Gold Coins";
            }

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Serverwide Leaderboard by Gold Coins");
            Embed.WithDescription(output);
            Embed.WithFooter("List includes top 5 in the server.");
            Embed.Color = Color.Orange;
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
