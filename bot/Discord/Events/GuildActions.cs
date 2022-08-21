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
            p = Program.m_MainAMX.FindPublic("OnInit");
            if (p != null) p.Execute();

            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                if (scr.m_amxFile.Contains("main")) continue;
                scr._FSInit();
            }

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

        public static Task ChannelUpdated(DiscordClient c, ChannelUpdateEventArgs a)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnChannelUpdated");
                if (p != null)
                {
                    var tmp1 = p.AMX.Push(a.ChannelAfter.Id.ToString());
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


        public static Task ThreadCreated(DiscordClient c, ThreadCreateEventArgs a)
        {
            // if (!a.NewlyCreated) return Task.CompletedTask;

            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnThreadCreated");
                if (p != null)
                {
                    var tmp3 = p.AMX.Push(a.Thread.Id.ToString());

                    var tmp2 = p.AMX.Push(a.Parent.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp2);
                    p.AMX.Release(tmp3);
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task ThreadDeleted(DiscordClient c, ThreadDeleteEventArgs a)
        {
            // if (!a.NewlyCreated) return Task.CompletedTask;

            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnThreadDeleted");
                if (p != null)
                {
                    var tmp3 = p.AMX.Push(a.Thread.Id.ToString());

                    var tmp2 = p.AMX.Push(a.Parent.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp2);
                    p.AMX.Release(tmp3);
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task ThreadUpdated(DiscordClient c, ThreadUpdateEventArgs a)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnThreadUpdated");
                if (p != null)
                {
                    var tmp4 = p.AMX.Push(a.ThreadAfter.Name.ToString());
                    var tmp3 = p.AMX.Push(a.ThreadAfter.Id.ToString());

                    var tmp2 = p.AMX.Push(a.Parent.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp2);
                    p.AMX.Release(tmp3);
                    p.AMX.Release(tmp4);
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task ThreadMembersUpdated(DiscordClient c, ThreadMembersUpdateEventArgs a)
        {
            //This is fucking TRICKY!

            //Are there added members in this event?
            if (a.AddedMembers.Count > 0)
            {
                //Yes, now lets loop through each new member from this thread
                foreach (DiscordThreadChannelMember mem in a.AddedMembers)
                {
                    //Basically call the AMX public for each new member. (Another loop since we do this for all scripts loaded, obviously..)
                    AMXPublic p = null;
                    foreach (Scripting.Script scr in Program.m_Scripts)
                    {
                        p = scr.m_Amx.FindPublic("OnThreadMemberJoined"); //<-- call it every time
                        if (p != null)
                        {
                            p.AMX.Push(Utils.Scripting.DCMember_ScrMember(mem.Member, Utils.Scripting.DCGuild_ScrGuild(a.Guild)).m_ID);
                            var tmp3 = p.AMX.Push(a.Thread.Id.ToString());
                            var tmp2 = p.AMX.Push(a.Thread.Parent.Id.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                        }
                        p = null;
                    }
                }
            }
            //are there removed members in this event?
            else if (a.RemovedMembers.Count > 0)
            {
                //Yes, now lets loop through each deleted member from this thread
                foreach (DiscordThreadChannelMember mem in a.AddedMembers)
                {
                    //Calling the amx public for every member removed. This assures handling each one scriptsided.
                    AMXPublic p = null;
                    foreach (Scripting.Script scr in Program.m_Scripts)
                    {
                        p = scr.m_Amx.FindPublic("OnThreadMemberLeft"); //<-- call it for each removed member
                        if (p != null)
                        {
                            p.AMX.Push(Utils.Scripting.DCMember_ScrMember(mem.Member, Utils.Scripting.DCGuild_ScrGuild(a.Guild)).m_ID);
                            var tmp3 = p.AMX.Push(a.Thread.Id.ToString());
                            var tmp2 = p.AMX.Push(a.Thread.Parent.Id.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(a.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                        }
                        p = null;
                    }
                }
            }
           
            return Task.CompletedTask;
        }

        public static Task GuildUpdated(DiscordClient c, GuildUpdateEventArgs arg)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.m_Amx.FindPublic("OnGuildUpdated");
                if (p != null)
                {
                    var tmp1 = p.AMX.Push(arg.GuildAfter.Name);
                    var tmp2 = p.AMX.Push(arg.GuildBefore.Name);
                    p.AMX.Push(arg.GuildAfter.MemberCount);
                    var tmp4 = p.AMX.Push(arg.GuildAfter.Id.ToString());
                    p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.GuildAfter).m_ID);
                    p.Execute();
                    p.AMX.Release(tmp4);
                    p.AMX.Release(tmp2);
                    p.AMX.Release(tmp1);
                    GC.Collect();
                }
                p = null;
            }
            return Task.CompletedTask;
        }

        public static Task UserUpdated(DiscordClient c, GuildMemberUpdateEventArgs arg)
        {
            //Since retrieving the roles after they have been edited, the user still appears to have the same rules as before.
            //This is wrong, so we use an reference value and make sure once the roles have been updated, we actually can access
            //a correct IEnumerable List for the members roles. 

            var guild = Utils.Scripting.DCGuild_ScrGuild(arg.Guild);
            guild.m_ScriptMembers[Utils.Scripting.ScrMemberDCMember_ID(arg.Member, Utils.Scripting.DCGuild_ScrGuild(arg.Guild))].m_Roles  = arg.RolesAfter;
            return Task.CompletedTask;
        }
    }
}
