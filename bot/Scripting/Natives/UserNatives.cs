using AMXWrapper;
using DSharpPlus.Entities;
using System;

namespace dcamx.Scripting.Natives
{
    public static class UserNatives
    {
        public static int DC_GetMemberName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            try
            {
                AMX.SetString(args1[2].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).Username, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberName' (dest_string must be a array, or invalid parameters!!)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_GetMemberDisplayName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            try
            {
                AMX.SetString(args1[2].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).DisplayName, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberDisplayName' (dest_string must be a array, or invalid parameters!!)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_GetMemberDiscriminator(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                AMX.SetString(args1[2].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).Discriminator, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberDiscriminator' (dest_string must be a array, or invalid parameters!)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_GetGuildMemberID(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            try
            {
                AMX.SetString(args1[2].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).Id.ToString(), true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberID' (dest_string must be a array, or invalid parameters!)", caller_script);
                return 0;
            }

            return 1;
        }

        public static int DC_BanGuildMember(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                DiscordMember usr = Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild));
                usr.BanAsync(args1[2].AsInt32(), args1[3].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberID' (dest_string must be a array, or invalid parameters!)", caller_script);
                return 0;
            }

            return 1;
        }

        public static int DC_GetMemberAvatarURL(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            try
            {
                AMX.SetString(args1[2].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).AvatarUrl, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberAvatarURL' (dest_string must be a array, or invalid parameters!!)", caller_script);
                return 0;
            }
            return 1;
        }
    }
}
