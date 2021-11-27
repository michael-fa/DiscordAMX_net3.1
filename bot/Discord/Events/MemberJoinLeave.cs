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
    public static class MemberJoinLeave
    {
        public static Task Join(DiscordClient c, GuildMemberAddEventArgs arg)
        {
            AMXPublic p = null;
            foreach (Scripting.Script scr in Program.m_Scripts)
            {
                p = scr.amx.FindPublic("OnMemberJoin");
                if (p != null)
                {
                    p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Member, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                    p.AMX.Push(arg.Guild.Id);
                    p.Execute();
                    GC.Collect();
                }
                p = null;
            }

            foreach (Scripting.Guild gld in Program.m_ScriptGuilds)
            {
                if (gld.m_DCGuild != arg.Guild) continue;
                gld.m_ScriptMembers.Add(new Scripting.Member(arg.Member, gld));
            }

            return Task.CompletedTask;
        }

        public static Task Leave(DiscordClient c, GuildMemberRemoveEventArgs arg)
        {
            if (arg.Member == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnMemberLeave");
            if (p != null)
            {
                p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Member, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                p.AMX.Push(arg.Guild.Id.ToString());
                p.Execute();
                GC.Collect();
            }

            Utils.Scripting.DCMember_ScrMember(arg.Member, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)).Remove();

            return Task.CompletedTask;
        }

    }
}
