using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;

namespace RPG_Bot.Resources
{
    public class Spell
    {
        string spellName { get; set; }
        string spellRarity { get; set; }
        string spellType { get; set; }
        string classExclusive { get; set; }
        float spellDamageMin { get; set; }
        float spellDamageMax { get; set; }
        float critChance { get; set; }
        float modifierID { get; set; }
        float spellHealMin { get; set; }
        float spellHealMax { get; set; }
        float manaCost { get; set; }
        float staminaCost { get; set; }
        float castChance { get; set; }
        string spellProperty { get; set; }
        string spell_element { get; set; }
        float spellTurns { get; set; }
        string spellDescription { get; set; }
        string spellIcon { get; set; }
        bool allowedInPvP { get; set; }
        bool allowedOnWorldBosses { get; set; }

        public Spell()
        {
            spellName = "Null";
            spellRarity = "Common";
            classExclusive = "NA";
            spellType = "NA";
            spellDamageMin = 0;
            spellDamageMax = 0;
            critChance = 0;
            modifierID = 0;
            spellHealMin = 0;
            spellHealMax = 0;
            manaCost = 0;
            staminaCost = 0;
            castChance = 0;
            spellProperty = "NA";
            spell_element = "NA";
            spellTurns = 0;
            spellDescription = "NULL";
            spellIcon = "https://wiki.p-insurgence.com/images/0/09/722.png";
            allowedInPvP = false;
            allowedOnWorldBosses = false;
        }

        public Spell(string name, string rarity, string type, string class_exclusive, float damage_min, float damage_max, float crit, float modID, float heal_min, float heal_max, float mana_cost,
              float stamina_cost, float cast_chance, string spell_type, string element, float spell_turns, string spell_description, string spell_icon, bool pvp, bool world_bosses)
        {
            spellName = name;
            spellRarity = rarity;
            spellType = type;
            classExclusive = class_exclusive;
            spellDamageMin = damage_min;
            spellDamageMax = damage_max;
            critChance = crit;
            modifierID = modID;
            spellHealMin = heal_min;
            spellHealMax = heal_max;
            manaCost = mana_cost;
            staminaCost = stamina_cost;
            castChance = cast_chance;
            spellProperty = spell_type;
            spell_element = element;
            spellTurns = spell_turns;
            spellDescription = spell_description;
            spellIcon = spell_icon;
            allowedInPvP = pvp;
            allowedOnWorldBosses = world_bosses;
        }
    }
}