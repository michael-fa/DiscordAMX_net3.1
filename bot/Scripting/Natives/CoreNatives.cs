using AMXWrapper;
using System;
using System.Collections.Generic;

namespace dcamx.Scripting.Natives
{
    public static class CoreNatives
    {
        public static int Loadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!", caller_script);
                return 0;
            }

            if (!System.IO.File.Exists("Scripts/" + args1[0].AsString() + ".amx"))
            {
                Utils.Log.Error(" [command] The script file " + args1[0].AsString() + ".amx does not exist in /Scripts/ folder.", caller_script);
                return 0;
            }

            foreach (Script x in Program.m_Scripts)
            {
                if (x.m_amxFile.Equals(args1[0].AsString())) //There is a better way, but still; we can always do or a unhandled error here.
                {
                    Utils.Log.Error(" [command] Script " + args1[0].AsString() + " is already loaded!");
                    return 0;
                }
            }

            Script scr = new Script(args1[0].AsString(), true);
            AMXWrapper.AMXPublic pub = scr.m_Amx.FindPublic("OnFilterscriptInit");
            if (pub != null) pub.Execute();
            
            return 1;
        }

        public static int Unloadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 1;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!");
                return 0;
            }

            foreach (Script sc in Program.m_Scripts)
            {
                if (sc.m_amxFile.Equals(args1[0].AsString()))
                {
                    AMXWrapper.AMXPublic pub = sc.m_Amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();
                    sc.m_Amx.Dispose();
                    sc.m_Amx = null;
                    Program.m_Scripts.Remove(sc);   
                    Utils.Log.Info("[CORE] Script '" + args1[0].AsString() + "' unloaded.");
                    return 1;
                }
            }
            Utils.Log.Error(" [command] The script '" + args1[0].AsString() + "' is not running.");
            return 1;
        }

        public static int CallRemoteFunction(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {
                if (args1.Length == 1)
                {
                    if (args1[0].AsString().Length < 2)
                        return 0;

                    AMXPublic tmp = null;
                    foreach (Script scr in Program.m_Scripts)
                    {
                        tmp = scr.m_Amx.FindPublic(args1[0].AsString());
                        if (tmp != null) tmp.Execute();
                    }
                    return 1;
                }
                else if (args1.Length >= 3)
                {
                    int count = (args1.Length - 1);

                    AMXPublic p = null;
                    List<CellPtr> Cells = new List<CellPtr>();

                    //Important so the format ( ex "iissii" ) is aligned with the arguments pushed to the callback, not being reversed
                    string reversed_format = Utils.Scripting.Reverse(args1[1].AsString());

                    foreach (Script scr in Program.m_Scripts)
                    {
                        if (scr.Equals(caller_script)) continue;
                        p = scr.m_Amx.FindPublic(args1[0].AsString());
                        if (p == null) continue;
                        foreach (char x in reversed_format.ToCharArray())
                        {
                            if (count == 1) break;
                            switch (x)
                            {
                                case 'i':
                                    {
                                        p.AMX.Push(args1[count].AsIntPtr());
                                        count--;
                                        continue;
                                    }
                                case 'f':
                                    {
                                        p.AMX.Push((float)args1[count].AsCellPtr().Get().AsFloat());
                                        count--;
                                        continue;
                                    }

                                case 's':
                                    {
                                        Cells.Add(p.AMX.Push(args1[count].AsString()));
                                        count--;
                                        continue;
                                    }
                            }
                        }
                        //Reset our arg index counter
                        count = (args1.Length - 1);
                        p.Execute();

                    }

                    foreach (CellPtr cell in Cells)
                    {
                        p.AMX.Release(cell);
                    }
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
            }
            return 1;
        }

        public static int gettimestamp(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            return (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static int SetTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1[2].AsInt32() > 1 || args1[2].AsInt32() < 0)
            {
                Utils.Log.Error("SetTimer: Argument 'repeating' is boolean. Please pass 0 or 1 only!");
                return 0;
            }

            try
            {
                ScriptTimer timer = new ScriptTimer(args1[1].AsInt32(), Convert.ToBoolean(args1[2].AsInt32()), args1[0].AsString(), caller_script);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
            }
            return (Program.m_ScriptTimers.Count);
        }

        /*public static int SetTimerEx(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if(args1.Length < 5) return 1;

            try
            {
                ScriptTimer timer = new ScriptTimer(args1[1].AsInt32(), Convert.ToBoolean(args1[2].AsInt32()), args1[0].AsString(), caller_script, args1[3].AsString(), args1);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
            }
            return (Program.m_ScriptTimers.Count);
        }*/

        public static int KillTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            foreach (ScriptTimer scrt in Program.m_ScriptTimers)
            {
                if (scrt != null)
                {
                    if (scrt.ID == args1[0].AsInt32())
                    {
                        scrt.KillTimer();
                        return 1;
                    }
                }
            }
            return 1;
        }

        public static int DC_GetBotPing(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {
                return Program.m_Discord.Client.Ping;
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'DC_GetBotPing'! (m_Discord->Client NullReference)", caller_script);
            } 
            return 1;
        }
    }
}
