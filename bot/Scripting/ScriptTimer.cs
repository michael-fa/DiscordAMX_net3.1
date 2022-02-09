using AMXWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace dcamx.Scripting
{
    public class ScriptTimer
    {
        public int ID;
        public bool m_Active = false;
        public bool m_Repeat = true;
        int m_msWait;
        string m_Func;
        DateTime m_lastCalled;
        TimeSpan m_TimeElapsed;
        Script m_ParentScript;
        System.Threading.Timer m_Timer;
        AMXPublic m_AMXCallback;
        public ScriptTimer(int interval,  bool rep, string funcCall, Script arg_parent_Script)
        {
            m_ParentScript = arg_parent_Script;
            m_AMXCallback = Program.m_Scripts[0].amx.FindPublic(funcCall);
            if (m_AMXCallback == null)
            {
                return;
            }
            m_msWait = interval;
            m_Func = funcCall;
            m_lastCalled = DateTime.Now;
            m_Active = true;
            m_Repeat = rep;
  
            Program.m_ScriptTimers.Add(this);
            this.ID = Program.m_ScriptTimers.Count;

            System.Threading.TimerCallback TimerDelegate =
            new System.Threading.TimerCallback(OnTimedEvent);
            

            m_Timer = new System.Threading.Timer(TimerDelegate, null, m_msWait, m_msWait);
            Utils.Log.Debug("Initialised Script-Timer (\"" + m_Func + "\") !", arg_parent_Script);
        }

        public void OnTimedEvent(Object state)
        {
            m_TimeElapsed = DateTime.Now.Subtract(m_lastCalled);
            if (m_TimeElapsed.TotalMilliseconds < m_msWait) return;
            m_lastCalled = DateTime.Now;

            try
            {
                m_AMXCallback.Execute();
                Utils.Log.Debug("Script-Timer invoked \"" + m_Func + "\"", m_ParentScript);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, m_ParentScript);
            }

            //No repeating?
            if(!m_Repeat)
            {
                m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
                this.m_Active = false;
            }

        }

        public bool KillTimer()
        {
            if (!this.m_Active) return false;


            m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.m_Active = false;

            return true;
        }
    }
}
