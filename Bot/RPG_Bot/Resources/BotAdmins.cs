using System;
using System.Collections.Generic;
using System.Text;

namespace RPG_Bot.Resources
{
    class BotAdmins
    {
        ////
        /// Bot admins are people given the permission to alter game data. This is not the same as a server admin, this is a game master.
        ////

        public static ulong[] users =
        {
            228344819422855168, // Justice
            333407605370126346 // Xan
        };

        public static bool HasPermission(ulong userId)
        {
            bool hasPerms = false;

            foreach (ulong user in users)
            {
                if(userId == user)
                {
                    hasPerms = true;
                    break;
                }
            }

            return hasPerms;
        }
    }
}