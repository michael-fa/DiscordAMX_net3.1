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
            /*var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            */

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

            //Commands.RegisterCommands<Commands.AdminCommands>();

            //Initial steps of something
            //string gettet = Utils.HttpGet("https://api.altstats.net/api/v1/server/723");
            //var srvInfo = JsonConvert.DeserializeObject<Program.ServerInfo>(gettet);

            // Program.UserCount = srvInfo.Players;

            try
            {
                await Client.ConnectAsync();
            }
            catch (Exception ex)
            {
                Utilities.Log.Print(ex);
            }



            //await Task.Delay(-1);
        }

        public async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
        }

        private Task GuildDownloadCompleted(DiscordClient c,  GuildDownloadCompletedEventArgs a)
        {

            //Console.WriteLine("Bot ready | " + a.Guilds.Count + " Guilds.");
            /*Task InfoUpdate = new Task(() => UpdateServerInfo()
            );
            InfoUpdate.Start();*/
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnInit");
            p.Execute();
            return Task.CompletedTask;
        }

        /* private void UpdateServerInfo()
         {
             while (Program.runme)
             {
                 Console.WriteLine("OnServerInfoUpdate");
                 string gettet = Utils.HttpGet("https://api.altstats.net/api/v1/server/671"); //Unser 723
                 var srvInfo = JsonConvert.DeserializeObject<Program.ServerInfo>(gettet);
                 Program.UserCount = srvInfo.Players;

                 var act = new DiscordActivity("Spieler: " + robomonkey.Program.UserCount, ActivityType.Playing);
                 this.Client.UpdateStatusAsync(act);
                 Thread.Sleep(10000);
             }
         }*/

        private static Task OnHeartbeated(DiscordClient c, HeartbeatEventArgs e)
        {
            AMXPublic p = Program.Scripts[0].amx.FindPublic("OnHeartbeat");
            p.Execute();
            return Task.CompletedTask;
        }
    }
}
