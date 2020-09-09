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
using RPG_Bot.Resources;
using static RPG_Bot.Emojis.Emojis;

namespace RPG_Bot.Commands
{
    public class GettingStarted : ModuleBase<SocketCommandContext>
    {
        [Command("begin"), Alias("Begin", "BEGIN", "start", "Start"), Summary("Get started using the bot! Use the command as -begin [Class] [Name] [Age]")]
        public async Task Begin([Remainder]string Input = "None")
        {
            string[] input = Input.Split();

            var vuser = Context.User as SocketGuildUser;

            #region Deprecated ID system
            /*var bronze = Context.Guild.GetRole(EnemyTemplates.Bronze);
            var silver = Context.Guild.GetRole(EnemyTemplates.Silver);
            var gold = Context.Guild.GetRole(EnemyTemplates.Gold);
            var quartz = Context.Guild.GetRole(EnemyTemplates.Quartz);
            var orichalcum = Context.Guild.GetRole(EnemyTemplates.Orichalcum);
            var platinum = Context.Guild.GetRole(EnemyTemplates.Platinum);
            var masterIII = Context.Guild.GetRole(EnemyTemplates.MasterIII);
            var masterII = Context.Guild.GetRole(EnemyTemplates.MasterII);
            var masterI = Context.Guild.GetRole(EnemyTemplates.MasterI);

            if (vuser.Roles.Contains(bronze) || vuser.Roles.Contains(silver) || vuser.Roles.Contains(quartz) || vuser.Roles.Contains(orichalcum) ||
                vuser.Roles.Contains(platinum) || vuser.Roles.Contains(gold) || vuser.Roles.Contains(masterIII) || vuser.Roles.Contains(masterII) ||
                vuser.Roles.Contains(masterI))*/
            #endregion

            string UsersRank = Data.Data.GetRank(Context.User.Id);

            if (UsersRank == "Bronze" || UsersRank == "Silver" || UsersRank == "Quartz" || UsersRank == "Orichalcum" ||
                UsersRank == "Platinum" || UsersRank == "Gold" || UsersRank == "Master1" || UsersRank == "Master2" ||
                UsersRank == "Master3")
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

                #region Check entered class
                if (uint.TryParse(input[2], out x) && (input[0] == "Archer" || input[0] == "Knight" ||
                    input[0] == "Rogue" || input[0] == "Trickster" || input[0] == "trickster" ||
                    input[0] == "Wizard" || input[0] == "Witch" || input[0] == "Assassin" ||
                    input[0] == "archer" || input[0] == "knight" || input[0] == "rogue" ||
                    input[0] == "wizard" || input[0] == "witch" || input[0] == "assassin" ||
                    input[0] == "Swordsman" || input[0] == "swordsman") || input[0] == "Paladin" ||
                    input[0] == "paladin" || input[0] == "Necromancer" || input[0] == "Necromancer" ||
                    input[0] == "Monk" || input[0] == "monk" || input[0] == "Kitsune" || input[0] == "kitsune" ||
                    input[0] == "Evangel" || input[0] == "evangel" || input[0] == "Berserker" || input[0] == "berserker" ||
                    input[0] == "Tamer" || input[0] == "tamer" || input[0] == "Berzerker" || input[0] == "berzerker")
                {
                    #endregion
                    #region Give bronze
                    x = uint.Parse(input[2]);
                    if (x < 1) x = 1;

                    EmbedBuilder Embed = new EmbedBuilder();
                    var user = Context.User;
                    //Give bronze.

                    //Deprecated
                    //var role = Context.Guild.GetRole(542123825370890250);
                    //await (user as IGuildUser).AddRoleAsync(role);
                    #endregion

                    if (input[0] == "Archer" || input[0] == "archer")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760748961797/Archer.png");
                        //var role2 = Context.Guild.GetRole(542217745690001409);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 50, 20, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Archer");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 2);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 3);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Knight" || input[0] == "knight")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767107526676/Knight.png");
                        //var role2 = Context.Guild.GetRole(542217921246658610);
                        //Console.WriteLine("\n[THIS HAPPENS BEFORE ADDROLEASYNC]\n");
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        //Console.WriteLine("\n[THIS HAPPENS BEFORE SAVE]\n");
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 20, 50, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Knight");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 2);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 2);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                        //Console.WriteLine("\n[THIS HAPPENS AFTER SAVE]\n");
                    }
                    else if (input[0] == "Witch" || input[0] == "witch")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767770095619/Witch.png");
                        //var role2 = Context.Guild.GetRole(542217744805003264);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 500, x, input[1], 45, 25, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Witch");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 1);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 2);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Rogue" || input[0] == "rogue")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225767220641792/Rogue.png");
                        //var role2 = Context.Guild.GetRole(542217746440519681);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 600, x, input[1], 60, 10, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Rogue");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 3);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Wizard" || input[0] == "wizard")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225769422913537/Wizard.png");
                        //var role2 = Context.Guild.GetRole(542217748009451551);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 48, 22, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Wizard");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 1);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 2);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Assassin" || input[0] == "assassin")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/542225760619069450/Assassin.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 65, 5, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Assassin");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 4);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Trickster" || input[0] == "trickster")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904660275757066/Trickster.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 55, 15, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Trickster");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 2);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 1);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Swordsman" || input[0] == "swordsman")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904710016139265/Swordsman.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 40, 30, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Swordsman");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 2);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Paladin" || input[0] == "paladin")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904668891119627/Paladin.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 15, 55, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Paladin");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 2);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 1);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Necromancer" || input[0] == "Necromancer")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904652009046026/Nechromancer.webp");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 65, 5, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Necromancer");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 2);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 3);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Monk" || input[0] == "monk")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904672250626048/Monk.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 35, 35, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Monk");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 1);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Kitsune" || input[0] == "kitsune")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904663740252161/Kitsune.webp");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 65, 5, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Kitsune");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 1);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 2);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Evangel" || input[0] == "evangel")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904680668725258/Evangel.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 65, 5, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Evangel");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 1);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 3);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Berserker" || input[0] == "berserker" || input[0] == "Berzerker" || input[0] == "berzerker")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904669427859456/Berserker.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 525, x, input[1], 40, 30, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Berserker");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 1);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 4);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 0);
                        await Data.Data.SetLuckPoints(Context.User.Id, 0);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 0);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }
                    else if (input[0] == "Tamer" || input[0] == "tamer")
                    {
                        Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/566904678713917445/Cat_Tamer.png");
                        //var role2 = Context.Guild.GetRole(542217793601536010);
                        //await (user as IGuildUser).AddRoleAsync(role2);
                        await Data.Data.SaveData(user.Id, 550, x, input[1], 65, 5, 1, 0, 0);
                        await Data.Data.SetClass(Context.User.Id, "Tamer");
                        await Data.Data.DeleteHelmet(Context.User.Id);
                        await Data.Data.DeleteChestplate(Context.User.Id);
                        await Data.Data.DeleteGauntlets(Context.User.Id);
                        await Data.Data.DeleteBelt(Context.User.Id);
                        await Data.Data.DeleteLeggings(Context.User.Id);
                        await Data.Data.DeleteBoots(Context.User.Id);
                        await Data.Data.SetSkillPoints(Context.User.Id, 0);
                        await Data.Data.SetStaminaPoints(Context.User.Id, 0);
                        await Data.Data.SetStabilityPoints(Context.User.Id, 0);
                        await Data.Data.SetDexterityPoints(Context.User.Id, 0);
                        await Data.Data.SetStrengthPoints(Context.User.Id, 1);
                        await Data.Data.SetLuckPoints(Context.User.Id, 1);
                        await Data.Data.SetCharismaPoints(Context.User.Id, 3);
                        await Data.Data.SetSmallPotionCount(Context.User.Id, 5);
                        await Data.Data.SetMediumPotionCount(Context.User.Id, 3);
                        await Data.Data.SetLargePotionCount(Context.User.Id, 2);
                        await Data.Data.SetDragonPotionCount(Context.User.Id, 1);
                        await Data.Data.SetAngelPotionCount(Context.User.Id, 0);
                        await Data.Data.SetCommonBoxCount(Context.User.Id, 2);
                        await Data.Data.SetUncommonBoxCount(Context.User.Id, 0);
                        await Data.Data.SetRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetVeryRareBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEpicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetLegendaryBoxCount(Context.User.Id, 0);
                        await Data.Data.SetMythicBoxCount(Context.User.Id, 0);
                        await Data.Data.SetGodlyBoxCount(Context.User.Id, 0);
                        await Data.Data.SetEventBoxCount(Context.User.Id, 0);
                        await Data.Data.SetWinCount(Context.User.Id, 0);
                        await Data.Data.SetLoseCount(Context.User.Id, 0);
                    }

