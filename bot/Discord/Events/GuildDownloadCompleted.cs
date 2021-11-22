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
                Utils.Log.Info("There are currently no servers available. Standing by.");
                return Task.CompletedTask;
            }


            //Loop through all discord guilds, add them to Guilds list.
            //While that, all the discord members to Guild's Members list.

            foreach (DiscordGuild gld in Program.m_Discord.Client.Guilds.Values)
            {
                Scripting.Guild _guild = new Scripting.Guild(gld);
                Program.m_ScriptGuilds.Add(_guild);

                _guild.m_ScriptMembers.Add(new Scripting.Member(gld.Owner, _guild)); // <- The Owner somehow is not included in guild.Members .. Owner is a separate member of the guild class instance.
               
                //Add all the guilds members to the guild's ScriptMembers list
                foreach (DiscordMember mem in gld.Members.Values)
                {
                    _guild.m_ScriptMembers.Add(new Scripting.Member(mem, _guild));

                }
            }



            //Init main amx OnLoad
            AMXPublic p = null;
            p = Program.m_Scripts[0].amx.FindPublic("OnInit");
            if (p != null)
            {
                p.Execute();

            }



            return Task.CompletedTask;
        }
    }
}
