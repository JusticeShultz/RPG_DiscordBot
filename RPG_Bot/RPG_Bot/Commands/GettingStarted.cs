using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;
using Discord.WebSocket;
using Discord.Net;
using System.Linq;


namespace RPG_Bot.Commands
{
    public class GettingStarted : ModuleBase<SocketCommandContext>
    {
        [Command("begin"), Alias("Begin", "BEGIN"), Summary("Get started using the bot! Use the command as -begin [Class] [Name] [Age]")]
        public async Task Embed([Remainder]string Input = "None")
        {
            string[] input = Input.Split();

            var vuser = Context.User as SocketGuildUser;
            var bronze = Context.Guild.GetRole(542123825370890250);
            if (vuser.Roles.Contains(bronze))
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Error!");
                Embed.WithColor(40, 200, 150);
                Embed.WithFooter("");
                Embed.WithDescription("You already applied to the guild and were accepted! You do not need to apply again...");
                Embed.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            if (input.Length != 3)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("You were not accepted to the guild " + Context.User.Username + "!", Context.User.GetAvatarUrl());
                Embed.WithColor(40, 200, 150);
                Embed.WithFooter("Signup failed...");
                Embed.WithDescription("Sorry but you were not accepted to the guild...\n" +
                                  "To sign up correctly do ``-begin [Class] [Name] [Age]``");
                Embed.Color = Color.Red;

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                uint x = 0;

                if (uint.TryParse(input[2], out x) && (input[0] == "Archer" || input[0] == "Knight" || input[0] == "Rogue" ||
                    input[0] == "Wizard" || input[0] == "Witch" || input[0] == "Assassin" ||
                    input[0] == "archer" || input[0] == "knight" || input[0] == "rogue" ||
                    input[0] == "wizard" || input[0] == "witch" || input[0] == "assassin"))
                {
                    x = uint.Parse(input[2]);
                    if (x < 1) x = 1;

                    EmbedBuilder Embed = new EmbedBuilder();
                    var user = Context.User;
                    //Give bronze.
                    var role = Context.Guild.GetRole(542123825370890250);
                    await (user as IGuildUser).AddRoleAsync(role);

                    if (input[0] == "Archer" || input[0] == "archer")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                        var role2 = Context.Guild.GetRole(542217745690001409);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 50, x, input[1], 35, 20, 1);
                    }
                    else if (input[0] == "Knight" || input[0] == "knight")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                        var role2 = Context.Guild.GetRole(542217921246658610);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 25, x, input[1], 20, 40, 1);
                    }
                    else if(input[0] == "Witch" || input[0] == "witch")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                        var role2 = Context.Guild.GetRole(542217744805003264);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 0, x, input[1], 45, 10, 1);
                    }
                    else if(input[0] == "Rogue" || input[0] == "rogue")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                        var role2 = Context.Guild.GetRole(542217746440519681);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 100, x, input[1], 50, 10, 1);
                    }
                    else if (input[0] == "Wizard" || input[0] == "wizard")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                        var role2 = Context.Guild.GetRole(542217748009451551);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 25, x, input[1], 40, 20, 1);
                    }
                    else if (input[0] == "Assassin" || input[0] == "assassin")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                        var role2 = Context.Guild.GetRole(542217793601536010);
                        await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 50, x, input[1], 65, 5, 1);
                    }

                    Embed.WithAuthor("The Guild has accepted your signup form!", Context.User.GetAvatarUrl());
                    Embed.WithColor(40, 200, 150);
                    Embed.WithFooter("Congrats adventurer!");
                    Embed.WithDescription("Class: " + input[0] + "\nName: " + input[1] + "\nAge: " + x + "\n\nNow that you are registered type -help");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //New user in the database here:
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("You were not accepted to the guild " + Context.User.Username + "!", Context.User.GetAvatarUrl());
                    Embed.WithColor(40, 200, 150);
                    Embed.WithFooter("Signup failed...");
                    Embed.WithDescription("Sorry but you were not accepted to the guild due to your class or age not being valid...\n" + "To sign up correctly do ``-begin [Class] [Name] [Age]``\nWant to know the available classes? Type ``-classes``");
                    Embed.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        [Command("Classes"), Alias("classes", "class", "Class"), Summary("Get started using the bot! Use the command as -begin [Class] [Name] [Age]")]
        public async Task GameClassList([Remainder]string Input = "None")
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("There is currently 6 classes available and they are: ");
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("");
            Embed.WithDescription("Archer - High Damage, Very Vulnerable." + 
                                 "\nAssassin - Extreme Damage, Very Fragile." +
                                 "\nKnight - Low Damage, Very Tanky" +
                                 "\nRogue - High Damage, Very Vulnerable" +
                                 "\nWitch - High Damage, Very Vulnerable" +
                                 "\nWizard - Medium Damage, Somewhat Durable");
            Embed.Color = Color.Gold;

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}