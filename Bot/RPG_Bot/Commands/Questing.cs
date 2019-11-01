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
using System.Timers;

namespace RPG_Bot.Commands
{
    public class Questing : ModuleBase<SocketCommandContext>
    {
        static Random rng = new Random();

        [Command("Quest"), Alias("quest", "Q", "q"), Summary("Go on a quest.")]
        public async Task DoQuest()
        {
            if (Context.Channel.Name != "questing")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Error");
                Embed.WithDescription("Quests cannot be done here.");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Red;
                Embed.WithFooter("Use the questing channel to do quests for the guild!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                return;
            }

            var time = Context.Message.Timestamp;
            int Hour = Data.Data.GetData_Hour(Context.User.Id);
            int Minute = Data.Data.GetData_Minute(Context.User.Id);
            int Second = Data.Data.GetData_Second(Context.User.Id);
            int Day = Data.Data.GetData_Day(Context.User.Id);

            //First quest.
            if (Hour == 0 && Minute == 0 && Second == 0 && Day == 0)
            {
                await Data.Data.UpdateTime(Context.User.Id, time.TimeOfDay.Hours, time.TimeOfDay.Minutes, time.TimeOfDay.Seconds, time.Day);
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.WithAuthor("Your first quest.");
                Embed.WithDescription("The guild assigns you your first quest ever.\nThey give the easiest" +
                    " quest available. You deliver 5 bags of bread to a nearby inn and make 50 Gold Coins!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                Embed.WithTimestamp(time);
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithFooter("You may now quest every 5 minutes!");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
                return;
            }
            else
            {
                if (time.Minute >= (Minute + 5) || time.Hour != Hour || Day != time.Day)
                {
                    if (Minute + 5 >= 60)
                        await Data.Data.UpdateTime(Context.User.Id, time.TimeOfDay.Hours, 0, time.TimeOfDay.Seconds, time.Day);
                    else
                        await Data.Data.UpdateTime(Context.User.Id, time.TimeOfDay.Hours, time.TimeOfDay.Minutes, time.TimeOfDay.Seconds, time.Day);

                    await GenerateQuestOutcome();

                    return;
                }
                else
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Failed to go on a quest!");

                    Embed.WithDescription("You may only go on a quest every 5 minutes! You may quest again in "
                            + ((Minute + 5) - time.Minute) + " minutes");

                    Embed.WithColor(40, 200, 150);
                    Embed.Color = Color.Red;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    return;
                }
            }
        }

        public async Task GenerateQuestOutcome()
        {
            int num = rng.Next(1, 20);

            if (num == 1)
            {
                int coins = rng.Next(1, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543140066927575047/Unit_ills_full_10050.png");
                Embed.WithAuthor("-Green trouble-");
                Embed.WithDescription("You get a quest to go and fight some Goblins off from a nearby village and return unscathed. You are paid 65 Gold Coins, you are also given " + coins + "<:GuildGem:545341213004529725>");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Green;
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 65, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 2)
            {
                int coins = rng.Next(1, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543140067519234068/image_3.png");
                Embed.WithAuthor("-A deadly fight-");

                if (Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.1) != 0)
                    Embed.WithDescription("You get a quest to go and fight a Chimera off from a nearby river and return injured. You are paid 30 Gold Coins and " + coins + "<:GuildGem:545341213004529725>! But you lose " + Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.1) + "XP while in bed resting...");
                else Embed.WithDescription("You get a quest to go and fight a Chimera off from a nearby river and return injured. You are paid 30 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");

                Embed.WithColor(40, 200, 150);
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.Color = Color.DarkRed;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 30, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, 0, 0, (uint)Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.1), 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 3)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139848987607050/Unit_ills_full_30050.png");
                Embed.WithAuthor("-A little tree problem-");

