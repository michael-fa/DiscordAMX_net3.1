using AMXWrapper;
using dcamx.Scripting;
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


        public static void Loop()
        {
            string wholecmd = Console.ReadLine(); //System.InvalidOperationException 
            string[] cmd = wholecmd.Split(' ');
            if (cmd.Length > 0)
            {
                switch (cmd[0])
                {
                    case "exit":
                        Program.StopSafely();
                        break;
                    case "loadscript":
                        LoadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                        break;
                    case "unloadscript":
                        UnloadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                        break;
                    case "reloadscript":
                        ReloadScript(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                        break;
                    case "callpub":
                        CallPublic(cmd.Skip(1).ToArray()); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                        break;
                    case "reloadall":
                        ReloadAll(); //Skip(1) will skip the command string itself and pass the rest of the whole string.
                        break;
                    case "help":
                        Help();
                        break;
                    case "guilds":
                        ListGuilds();
                        break;

                }
            }

            if (wholecmd.Length == 0) return;


            //Call OnConsoleInput for every script
            AMXPublic p = null;
            foreach (Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnConsoleInput");
                if (p != null)
                {
                    var tmp1 = p.AMX.Push(wholecmd);
                    p.Execute();
                    p.AMX.Release(tmp1);
                }
                p = null;
            }
        }







        public static void Help()
        {
            Console.WriteLine("\n\nCommmands available from console:\n   help                                          (Shows a list of commands)\n   exit                                          (Stops the server safely)\n" +
               "   callpub <scriptfile> <public>                 (Calls a public inside a script via console)\n   loadscript <scriptfile>                       (Loads a script. Enter scriptfile without .amx)\n   unloadscript <scriptfile>                     (Unloads a script that is loaded)" +
               "\n   reload <scriptfile>                           (Reloads a script- Pass scriptfile without '.amx')\n   reloadall                                     (Reloads all scripts)\n" +
               "   guilds                                        (Lists all the guilds available for the bot '" + Program.m_Discord.Client.CurrentUser.Username + "')");
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
            Script scr = new Script(args[0], true);
            AMXWrapper.AMXPublic pub = scr.m_Amx.FindPublic("OnInit");
            if (pub != null) pub.Execute();
        }

        public static void UnloadScript(string[] args)
        {
            if (args[0].Length == 0)
            {
                Log.Error(" [command] You did not specify a correct script file!");
                return;
            }

            AMXPublic pub;



            //Find the script
            foreach (Script sc in Program.m_Scripts)
            {
                if (sc.m_amxFile.Contains(args[0]))
                {
                    pub = sc.m_Amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();

                    Program.m_Scripts.Remove(sc);
                    sc.m_Amx.Dispose();
                    sc.m_Amx = null;
                    Log.Info("[CORE] Script '" + args[0] + "' unloaded.");
                    return;
                }
            }
            Log.Error(" [command] The script '" + args[0] + "' is not running.");
        }

        public static void CallPublic(string[] args)
        {
            if (args.Length == 0 || args.Length != 2)
            {
                Log.Error(" [command] You did not specify a script and public!");
                return;
            }

            if (args[1].Length < 1)
                Log.Error("[command] You should specify a public to call inside " + args[0]);

            foreach (Script sc in Program.m_Scripts)
            {
                //Log.Debug(sc.m_AmxFile);
                if (sc.m_amxFile.Equals(args[0]))
                {
                    AMXWrapper.AMXPublic pub = sc.m_Amx.FindPublic(args[1]);
                    if (pub != null)
                    {
                        pub.Execute();
                        Log.Info("[CORE] Public '" + args[1] + "' in script '" + args[0] + "' called.");
                        break;
                    }
                    else
                    {
                        Log.Error("[command] The callback " + args[0] + " has not been found.");
                        break;
                    }
                }
            }
        }


        public static void ReloadScript(string[] args)
        {
            if (args.Length != 1)
            {
                Log.Error(" [command] You did not specify a correct script name (without .amx)");
                return;
            }
            //Find the actual script
            bool _isFs = false;
            foreach (Script sc in Program.m_Scripts)
            {
                if (sc.m_Amx == null) continue;
                if (sc.m_amxFile.Equals(args[0]))
                {
                    if (sc.m_isFs)
                        _isFs = true;

                    UnloadScript(args);

                    if (!File.Exists("Scripts/" + args[0] + ".amx"))
                    {
                        Log.Error(" [command] The script file " + args[0] + ".amx does not exist in /Scripts/ folder.");
                        return;
                    }
                    Script scr = new Script(args[0], _isFs);

                    Log.Info("[CORE] Script '" + args[0] + "' reloaded.");
                    break;
                }
            }
        }

        public static void ReloadAll()
        {
            foreach (Script script in Program.m_Scripts)
            {
                if (script.m_Amx == null) continue;

                //script.StopAllTimers();

                if (script.m_Amx.FindPublic("OnUnload") != null)
                    script.m_Amx.FindPublic("OnUnload").Execute();

                script.m_Amx.Dispose();
                script.m_Amx = null;
                Log.WriteLine("Script " + script.m_amxFile + " unloaded.");
            }

            Program.m_Scripts.Clear();
            //Start all the stuff


            //Load main.amx, or error out if not available
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                return;
            }
            else
            {
                Script scr = new Script("main");
                AMXPublic p = null;
                p = scr.m_Amx.FindPublic("OnInit");
                if (p != null)
                {
                    p.Execute();

                }
            }

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
                Console.Write("     - ID: " + Utils.Scripting.DCGuild_ScrGuild(guild).m_ID + " | " + guild.Name + " | " + guild.MemberCount + " Members | Owner: " + guild.Owner.DisplayName + "#" + guild.Owner.Discriminator + " | Joined at " + guild.JoinedAt.DateTime.ToString());
            }
        }
    }
}