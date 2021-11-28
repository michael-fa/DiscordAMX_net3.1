﻿using dcamx.Scripting;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dcamx.Utils
{
    public static class ConsoleCommand
    {
        public static void Help()
        {
            Console.WriteLine("\n\nCommmands available from console:\n   /help                                          (Shows a list of commands)\n   /exit                                          (Stops the server safely)\n" +
               "   /loadscript <scriptfile>                       (Loads a script. Enter scriptfile without .amx)\n   /unloadscript <scriptfile>                     (Unloads a script that is loaded)" +
               "\n   /reload <scriptfile>                           (Reloads a script- Pass scriptfile without '.amx')\n   /reloadall                                     (Reloads all scripts)\n" +
               "   /guilds                                        (Lists all the guilds available for the bot '" + Program.m_Discord.Client.CurrentUser.Username + "')");
        }



        public static void LoadScript(string[] args)
        {
            if (args[0].Length == 0)
            {
                Log.Error(" [command] You did not specify a correct script file!");
                return;
            }

            if (!File.Exists("Scripts/" + args[0] + ".amx"))
            {
                Log.Error(" [command] The script file " + args[0] + ".amx does not exist in /Scripts/ folder.");
                return;
            }
            Script scr = new Script(args[0]);
            AMXWrapper.AMXPublic pub = scr.amx.FindPublic("OnInit");
            if (pub != null) pub.Execute();
        }

        public static void UnloadScript(string[] args)
        {
            if (args[0].Length == 0)
            {
                Log.Error(" [command] You did not specify a correct script file!");
                return;
            }

            foreach (Script sc in Program.m_Scripts)
            {
                if (sc._amxFile.Equals(args[0]))
                {
                    AMXWrapper.AMXPublic pub = sc.amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();
                    sc.amx.Dispose();
                    sc.amx = null;
                    Program.m_Scripts.Remove(sc);
                    Log.Info("[CORE] Script '" + args[0] + "' unloaded.");
                    return;
                }
            }
            Log.Error(" [command] The script '" + args[0] + "' is not running.");
        }

        public static void ReloadScript(string[] args)
        {
            if (args[0] == null)
            {
                Log.Error(" [command] You did not specify a correct script name (without .amx)");
                return;
            }
            //Find the actual script
            foreach (Script sc in Program.m_Scripts)
            {
                if (sc._amxFile.Equals(args[0]))
                {
                    AMXWrapper.AMXPublic pub = sc.amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();
                    sc.amx.Dispose();
                    sc.amx = null;
                    Program.m_Scripts.Remove(sc);

                    if (!File.Exists("Scripts/" + args[0] + ".amx"))
                    {
                        Log.Error(" [command] The script file " + args[0] + ".amx does not exist in /Scripts/ folder.");
                        return;
                    }
                    Script scr = new Script(args[0]);
                    pub = scr.amx.FindPublic("OnInit");
                    if (pub != null) pub.Execute();

                    Log.Info("[CORE] Script '" + args[0] + "' reloaded.");
                    return;
                }
            }
        }

        public static void ReloadAll()
        {
            foreach (Script script in Program.m_Scripts)
            {
                if (script.amx == null) continue;

                script.StopAllTimers();

                if (script.amx.FindPublic("OnUnload") != null)
                    script.amx.FindPublic("OnUnload").Execute();

                script.amx.Dispose();
                script.amx = null;
                Log.WriteLine("Script " + script._amxFile + " unloaded.");
            }

            Program.m_Scripts.Clear();
            //Start all the stuff


            //Load main.amx, or error out if not available
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                return;
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
                return;
            }

            Log.WriteLine("->    All scripts reloaded.");
        }

        public static void ListGuilds()
        {
            Console.WriteLine("----- Guilds currently available:");
            foreach(DiscordGuild guild in Program.m_Discord.Client.Guilds.Values)
            {
                Console.Write("   - " + guild.Name + " | " + guild.MemberCount + " Members | Owner: " + guild.Owner.DisplayName + "#" + guild.Owner.Discriminator + " | Joined at " + guild.JoinedAt.DateTime.ToString());
            }
        }
    }
}