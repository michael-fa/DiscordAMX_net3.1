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
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnMemberJoin");
            if (p != null)
            {
                p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Member));
                p.Execute();
                GC.Collect();
            }

            Program.m_ScriptMembers.Add(new Scripting.Member(arg.Member));

            return Task.CompletedTask;
        }

        public static Task Leave(DiscordClient c, GuildMemberRemoveEventArgs arg)
        {
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnMemberLeave");
            if (p != null)
            {
                p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Member));
                p.Execute();
                GC.Collect();
            }

            Utils.Scripting.DCMember_ScrMember(arg.Member).Remove();


            return Task.CompletedTask;
        }

    }
}
