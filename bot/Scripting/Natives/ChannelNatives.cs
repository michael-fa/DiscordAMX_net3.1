using AMXWrapper;
using DSharpPlus.Entities;
using System;
using DSharpPlus;
using System.Threading.Tasks;
using System.Diagnostics;

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
                        {
                            DiscordMessage msg = null;
                            msg = dtc.GetMessageAsync(Convert.ToUInt64(args1[2].AsString())).Result;
                            if(msg != null)
                            {
                                msg.DeleteAsync().Wait();
                            }
                        }
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

        public static int DC_SendEmbeddedImage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 5) return 0;

            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            DiscordMessage msg = null;
            try
            {
                if (guild == null)
                {
                    var channel = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[1].AsString())).Result;

                    msg = channel.SendMessageAsync(embed: new DSharpPlus.Entities.DiscordEmbedBuilder
                    {
                        Title = args1[3].AsString(),
                        Description = args1[4].AsString(),
                        ImageUrl = args1[2].AsString(),
                        Color = DiscordColor.Blue
                    }).Result;
                    return 1;
                }
                else
                {
                    DiscordChannel channel = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));

                    if (channel == null)
                    {
                        foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                        {
                            if (dtc.Id == Convert.ToUInt64(args1[1].AsString()))
                            {
                                msg = channel.SendMessageAsync(embed: new DSharpPlus.Entities.DiscordEmbedBuilder
                                {
                                    Title = args1[3].AsString(),
                                    Description = args1[4].AsString(),
                                    ImageUrl = args1[2].AsString(),
                                    Color = DiscordColor.Blue

                                }).Result;
                            }
                        }
                        return 1;
                    }

                    msg = channel.SendMessageAsync(embed: new DSharpPlus.Entities.DiscordEmbedBuilder
                    {
                        Title = args1[3].AsString(),
                        Description = args1[4].AsString(),
                        ImageUrl = args1[2].AsString(),
                        Color = DiscordColor.Blue

                    }).Result;
                }

                if (!args1[5].AsString().Equals("0")) AMX.SetString(args1[5].AsCellPtr(), msg.Id.ToString(), true);

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendEmbeddedImage' (Invalid Channel, wrong ID format, or you have not the right role permissions)", caller_script);
            }

            return 1;
        }

        public static int DC_NewEmbedBuilder(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 2) return 0;
            if (args1[0].AsString().Length == 0 || args1[1].AsString().Length == 0) return 0;
            var b = new Scripting.DiscordEmbedBuilder(args1[0].AsString(), args1[1].AsString());
            return b.m_ID;
        }
        public static int DC_SetEmbedAuthor(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 2) return 0;
            if (args1[1].AsString().Length == 0) return 0;
            string _Url = null;
            if (args1.Length > 1) _Url = args1[2].AsString();
            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.SetAuthor(args1[1].AsString(), _Url);
                break;
            }

            return 1;
        }

        public static int DC_SetEmbedFooter(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 2) return 0;
            if (args1[1].AsString().Length == 0) return 0;
            string _Url = null;
            if (args1.Length > 1) _Url = args1[2].AsString();
            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.SetFooter(args1[1].AsString(), _Url);
                break;
            }

            return 1;
        }

        public static int DC_SetEmbedImage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            if (args1[1].AsString().Length == 0) return 0;
            string _Url = null;
            if (args1.Length > 1) _Url = args1[2].AsString();
            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.SetImageUrl(args1[1].AsString());
                break;
            }

            return 1;
        }

        public static int DC_SetEmbedUrl(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            if (args1[1].AsString().Length == 0) return 0;
            string _Url = null;
            if (args1.Length > 1) _Url = args1[2].AsString();
            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.SetUrl(args1[1].AsString());
                break;
            }

            return 1;
        }

        public static int DC_SetEmbedColor(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            DiscordColor col = DiscordColor.Aquamarine;
            switch(args1[1].AsInt32())
            {
                case 0: col = DiscordColor.Aquamarine; break;
                case 1: col = DiscordColor.Azure; break;
                case 2: col = DiscordColor.Black; break;
                case 3: col = DiscordColor.Blue; break;
                case 4: col = DiscordColor.Blurple; break;
                case 5: col = DiscordColor.Brown; break;
                case 6: col = DiscordColor.Chartreuse; break;
                case 7: col = DiscordColor.CornflowerBlue; break;
                case 8: col = DiscordColor.Cyan; break;
                case 9: col = DiscordColor.DarkBlue; break;
                case 10: col = DiscordColor.DarkButNotBlack; break;
                case 11: col = DiscordColor.DarkGray; break;
                case 12: col = DiscordColor.DarkGreen; break;
                case 13: col = DiscordColor.DarkRed; break;
                case 14: col = DiscordColor.Gold; break;
                case 15: col = DiscordColor.Goldenrod; break;
                case 16: col = DiscordColor.Gray; break;
                case 17: col = DiscordColor.Grayple; break;
                case 18: col = DiscordColor.Green; break;
                case 19: col = DiscordColor.HotPink; break;
                case 20: col = DiscordColor.IndianRed; break;
                case 21: col = DiscordColor.LightGray; break;
                case 22: col = DiscordColor.Lilac; break;
                case 23: col = DiscordColor.Magenta; break;
                case 24: col = DiscordColor.MidnightBlue; break;
                case 25: col = DiscordColor.NotQuiteBlack; break;
                case 26: col = DiscordColor.Orange; break;
                case 27: col = DiscordColor.PhthaloBlue; break;
                case 28: col = DiscordColor.PhthaloGreen; break;
                case 29: col = DiscordColor.Purple; break;
                case 30: col = DiscordColor.Red; break;
                case 31: col = DiscordColor.Rose; break;
                case 32: col = DiscordColor.SapGreen; break;
                case 33: col = DiscordColor.Sienna; break;
                case 34: col = DiscordColor.SpringGreen; break;
                case 35: col = DiscordColor.Teal; break;
                case 36: col = DiscordColor.Turquoise; break;
                case 37: col = DiscordColor.VeryDarkGray; break;
                case 38: col = DiscordColor.Violet; break;
                case 39: col = DiscordColor.Wheat; break;
                case 40: col = DiscordColor.White; break;
                case 41: col = DiscordColor.Yellow; break;
                case 42: col = DiscordColor.Violet; break;
            }


            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.SetColor(col);
                break;
            }

            return 1;
        }

        public static int DC_AddEmbedText(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;
            if (args1[1].AsString().Length == 0) return 0;
            if (args1[2].AsString().Length == 0) return 0;

            if (args1[3].AsInt32() < 0 || args1[3].AsInt32() > 1) return 0;

            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.AddText(args1[1].AsString(), args1[2].AsString(), Convert.ToBoolean(args1[3].AsInt32()));
                break;
            }

            return 1;
        }

        public static int DC_ToggleEmbedTimestamp(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.ToggleTimestamp();
                break;
            }

            return 1;
        }

        public static int DC_SendEmbed(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;
            if (args1[2].AsString().Length == 0) return 0;
            DiscordChannel channel = null;
            if (args1[1].AsInt32() == 0)
            {
                DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[1].AsInt32());

                channel = guild.GetChannel(Convert.ToUInt64(args1[2].AsString()));


                if (channel == null)
                {
                    foreach (DiscordThreadChannel dtc in guild.Threads.Values)
                    {
                        if (dtc.Id == Convert.ToUInt64(args1[1].AsString()))
                        {
                            channel = dtc;
                        }
                    }
                    return 1;
                }
            }
            else
            {
                channel = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[2].AsString())).Result;
            }
            if (channel == null) return 0;

            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.Send(channel);
                break;
            }
            return 1;
        }

        public static int DC_UpdateEmbed(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;

            foreach (Scripting.DiscordEmbedBuilder x in Program.m_Embeds)
            {
                if (x.m_ID != args1[0].AsInt32()) continue;
                x.Update();
                break;
            }
            return 1;
        }

        public static int DC_SendMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
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
                        {
                            msg = dtc.SendMessageAsync(args1[2].AsString()).Result;
                        }
                    }
                }
                else msg = channel.SendMessageAsync(args1[2].AsString()).Result;

                if (!args1[3].AsString().Equals("0")) AMX.SetString(args1[3].AsCellPtr(), msg.Id.ToString(), true);

            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendChannelMessage' (Invalid Channel, wrong ID format, or you have not the right role permissions)", caller_script);
            }
            return 1;
        }

        public static int DC_SendPrivateMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 2) return 0;
            try
            {
                DiscordChannel dc = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[0].AsString())).Result;
                DiscordMessage msg = dc.SendMessageAsync(args1[1].AsString()).Result;

                if (!args1[2].AsString().Equals("0"))
                {
                    AMX.SetString(args1[2].AsCellPtr(), msg.Id.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_SendPrivateMessage' (Invalid pm channel, wrong ID format)", caller_script);
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
                //No guild found, check for dm channel
                if(guild == null)
                {
                    foreach (DiscordDmChannel dc in Program.m_DmUsers)
                    {
                        if (dc.Name.Equals(args1[1].AsString()))
                        {
                            AMX.SetString(args1[2].AsCellPtr(), dc.Id.ToString(), true);
                            return 1;
                        }
                    }
                    return 0;
                }


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

        public static int DC_FindThread(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            DiscordGuild guild = Utils.Scripting.ScrGuild_DCGuild(args1[0].AsInt32());
            try
            {
                foreach (DiscordThreadChannel dc in guild.Threads.Values)
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
                Utils.Log.Error("In native 'DC_FindThread'", caller_script);
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
                if (ch == null)
                {
                    foreach (DiscordThreadChannel dc in guild.Threads.Values)
                    {
                        if (dc.Id == Convert.ToUInt64(args1[1].AsString()))
                        {
                            AMX.SetString(args1[2].AsCellPtr(), dc.Name, true);
                            return 1;
                        }
                    }
                    return 0;
                }

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
                if (ch == null) return 0;

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
                if(guild == null)
                {
                    DiscordChannel dmch = Program.m_Discord.Client.GetChannelAsync(Convert.ToUInt64(args1[1].AsString())).Result;
                    if (dmch == null) return 0;
                    AMX.SetString(args1[2].AsCellPtr(), dmch.Mention, true);
                    return 1;
                }
                DiscordChannel ch = guild.GetChannel(Convert.ToUInt64(args1[1].AsString()));
                if (ch == null)
                {
                    foreach (DiscordThreadChannel dc in guild.Threads.Values)
                    {
                        if (dc.Id == Convert.ToUInt64(args1[1].AsString()))
                        {
                            AMX.SetString(args1[2].AsCellPtr(), dc.Mention, true);
                            return 1;
                        }
                    }
                    return 0;
                }
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
                    return 0;
                }
                else return (int)ch.Type;
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetChannelType'", caller_script);
                return 0;
            }
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
