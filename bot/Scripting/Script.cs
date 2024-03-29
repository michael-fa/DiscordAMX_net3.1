﻿using AMXWrapper;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dcamx.Scripting
{
    public class Script
    {
        public string m_amxFile = null;
        public AMX m_Amx;
        public bool m_isFs;


        public Script(string _amxFile, bool _isFilterscript = false)
        {
            this.m_amxFile = _amxFile;
            try
            {
                m_Amx = new AMX(_amxFile + ".amx");
                
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            this.m_Amx.LoadLibrary(AMXDefaultLibrary.Core | AMXDefaultLibrary.Float | AMXDefaultLibrary.String | AMXDefaultLibrary.Console | AMXDefaultLibrary.DGram | AMXDefaultLibrary.Time);
            this.RegisterNatives();

            if (!_isFilterscript) Program.m_MainAMX = this.m_Amx;


            Program.m_Scripts.Add(this);
            return;
        }

        public void StopAllTimers()
        {
            foreach(ScriptTimer timer in Program.m_ScriptTimers)
            {
                timer.KillTimer();
            }

        }

        public void _FSInit()
        {
                Utils.Log.Debug("Loading filterscript as ID: " + Program.m_Scripts.Count, this);
                AMXPublic p = this.m_Amx.FindPublic("OnFilterscriptInit");
                if (p != null) p.Execute();
        }

        /*public void RunTimerCallback(AMXArgumentList m_Args, string m_ArgFrmt, string m_Func)
        {
            AMXPublic m_AMXCallback = this.m_Amx.FindPublic(m_Func);
            if (m_AMXCallback == null) return;
            

            try
            {
                if (!m_ArgFrmt.Equals("Cx00A01"))
                {
                    int count = (m_Args.Length - 1);

                    List<CellPtr> Cells = new List<CellPtr>();

                    //Important so the format ( ex "iissii" ) is aligned with the arguments pushed to the callback, not being reversed
                    string reversed_format = Utils.Scripting.Reverse(m_ArgFrmt);

                    foreach (char x in reversed_format.ToCharArray())
                    {
                        if (count == 3) break; //stop at the format argument.
                        Console.WriteLine("Do again: " + count);
                        switch (x)
                        {
                            case 'i':
                                {
                                    m_AMXCallback.AMX.Push(4);
                                    count--;
                                    continue;
                                }
                            case 'f':
                                {
                                    m_AMXCallback.AMX.Push((float)m_Args[count].AsCellPtr().Get().AsFloat());
                                    count--;
                                    continue;
                                }

                            case 's':
                                {
                                    Cells.Add(m_AMXCallback.AMX.Push(m_Args[count].AsString()));
                                    count--;
                                    continue;
                                }
                        }
                    }

                    m_AMXCallback.Execute();

                    foreach (CellPtr cell in Cells)
                    {
                        m_AMXCallback.AMX.Release(cell);
                    }
                    GC.Collect();



                    Utils.Log.Debug("Script-Timer invoked \"" + m_Func + "\" | Format: " + m_ArgFrmt, this);
                }
                else
                {
                    //Call without ex arguments
                    m_AMXCallback.Execute();
                    Utils.Log.Debug("Script-Timer invoked  \"" + m_Func + "\"", this);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, this);
            }
        }*/

        public bool RegisterNatives()
        {
            m_Amx.Register("Loadscript", (amx1, args1) => Natives.CoreNatives.Loadscript(amx1, args1, this));
            m_Amx.Register("Unloadscript", (amx1, args1) => Natives.CoreNatives.Unloadscript(amx1, args1, this));
            m_Amx.Register("SetTimer", (amx1, args1) => Natives.CoreNatives.SetTimer(amx1, args1, this));
            //m_Amx.Register("SetTimerEx", (amx1, args1) => Natives.SetTimerEx(amx1, args1, this));
            m_Amx.Register("KillTimer", (amx1, args1) => Natives.CoreNatives.KillTimer(amx1, args1, this));
            m_Amx.Register("gettimestamp", (amx1, args1) => Natives.CoreNatives.gettimestamp(amx1, args1, this));
            m_Amx.Register("CallRemoteFunction", (amx1, args1) => Natives.CoreNatives.CallRemoteFunction(amx1, args1, this));
            m_Amx.Register("DC_GetBotPing", (amx1, args1) => Natives.CoreNatives.DC_GetBotPing(amx1, args1, this));

            m_Amx.Register("DC_SetMinLogLevel", (amx1, args1) => Natives.DiscordNatives.DC_SetMinLogLevel(amx1, args1, this));
            m_Amx.Register("DC_SetActivityText", (amx1, args1) => Natives.DiscordNatives.DC_SetActivityText(amx1, args1, this));

            //Native ini IO
            m_Amx.Register("INI_Delete", (amx1, args1) => Natives.ININatives.INI_Delete(amx1, args1, this));
            m_Amx.Register("INI_Open", (amx1, args1) => Natives.ININatives.INI_Open(amx1, args1, this));
            m_Amx.Register("INI_Close", (amx1, args1) => Natives.ININatives.INI_Close(amx1, args1, this));
            m_Amx.Register("INI_Read", (amx1, args1) => Natives.ININatives.INI_Read(amx1, args1, this));
            m_Amx.Register("INI_ReadInt", (amx1, args1) => Natives.ININatives.INI_ReadInt(amx1, args1, this));
            m_Amx.Register("INI_ReadFloat", (amx1, args1) => Cell.FromFloat(Natives.ININatives.INI_ReadFloat(amx1, args1, this)).AsCellPtr().Value.ToInt32());
            m_Amx.Register("INI_Write", (amx1, args1) => Natives.ININatives.INI_Write(amx1, args1, this));
            m_Amx.Register("INI_WriteInt", (amx1, args1) => Natives.ININatives.INI_WriteInt(amx1, args1, this));
            m_Amx.Register("INI_WriteFloat", (amx1, args1) => Natives.ININatives.INI_WriteFloat(amx1, args1, this));
            m_Amx.Register("INI_KeyExists", (amx1, args1) => Natives.ININatives.INI_KeyExists(amx1, args1, this));
            m_Amx.Register("INI_DeleteSection", (amx1, args1) => Natives.ININatives.INI_DeleteSection(amx1, args1, this));
            m_Amx.Register("INI_DeleteKey", (amx1, args1) => Natives.ININatives.INI_DeleteKey(amx1, args1, this));
            m_Amx.Register("INI_Exists", (amx1, args1) => Natives.ININatives.INI_Exists(amx1, args1, this));

            //Guilds
            m_Amx.Register("DC_GetGuildName", (amx1, args1) => Natives.GuildNatives.DC_GetGuildName(amx1, args1, this));
            m_Amx.Register("DC_GetGuildCount", (amx1, args1) => Natives.GuildNatives.DC_GetGuildCount(amx1, args1, this));

            //Members
            m_Amx.Register("DC_IsMemberValid", (amx1, args1) => Natives.UserNatives.DC_IsMemberValid(amx1, args1, this));
            m_Amx.Register("DC_GetMemberCount", (amx1, args1) => Natives.GuildNatives.DC_GetMemberCount(amx1, args1, this));
            m_Amx.Register("DC_GetMemberID", (amx1, args1) => Natives.UserNatives.DC_GetMemberID(amx1, args1, this));
            m_Amx.Register("DC_GetMemberName", (amx1, args1) => Natives.UserNatives.DC_GetMemberName(amx1, args1, this));
            m_Amx.Register("DC_GetMemberDisplayName", (amx1, args1) => Natives.UserNatives.DC_GetMemberDisplayName(amx1, args1, this));
            m_Amx.Register("DC_GetMemberDiscriminator", (amx1, args1) => Natives.UserNatives.DC_GetMemberDiscriminator(amx1, args1, this));
            m_Amx.Register("DC_BanMember", (amx1, args1) => Natives.UserNatives.DC_BanMember(amx1, args1, this));
            m_Amx.Register("DC_UnbanMember", (amx1, args1) => Natives.UserNatives.DC_UnbanMember(amx1, args1, this));
            m_Amx.Register("DC_TimeoutGuildMember", (amx1, args1) => Natives.UserNatives.DC_TimeoutMember(amx1, args1, this));
            m_Amx.Register("DC_GetMemberAvatarURL", (amx1, args1) => Natives.UserNatives.DC_GetMemberAvatarURL(amx1, args1, this));
            m_Amx.Register("DC_MemberHasRole", (amx1, args1) => Natives.UserNatives.DC_MemberHasRole(amx1, args1, this));
            m_Amx.Register("DC_SetMemberRole", (amx1, args1) => Natives.UserNatives.DC_SetMemberRole(amx1, args1, this));
            m_Amx.Register("DC_RevokeMemberRole", (amx1, args1) => Natives.UserNatives.DC_RevokeMemberRole(amx1, args1, this));
            m_Amx.Register("DC_BanUser", (amx1, args1) => Natives.UserNatives.DC_BanUser(amx1, args1, this));
            m_Amx.Register("DC_UnbanUser", (amx1, args1) => Natives.UserNatives.DC_UnbanUser(amx1, args1, this));
            m_Amx.Register("DC_IsUserBanned", (amx1, args1) => Natives.UserNatives.DC_IsUserBanned(amx1, args1, this));

            //Channels
            m_Amx.Register("DC_DeleteMessage", (amx1, args1) => Natives.ChannelNatives.DC_DeleteMessage(amx1, args1, this));
            m_Amx.Register("DC_SendMessage", (amx1, args1) => Natives.ChannelNatives.DC_SendMessage(amx1, args1, this));
            m_Amx.Register("DC_SendPrivateMessage", (amx1, args1) => Natives.ChannelNatives.DC_SendPrivateMessage(amx1, args1, this));
            m_Amx.Register("DC_SendEmbeddedImage", (amx1, args1) => Natives.ChannelNatives.DC_SendEmbeddedImage(amx1, args1, this));
            m_Amx.Register("DC_MessageValid", (amx1, args1) => Natives.ChannelNatives.DC_MessageValid(amx1, args1, this));

            m_Amx.Register("DC_NewEmbedBuilder", (amx1, args1) => Natives.ChannelNatives.DC_NewEmbedBuilder(amx1, args1, this));
            m_Amx.Register("DC_SetEmbedAuthor", (amx1, args1) => Natives.ChannelNatives.DC_SetEmbedAuthor(amx1, args1, this));
            m_Amx.Register("DC_SetEmbedFooter", (amx1, args1) => Natives.ChannelNatives.DC_SetEmbedFooter(amx1, args1, this));
            m_Amx.Register("DC_SetEmbedImage", (amx1, args1) => Natives.ChannelNatives.DC_SetEmbedImage(amx1, args1, this));
            m_Amx.Register("DC_SetEmbedUrl", (amx1, args1) => Natives.ChannelNatives.DC_SetEmbedUrl(amx1, args1, this));
            m_Amx.Register("DC_SetEmbedColor", (amx1, args1) => Natives.ChannelNatives.DC_SetEmbedColor(amx1, args1, this));
            m_Amx.Register("DC_AddEmbedText", (amx1, args1) => Natives.ChannelNatives.DC_AddEmbedText(amx1, args1, this));
            m_Amx.Register("DC_ToggleEmbedTimestamp", (amx1, args1) => Natives.ChannelNatives.DC_ToggleEmbedTimestamp(amx1, args1, this));
            m_Amx.Register("DC_SendEmbed", (amx1, args1) => Natives.ChannelNatives.DC_SendEmbed(amx1, args1, this));
            m_Amx.Register("DC_UpdateEmbed", (amx1, args1) => Natives.ChannelNatives.DC_UpdateEmbed(amx1, args1, this));
            
            m_Amx.Register("DC_DeletePrivateMessage", (amx1, args1) => Natives.ChannelNatives.DC_DeletePrivateMessage(amx1, args1, this));
            m_Amx.Register("DC_FindChannel", (amx1, args1) => Natives.ChannelNatives.DC_FindChannel(amx1, args1, this));
            m_Amx.Register("DC_FindThread", (amx1, args1) => Natives.ChannelNatives.DC_FindThread(amx1, args1, this));
            m_Amx.Register("DC_CreateChannel", (amx1, args1) => Natives.ChannelNatives.DC_CreateChannel(amx1, args1, this));
            m_Amx.Register("DC_DeleteChannel", (amx1, args1) => Natives.ChannelNatives.DC_DeleteChannel(amx1, args1, this));
            m_Amx.Register("DC_GetChannelName", (amx1, args1) => Natives.ChannelNatives.DC_GetChannelName(amx1, args1, this));
            m_Amx.Register("DC_GetChannelTopic", (amx1, args1) => Natives.ChannelNatives.DC_GetChannelTopic(amx1, args1, this));
            m_Amx.Register("DC_GetChannelMention", (amx1, args1) => Natives.ChannelNatives.DC_GetChannelMention(amx1, args1, this));
            m_Amx.Register("DC_GetChannelType", (amx1, args1) => Natives.ChannelNatives.DC_GetChannelType(amx1, args1, this));

            m_Amx.Register("DC_AddReaction", (amx1, args1) => Natives.DiscordNatives.DC_AddReaction(amx1, args1, this));
            m_Amx.Register("DC_AddPrivateReaction", (amx1, args1) => Natives.DiscordNatives.DC_AddPrivateReaction(amx1, args1, this));
            m_Amx.Register("DC_RemoveReaction", (amx1, args1) => Natives.DiscordNatives.DC_RemoveReaction(amx1, args1, this));
            m_Amx.Register("DC_RemovePrivateReaction", (amx1, args1) => Natives.DiscordNatives.DC_RemovePrivateReaction(amx1, args1, this));


            return true;
        }
    }
}
