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
        public static ulong GetData_GoldAmount(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.GoldAmount).FirstOrDefault();
            }
        }

        public static string GetData_Name(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return "owo what's this";

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Name).FirstOrDefault();
            }
        }

        public static ulong GetData_Age(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Age).FirstOrDefault();
            }
        }

        public static ulong GetData_Damage(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Damage).FirstOrDefault();
            }
        }

        public static ulong GetData_Health(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Health).FirstOrDefault();
            }
        }

        public static ulong GetData_Level(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                //No data to get.
                if (DbContext.Data.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;

                return DbContext.Data.Where(x => x.UserID == UserID).Select(x => x.Level).FirstOrDefault();
            }
        }

        public static async Task SaveData(ulong xUserID, uint xGoldAmount, uint xAge, string xName, uint xDamage, uint xHealth, uint xLevel)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Data.Where(x => x.UserID == xUserID).Count() < 1)
                {
                    //No user data, create new data.
                    DbContext.Data.Add(new UserData
                    {
                        //Added the x's due to broken functionality.
                        UserID = xUserID,
                        GoldAmount = xGoldAmount,
                        Age = xAge,
                        Name = xName,
                        Damage = xDamage,
                        Health = xHealth,
                        Level = xLevel
                    });
                }
                else
                {
                    UserData Current = DbContext.Data.Where(x => x.UserID == xUserID).FirstOrDefault();
                    Current.GoldAmount += xGoldAmount;
                    Current.Age += xAge;
                    Current.Damage += xDamage;
                    Current.Health += xHealth;
                    Current.Level += xLevel;

                    DbContext.Data.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        //Seperate to save overwork.
        /*public async void SaveWeaponData(ulong UserID, string Weapon_Name, int Weapon_ItemID, string Weapon_Rarity, int Weapon_Damage)
        {

        }*/
    }
}
