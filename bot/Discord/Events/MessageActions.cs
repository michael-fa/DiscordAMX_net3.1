using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using AMXWrapper;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace dcamx.Discord.Events
{
    public static class MessageActions 
    {
        static public bool SkipDeleteEvent = false;
        static public bool SkipDeleteEvent_DM = false;

        private static string UnicodeToUTF8(string strFrom)
        {
            byte[] bytes = Encoding.Default.GetBytes(strFrom);

            return Encoding.UTF8.GetString(bytes);

        }

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
                        var tmp1 = p.AMX.Push(arg.Message.Content);
                        var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp3 = p.AMX.Push(arg.Channel.Id.ToString());
                        p.Execute();
                        p.AMX.Release(tmp3);
                        p.AMX.Release(tmp1);
                        p.AMX.Release(tmp2);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            else if (arg.Channel.Type == ChannelType.Text)
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
                        p.AMX.Release(tmp3);
                        GC.Collect();
                        
                    }
                    p = null;
                }
            }
            else if (arg.Channel.Type == ChannelType.PublicThread)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnThreadMessage");
                    if (p != null)
                    {
                        var tmp2 = p.AMX.Push(arg.Message.Content);
                        var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                        p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                        var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                        var tmp4 = p.AMX.Push(arg.Message.Channel.Parent.ToString());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        p.AMX.Release(tmp3);
                        p.AMX.Release(tmp4);
                        GC.Collect();
                    }
                    p = null;
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageUpdated(DiscordClient c, MessageUpdateEventArgs arg)
        {
            if (arg.Author == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;
            if (arg.Channel.IsPrivate)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnPrivateMessageUpdated");
                    if (p != null)
                    {
                        if (arg.MessageBefore == null) //Need to figure out message caching, idk yet..
                        {
                            var tmp1 = p.AMX.Push(arg.Message.Content);
                            var tmp4 = p.AMX.Push("0");
                            var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                            var tmp3 = p.AMX.Push(arg.Message.Channel.Id.ToString());
                            p.Execute();
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp1);
                            p.AMX.Release(tmp2);
                            if (arg.MessageBefore != null) p.AMX.Release(tmp4);
                            GC.Collect();
                        }
                        else
                        {
                            var tmp1 = p.AMX.Push(arg.Message.Content);
                            var tmp4 = p.AMX.Push(arg.MessageBefore.Content);
                            var tmp2 = p.AMX.Push(arg.Message.Id.ToString());
                            var tmp3 = p.AMX.Push(arg.Message.Channel.Id.ToString());
                            p.Execute();
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp1);
                            p.AMX.Release(tmp2);
                            if (arg.MessageBefore != null) p.AMX.Release(tmp4);
                            GC.Collect();
                        }
                           
                    }
                    p = null;
                }
            }
            else if (arg.Channel.Type == ChannelType.Text)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnChannelMessageUpdated");
                    if (p != null)
                    {
                        if (arg.MessageBefore == null)
                        {
                            var tmp4 = p.AMX.Push(arg.Message.Content);
                            var tmp2 = p.AMX.Push("0");
                            var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                            p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                            var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp);
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp4);
                            GC.Collect();
                        }
                        else
                        {
                            var tmp4 = p.AMX.Push(arg.Message.Content);
                            var tmp2 = p.AMX.Push(arg.MessageBefore.Content);
                            var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                            p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                            var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp);
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp4);
                            GC.Collect();
                        }
                    }
                    p = null;
                }
            }
            else if (arg.Channel.Type == ChannelType.PublicThread)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnThreadMessageUpdated");
                    if (p != null)
                    {
                        if (arg.MessageBefore == null)
                        {
                            var tmp6 = p.AMX.Push(arg.Message.Content);
                            var tmp2 = p.AMX.Push("0");
                            var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                            p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                            var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                            var tmp4 = p.AMX.Push(arg.Message.Channel.Parent.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp);
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp4);
                            p.AMX.Release(tmp6);
                            GC.Collect();
                        }
                        else
                        {
                            var tmp6 = p.AMX.Push(arg.Message.Content);
                            var tmp2 = p.AMX.Push(arg.MessageBefore.Content);
                            var tmp3 = p.AMX.Push(arg.Message.Id.ToString());
                            p.AMX.Push(Utils.Scripting.ScrMemberDCMember_ID(arg.Author, Utils.Scripting.DCGuild_ScrGuild(arg.Guild)));
                            var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                            var tmp4 = p.AMX.Push(arg.Message.Channel.Parent.ToString());
                            p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                            p.Execute();
                            p.AMX.Release(tmp);
                            p.AMX.Release(tmp2);
                            p.AMX.Release(tmp3);
                            p.AMX.Release(tmp4);
                            p.AMX.Release(tmp6);
                            GC.Collect();
                        }
                        
                    }
                    p = null;
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageDeleted(DiscordClient c, MessageDeleteEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (c.CurrentUser == arg.Message.Author) return Task.CompletedTask;
            //if (arg.Message.Author == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;

            //Is private channel?
            if (arg.Message.Channel.IsPrivate)
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
            else if(arg.Message.Channel.Type == ChannelType.Text)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnChannelMessageDeleted");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp2 = p.AMX.Push(arg.Message.ChannelId.ToString());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
                        p.Execute();
                        p.AMX.Release(tmp);
                        p.AMX.Release(tmp2);
                        GC.Collect();
                    }
                }
            }
            else if (arg.Channel.Type == ChannelType.PublicThread)
            {
                AMXPublic p = null;
                foreach (Scripting.Script scr in Program.m_Scripts)
                {
                    p = scr.m_Amx.FindPublic("OnThreadMessageDeleted");
                    if (p != null)
                    {
                        var tmp = p.AMX.Push(arg.Message.Id.ToString());
                        var tmp2 = p.AMX.Push(arg.Message.ChannelId.ToString());
                        var tmp3 = p.AMX.Push(arg.Message.Channel.ParentId.ToString());
                        p.AMX.Push(Utils.Scripting.DCGuild_ScrGuild(arg.Guild).m_ID);
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


        public static Task ReactionAdded(DiscordClient c, MessageReactionAddEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (arg.User == Program.m_Discord.Client.CurrentUser) return Task.CompletedTask;

            //Is private channel?
            if (arg.Message.Channel.IsPrivate)
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
            if (arg.Message.Channel.IsPrivate)
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
