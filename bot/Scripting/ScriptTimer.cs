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
        //string m_ArgFrmt;
        //AMXArgumentList m_Args;
        public ScriptTimer(int interval,  bool rep, string funcCall, Script arg_parent_Script/*, string m_ArgsFrm, AMXArgumentList _args*/)
        {
            m_ParentScript = arg_parent_Script;
            m_AMXCallback = m_ParentScript.amx.FindPublic(funcCall);
            if (m_AMXCallback == null)
            {
                return;
            }
            m_msWait = interval;
            m_Func = funcCall;
            m_lastCalled = DateTime.Now;
            m_Active = true;
            m_Repeat = rep;
           // m_ArgFrmt = m_ArgsFrm;
           // m_Args = _args;


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
                /*Utils.Log.Debug("length: " + m_Args.Length + "frmtstr " + m_ArgFrmt);
                List<CellPtr> _list = new List<CellPtr>();
                int idx = 5;
                foreach(char x in m_ArgFrmt.ToCharArray())
                {
                    switch(x)
                    {
                        case 'i':
                            Utils.Log.Debug("1 " + m_Args[6].AsInt32());
                            m_AMXCallback.AMX.Push(m_Args[idx].AsInt32());
                            break;
                        case 's':
                            Utils.Log.Debug("2");
                            _list.Add(m_AMXCallback.AMX.Push(m_Args[idx].AsString()));
                            break;
                        case 'f':
                            Utils.Log.Debug("3");
                            m_AMXCallback.AMX.Push(m_Args[idx].AsFloat());
                            break;
                    }
                    idx++;
                }*/

                m_AMXCallback.Execute();

               /* foreach (CellPtr x in _list)
                {
                    m_AMXCallback.AMX.Release(x);
                }
                GC.Collect();
               */
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
