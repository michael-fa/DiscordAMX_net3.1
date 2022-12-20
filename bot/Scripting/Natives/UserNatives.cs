using AMXWrapper;
using DSharpPlus.Entities;
using System;

namespace dcamx.Scripting.Natives
{
    public static class UserNatives
    {
        public static int DC_GetMemberName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length > 3) return 0;
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

        public static int DC_GetMemberID(AMX amx1, AMXArgumentList args1, Script caller_script)
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

        public static int DC_IsMemberValid(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;

            DiscordGuild guild = null;
            if (args1[0].AsInt32() > -1)guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            if(guild == null)
            {
                DiscordChannel user = null;
                user = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[1].AsString())).Result;
                if (user != null) return 1;
            }
            else
            {
                foreach(DiscordMember mem in guild.Members.Values)
                {
                    Console.WriteLine(mem.DisplayName);
                    if (mem.Id == Convert.ToUInt64(args1[1].AsString()) || guild.Owner.Id == Convert.ToUInt64(args1[1].AsString()))
                        return 1;
                }
            }

            return 0;
        }

        public static int DC_BanUser(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0) return 0;

            Utils.UserBans.Add(args1[0].AsString());

            return 1;
        }

        public static int DC_UnbanUser(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0) return 0;

            Utils.UserBans.Remove(args1[0].AsString());

            return 1;
        }

        public static int DC_IsUserBanned(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0) return 0;

            if (!Utils.UserBans.IsBanned(args1[0].AsString())) return 0;

            else return 1;
        }

        public static int DC_BanMember(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                DiscordMember usr = Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild));
                if (args1[3].AsString().Equals("0")) usr.BanAsync(args1[2].AsInt32(), null); //no reason
                else usr.BanAsync(args1[2].AsInt32(), args1[3].AsString());

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_BanMember' (invalid parameters!)", caller_script);
                return 0;
            }

            return 1;
        }

        public static int DC_UnbanMember(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                DiscordMember usr = Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild));
                usr.UnbanAsync();

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_UnbanMember' (invalid parameters!)", caller_script);
                return 0;
            }

            return 1;
        }

        public static int DC_TimeoutMember(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 5) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                DiscordMember usr = Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild));
                if (args1[3].AsString().Equals("0")) usr.TimeoutAsync(DateTimeOffset.Now.AddSeconds(Convert.ToDouble(args1[2].AsInt32())), null);//no reason
                else usr.TimeoutAsync(DateTimeOffset.Now.AddSeconds(Convert.ToDouble(args1[2].AsInt32())), args1[3].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_TimeoutMember' (invalid parameters!)", caller_script);
                return 0;
            }

            return 1;
        }

        public static int DC_MemberHasRole(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            //This function does not use the api functions to get a role list from the member
            //For some reason, when editing member roles, during the runtime of the bot software, as of right now, it does not get updated for our api / bot until
            //it has been restarted.

            //This is bypassed by reusing OnMemberUpdate event to set and reference to the member's script_entity object
            //And so this is way we are doing it this way.

            if (args1.Length != 3) return 0;

            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                foreach (DiscordRole dr in Utils.Scripting.DCGuild_ScrGuild(guild).m_ScriptMembers[args1[1].AsInt32()].m_Roles)
                {
                    if (dr.Name.Equals(args1[2].AsString()))
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GuildMemberHasRole' (invalid parameters!)", caller_script);
                return 0;
            }

            return 0;
        }

        public static int DC_SetMemberRole(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                foreach (DiscordRole rl in guild.Roles.Values)
                {
                    if (rl.Name.Equals(args1[2].AsString()))
                    {
                        Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).GrantRoleAsync(rl);
                        return 1;
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SetMemberRole' (invalid parameters!)", caller_script);
                return 0;
            }

            return 0;
        }

        public static int DC_RevokeMemberRole(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;


            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                foreach (DiscordRole rl in guild.Roles.Values)
                {
                    if (rl.Name.Equals(args1[2].AsString()))
                    {
                        Utils.Scripting.ScrMemberID_DCMember(args1[1].AsInt32(), Utils.Scripting.DCGuild_ScrGuild(guild)).RevokeRoleAsync(rl);
                        return 1;
                    }
                }

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_RemoveMemberRole' (invalid parameters!)", caller_script);
                return 0;
            }

            return 0;
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
