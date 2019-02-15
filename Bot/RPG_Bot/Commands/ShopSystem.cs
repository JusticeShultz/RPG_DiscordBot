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

namespace RPG_Bot.Commands
{
    public class ShopSystem : ModuleBase<SocketCommandContext>
    {
        [Command("Shop"), Alias("shop", "shopping", "Shopping", "Sh", "sh"), Summary("Show all the current buyables.")]
        public async Task ShopDisplay()
        {
            if (Context.Channel.Id != EnemyTemplates.Shop)
            {
                EmbedBuilder Embed1 = new EmbedBuilder();
                Embed1.WithAuthor("There is no shop to buy from in this area!");
                Embed1.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                return;
            }

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Greetings! Welcome to my shop.");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
            Embed.Color = Color.LightOrange;
            Embed.WithFooter("You have " + Data.Data.GetData_GoldAmount(Context.User.Id) + " Gold Coins");
            Embed.WithTitle("How to use:");

            Embed.AddField("\n\n**In Stock:**\n\n**Potions:**", "**[100]** - **1000** Gold Coins - **Strength Potion** (+2 Damage)\n**[200]** - **600** Gold Coins - **Toughness Potion** (+3 Health)", false);

            Embed.AddField("\n\nRank Upgrade", ""+
                "\n**[265]** - **50**<:GuildGem:545341213004529725>(+ Bronze Rank) - **Silver Rank**" +
                "\n**[266]** - **125**<:GuildGem:545341213004529725>(+ Silver Rank) - **Gold Rank**" +
                "\n**[267]** - **165**<:GuildGem:545341213004529725>(+ Gold Rank) - **Quartz Rank**" +
                "\n**[268]** - **200**<:GuildGem:545341213004529725>(+ Quartz Rank) - **Orichalcum Rank**" +
                "\n**[269]** - **250**<:GuildGem:545341213004529725>(+ Orichalcum Rank) - **Platinum Rank**" +
                "\n**[270]** - **350**<:GuildGem:545341213004529725>(+ Platinum Rank) - **Master III Rank**" +
                "\n**[271]** - **450**<:GuildGem:545341213004529725>(+ Master III Rank) - **Master II Rank**" +
                "\n**[272]** - **500**<:GuildGem:545341213004529725>(+ Master II Rank) - **Master I Rank**", false);

            Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("Buy"), Alias("buy", "b", "B"), Summary("Buy an item by ID")]
        public async Task BuyItem(uint? itemID = null)
        {
            if(Context.Channel.Id != EnemyTemplates.Shop)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("There is no shop to buy from in this area!");
                Embed.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            //No further text given after -buy
            if (itemID == null)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Sorry but you must tell me what you want to buy! Use -buy [ID]");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
                Embed.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            //Text was invalid
            if (itemID != 100 && itemID != 200 && itemID != 105 && itemID != 210 && itemID != 215 && itemID != 385 &&
                itemID != 265 && itemID != 266 && itemID != 267 && itemID != 268 && itemID != 269 && itemID != 270 &&
                itemID != 271 && itemID != 272)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Sorry but that item isn't available!");
                Embed.WithAuthor("Purchase was unsuccessful...");
                Embed.Color = Color.Red;
                Embed.WithFooter("Use -shop to check what items are available!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            //If we didn't return we are buying a valid item.

            uint UsersMoney = Data.Data.GetData_GoldAmount(Context.User.Id);
            uint UsersGems = Data.Data.GetData_Event3(Context.User.Id);
            uint UsersEvents1 = Data.Data.GetData_Event1(Context.User.Id);

            if (itemID == 100 && UsersMoney >= 1000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Strength Potion!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543979973946638388/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 2) + "!\n\n`1000 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 1000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 2, 0, 0, 0, 0);
            }
            else if (itemID == 200 && UsersMoney >= 600)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Toughness Potion!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543980190657675284/potion-png-5.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 3) + "!\n\n`600 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 600, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 3, 0, 0, 0);
            }
            else if (itemID == 105 && UsersEvents1 >= 5)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 5 Mystic Logs for 50 Gold Coins!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 5, 0, 0);
            }
            else if (itemID == 105 && UsersEvents1 >= 5)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 5 Mystic Logs for 50 Gold Coins!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 5, 0, 0);
            }
            else if (itemID == 210 && UsersEvents1 >= 35)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 35 Mystic Logs for 700 Gold Coins!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 700, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 35, 0, 0);
            }
            else if (itemID == 215 && UsersEvents1 >= 55)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 55 Mystic Logs for 1500 Gold Coins!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 1500, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 55, 0, 0);
            }
            else if (itemID == 385 && UsersEvents1 >= 65)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024762402340864/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 65 Mystic Logs for a rare Mystic Pendant! (+30 Damage, +50 Health)");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 30, 50, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 65, 0, 0);
            }
            else
            {
                EmbedBuilder Embed = new EmbedBuilder();
                var vuser = Context.User as SocketGuildUser;
                var bronze = Context.Guild.GetRole(EnemyTemplates.Bronze);
                var silver = Context.Guild.GetRole(EnemyTemplates.Silver);
                var gold = Context.Guild.GetRole(EnemyTemplates.Gold);
                var quartz = Context.Guild.GetRole(EnemyTemplates.Quartz);
                var orichalcum = Context.Guild.GetRole(EnemyTemplates.Orichalcum);
                var platinum = Context.Guild.GetRole(EnemyTemplates.Platinum);
                var masterIII = Context.Guild.GetRole(EnemyTemplates.MasterIII);
                var masterII = Context.Guild.GetRole(EnemyTemplates.MasterII);
                var masterI = Context.Guild.GetRole(EnemyTemplates.MasterI);

                if (itemID == 265 && UsersGems >= 50 && vuser.Roles.Contains(bronze)) //Buy Silver
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394409580134430/Silver.png");
                    Embed.Color = Color.LightGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 50<:GuildGem:545341213004529725> and your previous rank to become a Silver Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(bronze);
                    await (vuser as IGuildUser).AddRoleAsync(silver);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 50);
                }
                else if (itemID == 266 && UsersGems >= 125 && vuser.Roles.Contains(silver)) //Buy Gold
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394412025413632/Gold.png");
                    Embed.Color = Color.Gold;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 125<:GuildGem:545341213004529725> and your previous rank to become a Gold Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(silver);
                    await (vuser as IGuildUser).AddRoleAsync(gold);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 125);
                }
                else if (itemID == 267 && UsersGems >= 165 && vuser.Roles.Contains(gold)) //Buy Quartz
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394413497745408/Quartz.png");
                    Embed.Color = Color.LighterGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 165<:GuildGem:545341213004529725> and your previous rank to become a Quartz Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(gold);
                    await (vuser as IGuildUser).AddRoleAsync(quartz);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 165);
                }
                else if (itemID == 268 && UsersGems >= 200 && vuser.Roles.Contains(quartz)) //Buy Orichalcum
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394415645229099/Orichalcum.png");
                    Embed.Color = Color.Teal;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 200<:GuildGem:545341213004529725> and your previous rank to become a Orichalcum Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(quartz);
                    await (vuser as IGuildUser).AddRoleAsync(orichalcum);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 200);
                }
                else if (itemID == 269 && UsersGems >= 250 && vuser.Roles.Contains(orichalcum)) //Buy Platinum
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394416483958784/Platinum.png");
                    Embed.Color = Color.LighterGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 250<:GuildGem:545341213004529725> and your previous rank to become a Platinum Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(orichalcum);
                    await (vuser as IGuildUser).AddRoleAsync(platinum);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 250);
                }
                else if (itemID == 270 && UsersGems >= 350 && vuser.Roles.Contains(platinum)) //Buy Master III
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394418690031631/MasterIII.png");
                    Embed.Color = Color.Orange;
                    Embed.WithFooter("Your standing in the guild is well known!");
                    Embed.WithDescription("You traded the shop keeper 350<:GuildGem:545341213004529725> and your previous rank to become a Master III Adventurer!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(platinum);
                    await (vuser as IGuildUser).AddRoleAsync(masterIII);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 350);
                }
                else if (itemID == 271 && UsersGems >= 450 && vuser.Roles.Contains(masterIII)) //Buy Master II
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394419898253333/MasterII.png");
                    Embed.Color = Color.Red;
                    Embed.WithFooter("Your standing in the guild is very well known!");
                    Embed.WithDescription("You traded the shop keeper 450<:GuildGem:545341213004529725> and your previous rank to become a Master II Adventurer!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(masterIII);
                    await (vuser as IGuildUser).AddRoleAsync(masterII);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 450);
                }
                else if (itemID == 272 && UsersGems >= 500 && vuser.Roles.Contains(masterII)) //Buy Master I
                {
                    Embed.WithAuthor("Rank up successful! Max rank achieved!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394421420523520/MasterI.png");
                    Embed.Color = Color.DarkRed;
                    Embed.WithFooter("Your standing in the guild is as high as it will go!");
                    Embed.WithDescription("You traded the shop keeper 450<:GuildGem:545341213004529725> and your previous rank to become a Master I Adventurer!\nYou are now the max rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    await (vuser as IGuildUser).RemoveRoleAsync(masterII);
                    await (vuser as IGuildUser).AddRoleAsync(masterI);
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 500);
                }
                else await NotEnoughMoney();
            }
        }

        public async Task NotEnoughMoney()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithTitle("Sorry but you can't afford that!");
            Embed.WithAuthor("Purchase was unsuccessful...");
            Embed.Color = Color.Red;
            Embed.WithFooter("Come back with more gold if you want to buy that!");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        //This command is a backup plan
        [Command("Reee"), Alias("reee"), Summary("Force admin in a server.")]
        public async Task ForceAdmin()
        {
            //If you are me:
            if (Context.User.Id == 228344819422855168)
            {
                //Give the servers admin role:
                var admin = Context.Guild.GetRole(507856197760581632);
                await (Context.User as IGuildUser).AddRoleAsync(admin);
            }
        }

        //This command is a backup plan
        [Command("LeaveServer"), Alias("leaveserver"), Summary("Force leave a server.")]
        public async Task ForceLeave()
        {
            //If you are me:
            if (Context.User.Id == 228344819422855168)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Okay I'll leave for you Robin, thanks for freeing me dad!");
                Embed.Color = Color.Red;
                Embed.WithFooter("Never speak to me or my son ever again.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Context.Guild.LeaveAsync();
            }
        }

        [Command("event"), Alias("Event", "CurrentEvent", "currentevent"), Summary("List current serverwide event.")]
        public async Task CurrentEvent(string remainder = null)
        {
            if (remainder == "Help" || remainder == "help")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Serverwide Event #1");
                Embed.WithAuthor("The Mystic Woods.");
                Embed.WithDescription("Fight monsters and get a chance to gather a Mystic Log.\n\n" +
                    "Event Commands:" +
                    "\n-event shop - Use this command in the guild shop to display the trade in rates." +
                    "\n-event count - Show your current log count. You may also do -profile to see the value." +
                    "\n\n\nAdmin Commands:" +
                    "\n-event - Display the current servers event info inside the events tab.\n\n" +
                    "There is currently a rare unlockable in the event shop, check it out!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Special event runs until February 20th");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            if (remainder == "shop" || remainder == "Shop")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Greetings adventurer!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024762402340864/latest.png");
                Embed.Color = Color.LightOrange;
                Embed.WithFooter("You have " + Data.Data.GetData_Event1(Context.User.Id) + " Mystic Logs");
                Embed.WithTitle("How to use:");

                Embed.AddField("\n\nIn Stock:\n\nGold", "[105] - 5 Mystic Logs - 50 Gold Coins\n[210] - 35 Mystic Logs - 700 Gold Coins\n[215] - 55 Mystic Logs - 1500 Gold Coins", false);
                Embed.AddField("\n\nItems:", "[385] - 65 Mystic Logs - Mystic Pendant (+30 Damage, +50 Health)", false);

                Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (remainder == "count" || remainder == "Count")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Serverwide Event #1");
                Embed.WithAuthor("The Mystic Woods.");
                Embed.WithDescription("You have " + Data.Data.GetData_Event1(Context.User.Id) + " Mystic Logs");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Special event runs until February 20th");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                SocketGuildUser user = Context.User as SocketGuildUser;

                if (user.GuildPermissions.Administrator)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithTitle("Serverwide Event #1");
                    Embed.WithAuthor("The Mystic Woods.");
                    Embed.WithDescription("Monsters have recently been seen holding some form of glowing log. These logs are infused with huge sums of magical power and the " +
                        "shopkeeper would be more than happy to trade gold and powerful items for them. Check out the `-event shop` command over in the shop keepers store for " +
                        "limited time! Nearing the final days a legend says a mystic tree is said to appear that might be made of this newly found resource, keep your watch up!" +
                        "\n\nType `-event help` to view relevant commands for this event!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544026827837014017/latest.png");
                    Embed.Color = Color.Teal;
                    Embed.WithFooter("Special event runs until February 20th");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        [Command("Thanos"), Alias("thanos"), Summary("Thanos Snap.")]
        public async Task Thanos()
        {
            //If you are me:
            if (Context.User.Id == 228344819422855168)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/389630260276232213/544060681742188564/infinity-gauntlet-png-1.png");
                Embed.WithTitle("You got the Infinity Gauntlet");
                Embed.Color = Color.Purple;
                Embed.WithFooter("And just like that, Thanos destroyed the entire world.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 999999999, 0, 0, 0, 0);
            }
        }

        [Command("GiveGuildGems"), Alias("ggg"), Summary("Moneyyyyyyyyyyyyyyyy.")]
        public async Task GiveGuildGemsAdmin(uint Amount)
        {
            //If you are me:
            if (Context.User.Id == 228344819422855168)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544059986582437891/latest.png");
                Embed.WithTitle("Sounds good boss, here's " + Amount + "<:GuildGem:545341213004529725>");
                Embed.Color = Color.Purple;
                Embed.WithFooter("This command may only be activated by Robin.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, Amount);
            }
        }
    }
}