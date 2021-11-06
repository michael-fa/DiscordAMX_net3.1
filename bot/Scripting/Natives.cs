using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus;
using DSharpPlus.Interactivity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace bot.Scripting
{

    public static class Natives
    {
        public static int printc(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {
                Utilities.Log.Print(args1[0].AsString(), 4, caller_script._amxFile);
            }
            catch (Exception ex)
            {
                Utilities.Log.Print(ex);
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
            if (String.IsNullOrEmpty(args1[0].AsString())) return 1;

            Program.dConfig.Token = args1[0].AsString();
            return 1;
        }








        public static int DC_SetActivityText(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (String.IsNullOrEmpty(args1[0].AsString())) return 1;

            //ActivityType.
            var act = new DiscordActivity(args1[0].AsString(), ActivityType.Playing);
            Program.botr.Client.UpdateStatusAsync(act);
            return 1;
        }




        public static int DC_SetMinLogLevel(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
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







        public static int loadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            Program.Scripts.Add(new Script(args1[0].AsString()));
            return 1;
        }





        public static int SetTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            ScriptTimer timer = new ScriptTimer(args1[1].AsInt32(), args1[0].AsString(), caller_script);
            return (Program.ScriptTimers.Count);
        }



        public static int KillTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            foreach (ScriptTimer scrt in Program.ScriptTimers)
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




        public static int unloadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            foreach (Script scr in Program.Scripts)
            {
                if (scr.amx == amx1)
                {
                    if (scr.amx.FindPublic("OnUnload") != null)
                        scr.amx.FindPublic("OnUnload").Execute();

                    amx1.Dispose();
                    amx1 = null;
                    Program.Scripts.Remove(scr);
                }


            }
            return 1;
        }







        public static int DC_SendChannelMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            // Program.botr.Client.GetGuildAsync(guild);
            DiscordGuild guild;

            bot.Program.botr.Client.Guilds.TryGetValue(906110121233514506, out guild);
            try
            {
                //Console.WriteLine("guild + " +  guild + " | id: " + args1[0].AsString());
                guild.GetChannel(Convert.ToUInt64(args1[0].AsString())).SendMessageAsync(args1[1].AsString());
            }
            catch (Exception ex)
            {
                Utilities.Log.Print(ex);
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

        public static int ALTV_GetPublicServerInfo(AMX amx1, AMXArgumentList args1, Script caller_script)
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
        }
    }
}