                    await Data.Data.SetRank(Context.User.Id, "Bronze");
                    await Gameplay.UpdateUserData();
                    #region Embed
                    Embed.WithAuthor("The Guild has accepted your signup form!", Context.User.GetAvatarUrl());
                    Embed.WithColor(40, 200, 150);

                    SocketTextChannel channel = null;

                    foreach (SocketGuildChannel channelsInServer in Context.Guild.Channels)
                    {
                        if(channelsInServer.Name == "entry-exam")
                            channel = channelsInServer as SocketTextChannel;
                    }

                    if (channel == null)
                    {
                        Embed.WithDescription("Greetings " + input[1] + ", and welcome to the final stage of your journey in joining the Asterian Adventurers’ Guild! As you know, only the best and brightest heroes are accepted to join the Guild. Congratulations on making it this far!\n\n" +
                            "As a " + Data.Data.GetClass(Context.User.Id) + " within the Guild, your tasks will be many and varied. You may defend innocent citizens from wicked bandits, escort merchants through haunted forests, or even unravel Asteria’s many secrets! Whatever your mission, we expect excellence " +
                            "from you at every turn, and we in turn will provide excellence for you.\n\nIn order to become a Guild Adventurer, you must pass this final test: " +
                            "Defeating one of our finely - tuned Training Automatons in single combat!\n\n\nIt looks like your server is missing the channel: entry-exam, you will not be able to continue because of this...");
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                    else
                    {
                        Embed.WithDescription("Greetings " + input[1] + ", and welcome to the final stage of your journey in joining the Asterian Adventurers’ Guild! As you know, only the best and brightest heroes are accepted to join the Guild. Congratulations on making it this far!\n\n" +
                            "As a " + Data.Data.GetClass(Context.User.Id) + " within the Guild, your tasks will be many and varied. You may defend innocent citizens from wicked bandits, escort merchants through haunted forests, or even unravel Asteria’s many secrets! Whatever your mission, we expect excellence " +
                            "from you at every turn, and we in turn will provide excellence for you.\n\nIn order to become a Guild Adventurer, you must pass this final test: " +
                            "Defeating one of our finely - tuned Training Automatons in single combat! When you are ready, type - explore in the <#" + channel.Id + "> to begin.");
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());

                        /*EmbedBuilder Linker = new EmbedBuilder();
                        var invite = await channel.CreateInviteAsync(300, null, false, false);
                        Linker.WithTitle("Click here to go to Entry Exams");
                        Linker.WithUrl(invite.Url);
                        Linker.WithColor(Color.Blue);
                        Linker.Color = Color.Red;
                        await Context.Channel.SendMessageAsync("", false, Linker.Build());*/
                    }
                    //New user in the database here:
                    #endregion
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

