using System.ComponentModel.DataAnnotations;

namespace RPG_Bot.Resources.Database
{
    public class UserData
    {
        [Key]
        public ulong UserID { get; set; }
        public uint GoldAmount { get; set; }
        public uint Age { get; set; }
        public string Name { get; set; }
        public uint Damage { get; set; }
        public uint Health { get; set; }
        public uint Level { get; set; }

        /*public string Weapon_Name { get; set; }
        public int Weapon_ItemID { get; set; }
        public string Weapon_Rarity { get; set; }
        public int Weapon_Damage { get; set; }*/
    }
}
