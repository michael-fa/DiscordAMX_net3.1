using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using DSharpPlus.Interactivity.Extensions;
using AMXWrapper;
using System.Diagnostics;

namespace dcamx.Discord.Events
{
    public static class GuildDownloadCompleted
    {
        public static Task Execute(DiscordClient c, GuildDownloadCompletedEventArgs a)
        {

            //Getting the first and only discord ID (for now)
            if (a.Guilds.Count == 0)
            {
                Utils.Log.WriteLine("Script main has no valid Discord Server ID declared!\nMake sure u are using DC_SetGuild() in script main!");
                return Task.CompletedTask;
            }


            //This piece of code is for the AMX (Scripting) side.. so you can work with AMX Int cells that basically make it way easier to script 
            //and work with instead of long hashes. 
            DiscordGuild guild;

            bool suc = Program.m_Discord.Client.Guilds.TryGetValue(Convert.ToUInt64(Program.m_GuildID), out guild);
            Program.m_ScriptMembers.Add(new Scripting.Member(guild.Owner)); // <- The Owner somehow is not included in guild.Members .. Owner is separate member of the guild class instance.


            //Getting all members every bot startup and to make scripters happy.
            foreach (DiscordMember mem in guild.Members.Values)
            {
                Program.m_ScriptMembers.Add(new Scripting.Member(mem));
            }


            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.amx.FindPublic("OnInit");
                if (p != null)
                {
                    p.Execute();

                }
            }


            return Task.CompletedTask;
        }
    }
}
