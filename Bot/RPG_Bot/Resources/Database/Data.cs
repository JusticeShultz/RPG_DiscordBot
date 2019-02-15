using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RPG_Bot.Resources.Database;

namespace RPG_Bot.Data
{
    public static class Data
    {
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

        public static async Task UpdateTime(ulong UserID, int Hour, int Minute, int Second)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var query = DbContext.Data.Where(x => x.UserID == UserID);

                if (query == null || query.Count() < 1) { return; }

                UserData Current = DbContext.Data.Where(x => x.UserID == UserID).FirstOrDefault();
                Current.Hour = Hour; Current.Minute = Minute; Current.Second = Second;
                DbContext.Data.Update(Current);

                await DbContext.SaveChangesAsync();
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
    }
}
