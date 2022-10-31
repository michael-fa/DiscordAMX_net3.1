using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace dcamx.Discord
{



    public class DCBot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public DCBot()
        {

            //BETA 1.0: Instead of calling the main.amx main entry we now read a bot.token file
            //that the bot-admin needs to setup right. This removes the need for the DC_SetBotToken native.

            if(!File.Exists(AppContext.BaseDirectory + "/bot.token"))
            {
                Utils.Log.Error("Failed to set up the bot's token! Make sure you've put your bot token in the 'bot.token' file!");
                File.Create(AppContext.BaseDirectory + "/bot.token");
                Program.StopSafely();
                return;
            }
            else
            {
                if(File.ReadAllText(AppContext.BaseDirectory + "/bot.token").Length == 0)
                {
                    Utils.Log.Error("Failed to set up the bot's token! Make sure you've put your bot token in the 'bot.token' file!");
                    Program.StopSafely();
                    return;
                }
                else Program.dConfig.Token = File.ReadAllText(AppContext.BaseDirectory + "/bot.token");
            }

            //But still call main() from first amx here!
            Program.m_MainAMX.ExecuteMain();
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
            Client.MessageUpdated                       += Events.MessageActions.MessageUpdated;
            Client.MessageDeleted                       += Events.MessageActions.MessageDeleted;
            Client.MessageReactionAdded                 += Events.MessageActions.ReactionAdded;
            Client.MessageReactionRemoved               += Events.MessageActions.ReactionRemoved;
            Client.GuildCreated                         += Events.GuildActions.GuildAdded;
            Client.GuildDeleted                         += Events.GuildActions.GuildRemoved;
            Client.GuildUpdated                         += Events.GuildActions.GuildUpdated;
            Client.ChannelCreated                       += Events.GuildActions.ChannelCreated;
            Client.ChannelDeleted                       += Events.GuildActions.ChannelDeleted;
            Client.ChannelUpdated                       += Events.GuildActions.ChannelUpdated;
            Client.ThreadCreated                        += Events.GuildActions.ThreadCreated;
            Client.ThreadDeleted                        += Events.GuildActions.ThreadDeleted;
            Client.GuildMemberUpdated                   += Events.GuildActions.UserUpdated;
            Client.ThreadUpdated                        += Events.GuildActions.ThreadUpdated;
            Client.ThreadMembersUpdated                 += Events.GuildActions.ThreadMembersUpdated;



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
