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

namespace dcamx.Discord
{

    public class DCBot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        

        public DCBot()
        {
            Program.m_Scripts[0].amx.ExecuteMain();
            Program.m_ScriptingInited = true;
        }

        public async Task RunAsync(DiscordConfiguration dConfig)
        {
            //Is a guild set?
            if(Program.m_GuildID == null || Program.m_GuildID.Length == 0 )
            {
                Utils.Log.WriteLine("\nScript main needs SetGuild function called!\n");
                return;
            }

            //Try to create a new discord client, this is in the scope of the DC+' code.. 
            try
            {
                Client = new DiscordClient(dConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.Source + "\n" + ex.StackTrace);
                Utils.Log.Exception(ex);
                Program.StopSafely();
            }


            //Liten to all the Discord Events
            Client.GuildDownloadCompleted += GuildDownloadCompleted;
            Client.Heartbeated += OnHeartbeated;
            Client.GuildMemberAdded += OnMemberJoin;
            Client.GuildMemberRemoved += OnMemberLeave;
            Client.MessageCreated += OnMessage;
            Client.MessageDeleted += OnMessageDeleted;
            Client.MessageReactionAdded += OnReactionAdded;
            Client.MessageReactionRemoved += OnReactionRemoved;


            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "?" },
                EnableMentionPrefix = true,
             
                EnableDms = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            Client.UseInteractivity(new InteractivityConfiguration
            {

            });


            //Finally, connect the bot. Also, in the scope of DC+' code.
            try
            {
                await Client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.Source + "\n" + ex.StackTrace + ex.InnerException);
                Utils.Log.Exception(ex);
                Program.StopSafely();
            }
        }





        public async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
        }






        private Task GuildDownloadCompleted(DiscordClient c, GuildDownloadCompletedEventArgs a)
        {

            //Getting the first and only discord ID (for now)
            if(a.Guilds.Count == 0)
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





        private static Task OnHeartbeated(DiscordClient c, HeartbeatEventArgs e)
        {
            AMXPublic p = Program.m_Scripts[0].amx.FindPublic("OnHeartbeat");
            if (p != null) p.Execute();
            return Task.CompletedTask;
        }









        private Task OnMemberJoin(DiscordClient c, GuildMemberAddEventArgs arg)
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

        private Task OnMemberLeave(DiscordClient c, GuildMemberRemoveEventArgs arg)
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

        private Task OnMessage(DiscordClient c, MessageCreateEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (arg.Message.Author == this.Client.CurrentUser) return Task.CompletedTask;

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

        private Task OnMessageDeleted(DiscordClient c, MessageDeleteEventArgs arg)
        {
            //If the trigger was the bot itself, skip calling the public
            if (c.CurrentUser == arg.Message.Author) return Task.CompletedTask;
            if (arg.Message.Author == this.Client.CurrentUser) return Task.CompletedTask;
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

        private Task OnReactionAdded(DiscordClient c, MessageReactionAddEventArgs arg)
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

        private Task OnReactionRemoved(DiscordClient c, MessageReactionRemoveEventArgs arg)
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