            Embed.WithAuthor("There are currently 15 classes available and they are: ");
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("");

            Embed.WithDescription(Archer + "Archer - High Damage, Very Vulnerable\n" +
                                 Assassin + "Assassin - Extreme Damage, Very Fragile\n" +
                                 Knight + "Knight - Low Damage, Very Tanky\n" +
                                 Rogue + "Rogue - High Damage, Very Vulnerable\n" +
                                 Witch + "Witch - High Damage, Very Vulnerable\n" +
                                 Wizard + "Wizard - Medium Damage, Somewhat Durable\n" +
                                 Trickster + "Trickster - High Damage, Very Vulnerable\n" +
                                 Swordsman + "Swordsman - Medium Damage, Somewhat Durable\n" +
                                 Paladin + "Paladin - Low Damage, Very Tanky\n" +
                                 Nechromancer + "Necromancer - Extreme Damage, Very Fragile\n" +
                                 Monk + "Monk - Medium Damage, Somewhat Durable\n" +
                                 Kitsune + "Kitsune - Extreme Damage, Very Fragile\n" +
                                 Evangel + "Evangel - Medium Damage, Somewhat Durable\n" +
                                 Berserker + "Berserker - Balanced Damage, Balanced Strength\n" +
                                 Tamer + "Tamer - Extreme Damage, Very Fragile\n\n" +
                                 "Note: Classes each have their own base stats, but after about level 10 the " +
                                 "difference in stats isn't very noticeable and becomes more of a cosmetic." +
                                 "\nYou may switch your class at any time for 500 Gold Coins, see ``-help`` for more info");

