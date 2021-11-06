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

namespace bot.Discord
{

    public class DCBot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        


        public bool WaitingForResponse = false;

        

        public DCBot()
        {
            Program.Scripts[0].amx.ExecuteMain();
        }

        public async Task RunAsync(DiscordConfiguration dConfig)
        {
            if(Program.m_GuildID == null || Program.m_GuildID.Length == 0 )
            {
                Utilities.Log.WriteLine("\nScript main needs SetGuild function called!\n");
                return;
            }

            try
            {
                Client = new DiscordClient(dConfig);
            }
            catch (Exception ex)
            {
                Utilities.Log.Print(ex);
            }


            Client.GuildDownloadCompleted += GuildDownloadCompleted;
            Client.Heartbeated += OnHeartbeated;
            Client.GuildMemberAdded += OnMemberJoin;
            Client.GuildMemberRemoved += OnMemberLeave;
            Client.MessageCreated += OnMessage;

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

            try
            {
                await Client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Utilities.Log.Print(ex);
            }
        }



        public async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
        }

        private Task GuildDownloadCompleted(DiscordClient c, GuildDownloadCompletedEventArgs a)
        {

            if(a.Guilds.Count == 0)
            {
                Utilities.Log.WriteLine("Script main has no valid Discord Server ID declared!\nMake sure u are using DC_SetGuild() in script main!");
                return Task.CompletedTask;
            }


            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnInit");
            if (p != null)
            {
                p.Execute();
                
            }
            return Task.CompletedTask;
        }

        private static Task OnHeartbeated(DiscordClient c, HeartbeatEventArgs e)
        {
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnHeartbeat");
            if (p == null) return Task.CompletedTask;
            p.Execute();
            return Task.CompletedTask;
        }




















        private Task OnMemberJoin(DiscordClient c, GuildMemberAddEventArgs arg)
        {
            // Console.WriteLine("\n\nmember joined " + arg.Member.Id.ToString() + "\n\n");
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnMemberJoin");
            if (p != null)
            {
                var tmp = p.AMX.Push(arg.Member.Id.ToString());
                p.Execute();
                p.AMX.Release(tmp);
                GC.Collect();
            }

            return Task.CompletedTask;
        }

        private Task OnMemberLeave(DiscordClient c, GuildMemberRemoveEventArgs arg)
        {
            //  Console.WriteLine("\n\nmember joined " + arg.Member.Id.ToString() + "\n\n");
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnMemberLeave");
            if (p != null)
            {
                var tmp = p.AMX.Push(arg.Member.Id.ToString());
                p.Execute();
                p.AMX.Release(tmp);
                GC.Collect();
            }

            return Task.CompletedTask;
        }

        private Task OnMessage(DiscordClient c, MessageCreateEventArgs arg)
        {
            //  Console.WriteLine("\n\nmember joined " + arg.Member.Id.ToString() + "\n\n");
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnMessage");
            if (p != null)
            {
                var tmp2 = p.AMX.Push(arg.Message.Content);
                var tmp1 = p.AMX.Push(arg.Message.Author.Id.ToString());
                var tmp = p.AMX.Push(arg.Message.ChannelId.ToString());
                p.Execute();
                p.AMX.Release(tmp);
                p.AMX.Release(tmp1);
                p.AMX.Release(tmp2);
                GC.Collect();
            }

            return Task.CompletedTask;
        }
    }
}
