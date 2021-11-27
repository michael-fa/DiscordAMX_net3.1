using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DSharpPlus;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using dcamx.Utils;
using dcamx.Scripting;
using System.Runtime.InteropServices;
using AMXWrapper;
using System.Runtime.CompilerServices;

namespace dcamx
{
    class Program
    {

        //Kerninformationen
        static bool m_isWindows;
        static bool m_isLinux;


        public static Discord.DCBot m_Discord = null;
        public static List<Scripting.ScriptTimer> m_ScriptTimers = null;
        public static List<Scripting.Script> m_Scripts = null;
        public static List<Scripting.Guild> m_ScriptGuilds = null;
        public static List<DiscordChannel> m_DmUsers = null;

        //GuildAvailable gets called for every first initialised guild. We don't want that.
        public static bool m_ScriptingInited = false;


        public static DiscordConfiguration dConfig = new DiscordConfiguration()
        {
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.DirectMessageReactions
            | DiscordIntents.DirectMessages 
            | DiscordIntents.GuildMessageReactions
            | DiscordIntents.GuildBans
            | DiscordIntents.GuildEmojis
            | DiscordIntents.GuildInvites
            | DiscordIntents.GuildMembers
            | DiscordIntents.GuildMessages
            | DiscordIntents.Guilds
            | DiscordIntents.GuildVoiceStates
            | DiscordIntents.GuildWebhooks,
            AutoReconnect = true
        };




        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    StopSafely();
                    return false;
                default:
                    return false;
            }
        }



        static void Main(string[] args)
        {
            //Environment - Set the OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) m_isWindows = true;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) m_isLinux = true;
            else goto __EXIT;

            //Set the console handler, catching events such as close.
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            //Check if LOG Dir exists
            if (!Directory.Exists("Logs/"))
                Directory.CreateDirectory("Logs/");

            //Check if Scripts dir exists
            if (!Directory.Exists("Scripts/"))
                Directory.CreateDirectory("Scripts/");

            

            //Print a time and date to log file
            File.AppendAllText("Logs/current.txt", "\n++++++++++++++++++++ | LOG " + DateTime.Now + " | ++++++++++++++++++++\n");//Print out log file header (file only)
            //Console initial message
            Log.Info("-> Discord AMX Bot © 2021 - www.fanter.eu <-"); 
            Log.Info("RUNNING ON " + Environment.OSVersion.VersionString + "\n\n");



            //Setting everything up
            Program.m_ScriptTimers = new List<Scripting.ScriptTimer>();
            Program.m_Scripts = new List<Scripting.Script>();   //Create list for scripts
            Program.m_ScriptGuilds = new List<Scripting.Guild>();   //Create list for scripts
            m_DmUsers = new List<DiscordChannel>();



            //Load main.amx, or error out if not available
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                goto __EXIT;
            }
            else new Script("main");

            m_Discord = new Discord.DCBot(); //AMX -> MAIN()
            m_Discord.RunAsync(dConfig).GetAwaiter().GetResult(); // AMX - OnLoad / OnConnect

            //Now add all filterscripts
            try
            {
                foreach (string fl in Directory.GetFiles("Scripts/"))
                {                                                       //no autoload
                    if (fl.Contains("main.amx") || !fl.EndsWith(".amx") || Regex.Match(fl, "(?=/!).*(?=.amx)").Success) continue;
                    Log.Info("[CORE] Found filterscript: '" + Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1) + "' !");
                    new Script(Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1), true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                goto __EXIT;
            }

        _CMDLOOP:
            string cmd = Console.ReadLine();

            if (cmd.StartsWith("exit"))
                goto __EXIT;

            else if(cmd.StartsWith("help") || cmd.StartsWith("?"))
            {
                Console.WriteLine("\n\nCommmands available from console:\n   /help                                          (Shows a list of commands)\n   /exit                                          (Stops the server safely)\n" +
                "   /loadscript <scriptfile>                       (Loads a script. Enter scriptfile without .amx)\n   /unloadscript <scriptfile>                     (Unloads a script that is loaded)" +
                "\n/reload <scriptfile>                           (Reloads a script- Pass scriptfile without '.amx')\n/reloadall                        (Reloads all scripts)");
            }

            else if (cmd.StartsWith("loadscript"))
            {
                string[] spl;
                try
                {
                    spl = cmd.Split(' ');
                }
                catch
                {
                    goto _CMDLOOP;
                }
                if (spl.Length != 2) goto _CMDLOOP;
                if (spl[1] == null)
                {
                    Log.Error(" [command] You did not specify a correct script file!");
                    goto _CMDLOOP;
                }
                if (!File.Exists("Scripts/" + spl[1] + ".amx"))
                {
                    Log.Error(" [command] The script file " + spl[1] + ".amx does not exists in /Scripts/ folder.");
                    goto _CMDLOOP;
                }
                Script scr = new Script(spl[1]);
                AMXWrapper.AMXPublic pub = scr.amx.FindPublic("OnInit");
                if (pub != null) pub.Execute();
            }




            else if (cmd.StartsWith("unloadscript"))
            {
                string[] spl;
                try
                {
                    spl = cmd.Split(' ');
                }
                catch
                {
                    goto _CMDLOOP;
                }
                if (spl.Length != 2) goto _CMDLOOP;
                if (spl[1] == null)
                {
                    Log.Error(" [command] You did not specify a correct script name (without .amx)");
                    goto _CMDLOOP;
                }

                foreach (Script sc in m_Scripts)
                {
                    if (sc._amxFile.Equals(spl[1]))
                    {
                        AMXWrapper.AMXPublic pub = sc.amx.FindPublic("OnUnload");
                        if (pub != null) pub.Execute();
                        sc.amx.Dispose();
                        sc.amx = null;
                        m_Scripts.Remove(sc);
                        Log.Info("[CORE] Script '" + spl[1] + "' unloaded.");
                        goto _CMDLOOP;
                    }
                }
                Log.Error(" [command] The script '" + spl[1] +  "' is not running.");
                
            }

            else if(cmd.StartsWith("reload"))
            {
                string[] spl;
                try
                {
                    spl = cmd.Split(' ');
                }
                catch
                {
                    goto _CMDLOOP;
                }
                if (spl.Length != 2) goto _CMDLOOP;
                if (spl[1] == null)
                {
                    Log.Error(" [command] You did not specify a correct script name (without .amx)");
                    goto _CMDLOOP;
                }
                //Find the actual script
                foreach (Script sc in m_Scripts)
                {
                    if (sc._amxFile.Equals(spl[1]))
                    {
                        AMXWrapper.AMXPublic pub = sc.amx.FindPublic("OnUnload");
                        if (pub != null) pub.Execute();
                        sc.amx.Dispose();
                        sc.amx = null;
                        m_Scripts.Remove(sc);

                        if (!File.Exists("Scripts/" + spl[1] + ".amx"))
                        {
                            Log.Error(" [command] The script file " + spl[1] + ".amx does not exists in /Scripts/ folder.");
                            goto _CMDLOOP;
                        }
                        Script scr = new Script(spl[1]);
                        pub = scr.amx.FindPublic("OnInit");
                        if (pub != null) pub.Execute();

                        Log.Info("[CORE] Script '" + spl[1] + "' reloaded.");
                        goto _CMDLOOP;
                    }
                }

            }

            else if(cmd.Equals("reloadall"))
            {
                Console.WriteLine("1");
                foreach (Script script in m_Scripts)
                {
                    if (script.amx == null) continue;

                    script.StopAllTimers();

                    if (script.amx.FindPublic("OnUnload") != null)
                        script.amx.FindPublic("OnUnload").Execute();

                    script.amx.Dispose();
                    script.amx = null;
                    Log.WriteLine("Script " + script._amxFile + " unloaded.");
                }

                m_Scripts.Clear();
                //Start all the stuff


                //Load main.amx, or error out if not available
                if (!File.Exists("Scripts/main.amx"))
                {
                    Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                    goto __EXIT;
                }
                else new Script("main");

                //Now add all other scripts
                try
                {
                    foreach (string fl in Directory.GetFiles("Scripts/"))
                    {
                        if (fl.Contains("main.amx") || !fl.EndsWith(".amx")) continue;
                        Log.Info("[CORE] Found filterscript: '" + Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1) + "' !");
                        new Script(Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1), true);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    goto __EXIT;
                }

                Log.WriteLine("->    All scripts reloaded.");
            }

            goto _CMDLOOP;


        __EXIT:
            StopSafely();
        }

        static public void StopSafely()
        {
            foreach (Script script in m_Scripts)
            {
                if (script.amx == null) continue;

                script.StopAllTimers();

                if (script.amx.FindPublic("OnUnload") != null)
                    script.amx.FindPublic("OnUnload").Execute();

                script.amx.Dispose();
                script.amx = null;
                Log.WriteLine("Script " + script._amxFile + " unloaded.");
            }

            if (m_Discord != null) _ = m_Discord.DisconnectAsync();

            File.Copy("Logs/current.txt", ("Logs/" + DateTime.Now.ToString().Replace(':', '-') + ".txt")); //copy current log txt to one with the date in name and delete the old on
            if (File.Exists("Logs/current.txt")) File.Delete("Logs/current.txt");

            Thread.CurrentThread.Abort();
        }
    }
}
