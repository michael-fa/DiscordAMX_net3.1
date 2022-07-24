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
        public static List<Plugins.PluginNatives> m_PluginNatives = null;
        public static List<Scripting.Guild> m_ScriptGuilds = null;
        public static List<IniFile> m_ScriptINIFiles = null;
        public static List<DiscordChannel> m_DmUsers = null;
        public static List<Guild.LastMessage> m_LastMsgs = null;

        //GuildAvailable gets called for every first initialised guild. We don't want that.
        public static bool m_ScriptingInited = false;



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
            Log.Info("INIT: -> Discord AMX Bot © 2022 - www.fanter.eu <-");
            if (m_isWindows) Log.Info("INIT: -> Running on Windows.");
            else if (m_isLinux) Log.Info("INIT: Running on Linux. (Make sure you are always up to date!");


            //PREPARE (not loading!) all plugins (extensions)
            try
            {
                foreach (string fl in Directory.GetFiles("Plugins/"))
                {
                    if (!fl.EndsWith(".dll")) continue;
                    Utils.Log.Info("[CORE] Found plugin: '" + fl + "' !");
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
                foreach (string fl in Directory.GetFiles("Scripts/"))
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
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                StopSafely();
                return;
            }
            else new Script("main");

            //We first want to prefetch (only call constructor methods, returning us the natives) and then, above, load all the scripts and finally "really load" the plugins.
            PluginTools.LoadAllPlugins();

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
            if (!Directory.Exists("Logs/"))
                Directory.CreateDirectory("Logs/");

            //Check if Plugins dir exists
            if (!Directory.Exists("Plugins/"))
                Directory.CreateDirectory("Plugins/");

            //Check if Scripts dir exists
            if (!Directory.Exists("Scripts/"))
                Directory.CreateDirectory("Scripts/");
        }



        //Setting everything up AFTER InitialChecks are done!
        static private void __InitialSetup()
        {
            dConfig = new DiscordConfiguration()
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


            //Setting everything up
            Program.m_ScriptTimers = new List<Scripting.ScriptTimer>();
            Program.m_Scripts = new List<Scripting.Script>();   //Create list for scripts
            Program.m_ScriptGuilds = new List<Scripting.Guild>();   //Create list for scripts
            Program.m_Plugins = new List<Plugins.Plugin>();   //Create list for plugins
            m_DmUsers = new List<DiscordChannel>();
            Program.m_ScriptINIFiles = new List<IniFile>();
            Program.m_LastMsgs = new List<Guild.LastMessage>();
        }



        static public void StopSafely()
        {
            if (m_Plugins != null)
            {
                foreach (Plugins.Plugin pl in m_Plugins)
                {
                    pl.Unload(0);

                    Log.WriteLine("Script " + pl._File + " unloaded.");
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

            //copy current log txt to one with the date in name and delete the old one
            File.Copy("Logs/current.txt", ("Logs/" + DateTime.Now.ToString().Replace(':', '-') + ".txt"));
            if (File.Exists("Logs/current.txt")) File.Delete("Logs/current.txt");
            Environment.Exit(0);

        }
    }
}
