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
            Log.Info("-> Discord AMX Bot © 2021 - www.fanter.eu <-"); 
            Log.Info("RUNNING ON " + Environment.OSVersion.VersionString + "\n\n");
            Utils.Scripting.ScriptFormat_Resolve("f", 7.9);


            //Load main.amx, or error out if not available
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                StopSafely();
                return;
            }
            else new Script("main");

            m_Discord = new Discord.DCBot(); //AMX -> MAIN()
            m_Discord.RunAsync(dConfig).GetAwaiter().GetResult(); // AMX - OnLoad / OnConnect

            //Now add all filterscripts
            try
            {
                foreach (string fl in Directory.GetFiles("Scripts/"))
                {                                               
                    Match mtch = Regex.Match(fl, "(?=/!).*(?=.amx)");
                    // demand load main.amx     ||  skip this file
                    if (fl.Contains("main.amx") || !fl.EndsWith(".amx") || mtch.Success) continue;
                    Log.Info("[CORE] Found filterscript: '" + mtch.Value.ToString().Remove(0, 1) + "' !");
                    new Script(mtch.Value.ToString().Remove(0, 1), true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                StopSafely();
                return;
            }


        //Handle commands.
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
            m_DmUsers = new List<DiscordChannel>();
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

            File.Copy("Logs/current.txt", ("Logs/" + DateTime.Now.ToString().Replace(':', '-') + ".txt")); //copy current log txt to one with the date in name and delete the old one
            if (File.Exists("Logs/current.txt")) File.Delete("Logs/current.txt");

            Environment.Exit(0);
        }
    }
}
