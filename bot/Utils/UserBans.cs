using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Utils
{
    public static class UserBans
    {
        public static void Add(string userid)
        {
            if (!File.Exists(System.AppContext.BaseDirectory + "bans.txt")) File.AppendAllText(System.AppContext.BaseDirectory + "bans.txt", "# ---------------- Private DM related bans ----------------\n# This file is written by the DiscordAMX program itself, since it uses it own private dm ban protection.\n# If you want to add a user manually, simply add his id in a new separate line at the bottom.");
            
            
            string[] whole_file = File.ReadAllLines(System.AppContext.BaseDirectory + "bans.txt");
            foreach (string line in whole_file)
            {
                if (line.StartsWith("#")) continue;
                if (line.Contains(userid))
                {
                    Log.Error("[ban] User " + userid + " is already banned.");
                    return;
                }
            }
            File.AppendAllText(System.AppContext.BaseDirectory + "bans.txt", "\n" + userid);
            Log.Info("[ban] User " + userid + " has been banned!");
        }

        public static void Remove(string userid)
        {
            if (!File.Exists(System.AppContext.BaseDirectory + "bans.txt"))
            {
                File.AppendAllText(System.AppContext.BaseDirectory + "bans.txt", "# ---------------- Private DM related bans ----------------\n# This file is written by the DiscordAMX program itself, since it uses it own private dm ban protection.\n# If you want to add a user manually, simply add his id in a new separate line at the bottom.");
                return;
            }

            string[] whole_file = File.ReadAllLines(System.AppContext.BaseDirectory + "bans.txt");
            bool found = false;
            foreach (string line in whole_file)
            {
                if (line.StartsWith("#")) continue;
                if (line.Contains(userid))
                {
                    found = true;
                    whole_file = whole_file.Where(o => o != line).ToArray();
                    Log.Info("[ban] User " + userid + " has been removed from the banlist!");
                }
            }

            if (found)
            {
                using (TextWriter writer = File.CreateText(System.AppContext.BaseDirectory + "bans.txt")) foreach (string line in whole_file) writer.WriteLine(line);
            }
            else Log.Error("[ban] User " + userid + " has not been banned yet!");


        }

        public static bool IsBanned(string userid)
        {
            if (!File.Exists(System.AppContext.BaseDirectory + "bans.txt"))
            {
                File.AppendAllText(System.AppContext.BaseDirectory + "bans.txt", "# ---------------- Private DM related bans ----------------\n# This file is written by the DiscordAMX program itself, since it uses it own private dm ban protection.\n# If you want to add a user manually, simply add his id in a new separate line at the bottom.");
                return false;
            }

            string[] whole_file = File.ReadAllLines(System.AppContext.BaseDirectory + "bans.txt");
            bool found = false;
            foreach (string line in whole_file)
            {
                if (line.StartsWith("#")) continue;
                if (line.Contains(userid)) found = true;
            }

            if (found) return true;
            else return false;
        }
    }
}
