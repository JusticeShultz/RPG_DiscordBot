﻿using System;
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
using static RPG_Bot.Emojis.Emojis;

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

            Embed.AddField("\n\n**In Stock:**\n\n**Tier 1 Items:**", "**[100]** - **6,000**" + Coins + " - **Strength Potion** (+2 Damage)\n**[200]** - **4,500**" + Coins + " - **Toughness Potion** (+1 Health)", false);

            Embed.AddField("\n\n**Tier 2 Items**", "" +
                "\n**[250]** - **12,000**" + Coins + " - **Strength Ring** (+5 Damage)" +
                "\n**[300]** - **11,250**" + Coins + " - **Toughness Pendant** (+5 Health)", false);

            Embed.AddField("\n\n**Tier 3 Items**", "" +
                "\n**[350]** - **48,000**" + Coins + " - **Dragon Potion** (+25 Damage)" +
                "\n**[450]** - **42,000**" + Coins + " - **Viper Potion** (+20 Health)", false);

            Embed.AddField("\n\n**Tier 4 Items**", "" +
                "\n**[250]** - **2,250,000**" + Coins + " - **Destiny Potion** (+1 Skill Point)", false);

            Embed.AddField("\n\n**Rank Upgrade**", "" +
                "\n**[265]** - **50**" + GuildGem + "(+ Bronze Rank) - **Silver Rank**" +
                "\n**[266]** - **125**" + GuildGem + "(+ Silver Rank) - **Gold Rank**" +
                "\n**[267]** - **165**" + GuildGem + "(+ Gold Rank) - **Quartz Rank**" +
                "\n**[268]** - **200**" + GuildGem + "(+ Quartz Rank) - **Orichalcum Rank**" +
                "\n**[269]** - **250**" + GuildGem + "(+ Orichalcum Rank) - **Platinum Rank**" +
                "\n**[270]** - **350**" + GuildGem + "(+ Platinum Rank) - **Master III Rank**" +
                "\n**[271]** - **450**" + GuildGem + "(+ Master III Rank) - **Master II Rank**" +
                "\n**[272]** - **500**" + GuildGem + "(+ Master II Rank) - **Master I Rank**", false);

            Embed.AddField("\n\n**Loot Boxes**", "" +
                "\n**[751]** - **25**" + GuildGem + " - **Common Loot Chest**" +
                "\n**[752]** - **50**" + GuildGem + " - **Uncommon Loot Chest**" +
                "\n**[753]** - **100**" + GuildGem + " - **Rare Loot Chest**" +
                "\n**[754]** - **185**" + GuildGem + " - **Ultra Rare Loot Chest**" +
                "\n**[755]** - **250**" + GuildGem + " - **Epic Loot Chest**" +
                "\n**[756]** - **475**" + GuildGem + " - **Legendary Loot Chest**" +
                "\n**[757]** - **1000**" + GuildGem + " - **Mythic Loot Chest**", false);

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
            if (itemID != 100 && itemID != 150 && itemID != 200 && itemID != 250 && itemID != 300 && itemID != 350 &&
                itemID != 400 && itemID != 450 && itemID != 500 && itemID != 750 && itemID != 751 && itemID != 752 &&
                itemID != 753 && itemID != 754 && itemID != 755 && itemID != 756 && itemID != 757)
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

            if (itemID == 250 && UsersMoney >= 12000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Strength Ring!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023381217902603/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 5) + "!\n\n`12000 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 12000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 5, 0, 0, 0, 0);
            } //Strength ring
            else if (itemID == 300 && UsersMoney >= 11250)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Toughness Pendant!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024782530805778/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 5) + "!\n\n`11250 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 11250, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 5, 0, 0, 0);
            } //Toughness pendant
            else if (itemID == 350 && UsersMoney >= 48000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Dragon Potion!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023219967885322/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 25) + "!\n\n`48000 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 48000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 25, 0, 0, 0, 0);
            } //Dragon potion
            else if (itemID == 450 && UsersMoney >= 42000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Viper Potion!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023243472896008/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 20) + "!\n\n`42000 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 42000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 20, 0, 0, 0);
            } //Viper potion
            else if (itemID == 100 && UsersMoney >= 6000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Strength Potion!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543979973946638388/latest.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint dmg = Data.Data.GetData_Damage(Context.User.Id);
                Embed.WithDescription("Your strength points have increased from " + dmg + " to " + (dmg + 2) + "!\n\n`6000 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 6000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 2, 0, 0, 0, 0);
            } //Strength potion
            else if (itemID == 200 && UsersMoney >= 4500)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Toughness Potion!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543980190657675284/potion-png-5.png");
                Embed.Color = Color.Gold;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("Your health points have increased from " + health + " to " + (health + 1) + "!\n\n`4500 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 4500, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 0, 1, 0, 0, 0);
            } //Toughness potion
            else if (itemID == 500 && UsersMoney >= 2250000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("You purchased a Destiny Potion!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023097020252163/latest.png");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You have gained 1 skill point! `2,250,000 Gold Coins` have been taken from your bank.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SubtractSaveData(Context.User.Id, 2250000, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 1);
            } //Destiny Potion
            else if (itemID == 751 && UsersGems >= 25)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Common Loot Chest Purchased!");
                Embed.Color = Color.LighterGrey;
                Embed.WithFooter("Use -lootbox common to open up your new lootbox or -lootbox to see your current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **25** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240876719210501/locked-chest.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 25);
                await Data.Data.AddCommonBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 752 && UsersGems >= 50)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Uncommon Loot Chest Purchased!");
                Embed.Color = Color.Green;
                Embed.WithFooter("Use -lootbox uncommon to open up your new lootbox or -lootbox to see your current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **50** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240878015381516/locked-chest_1.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 50);
                await Data.Data.AddUncommonBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 753 && UsersGems >= 100)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Rare Loot Chest Purchased!");
                Embed.Color = Color.Blue;
                Embed.WithFooter("Use -lootbox rare to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **100** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240879382462475/locked-chest_2.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 100);
                await Data.Data.AddRareBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 754 && UsersGems >= 185)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Very Rare Loot Chest Purchased!");
                Embed.Color = Color.Magenta;
                Embed.WithFooter("Use -lootbox veryrare to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **185** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240880749805568/locked-chest_3.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 185);
                await Data.Data.AddVeryRareBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 755 && UsersGems >= 250)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Epic Loot Chest Purchased!");
                Embed.Color = Color.Purple;
                Embed.WithFooter("Use -lootbox epic to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **250** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240881794449437/locked-chest_4.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 250);
                await Data.Data.AddEpicBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 756 && UsersGems >= 475)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Legendary Loot Chest Purchased!");
                Embed.Color = Color.Red;
                Embed.WithFooter("Use -lootbox legendary to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **475** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240883669172237/locked-chest_5.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 475);
                await Data.Data.AddLegendaryBoxCount(Context.User.Id, 1);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (itemID == 757 && UsersGems >= 1000)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Mythic Loot Chest Purchased!");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Use -lootbox mythic to open up your new lootbox or -lootbox to see current loot boxes!");
                Embed.WithDescription("You traded the shop keeper **1000** " + GuildGem);
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240886613704772/locked-chest_6.png");
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 1000);
                await Data.Data.AddMythicBoxCount(Context.User.Id, 1);
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
                    Embed.WithDescription("You traded the shop keeper 50" + GuildGem + "and your previous rank to become a Silver Rank!");
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
                    Embed.WithDescription("You traded the shop keeper 125" + GuildGem + " and your previous rank to become a Gold Rank!");
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
                    Embed.WithDescription("You traded the shop keeper 165" + GuildGem + " and your previous rank to become a Quartz Rank!");
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
                    Embed.WithDescription("You traded the shop keeper 200" + GuildGem + " and your previous rank to become a Orichalcum Rank!");
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
                    Embed.WithDescription("You traded the shop keeper 250" + GuildGem + " and your previous rank to become a Platinum Rank!");
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
                    Embed.WithDescription("You traded the shop keeper 350" + GuildGem + " and your previous rank to become a Master III Adventurer!");
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
                    Embed.WithDescription("You traded the shop keeper 450" + GuildGem + " and your previous rank to become a Master II Adventurer!");
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
                    Embed.WithDescription("You traded the shop keeper 450" + GuildGem + " and your previous rank to become a Master I Adventurer!\nYou are now the max rank!");
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());

                    //await (vuser as IGuildUser).RemoveRoleAsync(masterII);
                    //await (vuser as IGuildUser).AddRoleAsync(masterI);

                    await Data.Data.SetRank(vuser.Id, "Master1");
                    await Gameplay.UpdateUserData();

                    await Data.Data.TakeEventData(Context.User.Id, 0, 0, 500);
                }
                else await NotEnoughMoney();
            }

            #region Halloween event [Out of season]
            //else if (itemID == 850 && UsersCandies >= 3)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Common Loot Chest Purchased!");
            //    Embed.Color = Color.LighterGrey;
            //    Embed.WithFooter("Use -lootbox common to open up your new lootbox or -lootbox to see your current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **3** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240876719210501/locked-chest.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 3, 0);
            //    await Data.Data.AddCommonBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 851 && UsersCandies >= 8)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Uncommon Loot Chest Purchased!");
            //    Embed.Color = Color.Green;
            //    Embed.WithFooter("Use -lootbox uncommon to open up your new lootbox or -lootbox to see your current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **8** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240878015381516/locked-chest_1.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 8, 0);
            //    await Data.Data.AddUncommonBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 852 && UsersCandies >= 15)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Rare Loot Chest Purchased!");
            //    Embed.Color = Color.Blue;
            //    Embed.WithFooter("Use -lootbox rare to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **15** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240879382462475/locked-chest_2.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 15, 0);
            //    await Data.Data.AddRareBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 853 && UsersCandies >= 22)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Very Rare Loot Chest Purchased!");
            //    Embed.Color = Color.Magenta;
            //    Embed.WithFooter("Use -lootbox veryrare to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **22** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240880749805568/locked-chest_3.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 22, 0);
            //    await Data.Data.AddVeryRareBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 854 && UsersCandies >= 30)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Epic Loot Chest Purchased!");
            //    Embed.Color = Color.Purple;
            //    Embed.WithFooter("Use -lootbox epic to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **30** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240881794449437/locked-chest_4.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 30, 0);
            //    await Data.Data.AddEpicBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 855 && UsersCandies >= 50)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Legendary Loot Chest Purchased!");
            //    Embed.Color = Color.Red;
            //    Embed.WithFooter("Use -lootbox legendary to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **50** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240883669172237/locked-chest_5.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 50, 0);
            //    await Data.Data.AddLegendaryBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 856 && UsersCandies >= 100)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Mythic Loot Chest Purchased!");
            //    Embed.Color = Color.Teal;
            //    Embed.WithFooter("Use -lootbox mythic to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **100** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240886613704772/locked-chest_6.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 100, 0);
            //    await Data.Data.AddMythicBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            //else if (itemID == 857 && UsersCandies >= 125)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Godly Loot Chest Purchased!");
            //    Embed.Color = Color.LighterGrey;
            //    Embed.WithFooter("Use -lootbox godly to open up your new lootbox or -lootbox to see current loot boxes!");
            //    Embed.WithDescription("You traded the shop keeper **125** " + Candy);
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/639240875452661791/locked-chest_7.png");
            //    await Data.Data.TakeEventData(Context.User.Id, 0, 125, 0);
            //    await Data.Data.AddGodlyBoxCount(Context.User.Id, 1);
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            #endregion

            #region Pepe Event [Ended]
            //else if (itemID == 650 && UsersMoney >= 8500)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.Color = Color.Green;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper **8500**" + Coins + " to open a Pepe Chest, what a loser");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SubtractSaveData(Context.User.Id, 8500, 0, "", 0, 0, 0, 0, 0);
            //    await OpenPepeChest();
            //}
            #endregion

            #region Essence event [Out of season]
            //else if (itemID == 105 && UsersEvents1 >= 5)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
            //    Embed.Color = Color.Gold;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper 5 Essence for 50 Gold Coins!");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
            //    await Data.Data.TakeEventData(Context.User.Id, 5, 0, 0);
            //}
            //else if (itemID == 105 && UsersEvents1 >= 5)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
            //    Embed.Color = Color.Gold;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper 5 Essence for 50 Gold Coins!");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
            //    await Data.Data.TakeEventData(Context.User.Id, 5, 0, 0);
            //}
            //else if (itemID == 210 && UsersEvents1 >= 35)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
            //    Embed.Color = Color.Gold;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper 35 Essence for 700 Gold Coins!");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SaveData(Context.User.Id, 700, 0, "", 0, 0, 0, 0, 0);
            //    await Data.Data.TakeEventData(Context.User.Id, 35, 0, 0);
            //}
            //else if (itemID == 215 && UsersEvents1 >= 55)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
            //    Embed.Color = Color.Gold;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper 55 Essence for 1500 Gold Coins!");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SaveData(Context.User.Id, 1500, 0, "", 0, 0, 0, 0, 0);
            //    await Data.Data.TakeEventData(Context.User.Id, 55, 0, 0);
            //}
            //else if (itemID == 385 && UsersEvents1 >= 65)
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Trade was Successful.");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024948029653042/latest.png");
            //    Embed.Color = Color.Gold;
            //    Embed.WithFooter("Thanks for your business!");
            //    uint health = Data.Data.GetData_Health(Context.User.Id);
            //    Embed.WithDescription("You traded the shop keeper 65 Essence for a rare Essence Helm! (+50 Damage, +80 Health)");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //    await Data.Data.SaveData(Context.User.Id, 0, 0, "", 30, 50, 0, 0, 0);
            //    await Data.Data.TakeEventData(Context.User.Id, 65, 0, 0);
            //}
            #endregion

            #region [Deprecated]
            /*
            else if (itemID == 450 && UsersGems >= 285)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Trade was Successful.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024389239439369/Magic20Chest.png");
                Embed.Color = Color.DarkPurple;
                Embed.WithFooter("Thanks for your business!");
                uint health = Data.Data.GetData_Health(Context.User.Id);
                Embed.WithDescription("You traded the shop keeper 285" + GuildGem + " to open a Dragons Chest");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.TakeEventData(Context.User.Id, 0, 0, 285);
                await OpenDragonChest();
            }
             */
            #endregion
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
                Embed.WithTitle("Christmas Event");
                Embed.WithAuthor("Present Thiefs");
                Embed.WithDescription("Fight monsters and get a chance to gather a stolen present!\n\n" +
                    "Event Commands:" +
                    "\n-event shop - Use this command in the guild shop to display the trade in rates for stolen presents." +
                    "\n-event count - Show your current present count. You may also do -profile to see the value." +
                    "\n\n\nAdmin Commands:" +
                    "\n-event - Display the current servers event info inside the events tab.");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/650244722715000842/Banner_xmas_dungeon.webp");
                Embed.Color = Color.Green;
                Embed.WithFooter("Holiday event runs until January 31st");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            if (remainder == "shop" || remainder == "Shop")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Greetings adventurer!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543948020505640981/ShopKeep.png");
                Embed.Color = Color.LightOrange;
                Embed.WithFooter("You have " + Data.Data.GetData_Event1(Context.User.Id) + " Presents");
                Embed.WithTitle("Sorry about the inconvenience, but the guild has delayed my shipment of rewards... I will be restocked soon enough with plenty of " +
                    "new merchandise, but for now, stick to what is in stock, I'll let everyone know when my shipment arrives!");

                //Embed.AddField("\n\nIn Stock:\n\nGold", "[105] - 5 Essence - 50 Gold Coins\n[210] - 35 Essence - 700 Gold Coins\n[215] - 55 Essence - 1500 Gold Coins", false);
                //Embed.AddField("\n\nItems:", "[385] - 65 Essence - Essence Helm (+50 Damage, +80 Health)", false);

                //Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            //if (remainder == "candyshop" || remainder == "CandyShop")
            //{
            //    EmbedBuilder Embed = new EmbedBuilder();
            //    Embed.WithAuthor("Happy Halloween!");
            //    Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545322196872986641/Wendy.webp");
            //    Embed.Color = Color.Orange;
            //    Embed.WithFooter("You have " + Data.Data.GetData_Event2(Context.User.Id) + " Candies");
            //    Embed.WithTitle("How to use:");

            //    Embed.AddField("\n\nIn Stock:\n\nLoot Boxes", "[850] - 3 Candies - Common Loot Chest\n[851] - 8 Candies - Uncommon Loot Chest\n[852] - 15 Candies - Rare Loot Chest\n[853] - 22 Candies - Very Rare Loot Chest\n[854] - 30 Candies - Epic Loot Chest\n[855] - 50 Candies - Legendary Loot Chest\n[856] - 100 Candies - Mythic Loot Chest\n[857] - 125 Candies - Godly Loot Chest", false);

            //    Embed.WithDescription("To buy items you must type `-buy [ItemID]` - Item ID's are the numbers in [] on each item!");
            //    await Context.Channel.SendMessageAsync("", false, Embed.Build());
            //}
            else if (remainder == "count" || remainder == "Count")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("Christmas Event #1");
                Embed.WithAuthor("Present Thiefs");
                Embed.WithDescription("You have " + Data.Data.GetData_Event1(Context.User.Id) + " presents!");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/650246026749542440/gift_PNG5945.webp");
                Embed.Color = Color.Teal;
                Embed.WithFooter("Holiday event runs until January 31st");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }

            #region [Deprecated]
            //else
            //{
            //    SocketGuildUser user = Context.User as SocketGuildUser;

            //    if (user.GuildPermissions.Administrator)
            //    {
            //        EmbedBuilder Embed = new EmbedBuilder();
            //        //Embed.WithTitle("Serverwide Event #3");
            //        //Embed.WithAuthor("The Essence Moon.");
            //        ///*Embed.WithDescription("Monsters have recently been seen holding some form of glowing log. These logs are infused with huge sums of magical power and the " +
            //        //    "shopkeeper would be more than happy to trade gold and powerful items for them. Check out the `-event shop` command over in the shop keepers store for " +
            //        //    "limited time! Nearing the final days a legend says a mystic tree is said to appear that might be made of this newly found resource, keep your watch up!" +
            //        //    "\n\nType `-event help` to view relevant commands for this event!");
            //        //Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545308296463122442/Will_O_Wisp.webp");*/
            //        //Embed.WithDescription("The essence moon is rising and the demon king awakens with it. Collect essence " +
            //        //    "and gain ultimate power. (All previous event items will be transfered into this event, I will reset them next time if this doesn't work well)");
            //        //Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545308301089439754/Crom_Cruach.png");
            //        //Embed.Color = Color.Purple;
            //        //Embed.WithFooter("Special event runs until May 20th");
            //        //await Context.Channel.SendMessageAsync("", false, Embed.Build());

            //        //EmbedBuilder Embed1 = new EmbedBuilder();
            //        //Embed1.WithTitle("Serverwide Event #4");
            //        //Embed1.WithAuthor("The Gold Filled Pots.");
            //        //Embed1.WithDescription("A new monster is roaming throughout the plains and it's full of gold! For limited time fight these gold filled enemies" +
            //        //    " for huge amounts of gold! Low ranking areas will have Bronze Pots, medium areas will contain Silver Pots and high ranking areas will spawn " +
            //        //    "Golden Pots! Good luck adventurer, may luck be in your favor.\n\nNote: the event monsters will not spawn in master rank zones, this means " +
            //        //    "zones 60+ will not spawn them and their spawn tables will not have been affected by this event.");
            //        //Embed1.WithImageUrl("https://vignette.wikia.nocookie.net/quiz-rpg-the-world-of-mystic-wiz/images/f/f5/The_Golden_Pot_transparent.png/revision/latest?cb=20141025232107");
            //        //Embed1.Color = Color.Gold;
            //        //Embed1.WithFooter("Special event runs until May 20th");
            //        //await Context.Channel.SendMessageAsync("", false, Embed1.Build());

            //        EmbedBuilder Embed1 = new EmbedBuilder();
            //        Embed1.WithTitle("Holiday Event #1");
            //        Embed1.WithAuthor("Spooktober");
            //        Embed1.WithDescription("The mist is growing heavier!\n" +
            //            "Spooktober kicks off starting now, visit the ``spooktober`` zone and fight event monsters to earn candies. Candies can be used to unlock special " +
            //            "items during the Spooktober event, more details on that can be found soon! Additionally item drop rate will be doubled within this zone and there is" +
            //            " a variety of different leveled monsters! Happy hunting and keep an eye out for the lord of halloween, he may just crush you if you aren't strong enough!");
            //        Embed1.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/637578405281529858/EventIcon.webp");
            //        Embed1.Color = Color.Gold;
            //        Embed1.WithFooter("Special event runs until November 15th");
            //        await Context.Channel.SendMessageAsync("", false, Embed1.Build());
            //    }
            //}
            #endregion

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
            Embed.WithTitle(user.Username + " has recieved " + Amount + GuildGem + " from " + Context.User.Username + "(Admin)");
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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

                    await TakeClassStats(Data.Data.GetClass(Context.User.Id));
                    await GiveClassStats(txt);

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

        public async Task TakeClassStats(string _class)
        {
            if (_class == "Knight" || _class == "knight")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 20, 50, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 2, 1, 0, 0, 0, 2);
            }
            if (_class == "Rogue" || _class == "rogue")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 60, 10, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 1, 3, 1, 0, 0);
            }
            if (_class == "Archer" || _class == "archer")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 50, 20, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 0, 2, 3, 0, 0);
            }
            if (_class == "Assassin" || _class == "assassin")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 0, 1, 4, 0, 0);
            }
            if (_class == "Wizard" || _class == "wizard")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 48, 22, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 1, 0, 1, 2, 1, 0);
            }
            if (_class == "Witch" || _class == "witch")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 45, 25, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 1, 0, 1, 1, 2, 0);
            }
            if (_class == "Berserker" || _class == "berserker" || _class == "berzerker" || _class == "Berzerker")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 40, 30, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 1, 4, 0, 0, 0, 0);
            }
            if (_class == "Tamer" || _class == "tamer")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 0, 0, 1, 1, 3);
            }
            if (_class == "Monk" || _class == "monk")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 35, 35, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 1, 1, 1, 1, 1, 0);
            }
            if (_class == "Necromancer" || _class == "Necromancer")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 2, 0, 0, 3, 0);
            }
            if (_class == "Paladin" || _class == "paladin")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 15, 55, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 2, 1, 0, 1, 0, 1);
            }
            if (_class == "Swordsman" || _class == "swordsman")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 40, 30, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 2, 1, 0, 1, 1, 0);
            }
            if (_class == "Evangel" || _class == "evangel")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 0, 1, 0, 1, 3);
            }
            if (_class == "Kitsune" || _class == "kitsune")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 1, 1, 0, 1, 2);
            }
            if (_class == "Trickster" || _class == "trickster")
            {
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 55, 15, 1, 0, 0);
                await Data.Data.SubtractSkillPoints(Context.User.Id, 0, 1, 2, 0, 1, 1);
            }
        }

        public async Task GiveClassStats(string _class)
        {
            if (_class == "Knight" || _class == "knight")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 20, 50, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 2, 1, 0, 0, 0, 2);
            }
            if (_class == "Rogue" || _class == "rogue")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 60, 10, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 1, 3, 1, 0, 0);
            }
            if (_class == "Archer" || _class == "archer")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 50, 20, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 0, 2, 3, 0, 0);
            }
            if (_class == "Assassin" || _class == "assassin")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 0, 1, 4, 0, 0);
            }
            if (_class == "Wizard" || _class == "wizard")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 48, 22, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 1, 0, 1, 2, 1, 0);
            }
            if (_class == "Witch" || _class == "witch")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 45, 25, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 1, 0, 1, 1, 2, 0);
            }
            if (_class == "Berserker" || _class == "berserker" || _class == "berzerker" || _class == "Berzerker")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 40, 30, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 1, 4, 0, 0, 0, 0);
            }
            if (_class == "Tamer" || _class == "tamer")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 0, 0, 1, 1, 3);
            }
            if (_class == "Monk" || _class == "monk")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 35, 35, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 1, 1, 1, 1, 1, 0);
            }
            if (_class == "Necromancer" || _class == "Necromancer")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 2, 0, 0, 3, 0);
            }
            if (_class == "Paladin" || _class == "paladin")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 15, 55, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 2, 1, 0, 1, 0, 1);
            }
            if (_class == "Swordsman" || _class == "swordsman")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 40, 30, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 2, 1, 0, 1, 1, 0);
            }
            if (_class == "Evangel" || _class == "evangel")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 0, 1, 0, 1, 3);
            }
            if (_class == "Kitsune" || _class == "kitsune")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 65, 5, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 1, 1, 0, 1, 2);
            }
            if (_class == "Trickster" || _class == "trickster")
            {
                await Data.Data.SaveData(Context.User.Id, 0, 0, "", 55, 15, 1, 0, 0);
                await Data.Data.AddSkillPoints(Context.User.Id, 0, 1, 2, 0, 1, 1);
            }
        }

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
                health = 15;
                damage = 0;
            }
            else if (result > 60)
            {
                Embed.WithDescription("**Heavy Chestpiece** - ***Uncommon*** - *+30 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023555793223691/latest.png");
                health = 30;
                damage = 0;
            }
            else if (result > 50)
            {
                Embed.WithDescription("**Golden Plated Boots** - ***Rare*** - *+40 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023584197050372/latest.png");
                health = 40;
                damage = 0;
            }
            else if (result > 48)
            {
                Embed.WithDescription("**Phoenix's Plate** - ***Mythic*** - *+40 Damage, +150 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023358539431956/latest.png");
                health = 150;
                damage = 40;
            }
            else if (result > 40)
            {
                Embed.WithDescription("**Thunder Scroll** - ***Rare*** - *+30 Damage, +10 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024011772788761/latest.png");
                health = 10;
                damage = 30;
            }
            else if (result > 37)
            {
                Embed.WithDescription("**Ancient Brace** - ***Artifact*** - *+200 Damage, +150 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024548601757698/latest.png");
                health = 150;
                damage = 200;
            }
            else if (result > 30)
            {
                Embed.WithDescription("**Protection Ring** - ***Rare*** - *+65 Health*");
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
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023855946137615/latest.png");
                health = healthGen;
                damage = 0;
            }
            else
            {
                Embed.WithDescription("**Dragons Pendant** - ***Godly*** - *+125 Damage, +350 Health*");
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544024836272291840/latest.png");
                health = 350;
                damage = 125;
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
            foreach (SocketGuild guild in Program.Client.Guilds)
            {
                if (guild.Id == EnemyTemplates.ServerID)
                {
                    foreach (SocketGuildChannel channel in guild.Channels)
                    {
                        if (channel.Name == "bug-reports")
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

        [Command("Gamble"), Alias("gamble"), Summary("Gamble money.")]
        public async Task Gamble(uint? gold)
        {
            if (Context.Channel.Name != "testing" && Context.Channel.Name != "the-tavern")
            {
                EmbedBuilder EmbedFail = new EmbedBuilder();
                EmbedFail.WithTitle("Can't gamble here!");
                EmbedFail.WithDescription("Visit the tavern in Silverkeep if you'd like to do that...");
                EmbedFail.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());
            }
            else if (gold != null)
            {
                if (gold <= 0)
                {
                    EmbedBuilder EmbedFail = new EmbedBuilder();
                    EmbedFail.WithTitle("Invalid input!");
                    EmbedFail.WithDescription("Use the command as:\n\n-Gamble [Amount]");
                    EmbedFail.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());
                }
                else
                {
                    //valid?
                    uint usersGold = Data.Data.GetData_GoldAmount(Context.User.Id);

                    if (usersGold >= gold)
                    {
                        await Data.Data.SubtractSaveData(Context.User.Id, (uint)gold, 0, "", 0, 0, 0, 0, 0);

                        EmbedBuilder EmbedFail = new EmbedBuilder();
                        EmbedFail.WithTitle("Gambling " + gold + Coins + "!");
                        EmbedFail.WithDescription("Results in 3...");
                        EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064162511388719/Omake_Gif_Anime_-_Kakegurui_-_Episode_10_-_Ririka_Spreads_Deck.gif");
                        EmbedFail.Color = Color.Gold;
                        var msg = await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());

                        await Task.Delay(1000);

                        EmbedFail.WithDescription("Results in 2...");

                        if (msg != null)
                        {
                            await msg.ModifyAsync(x =>
                            {
                                x.Content = "";
                                x.Embed = EmbedFail.Build();
                            });
                        }


                        await Task.Delay(1000);

                        EmbedFail.WithDescription("Results in 1...");

                        if (msg != null)
                        {
                            await msg.ModifyAsync(x =>
                            {
                                x.Content = "";
                                x.Embed = EmbedFail.Build();
                            });
                        }

                        await Task.Delay(1000);

                        Random rng = new Random();

                        int result = rng.Next(0, 101);

                        if (result <= 45)
                        {
                            //win
                            EmbedFail.WithTitle("You won the gamble!!!");
                            EmbedFail.WithDescription("You gain " + (gold * 2) + Coins + "!\n\n***You now have " + Data.Data.GetData_GoldAmount(Context.User.Id) + Coins + "***");
                            EmbedFail.Color = Color.Gold;

                            int res = rng.Next(0, 51);

                            if (res < 10) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064110699151381/ReliableSkeletalCanvasback-size_restricted.gif");
                            else if (res < 20) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064107507286016/source46.gif");
                            else if (res < 30) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064113010343967/tenor5456.gif");
                            else if (res < 40) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064098636333087/source7571.gif");
                            else if (res < 55) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064030328029190/tenor12543.gif");

                            await Data.Data.SaveData(Context.User.Id, (uint)(gold * 2), 0, "", 0, 0, 0, 0, 0);
                        }
                        else
                        {
                            //lose
                            EmbedFail.WithTitle("You lost the gamble...");
                            EmbedFail.WithDescription("You lose " + gold + Coins + ".\n\n*You now have " + Data.Data.GetData_GoldAmount(Context.User.Id) + Coins + "*");
                            EmbedFail.Color = Color.DarkRed;

                            int res = rng.Next(0, 51);

                            if (res < 10) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064017967284225/mad-anime-gif-6.gif");
                            else if (res < 20) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064016138567718/mad-anime-girl-gif-4.gif");
                            else if (res < 25) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064076238618634/tumblr_of1urbUQ3O1udouqko1_500.gif");
                            else if (res < 30) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064017057251353/MKSe.gif");
                            else if (res < 35) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064095109054476/dMecNcX.gif");
                            else if (res < 40) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064023407427584/source6464.gif");
                            else if (res < 55) EmbedFail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/662064034878849025/tenor445.gif");
                        }

                        if (msg != null)
                        {
                            await msg.ModifyAsync(x =>
                            {
                                x.Content = "";
                                x.Embed = EmbedFail.Build();
                            });
                        }
                    }
                    else
                    {
                        EmbedBuilder EmbedFail = new EmbedBuilder();
                        EmbedFail.WithTitle("Not enough gold!");
                        EmbedFail.WithDescription("*You only have " + usersGold + Coins + "*");
                        EmbedFail.Color = Color.Red;
                        await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());
                    }
                }
            }
            else
            {
                EmbedBuilder EmbedFail = new EmbedBuilder();
                EmbedFail.WithTitle("Invalid input!");
                EmbedFail.WithDescription("Use the command as:\n\n-Gamble [Amount]");
                EmbedFail.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, EmbedFail.Build());
            }
        }
    }
}