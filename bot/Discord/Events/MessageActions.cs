﻿using System;
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
    public static class MessageActions
    {
        public static Task MessageAdded(DiscordClient c, MessageCreateEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (arg.Message.Author == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;

            if (arg.Message.Content.StartsWith("/"))
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.amx.FindPublic("OnCommand");
                    if (p != null)
                    {
                        var tmp2 = p.AMX.Push(arg.Message.Content.Remove(0, 1));
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author));
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        GC.Collect();
                    }

                }

            }
            else
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.amx.FindPublic("OnMessage");
                    if (p != null)
                    {

                        var tmp2 = p.AMX.Push(arg.Message.Content);
                        var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author));
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);
                        GC.Collect();
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageDeleted(DiscordClient c, MessageDeleteEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (c.CurrentUser == arg.Message.Author) return Task.CompletedTask;
            if (arg.Message.Author == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;

            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnMessageDeleted");
            if (p != null)
            {
                var tmp = p.AMX.Push(arg.Message.Id.ToString());
                p.Execute();
                p.AMX.Release(tmp);
                GC.Collect();
            }
            return Task.CompletedTask;
        }


        public static Task ReactionAdded(DiscordClient c, MessageReactionAddEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (c.CurrentUser == arg.Message.Author) return Task.CompletedTask;
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnReactionAdded");
            if (p != null)
            {
                var tmp = p.AMX.Push(arg.Channel.Id.ToString());
                p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.User));
                var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                var tmp3 = p.AMX.Push(arg.Emoji.Id.ToString());


                p.Execute();
                p.AMX.Release(tmp);
                p.AMX.Release(tmp2);
                p.AMX.Release(tmp3);
                GC.Collect();
            }
            return Task.CompletedTask;
        }

        public static Task ReactionRemoved(DiscordClient c, MessageReactionRemoveEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (c.CurrentUser == arg.Message.Author) return Task.CompletedTask;
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnReactionRemoved");
            if (p != null)
            {
                var tmp = p.AMX.Push(arg.Channel.Id.ToString());
                p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.User));
                var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                var tmp3 = p.AMX.Push(arg.Emoji.Id.ToString());


                p.Execute();
                p.AMX.Release(tmp);
                p.AMX.Release(tmp2);
                p.AMX.Release(tmp3);
                GC.Collect();
            }
            return Task.CompletedTask;
        }
    }
}