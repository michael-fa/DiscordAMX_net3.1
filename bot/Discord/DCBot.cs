using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
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
        }

        public async Task RunAsync(DiscordConfiguration dConfig)
        {
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
            Client.GuildDownloadCompleted               += Events.GuildActions.DownloadCompleted;
            Client.Heartbeated                          += Events.OnHeartbeated.Execute;
            Client.GuildMemberAdded                     += Events.MemberJoinLeave.Join;
            Client.GuildMemberRemoved                   += Events.MemberJoinLeave.Leave;
            Client.MessageCreated                       += Events.MessageActions.MessageAdded;
            Client.MessageDeleted                       += Events.MessageActions.MessageDeleted;
            Client.MessageReactionAdded                 += Events.MessageActions.ReactionAdded;
            Client.MessageReactionRemoved               += Events.MessageActions.ReactionRemoved;
            Client.GuildCreated                         += Events.GuildActions.GuildAdded;
            Client.GuildDeleted                         += Events.GuildActions.GuildRemoved;
            Client.GuildUpdated                         += Events.GuildActions.GuildUpdated;
            Client.ChannelCreated                       += Events.GuildActions.ChannelCreated;
            Client.ChannelDeleted                       += Events.GuildActions.ChannelDeleted;



            /*var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "?" },
                EnableMentionPrefix = true,
             
                EnableDms = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            */
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
    }
}
