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
    public static class OnHeartbeated
    {
        public static Task Execute(DiscordClient c, HeartbeatEventArgs e)
        {
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnHeartbeat");
            p.AMX.Push(e.Ping);
            if (p != null) p.Execute();
            return Task.CompletedTask;
        }
    }
}