            Embed.Color = Color.Gold;

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("Help"), Alias("help", "Helps", "helps"), Summary("Show general help.")]
        public async Task General([Remainder]string Input = null)
        {
            if (Input == "spawn")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Using -Spawn");
                Embed.WithDescription("-Spawn takes no additional text after it. Using this command requires you to be in a channel " +
                    "that allows monsters to spawn. Once a monster spawns you may chose what to do afterwards with what is prompted for you. (`-fight`)");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else if (Input == "gambling" || Input == "gamble")
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Using -Gamble");
                Embed.WithDescription("Gambling allows you to specify a sum of your gold with a chance to double it or lose it all!" +
                    "\n\nTo gamble, do `-Gamble [Amount]` in the tavern!");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("The current avialable commands are: ");
                Embed.WithDescription(
                    "\n**1.)** Help: `-help [Page/Command]`." +
                    "\n**2.)** Begin/Signup: `-begin [Class] [Name] [Age]`." +
                    "\n**3.)** List available classes: `-class` or `-classes`." +
                    "\n**4.)** Spawn an enemy: `-spawn` or `-s` (See `-help spawn` for more details." +
                    "\n**5.)** View your profile: `-profile` or `-p`." +
                    "\n**6.)** View another users profile: `-profile [@user]` or `-p [@user]`." +
                    "\n**7.)** Give another user gold: `-gold give [@user] [amount]`." +
                    "\n**8.)** View how much gold you have: `-profile` or `-gold` or `-gold me`." +
                    "\n**9.)** Check how many users are in the server: `-current`." +
                    "\n**10.)** Check the guilds gold leaderboard: `-leaderboard` or `-lb`." +
                    "\n**11.)** Go on a quest: `-quest` or `-q`." +
                    "\n**12.)** Fight a spawned monster: `-fight` or `-f`. Additionally you may do -f [x] to simulate x turns.." +
                    "\n**13.)** Change your class for 500" + Coins + ": `-SwitchClass` or `-ChangeClass`." +
                    "\n**14.)** Preview the starting stats of a class and the profile image of it: `-info [ClassName]`." +
                    "\n**15.)** Preview your current equipment: `-Equipment [Helmet, Gauntlets, Chestplate, Leggings, Boots]`." +
                    "\n**16.)** Equip a dropped item: `-Equip`." +
                    "\n**17.)** Sell an item you have equipped, or sell a dropped item: `-Sell [Helmet, Gauntlets, Chestplate, Leggings, Boots, Drop]`." +
                    "\n**18.)** See your current lootbox inventory: `-Lootbox`." +
                    "\n**19.)** Open up a lootbox you own: `-Lootbox [Common, Uncommon, Rare, VeryRare, Legendary, Mythic, Godly]`." +
                    "\n**20.)** See the current candy event shop: `-Event CandyShop`." +
                    "\n**21.)** See your current skill points: `-skills`." +
                    "\n**22.)** Upgrade a certain skill with a skill point: `-skills [Stamina, Stability, Dexterity, Strength, Charisma, Luck]`." +
                    "\n\n\n\nServer Admins & Owners:" +
                    "\n1.) Initialize the bot with your server(this will generate about 20 new text channels): ``-StartServer``." +
                    "\n2.) Delete all channels and roles the bot generated upon being started: ``-Cleanup``.\n\n" +
                    "See the full documentation of commands here: https://justiceshultz.github.io/pages/DiscordBotDocs.html");
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                await Context.User.SendMessageAsync("", false, Embed.Build());

                Embed.WithAuthor("Sounds good!");
                Embed.WithDescription("I DM'd you the general help commands " + Context.User.Mention);
                Embed.WithColor(40, 200, 150);
                Embed.Color = Color.Gold;
                var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());

                await Task.Delay(5500);
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
            }
        }

        [Command("Spam"), Alias("spam"), Summary("Spam Ray.")]
        public async Task Spam(IUser User = null, uint Amount = 0)
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            if (user.GuildPermissions.Administrator)
            {
                for (int i = 0; i < Amount; ++i)
                {
                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor(User.Username + " is dumb");
                    Embed.WithDescription(User.Mention);
                    Embed.WithColor(40, 200, 150);
                    Embed.Color = Color.Gold;
                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                }
            }
        }

        [Command("Notice"), Alias("notice"), Summary("Send a notice to the server.")]
        public async Task Announcement([Remainder]string Input = "None")
        {
            SocketGuildUser user = Context.User as SocketGuildUser;

            if (user.GuildPermissions.Administrator)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor(Input);
                Embed.WithColor(40, 200, 150);
                Embed.WithTimestamp(Context.Message.Timestamp);
                //Uncomment this and replace the URL if you want an image to display with the notice.
                //Embed.WithImageUrl("https://cdn.discordapp.com/attachments/542225685695954945/545445513596633108/WorkingState.PNG");
                Embed.Color = Color.Gold;
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }

        [Command("ProfPic"), Alias("profpic", "pp", "PP"), Summary("Get a users profile picture.")]
        public async Task ProfilePic([Remainder]IUser Input = null)
        {
            if (Input == null)
            {
                Input = Context.User;
            }

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(Input.Username);
            Embed.WithTimestamp(Context.Message.Timestamp);
            //Uncomment this and replace the URL if you want an image to display with the notice.
            Embed.WithImageUrl(Input.GetAvatarUrl());
            Embed.Color = Color.Gold;
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("AddRole"), Alias("addrole"), Summary("Role changing.")]
        public async Task RoleChanger1([Remainder] string role)
        {
            foreach (SocketRole guildRole in Context.Guild.Roles)
            {
                role = role.ToLower();

                if (role == "news" && guildRole.Name == "News")
                {
                    await Context.Channel.SendMessageAsync("Added the `News` role to you!");
                    await ((SocketGuildUser)Context.User).AddRoleAsync(guildRole);
                }
            }
        }

        [Command("RemoveRole"), Alias("removerole"), Summary("Role changing.")]
        public async Task RoleChanger2([Remainder] string role)
        {
            foreach (SocketRole guildRole in Context.Guild.Roles)
            {
                role = role.ToLower();

                if (role == "news" && guildRole.Name == "News")
                {
                    await Context.Channel.SendMessageAsync("Removed the `News` role from you!");
                    await ((SocketGuildUser)Context.User).RemoveRoleAsync(guildRole);
                }
            }
        }
    }
}