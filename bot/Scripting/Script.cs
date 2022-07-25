using AMXWrapper;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                m_Amx = new AMX("Scripts/" + _amxFile + ".amx");
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            this.m_Amx.LoadLibrary(AMXDefaultLibrary.Core | AMXDefaultLibrary.Float | AMXDefaultLibrary.String | AMXDefaultLibrary.Console | AMXDefaultLibrary.DGram | AMXDefaultLibrary.Time);
            this.RegisterNatives();

            if (_isFilterscript)
            {
                this.m_isFs = _isFilterscript;
                Utils.Log.Debug("Loading filterscript as ID: " + Program.m_Scripts.Count, this);
                AMXPublic p = this.m_Amx.FindPublic("OnFilterscriptInit");
                if (p != null)p.Execute();
            }

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
            m_Amx.Register("printc", (amx1, args1) => Natives.printc(amx1, args1, this));

            m_Amx.Register("Loadscript", (amx1, args1) => Natives.Loadscript(amx1, args1, this));
            m_Amx.Register("Unloadscript", (amx1, args1) => Natives.Unloadscript(amx1, args1, this));
            m_Amx.Register("SetTimer", (amx1, args1) => Natives.SetTimer(amx1, args1, this));
            //m_Amx.Register("SetTimerEx", (amx1, args1) => Natives.SetTimerEx(amx1, args1, this));
            m_Amx.Register("KillTimer", (amx1, args1) => Natives.KillTimer(amx1, args1, this));
            m_Amx.Register("gettimestamp", (amx1, args1) => Natives.gettimestamp(amx1, args1, this));
            m_Amx.Register("CallRemoteFunction", (amx1, args1) => Natives.CallRemoteFunction(amx1, args1, this));

            m_Amx.Register("DC_SetMinLogLevel", (amx1, args1) => Natives.DC_SetMinLogLevel(amx1, args1, this));
            m_Amx.Register("DC_SetActivityText", (amx1, args1) => Natives.DC_SetActivityText(amx1, args1, this));
            m_Amx.Register("DC_SetToken", (amx1, args1) => Natives.DC_SetToken(amx1, args1, this));

            //Native ini IO
            m_Amx.Register("INI_Delete", (amx1, args1) => Natives.INI_Delete(amx1, args1, this));
            m_Amx.Register("INI_Open", (amx1, args1) => Natives.INI_Open(amx1, args1, this));
            m_Amx.Register("INI_Close", (amx1, args1) => Natives.INI_Close(amx1, args1, this));
            m_Amx.Register("INI_Read", (amx1, args1) => Natives.INI_Read(amx1, args1, this));
            m_Amx.Register("INI_ReadInt", (amx1, args1) => Natives.INI_ReadInt(amx1, args1, this));
            m_Amx.Register("INI_ReadFloat", (amx1, args1) => Cell.FromFloat(Natives.INI_ReadFloat(amx1, args1, this)).AsCellPtr().Value.ToInt32());
            m_Amx.Register("INI_Write", (amx1, args1) => Natives.INI_Write(amx1, args1, this));
            m_Amx.Register("INI_WriteInt", (amx1, args1) => Natives.INI_WriteInt(amx1, args1, this));
            m_Amx.Register("INI_WriteFloat", (amx1, args1) => Natives.INI_WriteFloat(amx1, args1, this));
            m_Amx.Register("INI_KeyExists", (amx1, args1) => Natives.INI_KeyExists(amx1, args1, this));
            m_Amx.Register("INI_DeleteSection", (amx1, args1) => Natives.INI_DeleteSection(amx1, args1, this));
            m_Amx.Register("INI_DeleteKey", (amx1, args1) => Natives.INI_DeleteKey(amx1, args1, this));
            m_Amx.Register("INI_Exists", (amx1, args1) => Natives.INI_Exists(amx1, args1, this));

            //Guilds
            m_Amx.Register("DC_GetGuildName", (amx1, args1) => Natives.DC_GetGuildName(amx1, args1, this));
            m_Amx.Register("DC_GetGuildCount", (amx1, args1) => Natives.DC_GetGuildCount(amx1, args1, this));
            m_Amx.Register("DC_GetMemberCount", (amx1, args1) => Natives.DC_GetMemberCount(amx1, args1, this));

            //Members
            m_Amx.Register("DC_GetGuildMemberID", (amx1, args1) => Natives.DC_GetGuildMemberID(amx1, args1, this));
            m_Amx.Register("DC_GetMemberName", (amx1, args1) => Natives.DC_GetMemberName(amx1, args1, this));
            m_Amx.Register("DC_GetMemberDisplayName", (amx1, args1) => Natives.DC_GetMemberDisplayName(amx1, args1, this));
            m_Amx.Register("DC_GetMemberDiscriminator", (amx1, args1) => Natives.DC_GetMemberDiscriminator(amx1, args1, this));
            m_Amx.Register("DC_BanGuildMember", (amx1, args1) => Natives.DC_BanGuildMember(amx1, args1, this));
            m_Amx.Register("DC_GetMemberAvatarURL", (amx1, args1) => Natives.DC_GetMemberAvatarURL(amx1, args1, this));

            //Channels
            m_Amx.Register("DC_DeleteMessage", (amx1, args1) => Natives.DC_DeleteMessage(amx1, args1, this));
            m_Amx.Register("DC_SendChannelMessage", (amx1, args1) => Natives.DC_SendChannelMessage(amx1, args1, this));
            m_Amx.Register("DC_SendEmbed", (amx1, args1) => Natives.DC_SendEmbed(amx1, args1, this));
            m_Amx.Register("DC_SendPrivateMessage", (amx1, args1) => Natives.DC_SendPrivateMessage(amx1, args1, this));
            m_Amx.Register("DC_DeletePrivateMessage", (amx1, args1) => Natives.DC_DeletePrivateMessage(amx1, args1, this));
            m_Amx.Register("DC_FindChannel", (amx1, args1) => Natives.DC_FindChannel(amx1, args1, this));
            m_Amx.Register("DC_CreateChannel", (amx1, args1) => Natives.DC_CreateChannel(amx1, args1, this));
            m_Amx.Register("DC_DeleteChannel", (amx1, args1) => Natives.DC_DeleteChannel(amx1, args1, this));

            m_Amx.Register("DC_AddReaction", (amx1, args1) => Natives.DC_AddReaction(amx1, args1, this));
            m_Amx.Register("DC_AddPrivateReaction", (amx1, args1) => Natives.DC_AddPrivateReaction(amx1, args1, this));
            m_Amx.Register("DC_RemoveReaction", (amx1, args1) => Natives.DC_RemoveReaction(amx1, args1, this));
            m_Amx.Register("DC_RemovePrivateReaction", (amx1, args1) => Natives.DC_RemovePrivateReaction(amx1, args1, this));


            return true;
        }
    }
}
