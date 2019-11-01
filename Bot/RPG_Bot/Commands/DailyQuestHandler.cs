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
    public class DailyQuestHandler : ModuleBase<SocketCommandContext>
    {
        /*public static async Task ResetCheck() [DEPRECATED]
        {
            System.DateTime time = new System.DateTime();

            if (time.Date.Day != Date)
            {
                Date = time.Date.Day;
                Console.WriteLine("Midnight reset.");

                foreach(SocketGuildUser users in RPG_Bot.Program.Client.GetGuild(EnemyTemplates.ServerID).Users)
                {
                    await Data.Data.SetDailyClaimed(users.Id, false);
                }
            }

            await Task.Delay(10000);
            ResetCheck();
        }*/

        Random rng = new Random();

        [Command("Daily"), Alias("daily", "D", "d"), Summary("Go on a daily quest.")]
        public async Task DoDailyQuest()
        {
            if(Context.Channel.Name != "daily-blessings")
            {
                EmbedBuilder Fail = new EmbedBuilder();
                Fail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/546951983379251222/Red-Exclamation.png");
                Fail.WithAuthor("Daily Blessing Failed");
                Fail.WithDescription("You may not claim your daily blessing here.");
                Fail.WithColor(40, 200, 150);
                Fail.Color = Color.Red;
                await Context.Channel.SendMessageAsync("", false, Fail.Build());
                return;
            }

            int date = Context.Message.Timestamp.Day;

            if (Data.Data.GetLastDaily(Context.User.Id) == date)
            {
                EmbedBuilder Fail = new EmbedBuilder();
                Fail.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/546951983379251222/Red-Exclamation.png");
                Fail.WithAuthor("Daily Blessing Failed");
                Fail.WithDescription("You may only claim your daily blessing once per day!");
                Fail.WithColor(40, 200, 150);
                Fail.Color = Color.Red;
                Fail.WithFooter("A new daily blessing will be available at 4:00 PM PST.");
                await Context.Channel.SendMessageAsync("", false, Fail.Build());
                return;
            }

            int coins = rng.Next(250, 300 + (int)Math.Floor(Data.Data.GetData_GoldAmount(Context.User.Id) * 0.3));
            int gems = rng.Next(10, 100);

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Daily Blessing");
            string godTitle = "";
            int quest = rng.Next(1, 21);

            //Replace with a switch when you are less lazy
            if (quest == 1)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202417024958475/Chapal_God_Fists_transparent.webp");
                godTitle = "Dionysus, God of Wine";
            }

            if (quest == 2)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202410053894144/Yayoi_Princess_of_the_Water_God_transparent.webp");
                godTitle = "Neptunia, God of the Sea";
            }

            if (quest == 3)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202469004836874/Ignite_Dragon_of_Hellflames_transparent.webp");
                godTitle = "Ignite, God of Flames";
            }

            if (quest == 4)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202473371369473/Riner_God_of_the_Battlefield_transparent.webp");
                godTitle = "Riner, God of the Battlefield";
            }

            if (quest == 5)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202470472843265/Tetulia_Spirit_of_Harmony_transparent.webp");
                godTitle = "Tetulia, God of the Spirits";
            }

            if (quest == 6)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202431302500353/Claris_Goddess_of_the_Moon_transparent.webp");
                godTitle = "Lunas, God of the Moon";
            }

            if (quest == 7)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202436058710066/Claris_Luna_Goddess_of_the_Full_Moon_transparent.webp");
                godTitle = "Solas, God of the Sun";
            }

            if (quest == 8)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202427938537492/Irena_Goddess_of_Time_transparent.webp");
                godTitle = "Irena, God of Time";
            }

            if (quest == 9)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202481260855308/Barbra_Gods_Loyal_Shield_transparent.webp");
                godTitle = "Barbra, God of the Shield";
            }

            if (quest == 10)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202486742679552/Aura_Controller_of_Origin_transparent.webp");
                godTitle = "Aura, God of Balance";
            }

            if (quest == 11)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202487942119447/Zach_Dragon_Berserker_transparent.webp");
                godTitle = "Zack, God of Rage";
            }

            if (quest == 12)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202501250646026/Sid_Necromancer_transparent.webp");
                godTitle = "Sid, God of the Afterlife";
            }

            if (quest == 13)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202503540998145/Artemis_The_Priestess_transparent.webp");
                godTitle = "Artemis, God of Beasts";
            }

            if (quest == 14)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202512558751756/Bahamut_King_of_Dragon_transparent.webp");
                godTitle = "Bahamut, God of Dragons";
            }

            if (quest == 15)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202514861293583/Eve_The_Apple_Merchant_transparent.webp");
                godTitle = "Eve, God of Food";
            }

            if (quest == 16)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202517864415242/Shakti_Goddess_of_Beauty_transparent.webp");
                godTitle = "Lotus, God of Beauty";
            }

            if (quest == 17)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202530980134952/Infernagg_Evil_Obsidian_Dragon_God_transparent.webp");
                godTitle = "Infernagg, God of Power";
            }

            if (quest == 18)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202701210025993/Zygmunt_Baur_Dreams_and_Illusions_transparent.webp");
                godTitle = "Zygmunt, God of Illusions";
            }

            if (quest == 19)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202579172556820/Shadow_Servant_The_God_of_Death_transparent.webp");
                godTitle = "Kenthernul, God of Death";
            }

            if (quest >= 20)
            {
                Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/547202533567889415/Ingrit_Sacred_Hunter_transparent.webp");
                godTitle = "Ingrit, God of the Hunt";
            }

            uint candies = (uint)rng.Next(1, 15);

            Embed.WithDescription(godTitle + " grants you " + coins + " Gold Coins and " + gems + "<:GuildGem:545341213004529725> as a blessing!" +
                /* Event addition */ "\nAdditionally you are blessed with " + candies + "<:Candy:637578758986924035>!");
            Embed.WithColor(40, 200, 150);
            Embed.Color = Color.Gold;
            Embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            Embed.WithFooter("A new daily blessing will be available at 4:00 PM PST.");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
            await Data.Data.SaveData(Context.User.Id, (uint)coins, 0, "", 0, 0, 0, 0, 0);
            await Data.Data.SaveEventData(Context.User.Id, 0, candies, (uint)gems);
            await Data.Data.SetDailyClaimed(Context.User.Id, date);
        }
    }
}