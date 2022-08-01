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
    public static class GuildActions
    {

        public static Task DownloadCompleted(DiscordClient c, GuildDownloadCompletedEventArgs a)
        {
            //Getting the first and only discord ID (for now)
            if (a.Guilds.Count == 0)
            {
                Utils.Log.Info("There are currently no servers available. Standing by.");
                return Task.CompletedTask;
            }


            //AT THIS POINT, SKIP IF ALREADY DONE.
            if (Program.m_ScriptingInited) return Task.CompletedTask;

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
            p = Program.m_Scripts[0].m_Amx.FindPublic("OnInit");
            if (p != null) p.Execute();

            //While DownloadCompleted gets called more then once, we need only the first call (which we call our main script init, since atp all guilds and members are now available to script.)
            Program.m_ScriptingInited = true;

            return Task.CompletedTask;
        }

        public static Task GuildAdded(DiscordClient c, GuildCreateEventArgs a)
        {

            Scripting.Guild _guild = new Scripting.Guild(a.Guild);
            Program.m_ScriptGuilds.Add(_guild);

            _guild.m_ScriptMembers.Add(new Scripting.Member(a.Guild.Owner, _guild)); // <- The Owner somehow is not included in guild.Members .. Owner is a separate member of the guild class instance.

            //Add all the guilds members to the guild's ScriptMembers list
            foreach (DiscordMember mem in a.Guild.Members.Values)
            {
                _guild.m_ScriptMembers.Add(new Scripting.Member(mem, _guild));

            }


            if (!Program.m_ScriptingInited) return Task.CompletedTask;
            try
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnGuildAdded");
                    if (p != null)
                    {
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                        p.Execute();
                    }
                    p = null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
            }


            return Task.CompletedTask;
        }

        public static Task GuildRemoved(DiscordClient c, GuildDeleteEventArgs a)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnGuildRemoved");
                if (p != null)
                {
                    var tmp1 = p.AMX.Push(a.Guild.Name);
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp1);
                }
                p = null;
            }


            Program.m_ScriptGuilds.Remove(Utils.Scripting.DCGuild_ScrGuild(a.Guild));

            return Task.CompletedTask;
        }

        public static Task ChannelCreated(DiscordClient c, ChannelCreateEventArgs a)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnChannelCreated");
                if (p != null)
                {
                    p.AMX.Push((a.Channel.IsPrivate ? (1) : (0)));
                    var tmp1 = p.AMX.Push(a.Channel.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp1);
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task ChannelDeleted(DiscordClient c, ChannelDeleteEventArgs a)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnChannelDeleted");
                if (p != null)
                {
                    p.AMX.Push((a.Channel.IsPrivate ? (1) : (0)));
                    var tmp1 = p.AMX.Push(a.Channel.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp1);
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task GuildUpdated(DiscordClient c, GuildUpdateEventArgs a)
        {
            //What to pass to script:
            /* new ID, old ID
             * new NAME, old NAME
             * new MaxMembers, old MaxMembers
             * new Description, old Description
             * new MembCount, old Membcount
            */
            return Task.CompletedTask;
        }
    }
}
