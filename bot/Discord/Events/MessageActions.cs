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
using dcamx.Scripting;

namespace dcamx.Discord.Events
{
    public static class MessageActions
    {

        static public bool SkipDeleteEvent = false;
        static public bool SkipDeleteEvent_DM = false;
        public static Task MessageAdded(DiscordClient c, MessageCreateEventArgs arg)
        {
            if (arg.Author == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;
            if (arg.Channel.IsPrivate)
            {
                if (!Program.m_DmUsers.Contains((DiscordDmChannel)arg.Channel))
                    Program.m_DmUsers.Add((DiscordDmChannel)arg.Channel);



                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnPrivateMessage");
                    if (p != null)
                    {
                        var tmp2 = p.AMX.Push(arg.Message.Content);
                        var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp1 =  p.AMX.Push(arg.Channel.Id.ToString());
                        p.Execute();
                        p.AMX.Release(tmp1);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            else
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnChannelMessage");
                    if (p != null)
                    {
                        var tmp2 = p.AMX.Push(arg.Message.Content);
                        var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageDeleted(DiscordClient c, MessageDeleteEventArgs arg)
        {
            if (arg.Message.Channel.Type == ChannelType.Private)
            {
                if (SkipDeleteEvent_DM)
                {
                    SkipDeleteEvent_DM = false;
                    return Task.CompletedTask;
                }
                if (!Program.m_DmUsers.Contains((DiscordDmChannel)Program.m_Discord.Client.GetChannelAsync(arg.Message.ChannelId).Result))
                    Program.m_DmUsers.Add((DiscordDmChannel)arg.Message.Channel);

                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnPrivateMessageDeleted");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp2 = p.AMX.Push(arg.Message.ChannelId.ToString());
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        GC.Collect();
                    }
                }
            }
            
            else if (arg.Message.Channel.Type == ChannelType.Text)
            {
                if (SkipDeleteEvent)
                {
                    SkipDeleteEvent = false;
                    return Task.CompletedTask;
                }
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnChannelMessageDeleted");
                    if (p != null)
                    {
                        
                        var tmp = p.AMX.Push(arg.Message.Id.ToString());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                        p.Execute();
                        p.AMX.Release(tmp);
                        GC.Collect();
                    }
                }
            }


            return Task.CompletedTask;
        }

        public static Task ReactionAdded(DiscordClient c, MessageReactionAddEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (arg.User == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;

            //Is private channel?
            if (arg.Message.Channel == null)
            {
                if (!Program.m_DmUsers.Contains((DiscordDmChannel)arg.Channel))
                    Program.m_DmUsers.Add((DiscordDmChannel)arg.Channel);


                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnPrivateReactionAdded");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        var tmp1 = p.AMX.Push(arg.User.Id.ToString());
                        var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp3 = p.AMX.Push(arg.Emoji.GetDiscordName());


                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp1);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            else
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnReactionAdded");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Channel.Id.ToString());
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.User, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                        var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp3 = p.AMX.Push(arg.Emoji.GetDiscordName());
                        if (!arg.Message.Channel.IsPrivate) p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                        else
                        {
                            p.AMX.Push(-1);
                        }


                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            
            return Task.CompletedTask;
        }

        public static Task ReactionRemoved(DiscordClient c, MessageReactionRemoveEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public 
            if (arg.User == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;
            AMXPublic p = null;
            //Is private channel?
            if (arg.Channel == null)
            {
                if (!Program.m_DmUsers.Contains((DiscordDmChannel)arg.Message.Channel))
                    Program.m_DmUsers.Add((DiscordDmChannel)arg.Message.Channel);


                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnPrivateReactionRemoved");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        var tmp1 = p.AMX.Push(arg.User.Id.ToString());
                        var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp3 = p.AMX.Push(arg.Emoji.GetDiscordName());


                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp1);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);

                        GC.Collect();
                    }
                    p = null;
                }
            }
            else
            {
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnReactionRemoved");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Channel.Id.ToString());
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.User, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                        var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp3 = p.AMX.Push(arg.Emoji.GetDiscordName());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);



                        p.Execute();
                        p.AMX.Release(tmp3);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
