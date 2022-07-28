using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus;
using System.Threading.Tasks;

namespace dcamx.Scripting.Natives
{
    public static class ChannelNatives
    {
        public static int DC_DeleteMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                var channel = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                dcamx.Discord.Events.MessageActions.SkipDeleteEvent = true;

                //null exception; are we looking for an thread?
                if (channel == null)
                {
                    foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                    {
                        if (dtc.Id == Convert.ToUInt64(args1[1].AsString()))
                            dtc.GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result.DeleteAsync().Wait();
                    }
                }
                else channel.GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result.DeleteAsync(args1[3].AsString()).Wait();

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeleteMessage' (Invalid Channel ID, wrong ID format, or you have not the right role permissions)", caller_script);
            }
            return 1;
        }

        public static int DC_SendEmbed(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                var msg = guild.GetChannel(Convert.ToUInt64(args1[1].AsString())).SendMessageAsync(embed: new DiscordEmbedBuilder
                {
                    Title = args1[3].AsString(),
                    Description = args1[4].AsString(),
                    ImageUrl = args1[2].AsString(),
                    Color = DiscordColor.Blue

                });

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessage' (Invalid Channel, wrong ID format, or you have not the right role permissions)", caller_script);
            }

            return 1;
        }


        public static int DC_SendChannelMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                var channel = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));

                //null exception; are we looking for an thread?
                if (channel == null)
                {
                    foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                    {
                        if (dtc.Id == Convert.ToUInt64(args1[1].AsString()))
                            dtc.SendMessageAsync(args1[2].AsString());
                    }
                }
                else channel.SendMessageAsync(args1[2].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessage' (Invalid Channel, wrong ID format, or you have not the right role permissions)", caller_script);
            }
            return 1;
        }

        public static int DC_SendChannelMessageEx(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                var channel = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                DiscordMessage msg = null;
                //null exception; are we looking for an thread?
                if (channel == null)
                {
                    foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                    {
                        if (dtc.Id == Convert.ToUInt64(args1[1].AsString()))
                            msg = dtc.SendMessageAsync(args1[2].AsString()).Result;
                    }
                }
                else msg = channel.SendMessageAsync(args1[2].AsString()).Result;



                if (args1.Length == 4)
                {
                    AMX.SetString(args1[3].AsCellPtr(), msg.Id.ToString(), true);
                }

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessageEx' (Invalid Channel, wrong ID format, or you have not the right role permissions)", caller_script);
            }
            return 1;
        }

        public static int DC_SendPrivateMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                Task<DiscordMessage> msg = dc.SendMessageAsync(args1[1].AsString());


            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendPrivateMessage' (Invalid pm channel, wrong ID format)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_SendPrivateMessageEx(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 2) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                Task<DiscordMessage> msg = dc.SendMessageAsync(args1[1].AsString());
                if (args1.Length == 3)
                {
                    AMX.SetString(args1[2].AsCellPtr(), msg.Result.Id.ToString(), true);
                }

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendPrivateMessageEx' (Invalid pm channel, wrong ID format)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_DeletePrivateMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                Task<DiscordMessage> x = dc.GetMessageAsync(Convert.ToUInt64(args1[1].AsString()));
                if (x == null) return 0;
                //if (x.Result.Author != Program.m_Discord.Client.CurrentUser) return 0;
                x.Result.DeleteAsync(args1[2].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeletePrivateMessage' (MESSAGE NOT FOUND, Invalid pm channel, wrong ID format)", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_FindChannel(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                foreach (DiscordChannel dc in guild.Channels.Values)
                {
                    if (dc.Name.Equals(args1[1].AsString()))
                    {
                        AMX.SetString(args1[2].AsCellPtr(), dc.Id.ToString(), true);
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_FindChannel'", caller_script);
                return 0;
            }
            return 0;
        }

        public static int DC_GetChannelName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                DiscordChannel ch = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                AMX.SetString(args1[2].AsCellPtr(), ch.Name, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetChannelName'", caller_script);
                return 0;
            }
            return 0;
        }

        public static int DC_GetChannelTopic(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                DiscordChannel ch = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                if (ch.Type == ChannelType.Voice) return 0;
                AMX.SetString(args1[2].AsCellPtr(), ch.Topic, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetChannelTopic'", caller_script);
                return 0;
            }
            return 0;
        }
        public static int DC_GetChannelMention(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                DiscordChannel ch = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                AMX.SetString(args1[2].AsCellPtr(), ch.Mention, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetChannelMention'", caller_script);
                return 0;
            }
            return 0;
        }

        public static int DC_GetChannelType(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
                DiscordChannel ch = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));

                //null exception; are we looking for an thread?
                if (ch == null)
                {
                    foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                    {
                        if (dtc.Id == Convert.ToUInt64(args1[1].AsString())) return (int)dtc.Type;
                    }
                }
                else return (int)ch.Type;
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetChannelType'", caller_script);
                return 0;
            }
            return 0;
        }

        public static int DC_CreateChannel(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 6) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                //0 = guildid
                //1 = channel nameFINI_Read
                //2 = channel type
                //3 = parent id
                //4 = topic
                //5 = nsfw
                if (args1[1].AsString().Length == 0 || args1[3].AsString().Length == 0) return 0;
                if (args1[2].AsInt32() > 7 || args1[2].AsInt32() < 0) return 0;

                DiscordChannel pdc = null;
                foreach (DiscordChannel x in guild.Channels.Values)
                {
                    if (x.Id == Convert.ToUInt64(args1[3].AsString()))
                    {
                        pdc = x;
                        break;
                    }
                }
                guild.CreateChannelAsync(args1[1].AsString(), (ChannelType)args1[2].AsInt32(), pdc, args1[4].AsString(), null, null, null, Convert.ToBoolean(args1[5].AsInt32()));

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_CreateChannel' ", caller_script);
                return 0;
            }
            return 1;
        }

        public static int DC_DeleteChannel(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());

            try
            {
                if (args1[1].AsString().Length == 0) return 0;

                DiscordChannel pdc = null;
                foreach (DiscordChannel x in guild.Channels.Values)
                {
                    if (x.Id.ToString().Equals(args1[1].AsString()))
                    {
                        pdc = x;
                        break;
                    }
                }

                if (pdc == null) return 0;
                pdc.DeleteAsync(args1[2].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_DeleteChannel' ", caller_script);
                return 0;
            }
            return 1;
        }
    }
}
