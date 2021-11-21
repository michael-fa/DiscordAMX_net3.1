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
        public string _amxFile = null;
        public AMX amx;

       
        public Script(string _amxFile, bool _isFilterscript = false)
        {
            this._amxFile = _amxFile;
            try
            {
                amx = new AMX("Scripts/" + _amxFile + ".amx");
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            amx.LoadLibrary(AMXDefaultLibrary.Core);

            amx.LoadLibrary(AMXDefaultLibrary.String);
            amx.LoadLibrary(AMXDefaultLibrary.Console);
            amx.LoadLibrary(AMXDefaultLibrary.DGram);
            amx.LoadLibrary(AMXDefaultLibrary.Float);
            amx.LoadLibrary(AMXDefaultLibrary.Time);
            amx.LoadLibrary(AMXDefaultLibrary.Fixed);
            this.RegisterNatives();

            Program.m_Scripts.Add(this);
            Utils.Log.WriteLine("ID " + this._amxFile + ": " + (Program.m_Scripts.Count - 1));
            return;
        }

        public void StopAllTimers()
        {
            foreach(ScriptTimer timer in Program.m_ScriptTimers)
            {
                timer.KillTimer();
            }

        }

        public bool RegisterNatives()
        {
            amx.Register("printc", (amx1, args1) => Natives.printc(amx1, args1, this));
            
            //amx.Register("LoadScript", (amx1, args1) => Natives.loadscript(amx1, args1, this));
            //amx.Register("UnloadScript", (amx1, args1) => Natives.unloadscript(amx1, args1, this));
            amx.Register("SetTimer", (amx1, args1) => Natives.SetTimer(amx1, args1, this));
            amx.Register("KillTimer", (amx1, args1) => Natives.KillTimer(amx1, args1, this));
            amx.Register("gettimestamp", (amx1, args1) => Natives.gettimestamp(amx1, args1, this));

            amx.Register("DC_SetMinLogLevel", (amx1, args1) => Natives.DC_SetMinLogLevel(amx1, args1, this));
            amx.Register("DC_SetGuild", (amx1, args1) => Natives.DC_SetGuild(amx1, args1, this));
            amx.Register("DC_SetActivityText", (amx1, args1) => Natives.DC_SetActivityText(amx1, args1, this));
            amx.Register("DC_SetToken", (amx1, args1) => Natives.DC_SetToken(amx1, args1, this));
            
            
            
            //amx.Register("ALTV_GetPublicServerInfo", (amx1, args1) => Natives.ALTV_GetPublicServerInfo(amx1, args1, this));

            //Members
            amx.Register("DC_GetMemberName", (amx1, args1) => Natives.DC_GetMemberName(amx1, args1, this));
            amx.Register("DC_GetMemberDisplayName", (amx1, args1) => Natives.DC_GetMemberDisplayName(amx1, args1, this));
            amx.Register("DC_GetMemberDiscriminator", (amx1, args1) => Natives.DC_GetMemberDiscriminator(amx1, args1, this));

            //Channels
            amx.Register("DC_DeleteMessage", (amx1, args1) => Natives.DC_DeleteMessage(amx1, args1, this));
            amx.Register("DC_SendChannelMessage", (amx1, args1) => Natives.DC_SendChannelMessage(amx1, args1, this));

            return true;
        }
    }
}
