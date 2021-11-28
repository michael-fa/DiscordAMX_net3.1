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
using System.Reflection;

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
            else StopSafely();

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
                {                                                       //no autoload
                    if (fl.Contains("main.amx") || !fl.EndsWith(".amx") || Regex.Match(fl, "(?=/!).*(?=.amx)").Success) continue;
                    Log.Info("[CORE] Found filterscript: '" + Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1) + "' !");
                    new Script(Regex.Match(fl, "(?=/).*(?=.amx)").Value.ToString().Remove(0, 1), true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                StopSafely();
                return;
            }

        _CMDLOOP:
            string wholecmd = Console.ReadLine();
            string[] cmd = wholecmd.Split(' ');
            if(cmd.Length > 0)
            {
                if(cmd[0].Length > 0)
                    switch(cmd[0])
                    {
                        case "exit":
                            StopSafely();
                            break;
                        case "loadscript":
                            ConsoleCommand.LoadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                            break;
                        case "unloadscript":
                            ConsoleCommand.UnloadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                            break;
                        case "reloadscript":
                            ConsoleCommand.ReloadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                            break;
                        case "reloadall":
                            ConsoleCommand.ReloadAll(); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                            break;
                        case "help":
                            ConsoleCommand.Help();
                            break;
                        case "guilds":
                            ConsoleCommand.ListGuilds();
                            break;

                    }
            }

            goto _CMDLOOP;
            
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

            Environment.Exit(0);
        }
    }
}
