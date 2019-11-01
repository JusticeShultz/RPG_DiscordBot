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
            if (Context.Channel.Name != "guild-shop")
            {
                EmbedBuilder Embed1 = new EmbedBuilder();
                Embed1.WithAuthor("There is no shop to buy from in this area!");
                Embed1.Color = Color.Red;
                var mess = await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                await Context.Message.DeleteAsync();
                await Task.Delay(6000);
                await mess.DeleteAsync();
                return;
            }

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Greetings! Welcome to my shop.");
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
            Embed.Color = Color.LightOrange;
            Embed.WithFooter("You have " + Data.Data.GetData_GoldAmount(Context.User.Id) + " Gold Coins & " + Data.Data.GetData_Event3(Context.User.Id) + " Guild Gem's", Context.User.GetAvatarUrl());
            Embed.WithTitle("How to use:");

            Embed.AddField("\n\n**In Stock:**\n\n**Cheap Items:**", "**[100]** - **1000**<:Coins:543112388493312000> - **Strength Potion** (+2 Damage)\n**[200]** - **600**<:Coins:543112388493312000> - **Toughness Potion** (+3 Health)", false);

            Embed.AddField("\n\n**Powerful Items**", "" +
                "\n**[300]** - **8000**<:Coins:543112388493312000> - **Strength Ring** (+18 Damage)" +
                "\n**[400]** - **6000**<:Coins:543112388493312000> - **Toughness Pendant** (+22 Health)" +
                "\n**[500]** - **32000**<:Coins:543112388493312000> - **Dragon Potion** (+80 Damage)" +
                "\n**[600]** - **28000**<:Coins:543112388493312000> - **Viper Potion** (+108 Health)", false);

            Embed.AddField("\n\n**Rank Upgrade**", "" +
                "\n**[265]** - **50**<:GuildGem:545341213004529725>(+ Bronze Rank) - **Silver Rank**" +
                "\n**[266]** - **125**<:GuildGem:545341213004529725>(+ Silver Rank) - **Gold Rank**" +
                "\n**[267]** - **165**<:GuildGem:545341213004529725>(+ Gold Rank) - **Quartz Rank**" +
                "\n**[268]** - **200**<:GuildGem:545341213004529725>(+ Quartz Rank) - **Orichalcum Rank**" +
                "\n**[269]** - **250**<:GuildGem:545341213004529725>(+ Orichalcum Rank) - **Platinum Rank**" +
                "\n**[270]** - **350**<:GuildGem:545341213004529725>(+ Platinum Rank) - **Master III Rank**" +
                "\n**[271]** - **450**<:GuildGem:545341213004529725>(+ Master III Rank) - **Master II Rank**" +
                "\n**[272]** - **500**<:GuildGem:545341213004529725>(+ Master II Rank) - **Master I Rank**", false);

            Embed.AddField("\n\n**Loot Boxes**", "" +
                "\n**[450]** - **285**<:GuildGem:545341213004529725> - **Dragon Chest** - 10 Unique Items" +
                "\n**[650]** - **8500**<:Coins:543112388493312000> - **Pepe Chest** - 15 Unique Items", false);

            Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
            await Context.Message.DeleteAsync();
        }

        [Command("Buy"), Alias("buy", "b", "B"), Summary("Buy an item by ID")]
        public async Task BuyItem(uint? itemID = null)
        {
            if (Context.Channel.Name != "guild-shop")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("There is no shop to buy from in this area!");
                Embed.Color = Color.Red;
                var mseg = await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Context.Message.DeleteAsync();
                await Task.Delay(6000);
                await mseg.DeleteAsync();
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
                itemID != 271 && itemID != 272 && itemID != 300 && itemID != 400 && itemID != 500 && itemID != 600 &&
                itemID != 450 && itemID != 650 && itemID != 851 && itemID != 852 && itemID != 853 && itemID != 854 &&
                itemID != 855 && itemID != 856 && itemID != 857 && itemID != 850)
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
            uint UsersCandies = Data.Data.GetData_Event2(Context.User.Id);
            uint UsersEvents1 = Data.Data.GetData_Event1(Context.User.Id);

            if (itemID == 300 && UsersMoney >= 8000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Strength Ring!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023381217902603/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 18) + "!\n\n`8000 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 8000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 18, 0, 0, 0, 0);
            }
            else if (itemID == 400 && UsersMoney >= 6000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Toughness Pendant!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024782530805778/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 22) + "!\n\n`6000 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 6000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 22, 0, 0, 0);
            }
            else if (itemID == 500 && UsersMoney >= 32000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Dragon Potion!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023219967885322/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 80) + "!\n\n`32000 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 32000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 80, 0, 0, 0, 0);
            }
            else if (itemID == 600 && UsersMoney >= 28000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Viper Potion!");
                Embed.WithAuthor("Purchase was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023243472896008/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 108) + "!\n\n`28000 Gold Coins` have been taken from your account.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 28000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 108, 0, 0, 0);
            }
            else if (itemID == 100 && UsersMoney >= 1000)
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
                Embed.WithDescription("You traded the shop keeper 5 Essence for 50 Gold Coins!");
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
                Embed.WithDescription("You traded the shop keeper 5 Essence for 50 Gold Coins!");
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
                Embed.WithDescription("You traded the shop keeper 35 Essence for 700 Gold Coins!");
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
                Embed.WithDescription("You traded the shop keeper 55 Essence for 1500 Gold Coins!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 1500, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 55, 0, 0);
            }
            else if (itemID == 385 && UsersEvents1 >= 65)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024948029653042/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 65 Essence for a rare Essence Helm! (+50 Damage, +80 Health)");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 30, 50, 0, 0, 0);
                await Data.Data.TakeEventData(Context.User.Id, 65, 0, 0);
            }
            else if (itemID == 450 && UsersGems >= 285)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024389239439369/Magic20Chest.png");
                Embed.Color = Color.DarkPurple;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 285<:GuildGem:545341213004529725> to open a Dragons Chest");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 285);
                await OpenDragonChest();
            }
            else if (itemID == 650 && UsersMoney >= 8500)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.Color = Color.Green;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper **8500**<:Coins:543112388493312000> to open a Pepe Chest, what a loser");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 8500, 0, "", 0, 0, 0, 0, 0);
                await OpenPepeChest();
            }
            else if (itemID == 850 && UsersCandies >= 3)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Common Loot Chest Purchased!");
                Embed.Color = Color.LighterGrey;
                Embed.WithFooter("Use -lootbox common to open up your new lootbox or -lootbox to see your current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **3** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240876719210501/locked-chest.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 3, 0);
                await Data.Data.AddCommonBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 851 && UsersCandies >= 8)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Uncommon Loot Chest Purchased!");
                Embed.Color = Color.Green;
                Embed.WithFooter("Use -lootbox uncommon to open up your new lootbox or -lootbox to see your current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **8** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240878015381516/locked-chest_1.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 8, 0);
                await Data.Data.AddUncommonBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 852 && UsersCandies >= 15)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Rare Loot Chest Purchased!");
                Embed.Color = Color.Blue;
                Embed.WithFooter("Use -lootbox rare to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **15** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240879382462475/locked-chest_2.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 15, 0);
                await Data.Data.AddRareBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 853 && UsersCandies >= 22)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Very Rare Loot Chest Purchased!");
                Embed.Color = Color.Magenta;
                Embed.WithFooter("Use -lootbox veryrare to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **22** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240880749805568/locked-chest_3.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 22, 0);
                await Data.Data.AddVeryRareBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 854 && UsersCandies >= 30)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Epic Loot Chest Purchased!");
                Embed.Color = Color.Purple;
                Embed.WithFooter("Use -lootbox epic to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **30** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240881794449437/locked-chest_4.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 30, 0);
                await Data.Data.AddEpicBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 855 && UsersCandies >= 50)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Legendary Loot Chest Purchased!");
                Embed.Color = Color.Red;
                Embed.WithFooter("Use -lootbox legendary to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **50** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240883669172237/locked-chest_5.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 50, 0);
                await Data.Data.AddLegendaryBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 856 && UsersCandies >= 100)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Mythic Loot Chest Purchased!");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Use -lootbox mythic to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **100** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240886613704772/locked-chest_6.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 100, 0);
                await Data.Data.AddMythicBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 857 && UsersCandies >= 125)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Godly Loot Chest Purchased!");
                Embed.Color = Color.LighterGrey;
                Embed.WithFooter("Use -lootbox godly to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **125** <:Candy:637578758986924035>");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240875452661791/locked-chest_7.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 125, 0);
                await Data.Data.AddGodlyBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
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

                if (itemID == 265 && UsersGems >= 50 && Data.Data.GetRank(Context.User.Id) == "Bronze") //Buy Silver
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394409580134430/Silver.png");
                    Embed.Color = Color.LightGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 50<:GuildGem:545341213004529725> and your previous rank to become a Silver Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(bronze);
                    //await (vuser as IGuildUser).AddRoleAsync(silver);
                    await Data.Data.SetRank(vuser.Id, "Silver");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 50);
                }
                else if (itemID == 266 && UsersGems >= 125 && Data.Data.GetRank(Context.User.Id) == "Silver") //Buy Gold
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394412025413632/Gold.png");
                    Embed.Color = Color.Gold;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 125<:GuildGem:545341213004529725> and your previous rank to become a Gold Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(silver);
                    //await (vuser as IGuildUser).AddRoleAsync(gold);
                    await Data.Data.SetRank(vuser.Id, "Gold");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 125);
                }
                else if (itemID == 267 && UsersGems >= 165 && Data.Data.GetRank(Context.User.Id) == "Gold") //Buy Quartz
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394413497745408/Quartz.png");
                    Embed.Color = Color.LighterGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 165<:GuildGem:545341213004529725> and your previous rank to become a Quartz Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(gold);
                    //await (vuser as IGuildUser).AddRoleAsync(quartz);
                    await Data.Data.SetRank(vuser.Id, "Quartz");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 165);
                }
                else if (itemID == 268 && UsersGems >= 200 && Data.Data.GetRank(Context.User.Id) == "Quartz") //Buy Orichalcum
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394415645229099/Orichalcum.png");
                    Embed.Color = Color.Teal;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 200<:GuildGem:545341213004529725> and your previous rank to become a Orichalcum Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(quartz);
                    //await (vuser as IGuildUser).AddRoleAsync(orichalcum);
                    await Data.Data.SetRank(vuser.Id, "Orichalcum");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 200);
                }
                else if (itemID == 269 && UsersGems >= 250 && Data.Data.GetRank(Context.User.Id) == "Orichalcum") //Buy Platinum
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394416483958784/Platinum.png");
                    Embed.Color = Color.LighterGrey;
                    Embed.WithFooter("The guild applauds your dedication.");
                    Embed.WithDescription("You traded the shop keeper 250<:GuildGem:545341213004529725> and your previous rank to become a Platinum Rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(orichalcum);
                    //await (vuser as IGuildUser).AddRoleAsync(platinum);
                    await Data.Data.SetRank(vuser.Id, "Platinum");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 250);
                }
                else if (itemID == 270 && UsersGems >= 350 && Data.Data.GetRank(Context.User.Id) == "Platinum") //Buy Master III
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394418690031631/MasterIII.png");
                    Embed.Color = Color.Orange;
                    Embed.WithFooter("Your standing in the guild is well known!");
                    Embed.WithDescription("You traded the shop keeper 350<:GuildGem:545341213004529725> and your previous rank to become a Master III Adventurer!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(platinum);
                    //await (vuser as IGuildUser).AddRoleAsync(masterIII);
                    await Data.Data.SetRank(vuser.Id, "Master3");
                    await Gameplay.UpdateUserData();
                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 350);
                }
                else if (itemID == 271 && UsersGems >= 450 && Data.Data.GetRank(Context.User.Id) == "Master3") //Buy Master II
                {
                    Embed.WithAuthor("Rank up successful!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394419898253333/MasterII.png");
                    Embed.Color = Color.Red;
                    Embed.WithFooter("Your standing in the guild is very well known!");
                    Embed.WithDescription("You traded the shop keeper 450<:GuildGem:545341213004529725> and your previous rank to become a Master II Adventurer!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(masterIII);
                    //await (vuser as IGuildUser).AddRoleAsync(masterII);
                    await Data.Data.SetRank(vuser.Id, "Master2");
                    await Gameplay.UpdateUserData();

                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 450);
                }
                else if (itemID == 272 && UsersGems >= 500 && Data.Data.GetRank(Context.User.Id) == "Master2") //Buy Master I
                {
                    Embed.WithAuthor("Rank up successful! Max rank achieved!");
                    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545394421420523520/MasterI.png");
                    Embed.Color = Color.DarkRed;
                    Embed.WithFooter("Your standing in the guild is as high as it will go!");
                    Embed.WithDescription("You traded the shop keeper 450<:GuildGem:545341213004529725> and your previous rank to become a Master I Adventurer!\nYou are now the max rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(masterII);
                    //await (vuser as IGuildUser).AddRoleAsync(masterI);

                    await Data.Data.SetRank(vuser.Id, "Master1");
                    await Gameplay.UpdateUserData();

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
            if (Context.User.Id == 228344819422855168 || Context.User.Id == 409566463658033173)
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
            if (Context.User.Id == 228344819422855168 || Context.User.Id == 409566463658033173)
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
                Embed.WithTitle("Serverwide Event #3");
                Embed.WithAuthor("The Essence Moon.");
                Embed.WithDescription("Fight monsters and get a chance to gather an Essence.\n\n" +
                    "Event Commands:" +
                    "\n-event shop - Use this command in the guild shop to display the trade in rates." +
                    "\n-event count - Show your current log count. You may also do -profile to see the value." +
                    "\n\n\nAdmin Commands:" +
                    "\n-event - Display the current servers event info inside the events tab.\n\n" +
                    "There is currently a rare unlockable in the event shop, check it out!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566850283670601749/Unit_ills_full_50801.webp");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Special event runs until May 20th");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            if (remainder == "shop" || remainder == "Shop")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Greetings adventurer!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024948029653042/latest.png");
                Embed.Color = Color.LightOrange;
                Embed.WithFooter("You have " + Data.Data.GetData_Event1(Context.User.Id) + " Essence");
                Embed.WithTitle("How to use:");

                Embed.AddField("\n\nIn Stock:\n\nGold", "[105] - 5 Essence - 50 Gold Coins\n[210] - 35 Essence - 700 Gold Coins\n[215] - 55 Essence - 1500 Gold Coins", false);
                Embed.AddField("\n\nItems:", "[385] - 65 Essence - Essence Helm (+50 Damage, +80 Health)", false);

                Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            if (remainder == "candyshop" || remainder == "CandyShop")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Happy Halloween!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545322196872986641/Wendy.webp");
                Embed.Color = Color.Orange;
                Embed.WithFooter("You have " + Data.Data.GetData_Event2(Context.User.Id) + " Candies");
                Embed.WithTitle("How to use:");

                Embed.AddField("\n\nIn Stock:\n\nLoot Boxes", "[850] - 3 Candies - Common Loot Chest\n[851] - 8 Candies - Uncommon Loot Chest\n[852] - 15 Candies - Rare Loot Chest\n[853] - 22 Candies - Very Rare Loot Chest\n[854] - 30 Candies - Epic Loot Chest\n[855] - 50 Candies - Legendary Loot Chest\n[856] - 100 Candies - Mythic Loot Chest\n[857] - 125 Candies - Godly Loot Chest", false);

                Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (remainder == "count" || remainder == "Count")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Serverwide Event #3");
                Embed.WithAuthor("The Essence Moon.");
                Embed.WithDescription("You have " + Data.Data.GetData_Event1(Context.User.Id) + " Essence");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566850283670601749/Unit_ills_full_50801.webp");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Special event runs until May 10th");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                SocketGuildUser user = Context.User as SocketGuildUser;

                if (user.GuildPermissions.Administrator)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    //Embed.WithTitle("Serverwide Event #3");
                    //Embed.WithAuthor("The Essence Moon.");
                    ///*Embed.WithDescription("Monsters have recently been seen holding some form of glowing log. These logs are infused with huge sums of magical power and the " +
                    //    "shopkeeper would be more than happy to trade gold and powerful items for them. Check out the `-event shop` command over in the shop keepers store for " +
                    //    "limited time! Nearing the final days a legend says a mystic tree is said to appear that might be made of this newly found resource, keep your watch up!" +
                    //    "\n\nType `-event help` to view relevant commands for this event!");
                    //Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545308296463122442/Will_O_Wisp.webp");*/
                    //Embed.WithDescription("The essence moon is rising and the demon king awakens with it. Collect essence " +
                    //    "and gain ultimate power. (All previous event items will be transfered into this event, I will reset them next time if this doesn't work well)");
                    //Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545308301089439754/Crom_Cruach.png");
                    //Embed.Color = Color.Purple;
                    //Embed.WithFooter("Special event runs until May 20th");
                    //await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //EmbedBuilder Embed1 = new EmbedBuilder();
                    //Embed1.WithTitle("Serverwide Event #4");
                    //Embed1.WithAuthor("The Gold Filled Pots.");
                    //Embed1.WithDescription("A new monster is roaming throughout the plains and it's full of gold! For limited time fight these gold filled enemies" +
                    //    " for huge amounts of gold! Low ranking areas will have Bronze Pots, medium areas will contain Silver Pots and high ranking areas will spawn " +
                    //    "Golden Pots! Good luck adventurer, may luck be in your favor.\n\nNote: the event monsters will not spawn in master rank zones, this means " +
                    //    "zones 60+ will not spawn them and their spawn tables will not have been affected by this event.");
                    //Embed1.WithImageUrl("https://vignette.wikia.nocookie.net/quiz-rpg-the-world-of-mystic-wiz/images/f/f5/The_Golden_Pot_transparent.png/revision/latest?cb=20141025232107");
                    //Embed1.Color = Color.Gold;
                    //Embed1.WithFooter("Special event runs until May 20th");
                    //await Context.Channel.SendMessageAsync("", false, Embed1.Build());

                    EmbedBuilder Embed1 = new EmbedBuilder();
                    Embed1.WithTitle("Holiday Event #1");
                    Embed1.WithAuthor("Spooktober");
                    Embed1.WithDescription("The mist is growing heavier!\n" +
                        "Spooktober kicks off starting now, visit the ``spooktober`` zone and fight event monsters to earn candies. Candies can be used to unlock special " +
                        "items during the Spooktober event, more details on that can be found soon! Additionally item drop rate will be doubled within this zone and there is" +
                        " a variety of different leveled monsters! Happy hunting and keep an eye out for the lord of halloween, he may just crush you if you aren't strong enough!");
                    Embed1.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/637578405281529858/EventIcon.webp");
                    Embed1.Color = Color.Gold;
                    Embed1.WithFooter("Special event runs until November 15th");
                    await Context.Channel.SendMessageAsync("", false, Embed1.Build());
                }
            }

            await Context.Message.DeleteAsync();
        }

        [Command("Thanos"), Alias("thanos"), Summary("Thanos Snap.")]
        public async Task Thanos()
        {
            //If you are me:
            if (Context.User.Id == 228344819422855168 || Context.User.Id == 409566463658033173)
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
        public async Task GiveGuildGemsAdmin(SocketGuildUser target, uint Amount)
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            //if (!user.GuildPermissions.Administrator) return;
            if (Context.User.Id != 228344819422855168 && Context.User.Id != 409566463658033173) return;

            if (target != null) user = target as SocketGuildUser;

            //If you are me: (Changed)
            //if (Context.User.Id == 228344819422855168)
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544059986582437891/latest.png");
            Embed.WithTitle(user.Username + " has recieved " + Amount + "<:GuildGem:545341213004529725> from " + Context.User.Username + "(Admin)");
            Embed.Color = Color.Purple;
            Embed.WithThumbnailUrl(user.GetAvatarUrl());
            Embed.WithFooter("This command may only be activated by a Bot Admin.");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
            await Data.Data.SaveEventData(user.Id, 0, 0, Amount);
        }

        [Command("SwitchClass"), Alias("switchclass", "sc", "SC", "Switch", "switch", "ChangeClass", "changeclass"), Summary("Switch your class for 500 gold.")]
        public async Task SwitchClass(string txt = "None")
        {
            if (txt == "" || txt == null || txt == "None")
            {
                EmbedBuilder EmbedHelp = new EmbedBuilder();
                EmbedHelp.WithTitle("Switching Classes");
                EmbedHelp.WithDescription("To switch your class you must do ``-SwitchClass [Class]`` or ``-Switch [Class]``\n" +
                    "**Switching your class will cost 500 gold and is purely cosmetic. Your stats will not change.**");
                EmbedHelp.Color = Color.Purple;
                await Context.Channel.SendMessageAsync("", false, EmbedHelp.Build());
                return;
            }

            if (Data.Data.GetData_GoldAmount(Context.User.Id) < 500)
            {
                EmbedBuilder EmbedMoneyFailure = new EmbedBuilder();
                EmbedMoneyFailure.WithTitle("Failed to switch class");
                EmbedMoneyFailure.WithDescription("You do not have the necessary funds!");
                EmbedMoneyFailure.WithFooter("The guild denies your change request.");
                EmbedMoneyFailure.Color = Color.Purple;
                await Context.Channel.SendMessageAsync("", false, EmbedMoneyFailure.Build());
                return;
            }

            /*var archer = Context.Guild.GetRole(542217745690001409);
            var knight = Context.Guild.GetRole(542217921246658610);
            var witch = Context.Guild.GetRole(542217744805003264);
            var rogue = Context.Guild.GetRole(542217746440519681);
            var wizard = Context.Guild.GetRole(542217748009451551);
            var assassin = Context.Guild.GetRole(542217793601536010);*/

            var vuser = Context.User as SocketGuildUser;

            if (txt == "Knight" || txt == "knight")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Knight")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Knight!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(knight);
                    await Data.Data.SetClass(Context.User.Id, "Knight");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Rogue" || txt == "rogue")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Rogue")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Rogue!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(rogue);
                    await Data.Data.SetClass(Context.User.Id, "Rogue");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Archer" || txt == "archer")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Archer")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Archer!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(archer);
                    await Data.Data.SetClass(Context.User.Id, "Archer");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Assassin" || txt == "assassin")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Assassin")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Assassin!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(assassin);
                    await Data.Data.SetClass(Context.User.Id, "Assassin");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Wizard" || txt == "wizard")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Wizard")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Wizard!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(wizard);
                    await Data.Data.SetClass(Context.User.Id, "Wizard");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Witch" || txt == "witch")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Witch")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Witch!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());

                    //await RemoveClass();
                    //await vuser.AddRoleAsync(witch);
                    await Data.Data.SetClass(Context.User.Id, "Witch");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Berserker" || txt == "berserker" || txt == "berzerker" || txt == "Berzerker")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Berserker")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Berserker!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Berserker");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Tamer" || txt == "tamer")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Tamer")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Tamer!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Tamer");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Monk" || txt == "monk")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Monk")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Monk!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Monk");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Necromancer" || txt == "Necromancer")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Necromancer")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Necromancer!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Necromancer");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Paladin" || txt == "paladin")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Paladin")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Paladin!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Paladin");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Swordsman" || txt == "swordsman")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Swordsman")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Swordsman!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Swordsman");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Evangel" || txt == "evangel")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Evangel")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Evangel!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Evangel");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Kitsune" || txt == "kitsune")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Kitsune")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Kitsune!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Kitsune");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }
            if (txt == "Trickster" || txt == "trickster")
            {
                if (Data.Data.GetClass(Context.User.Id) != "Trickster")
                {
                    await Data.Data.SubtractSaveData(Context.User.Id, 500, 0, "", 0, 0, 0, 0, 0);

                    EmbedBuilder E = new EmbedBuilder();
                    E.WithTitle("You are now a Trickster!");
                    E.WithFooter("500 Gold Coins have been taken from your account.");
                    E.Color = Color.Teal;
                    await Context.Channel.SendMessageAsync("", false, E.Build());
                    await Data.Data.SetClass(Context.User.Id, "Trickster");
                    await Gameplay.UpdateUserData();
                    return;
                }
            }

            EmbedBuilder EmbedFail = new EmbedBuilder();
            EmbedFail.WithTitle("Failed to switch class");
            EmbedFail.WithDescription("You may not switch to a class that you already are!");
            EmbedFail.WithFooter("The guild denies your change request and you are refunded your 500 Gold Coins.");
            EmbedFail.Color = Color.Purple;
            await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());
            return;
        }

        #region Deprecated Remove Class Method
        /*[Deprecated] public async Task RemoveClass()
        {
            var archer = Context.Guild.GetRole(542217745690001409);
            var knight = Context.Guild.GetRole(542217921246658610);
            var witch = Context.Guild.GetRole(542217744805003264);
            var rogue = Context.Guild.GetRole(542217746440519681);
            var wizard = Context.Guild.GetRole(542217748009451551);
            var assassin = Context.Guild.GetRole(542217793601536010);
            var vuser = Context.User as SocketGuildUser;

            if (vuser.Roles.Contains(rogue))
                await vuser.RemoveRoleAsync(rogue);
            if (vuser.Roles.Contains(wizard))
                await vuser.RemoveRoleAsync(wizard);
            if (vuser.Roles.Contains(assassin))
                await vuser.RemoveRoleAsync(assassin);
            if (vuser.Roles.Contains(witch))
                await vuser.RemoveRoleAsync(witch);
            if (vuser.Roles.Contains(archer))
                await vuser.RemoveRoleAsync(archer);
            if (vuser.Roles.Contains(knight))
                await vuser.RemoveRoleAsync(knight);
        }*/
        #endregion

        public async Task OpenDragonChest()
        {
            Random rng = new Random();

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Dragon Chest opened!");

            int damage = 0;
            int health = 0;

            int result = rng.Next(1, 100);

            if (result > 80)
            {
                Embed.WithDescription("**Leather Brace** - ***Common*** - *+15 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024571779481600/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/508085504588120074.gif?v=1");
                health = 15;
                damage = 0;
            }
            else if (result > 60)
            {
                Embed.WithDescription("**Heavy Chestpiece** - ***Uncommon*** - *+30 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023555793223691/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/414149713705828352.png?v=1");
                health = 30;
                damage = 0;
            }
            else if (result > 50)
            {
                Embed.WithDescription("**Golden Plated Boots** - ***Rare*** - *+40 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023584197050372/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/402867853906280450.png?v=1");
                health = 40;
                damage = 0;
            }
            else if (result > 48)
            {
                Embed.WithDescription("**Phoenix's Plate** - ***Mythic*** - *+20 Damage, +150 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023358539431956/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/443009728746881026.gif?v=1");
                health = 150;
                damage = 20;
            }
            else if (result > 40)
            {
                Embed.WithDescription("**Thunder Scroll** - ***Rare*** - *+30 Damage, +10 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024011772788761/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/402867853906280450.png?v=1");
                health = 10;
                damage = 30;
            }
            else if (result > 36)
            {
                Embed.WithDescription("**Ancient Brace** - ***Artifact*** - *+5 Damage, +50 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024548601757698/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/402867853906280450.png?v=1");
                health = 50;
                damage = 5;
            }
            else if (result > 30)
            {
                Embed.WithDescription("**Protection Ring** - ***Rare*** - *+65 Health*");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/402867853906280450.png?v=1");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544025166372405249/latest.png");
                health = 65;
                damage = 0;
            }
            else if (result > 15)
            {
                int healthGen = rng.Next(20, 60);
                Embed.WithDescription("**Holy Health Potion** - ***Uncommon*** - +" + healthGen + " *Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544022784905445388/latest.png");
                health = healthGen;
                damage = 0;
            }
            else if (result > 2)
            {
                int healthGen = rng.Next(10, 35);
                Embed.WithDescription("**Ultra Strength Potion** - ***Uncommon*** - +" + healthGen + " *Health*");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/414149713705828352.png?v=1");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023855946137615/latest.png");
                health = healthGen;
                damage = 0;
            }
            else
            {
                Embed.WithDescription("**Dragons Pendant** - ***Special Event Item*** - *+40 Damage, +190 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024836272291840/latest.png");
                Embed.WithThumbnailUrl("https://cdn.discordapp.com/emojis/443009728746881026.gif?v=1");
                health = 190;
                damage = 40;
            }

            Embed.Color = Color.Red;
            await Context.Channel.SendMessageAsync("", false, Embed.Build());

            await Data.Data.SaveData(Context.User.Id, 0, 0, "", (uint)damage, (uint)health, 0, 0, 0);
        }

        public async Task OpenPepeChest()
        {
            Random rng = new Random();

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Pepe Chest Opened!");
            Embed.WithTitle("You collected a:");

            int damage = 0;
            int health = 0;

            int result = rng.Next(1, 100);

            if (result > 80)
            {
                Embed.WithDescription("**Common** Sad Blanket Pepe, **+10 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/524248158704762890.png?v=1");
                health = 10;
                damage = 0;
                Embed.Color = Color.LighterGrey;
            }
            else if (result > 60)
            {
                Embed.WithDescription("**Common** Heart Broken Pepe, **+12 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/437306037150810112.png?v=1");
                health = 12;
                damage = 0;
                Embed.Color = Color.LighterGrey;
            }
            else if (result > 50)
            {
                Embed.WithDescription("**Uncommon** Pinging Pepe, **+10 Damage, +5 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/508087677283860500.png?v=1");
                health = 5;
                damage = 10;

                for (int i = 0; i < 5; ++i)
                {
                    await Context.Channel.SendMessageAsync(Context.User.Mention);
                }

                Embed.Color = Color.Green;
            }
            else if (result > 48)
            {
                Embed.WithDescription("**Ultra Spicy** Poggers Pepe, **+25 Damage, +35 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/415625405572186112.gif?v=1");
                health = 35;
                damage = 25;

                Embed.Color = Color.Orange;
            }
            else if (result > 40)
            {
                Embed.WithDescription("**Uncommon** Feelz Music Man Pepe, **+28 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/472245462590816286.gif?v=1");
                health = 28;
                damage = 0;

                Embed.Color = Color.Green;
            }
            else if (result > 36)
            {
                Embed.WithDescription("**Uncommon** Clapping Pepe, **+35 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/422070018440953856.gif?v=1");
                health = 35;
                damage = 0;

                Embed.Color = Color.Green;
            }
            else if (result > 30)
            {
                Embed.WithDescription("**Rare** Clown Pepe, **+30 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/548007030401662982.png?v=1");
                health = 30;
                damage = 0;

                Embed.Color = Color.Blue;
            }
            else if (result > 15)
            {
                Embed.WithDescription("**Rare** Dabbing Pepe, **+32 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/505073491989495808.png?v=1");
                health = 32;
                damage = 0;
                Embed.Color = Color.Blue;
            }
            else if (result > 2)
            {
                Embed.WithDescription("**Uncommon** Happy Pepe, **+26 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/411876348739584000.png?v=1");
                health = 26;
                damage = 0;
                Embed.Color = Color.Green;
            }
            else
            {
                Embed.WithDescription("**Super Ultra Dank Spicey Rarest Poggers Pepe, +45 Damage, +100 Health**");
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/443009728746881026.gif?v=1");
                health = 100;
                damage = 45;

                for (int i = 0; i < 5; ++i)
                {
                    await Context.Channel.SendMessageAsync(Context.User.Mention);
                }

                Embed.Color = Color.Red;
            }

            await Context.Channel.SendMessageAsync("", false, Embed.Build());

            await Data.Data.SaveData(Context.User.Id, 0, 0, "", (uint)damage, (uint)health, 0, 0, 0);
        }

        [Command("report"), Alias("reportbug", "Report", "ReportBug"), Summary("Report a bug to the Bot Admins.")]
        public async Task BugReport([Remainder]string txt = "None")
        {
            foreach(SocketGuild guild in Program.Client.Guilds)
            {
                if(guild.Id == EnemyTemplates.ServerID)
                {
                    foreach(SocketGuildChannel channel in guild.Channels)
                    {
                        if(channel.Name == "bug-reports")
                        {
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor("Bug report has been submitted - Thank you!", Context.User.GetAvatarUrl());
                            Embed.WithDescription("Please note, submitting a false bug or abusing the command will result in a temporary or permanent ban of using the bot, after 1-2 offenses you will be added to the blacklist.");
                            Embed.Color = Color.DarkMagenta;
                            Embed.WithFooter(txt);
                            var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());

                            EmbedBuilder Embed2 = new EmbedBuilder();
                            Embed2.WithTitle("Bug report has been submitted");
                            Embed2.WithAuthor("Bug reported by user: " + Context.User.Username);
                            Embed2.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/550773144659427331/Test.png");
                            Embed2.Color = Color.DarkMagenta;
                            Embed2.WithTimestamp(Context.Message.Timestamp);
                            Embed2.WithDescription
                            (
                                txt +
                                "\n\nBug reported by:" +
                                "\nId: " + Context.User.Id +
                                "\nUsername: " + Context.User.Username +
                                "\nDiscriminator: " + Context.User.Discriminator +
                                "\nDiscriminator Value: " + Context.User.DiscriminatorValue +
                                "\nMention: " + Context.User.Mention +
                                "\nDiscord Tag: " + Context.User.Username + "#" + Context.User.DiscriminatorValue
                            );

                            var ch = channel as ISocketMessageChannel;
                            await ch.SendMessageAsync("", false, Embed2.Build());

                            await Task.Delay(15000);
                            await Context.Message.DeleteAsync();
                            await msg.DeleteAsync();
                        }
                    }
                }
            }
        }
    }
}