using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace dcamx.Scripting
{

    public static class Natives
    {
        public static int printc(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {
                Utils.Log.WriteLine(args1[0].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
            }


            return 1;
        }

        public static int printf(AMX amx1, AMXArgumentList args1, Script caller_script)
        {

            /*Console.WriteLine("Length: " + args1.Length + "ARG: " + args1[1].AsInt32());

            string orig_string = args1[0].AsString();
            int param_Index = 0;
            int idx = 0;
            bool expect_opcode = false;

            if (args1.Length == 1) goto skipformat;

            param_Index = 1;
            foreach (char c in orig_string)
            {
                if (c == '%')
                {
                    expect_opcode = true;
                    idx++;
                    continue;
                }


                if (expect_opcode)
                {
                    if(c == 'd')
                    {
                        Console.WriteLine(args1[1].AsInt32());
                        orig_string = orig_string.Remove(idx - 1, 2).Insert(idx-1, args1[1].AsInt32().ToString());
                        param_Index++;
                        expect_opcode = false;
                    }
                    
                }

                idx++;

            }
            skipformat:
            Console.WriteLine("STRING: " + orig_string);
            
            *//*
            string str = "";
            for(int i=0; i< args1.Length; i++)
            {
                str.Insert(0, args1[i].ToString());
            }*/
            return 1;
        }

        public static int DC_SetToken(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (Program.m_Discord != null) return 0;
            if (String.IsNullOrEmpty(args1[0].AsString())) return 1;
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

        public static int Loadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!");
                return 0;
            }

            if (!File.Exists("Scripts/" + args1[0].AsString() + ".amx"))
            {
                Utils.Log.Error(" [command] The script file " + args1[0].AsString() + ".amx does not exist in /Scripts/ folder.");
                return 0;
            }
            Script scr = new Script(args1[0].AsString());
            AMXWrapper.AMXPublic pub = scr.amx.FindPublic("OnInit");
            if (pub != null) pub.Execute();
            return 1;
        }

        public static int Unloadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 1;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!");
                return 0;
            }

            foreach (Script sc in Program.m_Scripts)
            {
                if (sc._amxFile.Equals(args1[0].AsString()))
                {
                    AMXWrapper.AMXPublic pub = sc.amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();
                    sc.amx.Dispose();
                    sc.amx = null;
                    Program.m_Scripts.Remove(sc);
                    Utils.Log.Info("[CORE] Script '" + args1[0].AsString() + "' unloaded.");
                    return 1;
                }
            }
            Utils.Log.Error(" [command] The script '" + args1[0].AsString()  + "' is not running.");
            return 1;
        }

        public static int gettimestamp(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            return (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static int SetTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1[1].AsInt32() > 1 || args1[1].AsInt32() < 0)
            {
                Utils.Log.Error("SetTimer: Argument 'repeating' is boolean. Please pass 0 or 1 only!");
                return 1;
            }

            try { ScriptTimer timer = new ScriptTimer(args1[2].AsInt32(), Convert.ToBoolean(args1[1].AsInt32()), args1[0].AsString(), caller_script); 
            }catch(Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
            }
            return (Program.m_ScriptTimers.Count);
        }

        public static int KillTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            foreach (ScriptTimer scrt in Program.m_ScriptTimers)
            {
                if (scrt != null)
                {
                    if (scrt.ID == args1[0].AsInt32())
                    {
                        scrt.KillTimer();
                        return 1;
                    }
                }
            }
            return 1;
        }












        public static int DC_DeleteMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

                guild.GetChannel(Convert.ToUInt64(args1[1].AsString())).GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result.DeleteAsync(args1[3].AsString()).Wait();
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeleteMessage' (Invalid Channel ID, wrong ID format, or you have not the right role permissions)" + caller_script);
            }
            return 1;
        }

        public static int DC_SendChannelMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                guild.GetChannel(Convert.ToUInt64(args1[1].AsString())).SendMessageAsync(args1[2].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessage' (Invalid Channel, wrong ID format, or you have not the right role permissions)" + caller_script);
            }
            return 1;
        }

        public static int DC_SendPrivateMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                dc.SendMessageAsync(args1[1].AsString());

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendPrivateMessage' (Invalid pm channel, wrong ID format)" + caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_DeletePrivateMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                dc.GetMessageAsync(Convert.ToUInt64(args1[1].AsString())).Result.DeleteAsync(args1[2].AsString()).Wait();
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeletePrivateMessage' (Invalid pm channel, wrong ID format)" + caller_script);
                return 0;
            }
            return 1;
        }










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
                Utils.Log.Error("In native 'DC_GetMemberName' (dest_string must be a array, or invalid parameters!!)" + caller_script);
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
                Utils.Log.Error("In native 'DC_GetMemberDisplayName' (dest_string must be a array, or invalid parameters!!)" + caller_script);
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
                Utils.Log.Error("In native 'DC_GetMemberDiscriminator' (dest_string must be a array, or invalid parameters!)" + caller_script);
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
                Utils.Log.Error("In native 'DC_GetMemberID' (dest_string must be a array, or invalid parameters!)" + caller_script);
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
                Utils.Log.Error("In native 'DC_GetMemberID' (dest_string must be a array, or invalid parameters!)" + caller_script);
                return 0;
            }

            return 1;
        }


        //Guilds



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
                Utils.Log.Error("In native 'DC_GetGuildName' (dest_string must be a array, or invalid parameters!)" + caller_script);
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
                Utils.Log.Error("In native 'DC_GetMemberCount' (Invalid guildid?)" + caller_script);
            }
            if (guild != null) return guild.MemberCount;
            else return 0;
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
                Utils.Log.Error("In native 'DC_RemoveReaction' (Invalid guildid?)" + caller_script);
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
                Utils.Log.Error("In native 'DC_DeletPrivateReaction' (Invalid PM channel, wrong ID format)" + caller_script);
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
                Utils.Log.Error("In native 'DC_DeletPrivateReaction' (Invalid PM channel, wrong ID format)" + caller_script);
            }
            return 0;
        }
    }
}