                if (Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.05) != 0)
                    Embed.WithDescription("You get a quest to go and destroy a nest of Tree Ents in the nearby woods and return injured. You are paid 50 Gold Coins and lose " + Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.05) + "XP");
                else Embed.WithDescription("You get a quest to go and destroy a nest of Tree Ents in the nearby woods and return injured. You are paid 50 Gold Coins");
                Embed.WithColor(40, 200, 150);
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.Color = Color.DarkGreen;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 30, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SubtractSaveData(Context.User.Id, 0, 0, "", 0, 0, 0, (uint)Math.Floor((int)Data.Data.GetData_XP(Context.User.Id) * 0.05), 0);
            }

            if (num == 4)
            {
                int coins = rng.Next(5, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/emojis/543112388493312000.png?v=1");
                Embed.WithAuthor("-Criminal Arrest-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You get a quest to go and capture a thief. You are paid 75 Gold Coins and " + coins + "<:GuildGem:545341213004529725> once you bring him in!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.LightOrange;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 70, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 5)
            {
                int coins = rng.Next(3, 30);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139937457930300/Unit_ills_full_10052.png");
                Embed.WithAuthor("-Goblins Blood-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You get a quest to go and hunt a Hob Goblin for some of its blood. You are paid 50 Gold Coins once you bring a vial of its blood back and also awarded " + coins + "<:GuildGem:545341213004529725>.");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.DarkTeal;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 50, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 6)
            {
                int coins = rng.Next(3, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023145955459073/latest.png");
                Embed.WithAuthor("-Local Herbes-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You get a quest to harvest some local herbes. You are paid 75 Gold Coins once you turn them in and additionally make " + coins + "<:GuildGem:545341213004529725>.");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.DarkGreen;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 75, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 7)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023282672861214/latest.png");
                Embed.WithAuthor("-Little Green Capped Man-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("A strange mute man in green plays a song on a flute like device. Once he finishes he hands you a red crystal and runs off. You trade the red crystal in with the shop keeper for 100 Gold Coins!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Green;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 100, 0, "", 0, 0, 0, 0, 0);
            }

            if (num == 8)
            {
                int coins = rng.Next(5, 10);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023180185174143/latest.png");
                Embed.WithAuthor("-Magical Paste-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You are asked by the guild to deliver an item to a royal manner safely and are rewarded 25 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Teal;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 25, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 9)
            {
                int coins = rng.Next(20, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544023069992419338/latest.png");
                Embed.WithAuthor("-Special Delivery-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You are asked by the guild to deliver an item to a royal manner safely and are rewarded 15 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Orange;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 15, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 10)
            {
                int coins = rng.Next(15, 20);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544022848503676950/latest.png");
                Embed.WithAuthor("-Holy Scroll-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You are asked by the guild to deliver an item to the holy library safely and are rewarded 12 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 12, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 11)
            {
                int coins = rng.Next(5, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543140268845694987/giphy_1.gif");
                Embed.WithAuthor("-Rogue Robot-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("A rogue training bot is found causing tons of damage to the nearby village. You easily slay it and the guild rewards you 30 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.LighterGrey;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 30, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 12)
            {
                int coins = rng.Next(10, 55);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543140114981715990/Unit_ills_full_750005.png");
                Embed.WithAuthor("-Celestial Being-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You are asigned to guide a mystic being to a nearby village safeley and are paid 30 Gold Coins and " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Orange;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 30, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 13)
            {
                int coins = rng.Next(2, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139900963160109/Unit_ills_full_60181.png");
                Embed.WithAuthor("-Little Red Trickster-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("An Imp was reported to be causing mischief in the local area. You are paid 55 Gold Coins and " + coins + "<:GuildGem:545341213004529725> for slaying it!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.DarkRed;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 55, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 14)
            {
                int coins = rng.Next(2, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139900329951243/Unit_ills_full_20274.png");
                Embed.WithAuthor("-Ice Fortress-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("The guild asks you to travel far north to the Great Ice Fortress with a small metal lockbox. You are paid 150 Gold Coins for your work and awarded " + coins + "<:GuildGem:545341213004529725>!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Teal;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 150, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 15)
            {
                int coins = rng.Next(2, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139849809559562/Unit_ills_full_60142.png");
                Embed.WithAuthor("-Mimic Murder-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("A mimic was found in a local town home surrounded in its inhabitants blood. You are awarded 95 Gold Coins and " + coins + "<:GuildGem:545341213004529725> for avenging the family by the guild.");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Red;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 95, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 16)
            {
                int coins = rng.Next(3, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/544791450068713483/439083435961745409.gif");
                Embed.WithAuthor("-Quack-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You find a duck and sell it to a butcher for 85 Gold Coins. Your greed just killed that duck but you also got " + coins + "<:GuildGem:545341213004529725> so it may be okay...");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Red;
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 85, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 17)
            {
                int coins = rng.Next(3, 25);

                Gauntlets gaunts = Data.Data.GetGauntlet(Context.User.Id);
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl(gaunts == null ? "" : gaunts.WebURL);
                Embed.WithAuthor("-Pumpkin Patch Genocide-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                string result = gaunts == null ? "your bare hands" : "your " + gaunts.ItemName + " on";
                Embed.WithDescription("You recieve a quest from the guild to go squash all of the pumpkinlings in the pumpkin patches. You brutally murder tons of them with " + result + ". You are awarded 85 coins, " + coins + "<:GuildGem:545341213004529725> and 3<:Candy:637578758986924035>");
                Embed.WithColor(Color.DarkRed);
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 85, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 3, (uint)coins);
            }

            if (num == 18)
            {
                int coins = rng.Next(3, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/637578438735036426/Unit_ills_full_50802.webp");
                Embed.WithAuthor("-Phantom Problem-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You are sent out to slay a phantom and are awarded 85 Gold Coins and " + coins + "<:GuildGem:545341213004529725>. The guild also awards you 1<:Candy:637578758986924035> for your effort.");
                Embed.WithColor(Color.DarkBlue);
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 85, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 0, (uint)coins);
            }

            if (num == 19 || num == 20)
            {
                int coins = rng.Next(3, 25);

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/543139937948532778/Unit_ills_full_10050.png");
                Embed.WithAuthor("-Free Candy-");
                Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
                Embed.WithDescription("You start to go on a quest to slay some goblins but you end up finding 5<:Candy:637578758986924035> on a pumpkin statue. You continue on your quest and successfully slay the goblins, returning back to the guild for a reward of 25 Gold Coins and " + coins + "<:GuildGem:545341213004529725>.");
                Embed.WithColor(Color.LightOrange);
                Embed.WithFooter("A new quest will be available in 5 minutes.");
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
                await Data.Data.SaveData(Context.User.Id, 25, 0, "", 0, 0, 0, 0, 0);
                await Data.Data.SaveEventData(Context.User.Id, 0, 5, (uint)coins);
            }
        }
    }
}