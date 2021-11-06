using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bot.Utilities;
using System.Drawing;
using DSharpPlus;
using System.IO;

namespace bot
{
    class Program
    {
        public static Discord.DCBot botr;
        public static List<Scripting.ScriptTimer> ScriptTimers;
        public static List<Scripting.Script> Scripts;
        public static string m_GuildID = null;

        public static DiscordConfiguration dConfig = new DiscordConfiguration()
        {

            // Token = "",
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.DirectMessageReactions
            | DiscordIntents.DirectMessages
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




        [System.Runtime.InteropServices.DllImport("Kernel32")]
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
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //Check if LOG Dir exists
            if (!Directory.Exists("Logs/"))
                Directory.CreateDirectory("Logs/");

            if (!Directory.Exists("Scripts/"))
                Directory.CreateDirectory("Scripts/");

            File.AppendAllText("Logs/current.txt", "\n++++++++++++++++++++ | LOG " + DateTime.Now + " | ++++++++++++++++++++\n");//Print out log file header (file only)
            Log.WriteLine("Discord AMX Bot © fanter.eu", Color.Cyan);

            //Setting everything up
            ScriptTimers = new List<Scripting.ScriptTimer>();
            Scripts = new List<Scripting.Script>();   //Create list for scripts

            if(!File.Exists("Scripts/main.amx"))
            {
                Utilities.Log.WriteLine("no scripts found!");
                StopSafely();
            }

            try
            {
                Scripting.Script scr = new Scripting.Script("main"); //The MAIN Script! ALWAYS 0 INDEX IN LIST "Scripts"!
            }
            catch(Exception ex)
            {
                Utilities.Log.Print(ex);
            }

            botr = new Discord.DCBot(); //AMX -> MAIN()
            botr.RunAsync(dConfig).GetAwaiter().GetResult(); // AMX - OnLoad / OnConnect



        cmdloop:
            string cmd = Console.ReadLine();

            if (cmd.Equals("exit"))
                StopSafely();
            goto cmdloop;
        }

        static private void StopSafely()
        {
            

            foreach (Scripting.Script script in Scripts)
            {
                if (script.amx == null) continue;

                script.StopAllTimers();
                
                if (script.amx.FindPublic("OnUnload") != null)
                    script.amx.FindPublic("OnUnload").Execute();

                script.amx.Dispose();
                script.amx = null;
                Utilities.Log.WriteLine("Script " + script._amxFile + " unloaded.");
            }

            _ = botr.DisconnectAsync();

            File.Copy("Logs/current.txt", ("Logs/" + DateTime.Now.ToString().Replace(':', '-') + ".txt")); //copy current log txt to one with the date in name and delete the old on
            if (File.Exists("Logs/current.txt")) File.Delete("Logs/current.txt");
        }
    }
}
