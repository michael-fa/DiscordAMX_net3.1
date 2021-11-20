using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using MySql.Data.MySqlClient;
using DSharpPlus.Interactivity.Extensions;
using AMXWrapper;
using System.Windows.Forms;

namespace dcamx.Discord.Commands
{
    class AdminCommands : BaseCommandModule
    {

        /*        [Command("spieler")]
                [Description("Zeigt die Spieleranzahl an.")]
                public async Task CmdPrintPlayerCount(CommandContext ctx)
                {
                    await ctx.Channel.SendMessageAsync("Es sind " + Program.UserCount.ToString() + " Affen auf Monkey Reallife.").ConfigureAwait(false);
                }

                [Command("restart")]
                [Description("Restartet den Bot.")]
                public async Task CmdRestartBot(CommandContext ctx)
                {

                    await ctx.Channel.SendMessageAsync("Ich werde mich nun neu verbinden.").ConfigureAwait(false);
                    await ctx.Client.ReconnectAsync(true).ConfigureAwait(false);
                }

                */
        /*
        [Command("whitelist")]
        [Description("Fügt einen Spieler in der Whitelist hinzu oder entfernt ihn.")]
        public async Task ACMD_Whitelist(CommandContext ctx, params string[] action)
        {
            if (action.GetValue(0).Equals("add"))
            {
                await ctx.Channel.SendMessageAsync("BEFEHL-PAPAMS: " + action[0] + " | " + action[1] + " | Wird hinzugefügt.").ConfigureAwait(false);

                var cmd = new MySqlCommand();
                cmd.CommandText = "INSERT INTO whitelist(id, name) VALUES(0, '" + action[1] + "')";
                Console.WriteLine(cmd.CommandText);
                cmd.Connection = Program.myCONN;
                cmd.ExecuteNonQuery();

            }
            else if(action.GetValue(0).Equals("ban"))
            {
                DBMysqlUtils.Update("whitelist", "banned", 1, "name = '" +  action[1].ToString() + "'");
                await ctx.Channel.SendMessageAsync("Von der Whitelist gebannt").ConfigureAwait(false);
            }
        }*/
    }
}
