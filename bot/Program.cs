using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using dcamx.Utils;
using dcamx.Scripting;
using System.Runtime.InteropServices;
using DSharpPlus;

namespace dcamx
{
    class Program
    {

        //Coreinfo
        static bool m_isWindows;
        static bool m_isLinux;


        public static Discord.DCBot m_Discord = null;
        public static List<Scripting.ScriptTimer> m_ScriptTimers = null;
        public static List<Plugins.Plugin> m_Plugins = null;
        public static List<Scripting.Script> m_Scripts = null;
        public static List<Scripting.Guild> m_ScriptGuilds = null;
        public static List<IniFile> m_ScriptINIFiles = null;
        public static List<DiscordChannel> m_DmUsers = null;
        public static List<Scripting.DiscordEmbedBuilder> m_Embeds = null;

        //GuildAvailable gets called for every first initialised guild. We don't want that.
        public static bool m_ScriptingInited = false;
        public static AMXWrapper.AMX m_MainAMX = null;



        //Discord
        public static DiscordConfiguration dConfig = null;



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
            else StopSafely();

            //Set the console handler, catching events such as close.
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            __InitialChecks();
            __InitialSetup();


            //Print a time and date to log file
            File.AppendAllText("Logs/current.txt", "\n++++++++++++++++++++ | LOG " + DateTime.Now + " | ++++++++++++++++++++\n");//Print out log file header (file only)
            //Console initial message
            Log.Info("INIT: -> DiscordAMX BETA © 2022 - www.fanter.eu <-");
            if (m_isWindows) Log.Info("INIT: -> Running on Windows.");
            else if (m_isLinux) Log.Info("INIT: Running on Linux. (Make sure you are always up to date!");


            //PREPARE (not loading!) all plugins (extensions)
            try
            {
                foreach (string fl in Directory.GetFiles("Plugins/"))
                {
                    if (!fl.EndsWith(".dll")) continue;
                    Utils.Log.Info("\n---------------------------------------------\n[CORE] Found plugin: '" + fl + "' !");
                    Utils.Log.Info("\n---------------------------------------------");
                    new Plugins.Plugin(fl);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
                Program.StopSafely();
                return;
            }

            //Now add all filterscripts (before main amx)
            try
            {
                foreach (string fl in Directory.GetFiles(System.AppContext.BaseDirectory + "/Scripts/"))
                {
                    // demand load main.amx     ||  skip this file
                    if (fl.StartsWith("!") || fl.Contains("main.amx") || !fl.EndsWith(".amx")) continue;
                    Log.Info("[CORE] Found filterscript: '" + fl.Remove(0, 8).Replace(".amx", "") + "' !");
                    new Script(fl.Remove(0, 8).Replace(".amx", ""), true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                StopSafely();
                return;
            }

            //Load main.amx, or error out if not available
            if (!File.Exists(System.AppContext.BaseDirectory + "/Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                StopSafely();
                return;
            }
            else new Script("main");

            //We first want to prefetch (only call constructor methods, returning us the natives) and then, above, load all the scripts and finally "really load" the plugins.
            PluginTools.RegisterNatives_Late(); //AMXRegister
            PluginTools.LoadAllPlugins(); //InvokeLoad

            m_Discord = new Discord.DCBot(); //AMX -> MAIN()
            m_Discord.RunAsync(dConfig).GetAwaiter().GetResult(); // AMX - OnLoad / OnConnect

            //Handle commands.
            Console.CancelKeyPress += delegate {
                StopSafely();
                return;
            };


        _CMDLOOP:
            ConsoleCommand.Loop();
            goto _CMDLOOP;

        }



        //Checking for all different kinds of stuff so we're good to go.
        static private void __InitialChecks()
        {
            //Check if LOG Dir exists
            if (!Directory.Exists(System.AppContext.BaseDirectory + "Logs/"))
                Directory.CreateDirectory(System.AppContext.BaseDirectory+ "Logs/");

            //Check if Plugins dir exists
            if (!Directory.Exists(System.AppContext.BaseDirectory+ "Plugins/"))
                Directory.CreateDirectory(System.AppContext.BaseDirectory +"Plugins/");

            //Check if Scripts dir exists
            if (!Directory.Exists(System.AppContext.BaseDirectory + "Scripts/"))
                Directory.CreateDirectory(System.AppContext.BaseDirectory + "Scripts/");

            //Internal banlist
            if (!File.Exists(System.AppContext.BaseDirectory + "bans.txt"))
            {
                File.AppendAllText(System.AppContext.BaseDirectory + "bans.txt", "# ---------------- Private DM related bans ----------------\n# This file is written by the DiscordAMX program itself, since it uses it own private dm ban protection.\n# If you want to add a user manually, simply add his id in a new separate line at the bottom.");

            }

        }



        //Setting everything up AFTER InitialChecks are done!
        static private void __InitialSetup()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

#if DEBUG
            Console.Title = "DiscordAMX Debug: " + Environment.UserName + " | " + DateTime.Now.ToString();
#endif
            dConfig = new DiscordConfiguration()
            {
                TokenType = TokenType.Bot,
                AlwaysCacheMembers = false,
                MessageCacheSize = 4065,
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


            //Setting everything up
            Program.m_ScriptTimers = new List<Scripting.ScriptTimer>();
            Program.m_Scripts = new List<Scripting.Script>();   //Create list for scripts
            Program.m_ScriptGuilds = new List<Scripting.Guild>();   //Create list for scripts
            Program.m_Plugins = new List<Plugins.Plugin>();   //Create list for plugins
            m_DmUsers = new List<DiscordChannel>();
            Program.m_ScriptINIFiles = new List<IniFile>();
            Program.m_Embeds = new List<Scripting.DiscordEmbedBuilder>();
        }



        static public void StopSafely()
        {
            if (m_Plugins != null)
            {
                foreach (Plugins.Plugin pl in m_Plugins)
                {
                    pl.Unload(0);

                    Log.WriteLine("Plugin " + pl._File + " unloaded.");
                }
            }
            
            //Check for unclosed INI File handlers (scripts)
            foreach(IniFile x in Program.m_ScriptINIFiles)
            {
                Log.Warning("[INI FILE] Unclosed ini file handler found for \"" + x.Path  + "\"");
            }

            foreach (Script script in m_Scripts)
            {
                if (script.m_Amx == null) continue;

                script.StopAllTimers();

                if (script.m_Amx.FindPublic("OnUnload") != null)
                    script.m_Amx.FindPublic("OnUnload").Execute();

                script.m_Amx.Dispose();
                script.m_Amx = null;
                Log.WriteLine("Script " + script.m_amxFile + " unloaded.");
            }

            //copy current log txt to one with the date in name and delete the old one | we also replace : or / to - so that theres no language based error in folder/file names
            File.Copy(System.AppContext.BaseDirectory + "/Logs/current.txt", (System.AppContext.BaseDirectory+ "Logs/" + DateTime.Now.ToString().Replace(':', '-').Replace('/', '-') + ".txt"));
            if (File.Exists(System.AppContext.BaseDirectory + "/Logs/current.txt")) File.Delete(System.AppContext.BaseDirectory + "/Logs/current.txt");
            Environment.Exit(0);

        }
    }
}
