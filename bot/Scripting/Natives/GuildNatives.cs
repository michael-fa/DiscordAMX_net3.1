using AMXWrapper;
using DSharpPlus.Entities;
using System;

namespace dcamx.Scripting.Natives
{
    class GuildNatives
    {
        public static int DC_GetGuildName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                AMX.SetString(args1[1].AsCellPtr(), guild.Name, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetGuildName' (dest_string must be a array, or invalid parameters!)", caller_script);
            }
            return 1;
        }

        public static int DC_GetGuildCount(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            return Program.m_ScriptGuilds.Count;
        }

        public static int DC_GetMemberCount(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            DiscordGuild guild = null;
            try
            {
                guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberCount' (Invalid guildid?)", caller_script);
            }
            if (guild != null) return guild.MemberCount;
            else return 0;
        }
    }
}
