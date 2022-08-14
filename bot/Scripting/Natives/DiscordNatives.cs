using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus.SlashCommands;
using System.Diagnostics;

namespace dcamx.Scripting.Natives
{
    public static class DiscordNatives
    {

        public static int DC_SetToken(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (Program.m_Discord != null) return 0;
            if (String.IsNullOrEmpty(args1[0].AsString())) return 0;
            Program.dConfig.Token = args1[0].AsString();
            
            return 1;
        }

        public static int DC_SetActivityText(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {

                if (String.IsNullOrEmpty(args1[0].AsString()))
                {
                    Utils.Log.Error("DC_SetActivityText -> argument 1 is empty! (choose from 0-3)", caller_script);
                    return 0;
                }
                if (args1[1].AsInt32() > 2 || args1[1].AsInt32() < 0)
                {
                    Utils.Log.Error("DC_SetActivityText -> argument 2 is invalid number! (choose from 0-3)", caller_script);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
            }
            var act = new DiscordActivity();
            act.Name = args1[0].AsString();


            switch (args1[1].AsInt32())
            {
                case 0:
                    act.ActivityType = ActivityType.Playing;
                    break;
                case 1:
                    act.ActivityType = ActivityType.ListeningTo;
                    break;
                case 2:
                    act.ActivityType = ActivityType.Competing;
                    break;


            }

            Program.m_Discord.Client.UpdateStatusAsync(act);
            return 1;
        }

        public static int DC_SetMinLogLevel(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (Program.m_Discord != null) return 0;
            if (args1[0].AsInt32() < 0 || args1[0].AsInt32() > 5) return 1;
            switch (args1[0].AsInt32())
            {
                case 0:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Trace;
                    break;
                case 1:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug;
                    break;
                case 2:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                    break;
                case 3:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                    break;
                case 4:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Error;
                    break;
                case 5:
                    Program.dConfig.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Critical;
                    break;
            }
            return 1;
        }

        //Reactions
        public static int DC_AddReaction(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;
            DiscordGuild guild = null;
            try
            {
                guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_AddReaction' (Invalid guildid?)" + caller_script);
            }

            DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[1].AsString())).Result;
            dc.GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result.CreateReactionAsync(DiscordEmoji.FromName(Program.m_Discord.Client, args1[3].AsString()));

            return 0;
        }

        public static int DC_RemoveReaction(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;
            DiscordGuild guild = null;
            try
            {
                guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_RemoveReaction' (Invalid guildid?)", caller_script);
            }

            DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[1].AsString())).Result;
            dc.GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result.DeleteOwnReactionAsync(DiscordEmoji.FromName(Program.m_Discord.Client, args1[3].AsString()));
            //CreateReactionAsync(DiscordEmoji.FromName(Program.m_Discord.Client, args1[3].AsString()));

            return 0;
        }

        public static int DC_AddPrivateReaction(AMX amx1, AMXArgumentList args1, Script caller_script)
        {

            if (args1.Length != 3) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                dc.GetMessageAsync(Convert.ToUInt64(args1[1].AsString())).Result.CreateReactionAsync(DiscordEmoji.FromName(Program.m_Discord.Client, args1[2].AsString()));
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeletPrivateReaction' (Invalid PM channel, wrong ID format)", caller_script);
            }
            return 0;
        }

        public static int DC_RemovePrivateReaction(AMX amx1, AMXArgumentList args1, Script caller_script)
        {

            if (args1.Length != 3) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                dc.GetMessageAsync(Convert.ToUInt64(args1[1].AsString())).Result.DeleteOwnReactionAsync(DiscordEmoji.FromName(Program.m_Discord.Client, args1[2].AsString()));
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_RemovePrivateReaction' (Invalid PM channel, wrong ID format)", caller_script);
            }
            return 0;
        }

        public static int DC_RegisterCommand(AMX amx1, AMXArgumentList args1, Script caller_script)
        {

            // DSharpPlus.SlashCommands.S
            return 1;
        }
    }
}
