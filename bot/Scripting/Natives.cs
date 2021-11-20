using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            if (Program.m_ScriptingInited) return 0;
            if (String.IsNullOrEmpty(args1[0].AsString())) return 1;

            Program.dConfig.Token = args1[0].AsString();
            return 1;
        }

        public static int DC_SetGuild(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if(Program.m_ScriptingInited) return 0;
            if (String.IsNullOrEmpty(args1[0].AsString())) return 1;

            Program.m_GuildID = args1[0].AsString();
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
            if (Program.m_ScriptingInited) return 0;
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

        public static int gettimestamp(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            return (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static int SetTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try { ScriptTimer timer = new ScriptTimer(args1[1].AsInt32(), args1[0].AsString(), caller_script); 
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
            if (args1.Length != 3) return 0;
            try
            {
                DiscordGuild guild;

                bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
                guild.GetChannel(Convert.ToUInt64(args1[0].AsString())).GetMessageAsync(Convert.ToUInt64(args1[1].AsString())).Result.DeleteAsync(args1[2].AsString()).Wait();
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeleteMessage' (Invalid Channel or Message ID, wrong ID format, or you have not the right role permissions)" + caller_script);
            }
            return 1;
        }


        public static int DC_SendChannelMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            // Program.botr.Client.GetGuildAsync(guild);
            DiscordGuild guild;

            bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
            try
            {
                //Console.WriteLine("guild + " +  guild + " | id: " + args1[0].AsString());
                guild.GetChannel(Convert.ToUInt64(args1[0].AsString())).SendMessageAsync(args1[1].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessage' (Invalid Channel or Message ID, wrong ID format, or you have not the right role permissions)" + caller_script);
            }
            return 1;
        }











        public static int DC_GetMemberName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            DiscordGuild guild;

            bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
            try
            {
                //Console.WriteLine(Utils.Scripting.ScrMemberID_DCMember(args1[0].AsInt32()).Username);
                AMX.SetString(args1[1].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[0].AsInt32()).Username, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberName' (dest_string must be a array!)" + caller_script);
            }
            return 1;
        }

        public static int DC_GetMemberDisplayName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            DiscordGuild guild;

            bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
            try
            {
                //Console.WriteLine(Utils.Scripting.ScrMemberID_DCMember(args1[0].AsInt32()).Username);
                AMX.SetString(args1[1].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[0].AsInt32()).DisplayName, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberDisplayName' (dest_string must be a array!)" + caller_script);
            }
            return 1;
        }

        public static int DC_GetMemberDiscriminator(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            DiscordGuild guild;

            bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
            try
            {
                AMX.SetString(args1[1].AsCellPtr(), Utils.Scripting.ScrMemberID_DCMember(args1[0].AsInt32()).Discriminator, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetMemberDiscriminator' (dest_string must be a array!)" + caller_script);
            }
            return 1;
        }













        public struct ALTV_ServerInfo
        {
            [JsonProperty("id")]
            public string id { get; private set; }

            [JsonProperty("Players")]
            public int Players { get; private set; }

            [JsonProperty("name")]
            public string name { get; private set; }

            [JsonProperty("locked")]
            public bool locked { get; private set; }

        }

        /*public static int ALTV_GetPublicServerInfo(AMX amx1, AMXArgumentList args1, Script caller_script)
        {

            string gettet = bot.Utilities.HTTP.Get(args1[0].AsString().ToString());
            var srvInfo = JsonConvert.DeserializeObject<ALTV_ServerInfo>(gettet);

            AMXPublic pub = caller_script.amx.FindPublic(args1[1].AsString());
            if (pub != null)
            {
                //var ptr = caller_script.amx.Push(gettet, true);
                caller_script.amx.Push(srvInfo.Players);
                pub.Execute();
                //caller_script.amx.Release(ptr);

                GC.Collect(); // To check for leaks
            }

            return 1;
        }*/
    }
}