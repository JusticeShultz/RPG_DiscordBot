using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;


namespace RPG_Bot.Currency
{
    public class Gold : ModuleBase<SocketCommandContext>
    {
        [Command("profile"), Alias("Profile", "P", "p"), Summary("See a users profile.")]
        public async Task Profile(IUser User = null)
        {
            var vuser = User as SocketGuildUser;

            if(User == null) vuser = Context.User as SocketGuildUser;

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
            var user = Context.User;
            string classType = "";

            if (vuser.Roles.Contains(archer))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                var role2 = Context.Guild.GetRole(542217745690001409);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "archer";
            }
            else if (vuser.Roles.Contains(knight))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                var role2 = Context.Guild.GetRole(542217921246658610);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "knight";
            }
            else if (vuser.Roles.Contains(witch))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                var role2 = Context.Guild.GetRole(542217744805003264);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "witch";
            }
            else if (vuser.Roles.Contains(rogue))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                var role2 = Context.Guild.GetRole(542217746440519681);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "rogue";
            }
            else if (vuser.Roles.Contains(wizard))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                var role2 = Context.Guild.GetRole(542217748009451551);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "wizard";
            }
            else if (vuser.Roles.Contains(assassin))
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                var role2 = Context.Guild.GetRole(542217793601536010);
                await (user as IGuildUser).AddRoleAsync(role2);
                classType = "assassin";
            }

            string name = Data.Data.GetData_Name(user.Id);
            ulong age = Data.Data.GetData_Age(user.Id);
            ulong coins = Data.Data.GetData_GoldAmount(user.Id);
            ulong level = Data.Data.GetData_Level(user.Id);
            ulong health = Data.Data.GetData_Health(user.Id);
            ulong damage = Data.Data.GetData_Damage(user.Id);

            Embed.WithAuthor("Profile of: " + user.Username, user.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("");
            Embed.WithDescription("Class: " + classType + "\nName: " + name + "\nAge: " + age + "\n" + "\nGold Coins: " + coins + "\n" +
                "\nLevel: " + level + "\n" + "Health: " + health + "\n" + "Damage: " + damage + "\n");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Group("gold"), Alias("golds, Gold, Golds"), Summary("The base keyword for working with gold.")]
        public class GoldGroup : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("me", "my", "wealth"), Summary("Shows your current gold. You may also do ``-Profile`` for the more data about yourself.")]
            public async Task Me()
            {

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
                if(Amount >= 1)
                {
                    //A user is provided and amount is valid.
                    SocketGuildUser User1 = Context.User as SocketGuildUser;
                    if(!User1.GuildPermissions.Administrator)
                    {
                        if(User1 == User)
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
                            await Context.Channel.SendMessageAsync($"{User.Mention}, you have recieved **{Amount}** gold coins from {Context.User.Username}");
                        }
                    }
                    else
                    {
                        Embed.WithAuthor("Gift has been sent to " + User.Username);
                        Embed.WithFooter("Admin fund transfer.");
                        Embed.WithDescription("An Administrator sent " + User.Username + " " + Amount + " Gold Coins!");
                        Embed.Color = Color.Gold;

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

                await Data.Data.SaveData(User.Id, Amount, 0, "owo what's this", 0, 0, 0);
            }
        }
    }
}
