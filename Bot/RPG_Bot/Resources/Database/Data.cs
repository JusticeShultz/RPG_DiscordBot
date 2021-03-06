﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RPG_Bot.Resources.Database;

namespace RPG_Bot.Data
{
    public static class Data
    {
        #region [GETTERS]
        public static uint GetData_GoldAmount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.GoldAmount).FirstOrDefault();
            }
        }

        public static string GetData_Name(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return "Sorry but you need to create an account!";

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Name).FirstOrDefault();
            }
        }

        public static uint GetData_Age(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Age).FirstOrDefault();
            }
        }

        public static uint GetData_Damage(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Damage).FirstOrDefault();
            }
        }

        public static uint GetData_Health(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Health).FirstOrDefault();
            }
        }

        public static uint GetData_Level(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Level).FirstOrDefault();
            }
        }

        public static uint GetData_XP(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.XP).FirstOrDefault();
            }
        }

        public static uint GetData_CurrentHealth(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.CurrentHealth).FirstOrDefault();
            }
        }

        public static uint GetData_Event1(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.EventItem1).FirstOrDefault();
            }
        }

        public static uint GetData_Event2(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.EventItem2).FirstOrDefault();
            }
        }

        public static uint GetData_Event3(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.EventItem3).FirstOrDefault();
            }
        }

        public static int GetData_Hour(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return 0;
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Hour).FirstOrDefault();
            }
        }

        public static int GetData_Minute(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return 0;
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Minute).FirstOrDefault();
            }
        }

        public static int GetData_Second(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return 0;
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Second).FirstOrDefault();
            }
        }

        public static int GetData_Day(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return 0;
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Day).FirstOrDefault();
            }
        }

        public static int GetLastDaily(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return 0;
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.DailyClaimed).FirstOrDefault();
            }
        }

        public static string GetRank(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return "No Rank";
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Rank).FirstOrDefault();
            }
        }

        public static string GetClass(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return "No Class Selected";
                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Class).FirstOrDefault();
            }
        }

        public static RPG_Bot.Resources.Helmet GetHelmet(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Helmet helmet = new Resources.Helmet
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Helmet_Regen).FirstOrDefault()
                    );

                    if (helmet.ItemName == "0")
                        return null;

                    return helmet;
                }
            }
        }

        public static RPG_Bot.Resources.Gauntlets GetGauntlet(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Gauntlets gauntlets = new Resources.Gauntlets
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Gauntlet_Regen).FirstOrDefault()
                    );

                    if (gauntlets.ItemName == "0")
                        return null;

                    return gauntlets;
                }
            }
        }

        public static RPG_Bot.Resources.Chestplate GetChestplate(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Chestplate chestplate = new Resources.Chestplate
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Chestplate_Regen).FirstOrDefault()
                    );

                    if (chestplate.ItemName == "0")
                        return null;

                    return chestplate;
                }
            }
        }

        public static RPG_Bot.Resources.Belt GetBelt(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Belt belt = new Resources.Belt
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Belt_Regen).FirstOrDefault()
                    );

                    if (belt.ItemName == "0")
                        return null;

                    return belt;
                }
            }
        }

        public static RPG_Bot.Resources.Leggings GetLeggings(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Leggings leggings = new Resources.Leggings
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Legging_Regen).FirstOrDefault()
                    );

                    if (leggings.ItemName == "0")
                        return null;

                    return leggings;
                }
            }
        }

        public static RPG_Bot.Resources.Boots GetBoots(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == UserID) == null) return null;
                {
                    Resources.Boots boots = new Resources.Boots
                    (
                        UserID,
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_ModID).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_URL).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Name).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Cost).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Rarity).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Armor).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Health).FirstOrDefault(),
                        DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Boot_Regen).FirstOrDefault()
                    );

                    if (boots.ItemName == "0")
                        return null;

                    return boots;
                }
            }
        }

        public static uint GetData_SkillPoints(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.SkillPoints).FirstOrDefault();
            }
        }

        public static uint GetData_Stamina(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Stamina).FirstOrDefault();
            }
        }

        public static uint GetData_Stability(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Stability).FirstOrDefault();
            }
        }

        public static uint GetData_Dexterity(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Dexterity).FirstOrDefault();
            }
        }

        public static uint GetData_Strength(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Strength).FirstOrDefault();
            }
        }

        public static uint GetData_Luck(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Luck).FirstOrDefault();
            }
        }

        public static uint GetData_Charisma(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Charisma).FirstOrDefault();
            }
        }

        public static uint GetData_SmallPotionCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.SmallPotionCount).FirstOrDefault();
            }
        }

        public static uint GetData_MediumPotionCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.MediumPotionCount).FirstOrDefault();
            }
        }

        public static uint GetData_LargePotionCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.LargePotionCount).FirstOrDefault();
            }
        }

        public static uint GetData_DragonPotionCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.DragonPotionCount).FirstOrDefault();
            }
        }

        public static uint GetData_AngelPotionCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.AngelPotionCount).FirstOrDefault();
            }
        }

        public static uint GetData_CommonBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.CommonBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_UncommonBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.UncommonBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_RareBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.RareBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_VeryRareBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.VeryRareBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_EpicBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.EpicBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_LegendaryBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.LegendaryBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_MythicBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.MythicBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_GodlyBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.GodlyBoxCount).FirstOrDefault();
            }
        }

        public static uint GetData_EventBoxCount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.EventBoxCount).FirstOrDefault();
            }
        }

        public static string GetData_GodChoice(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return "No god";

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.GodChoice).FirstOrDefault();
            }
        }
        #endregion

        #region [SETTERS]
        public static async Task SetXP(ulong UserID, uint XP)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);
                //Console.WriteLine(DbContext.Data.Where(x => x.UserID == xUserID));
                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                        Current.XP = XP;
                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SaveData(ulong UserID, uint GoldAmount, uint Age, string Name, uint Damage, uint Health, uint Level, uint XP, uint CurrentHealth)
        {
            //Console.WriteLine("-!-!-!- [Save task requested] -!-!-!-");
            using (var DbContext = new SqliteDbContext())
            {
              
                var query = DbContext.Data.Where(x => x.UserID == UserID);
                //Console.WriteLine(DbContext.Data.Where(x => x.UserID == xUserID));
                if (query != null && query.Count() < 1)
                {
                    //Console.WriteLine("USER DID NOT EXIST. CREATING NEW USER.");

                    //No user data, create new data.
                    DbContext.Data.Add(new UserData
                    {
                        UserID = UserID,
                        GoldAmount = GoldAmount,
                        Age = Age,
                        Name = Name,
                        Damage = Damage,
                        Health = Health,
                        Level = Level,
                        XP = XP,
                        CurrentHealth = Health,
                        EventItem1 = 0,
                        EventItem2 = 0,
                        EventItem3 = 0
                    });
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                        Current.GoldAmount += GoldAmount;
                        Current.Age += Age;
                        Current.Damage += Damage;
                        Current.Health += Health;
                        Current.Level += Level;

                        int predetermineXP = ((int)Current.XP + (int)XP);
                        if (predetermineXP < 0) Current.XP = 0;
                        else Current.XP += XP;

                        int predetermineHealth = ((int)Current.CurrentHealth + (int)CurrentHealth);
                        if (predetermineHealth < 0) Current.CurrentHealth = 0;
                        else Current.CurrentHealth += CurrentHealth;

                        //Cap health.
                        if (Current.CurrentHealth > Current.Health)
                            Current.CurrentHealth = Current.Health;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SubtractSaveData(ulong UserID, uint GoldAmount, uint Age, string Name, uint Damage, uint Health, uint Level, uint XP, uint CurrentHealth)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1) { return; }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        if((int)Current.GoldAmount - (int)GoldAmount < 0)
                            Current.GoldAmount = 0;
                        else Current.GoldAmount -= GoldAmount;

                        if ((int)Current.Age - (int)Age < 0)
                            Current.Age = 0;
                        else Current.Age -= Age;

                        if ((int)Current.Damage - (int)Damage < 0)
                            Current.Damage = 0;
                        else Current.Damage -= Damage;

                        if ((int)Current.Health - (int)Health < 0)
                            Current.Health = 0;
                        else Current.Health -= Health;

                        if ((int)Current.Level - (int)Level < 0)
                            Current.Level = 0;
                        else Current.Level -= Level;

                        if ((int)Current.XP - (int)XP < 0)
                            Current.XP = 0;
                        else Current.XP -= XP;

                        if ((int)Current.CurrentHealth - (int)CurrentHealth < 0)
                            Current.CurrentHealth = 0;
                        else Current.CurrentHealth -= CurrentHealth;

                        //Cap health.
                        if (Current.CurrentHealth > Current.Health)
                            Current.CurrentHealth = Current.Health;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SubtractSkillPoints(ulong UserID, uint stamina, uint stability, uint dexterity, uint strength, uint luck, uint charisma)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1) { return; }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        if ((int)Current.Stamina - (int)stamina < 0)
                            Current.Stamina = 0;
                        else Current.Stamina -= stamina;

                        if ((int)Current.Stability - (int)stability < 0)
                            Current.Stability = 0;
                        else Current.Stability -= stability;

                        if ((int)Current.Dexterity - (int)dexterity < 0)
                            Current.Dexterity = 0;
                        else Current.Dexterity -= dexterity;

                        if ((int)Current.Strength - (int)strength < 0)
                            Current.Strength = 0;
                        else Current.Strength -= strength;

                        if ((int)Current.Luck - (int)luck < 0)
                            Current.Luck = 0;
                        else Current.Luck -= luck;

                        if ((int)Current.Charisma - (int)charisma < 0)
                            Current.Charisma = 0;
                        else Current.Charisma -= charisma;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddSkillPoints(ulong UserID, uint stamina, uint stability, uint dexterity, uint strength, uint luck, uint charisma)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1) { return; }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Stamina += stamina;

                        Current.Stability += stability;

                        Current.Dexterity += dexterity;

                        Current.Strength += strength;

                        Current.Luck += luck;

                        Current.Charisma += charisma;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SaveEventData(ulong UserID, uint E1, uint E2, uint E3)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1) { return; }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                        Current.EventItem1 += E1;
                        Current.EventItem2 += E2;
                        Current.EventItem3 += E3;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task TakeEventData(ulong UserID, uint E1, uint E2, uint E3)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1) { return; }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        if (((int)Current.EventItem1 - (int)E1) > 0)
                            Current.EventItem1 -= E1;
                        else Current.EventItem1 = 0;

                        if (((int)Current.EventItem2 - (int)E2) > 0)
                            Current.EventItem2 -= E2;
                        else Current.EventItem2 = 0;

                        if (((int)Current.EventItem3 - (int)E3) > 0)
                            Current.EventItem3 -= E3;
                        else Current.EventItem3 = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task UpdateTime(ulong UserID, int Hour, int Minute, int Second, int Day)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query == null || query.Count() < 1) { return; }

                UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                Current.Hour = Hour; Current.Minute = Minute; Current.Second = Second; Current.Day = Day;
                DbContext.Data.Update(Current);

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetDailyClaimed(ulong UserID, int Date)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);
                if (query == null || query.Count() < 1) { return; }
                UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                Current.DailyClaimed = Date;
                DbContext.Data.Update(Current);
                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetRank(ulong UserID, string SetRank)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);
                if (query == null || query.Count() < 1) { return; }
                UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                Current.Rank = SetRank;
                DbContext.Data.Update(Current);
                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetClass(ulong UserID, string SetClass)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);
                if (query == null || query.Count() < 1) { return; }
                UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                Current.Class = SetClass;
                DbContext.Data.Update(Current);
                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetHelmet(ulong UserID, Resources.Helmet helmet)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Helmet_ModID = helmet.ItemID;
                        Current.Helmet_Name = helmet.ItemName;
                        Current.Helmet_URL = helmet.WebURL;
                        Current.Helmet_Rarity = helmet.ItemRarity;
                        Current.Helmet_Armor = helmet.Armor;
                        Current.Helmet_Health = helmet.Health;
                        Current.Helmet_Regen = helmet.HealthGainOnDamage;
                        Current.Helmet_Cost = helmet.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteHelmet(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Helmet_ModID = 0;
                        Current.Helmet_Name = "0";
                        Current.Helmet_URL = "0";
                        Current.Helmet_Rarity = "0";
                        Current.Helmet_Armor = 0;
                        Current.Helmet_Health = 0;
                        Current.Helmet_Regen = 0;
                        Current.Helmet_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetChestplate(ulong UserID, Resources.Chestplate chestplate)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Chestplate_ModID = chestplate.ItemID;
                        Current.Chestplate_Name = chestplate.ItemName;
                        Current.Chestplate_URL = chestplate.WebURL;
                        Current.Chestplate_Rarity = chestplate.ItemRarity;
                        Current.Chestplate_Armor = chestplate.Armor;
                        Current.Chestplate_Health = chestplate.Health;
                        Current.Chestplate_Regen = chestplate.HealthGainOnDamage;
                        Current.Chestplate_Cost = chestplate.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteChestplate(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Chestplate_ModID = 0;
                        Current.Chestplate_Name = "0";
                        Current.Chestplate_URL = "0";
                        Current.Chestplate_Rarity = "0";
                        Current.Chestplate_Armor = 0;
                        Current.Chestplate_Health = 0;
                        Current.Chestplate_Regen = 0;
                        Current.Chestplate_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetGauntlets(ulong UserID, Resources.Gauntlets gauntlets)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Gauntlet_ModID = gauntlets.ItemID;
                        Current.Gauntlet_Name = gauntlets.ItemName;
                        Current.Gauntlet_URL = gauntlets.WebURL;
                        Current.Gauntlet_Rarity = gauntlets.ItemRarity;
                        Current.Gauntlet_Armor = gauntlets.Armor;
                        Current.Gauntlet_Health = gauntlets.Health;
                        Current.Gauntlet_Regen = gauntlets.HealthGainOnDamage;
                        Current.Gauntlet_Cost = gauntlets.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteGauntlets(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Gauntlet_ModID = 0;
                        Current.Gauntlet_Name = "0";
                        Current.Gauntlet_URL = "0";
                        Current.Gauntlet_Rarity = "0";
                        Current.Gauntlet_Armor = 0;
                        Current.Gauntlet_Health = 0;
                        Current.Gauntlet_Regen = 0;
                        Current.Gauntlet_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetBelt(ulong UserID, Resources.Belt belt)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Belt_ModID = belt.ItemID;
                        Current.Belt_Name = belt.ItemName;
                        Current.Belt_URL = belt.WebURL;
                        Current.Belt_Rarity = belt.ItemRarity;
                        Current.Belt_Armor = belt.Armor;
                        Current.Belt_Health = belt.Health;
                        Current.Belt_Regen = belt.HealthGainOnDamage;
                        Current.Belt_Cost = belt.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteBelt(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Belt_ModID = 0;
                        Current.Belt_Name = "0";
                        Current.Belt_URL = "0";
                        Current.Belt_Rarity = "0";
                        Current.Belt_Armor = 0;
                        Current.Belt_Health = 0;
                        Current.Belt_Regen = 0;
                        Current.Belt_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetLeggings(ulong UserID, Resources.Leggings leggings)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Legging_ModID = leggings.ItemID;
                        Current.Legging_Name = leggings.ItemName;
                        Current.Legging_URL = leggings.WebURL;
                        Current.Legging_Rarity = leggings.ItemRarity;
                        Current.Legging_Armor = leggings.Armor;
                        Current.Legging_Health = leggings.Health;
                        Current.Legging_Regen = leggings.HealthGainOnDamage;
                        Current.Legging_Cost = leggings.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteLeggings(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Legging_ModID = 0;
                        Current.Legging_Name = "0";
                        Current.Legging_URL = "0";
                        Current.Legging_Rarity = "0";
                        Current.Legging_Armor = 0;
                        Current.Legging_Health = 0;
                        Current.Legging_Regen = 0;
                        Current.Legging_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SeBoots(ulong UserID, Resources.Boots boots)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Boot_ModID = boots.ItemID;
                        Current.Boot_Name = boots.ItemName;
                        Current.Boot_URL = boots.WebURL;
                        Current.Boot_Rarity = boots.ItemRarity;
                        Current.Boot_Armor = boots.Armor;
                        Current.Boot_Health = boots.Health;
                        Current.Boot_Regen = boots.HealthGainOnDamage;
                        Current.Boot_Cost = boots.ItemCost;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task DeleteBoots(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Boot_ModID = 0;
                        Current.Boot_Name = "0";
                        Current.Boot_URL = "0";
                        Current.Boot_Rarity = "0";
                        Current.Boot_Armor = 0;
                        Current.Boot_Health = 0;
                        Current.Boot_Regen = 0;
                        Current.Boot_Cost = 0;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddSkillPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.SkillPoints = Current.SkillPoints + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetSkillPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.SkillPoints = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddStaminaPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Stamina = Current.Stamina + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetStaminaPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Stamina = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddStabilityPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Stability = Current.Stability + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetStabilityPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Stability = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddDexterityPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Dexterity = Current.Dexterity + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetDexterityPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Dexterity = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddStrengthPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Strength = Current.Strength + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetStrengthPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Strength = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddLuckPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Luck = Current.Luck + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetLuckPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Luck = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddCharismaPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Charisma = Current.Charisma + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetCharismaPoints(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.Charisma = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddSmallPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.SmallPotionCount = Current.SmallPotionCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetSmallPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.SmallPotionCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddMediumPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.MediumPotionCount = Current.MediumPotionCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetMediumPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.MediumPotionCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddLargePotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LargePotionCount = Current.LargePotionCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetLargePotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LargePotionCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddDragonPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.DragonPotionCount = Current.DragonPotionCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetDragonPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.DragonPotionCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddAngelPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.AngelPotionCount = Current.AngelPotionCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetAngelPotionCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.AngelPotionCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddCommonBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.CommonBoxCount = Current.CommonBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetCommonBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.CommonBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddUncommonBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.UncommonBoxCount = Current.UncommonBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetUncommonBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.UncommonBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddRareBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.RareBoxCount = Current.RareBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetRareBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.RareBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddVeryRareBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.VeryRareBoxCount = Current.VeryRareBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetVeryRareBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.VeryRareBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddEpicBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.EpicBoxCount = Current.EpicBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetEpicBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.EpicBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddLegendaryBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LegendaryBoxCount = Current.LegendaryBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetLegendaryBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LegendaryBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddMythicBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.MythicBoxCount = Current.MythicBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetMythicBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.MythicBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddGodlyBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.GodlyBoxCount = Current.GodlyBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetGodlyBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.GodlyBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddEventBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.EventBoxCount = Current.EventBoxCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetEventBoxCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.EventBoxCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddAmountDonated(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.AmountDonated = Current.AmountDonated + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetAmountDonated(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.AmountDonated = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddWinCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.WinCount = Current.WinCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetWinCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.WinCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task AddLoseCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LoseCount = Current.LoseCount + amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetLoseCount(ulong UserID, uint amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.LoseCount = amount;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SetGod(ulong UserID, string god)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        Current.GodChoice = god;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region [DELETER]
        public static void DeleteSave(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return;
                else DbContext.Data.Remove(DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault());
            }
        }
        #endregion

        #region EXPLORE
        public static bool Explored(ulong UserID, string RegionName)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID) == null)
                    return false;

                if (RegionName == "Training_Forts")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Training_Forts).FirstOrDefault() == 1;
                else if (RegionName == "Rayels_Terror_Tower")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Rayels_Terror_Tower).FirstOrDefault() == 1;
                else if (RegionName == "Skeleton_Caves")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Skeleton_Caves).FirstOrDefault() == 1;
                else if (RegionName == "Forest_Path")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Forest_Path).FirstOrDefault() == 1;
                else if (RegionName == "Goblin_Outpost")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Goblin_Outpost).FirstOrDefault() == 1;
                else if (RegionName == "Tomb_of_Heroes")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Tomb_of_Heroes).FirstOrDefault() == 1;
                else if (RegionName == "Lair_of_Trenthola")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Lair_of_Trenthola).FirstOrDefault() == 1;
                else if (RegionName == "Snowy_Woods")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Snowy_Woods).FirstOrDefault() == 1;
                else if (RegionName == "Makkosi_Camp")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Makkosi_Camp).FirstOrDefault() == 1;
                else if (RegionName == "Frigid_Wastes")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Frigid_Wastes).FirstOrDefault() == 1;
                else if (RegionName == "Abandoned_Village")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Abandoned_Village).FirstOrDefault() == 1;
                else if (RegionName == "Frozen_Peaks")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Frozen_Peaks).FirstOrDefault() == 1;
                else if (RegionName == "Cliffside_Barricade")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Cliffside_Barricade).FirstOrDefault() == 1;
                else if (RegionName == "Roosting_Crags")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Roosting_Crags).FirstOrDefault() == 1;
                else if (RegionName == "Taken_Temple")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Taken_Temple).FirstOrDefault() == 1;
                else if (RegionName == "Undying_Storm")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Undying_Storm).FirstOrDefault() == 1;
                else if (RegionName == "Crawling_Coastline")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Crawling_Coastline).FirstOrDefault() == 1;
                else if (RegionName == "Megaton_Cove")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Megaton_Cove).FirstOrDefault() == 1;
                else if (RegionName == "Lunar_Temple")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Lunar_Temple).FirstOrDefault() == 1;
                else if (RegionName == "Hms_Reliant")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Hms_Reliant).FirstOrDefault() == 1;
                else if (RegionName == "Logada_Summits")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Logada_Summits).FirstOrDefault() == 1;
                else if (RegionName == "Burning_Outcrop")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Burning_Outcrop).FirstOrDefault() == 1;
                else if (RegionName == "Magma_Pits")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Magma_Pits).FirstOrDefault() == 1;
                else if (RegionName == "Draconic_Nests")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Draconic_Nests).FirstOrDefault() == 1;
                else if (RegionName == "Twisted_Riverbed")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Twisted_Riverbed).FirstOrDefault() == 1;
                else if (RegionName == "Quarantined_Village")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Quarantined_Village).FirstOrDefault() == 1;
                else if (RegionName == "The_Breach")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_The_Breach).FirstOrDefault() == 1;
                else if (RegionName == "Within_the_Breach")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Within_the_Breach).FirstOrDefault() == 1;
                else if (RegionName == "Outer_Blockade")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Outer_Blockade).FirstOrDefault() == 1;
                else if (RegionName == "Shattered_Streets")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Shattered_Streets).FirstOrDefault() == 1;
                else if (RegionName == "Catacombs")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Catacombs).FirstOrDefault() == 1;
                else if (RegionName == "Corrupt_Citadel")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Corrupt_Citadel).FirstOrDefault() == 1;
                else if (RegionName == "Ruined_Outpost")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Ruined_Outpost).FirstOrDefault() == 1;
                else if (RegionName == "Sombris_Monument")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Sombris_Monument).FirstOrDefault() == 1;
                else if (RegionName == "End_of_Asteria")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_End_of_Asteria).FirstOrDefault() == 1;
                else if (RegionName == "Borealis_Gates")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Borealis_Gates).FirstOrDefault() == 1;
                else if (RegionName == "Stellar_Fields")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Stellar_Fields).FirstOrDefault() == 1;
                else if (RegionName == "Astral_Reaches")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Astral_Reaches).FirstOrDefault() == 1;
                else if (RegionName == "Shifted_Wastes")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Shifted_Wastes).FirstOrDefault() == 1;
                else if (RegionName == "Fell_Pantheon")
                    return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Explored_Fell_Pantheon).FirstOrDefault() == 1;
                else
                {
                    Console.WriteLine("Error loading result of regions exploration: " + RegionName);
                    return false;
                }
            }
        }

        public static async Task Explore(ulong UserID, string RegionName)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query != null && query.Count() < 1)
                {
                }
                else
                {
                    if (query != null)
                    {
                        UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();

                        if (RegionName == "Training_Forts")
                            Current.Explored_Training_Forts = 1;
                        else if (RegionName == "Rayels_Terror_Tower")
                            Current.Explored_Rayels_Terror_Tower = 1;
                        else if (RegionName == "Skeleton_Caves")
                            Current.Explored_Skeleton_Caves = 1;
                        else if (RegionName == "Forest_Path")
                            Current.Explored_Forest_Path = 1;
                        else if (RegionName == "Goblin_Outpost")
                            Current.Explored_Goblin_Outpost = 1;
                        else if (RegionName == "Tomb_of_Heroes")
                            Current.Explored_Tomb_of_Heroes = 1;
                        else if (RegionName == "Lair_of_Trenthola")
                            Current.Explored_Lair_of_Trenthola = 1;
                        else if (RegionName == "Snowy_Woods")
                            Current.Explored_Snowy_Woods = 1;
                        else if (RegionName == "Makkosi_Camp")
                            Current.Explored_Makkosi_Camp = 1;
                        else if (RegionName == "Frigid_Wastes")
                            Current.Explored_Frigid_Wastes = 1;
                        else if (RegionName == "Abandoned_Village")
                            Current.Explored_Abandoned_Village = 1;
                        else if (RegionName == "Frozen_Peaks")
                            Current.Explored_Frozen_Peaks = 1;
                        else if (RegionName == "Cliffside_Barricade")
                            Current.Explored_Cliffside_Barricade = 1;
                        else if (RegionName == "Roosting_Crags")
                            Current.Explored_Roosting_Crags = 1;
                        else if (RegionName == "Taken_Temple")
                            Current.Explored_Taken_Temple = 1;
                        else if (RegionName == "Undying_Storm")
                            Current.Explored_Undying_Storm = 1;
                        else if (RegionName == "Crawling_Coastline")
                            Current.Explored_Crawling_Coastline = 1;
                        else if (RegionName == "Megaton_Cove")
                            Current.Explored_Megaton_Cove = 1;
                        else if (RegionName == "Lunar_Temple")
                            Current.Explored_Lunar_Temple = 1;
                        else if (RegionName == "Hms_Reliant")
                            Current.Explored_Hms_Reliant = 1;
                        else if (RegionName == "Logada_Summits")
                            Current.Explored_Logada_Summits = 1;
                        else if (RegionName == "Burning_Outcrop")
                            Current.Explored_Burning_Outcrop = 1;
                        else if (RegionName == "Magma_Pits")
                            Current.Explored_Magma_Pits = 1;
                        else if (RegionName == "Draconic_Nests")
                            Current.Explored_Draconic_Nests = 1;
                        else if (RegionName == "Twisted_Riverbed")
                            Current.Explored_Twisted_Riverbed = 1;
                        else if (RegionName == "Quarantined_Village")
                            Current.Explored_Quarantined_Village = 1;
                        else if (RegionName == "The_Breach")
                            Current.Explored_The_Breach = 1;
                        else if (RegionName == "Within_the_Breach")
                            Current.Explored_Within_the_Breach = 1;
                        else if (RegionName == "Outer_Blockade")
                            Current.Explored_Outer_Blockade = 1;
                        else if (RegionName == "Shattered_Streets")
                            Current.Explored_Shattered_Streets = 1;
                        else if (RegionName == "Catacombs")
                            Current.Explored_Catacombs = 1;
                        else if (RegionName == "Corrupt_Citadel")
                            Current.Explored_Corrupt_Citadel = 1;
                        else if (RegionName == "Ruined_Outpost")
                            Current.Explored_Ruined_Outpost = 1;
                        else if (RegionName == "Sombris_Monument")
                            Current.Explored_Sombris_Monument = 1;
                        else if (RegionName == "End_of_Asteria")
                            Current.Explored_End_of_Asteria = 1;
                        else if (RegionName == "Borealis_Gates")
                            Current.Explored_Borealis_Gates = 1;
                        else if (RegionName == "Stellar_Fields")
                            Current.Explored_Stellar_Fields = 1;
                        else if (RegionName == "Astral_Reaches")
                            Current.Explored_Astral_Reaches = 1;
                        else if (RegionName == "Shifted_Wastes")
                            Current.Explored_Shifted_Wastes = 1;
                        else if (RegionName == "Fell_Pantheon")
                            Current.Explored_Fell_Pantheon = 1;

                        DbContext.Data.Update(Current);
                    }
                }

                await DbContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}