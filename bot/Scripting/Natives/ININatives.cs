using AMXWrapper;
using System;
using System.IO;

namespace dcamx.Scripting.Natives
{
    public static class ININatives
    {
        public static int INI_Delete(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\scriptfiles"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\scriptfiles");
                    Utils.Log.Warning("'/scriptfiles' folder not found, creating a new one.. Note that script-specific folders need to be created manually.");
                }
                IniFile del = null;
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;

                    del = x;
                }
                if (del != null)
                {
                    File.Delete(del.Path);
                    Program.m_ScriptINIFiles.Remove(del);
                    del = null;
                }
                return 1;
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Delete'", caller_script);
            }
            return -1;
        }

        public static int INI_Open(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\scriptfiles"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\scriptfiles");
                    Utils.Log.Warning("'/scriptfiles' folder not found, creating a new one.. Note that sub-folders for scripts need to be created manually.");
                }
                Utils.Log.Debug("Open file handler for " + "scriptfiles\\" + args1[0].AsString());
                Program.m_ScriptINIFiles.Add(new IniFile("scriptfiles\\" + args1[0].AsString() + ".ini"));
                return (Program.m_ScriptINIFiles.Count - 1);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Open'", caller_script);
            }
            return -1;
        }

        public static int INI_Close(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                IniFile del = null;
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;

                    del = x;
                }
                if (del != null)
                {
                    Program.m_ScriptINIFiles.Remove(del);
                    del = null;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Close'", caller_script);
            }
            return 0;
        }

        public static int INI_Write(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    x.Write(args1[1].AsString(), args1[2].AsString(), args1[3].AsString());
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Write'", caller_script);
            }
            return 0;
        }

        public static int INI_WriteInt(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    x.Write(args1[1].AsString(), args1[2].AsInt32().ToString(), args1[3].AsString());
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_WriteInt'", caller_script);
            }
            return 0;
        }

        public static int INI_WriteFloat(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length < 3) return 0;

            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    x.Write(args1[1].AsString(), args1[2].AsFloat().ToString(), args1[3].AsString());
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_WriteFloat'", caller_script);
            }
            return 0;
        }

        public static float INI_ReadFloat(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    return (float)Convert.ToDouble(x.Read(args1[1].AsString(), args1[2].AsString()));
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_ReadInt'", caller_script);
            }
            return 0.0f;
        }

        public static int INI_Read(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 4) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    AMX.SetString(args1[3].AsCellPtr(), x.Read(args1[1].AsString(), args1[2].AsString()), true);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Read'", caller_script);
            }
            return 0;
        }

        public static int INI_ReadInt(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;

                    return Convert.ToInt32(x.Read(args1[1].AsString(), args1[2].AsString()));
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_ReadInt'", caller_script);
            }
            return 0;
        }

        public static int INI_KeyExists(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    if (x.KeyExists(args1[1].AsString(), args1[2].AsString()))
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_KeyExists'", caller_script);
            }
            return 0;
        }

        public static int INI_Exists(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\scriptfiles\\" + args1[0].AsString() + ".ini"))
                    return 1;
                else return 0;
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_Exists'", caller_script);
            }
            return 0;
        }

        public static int INI_DeleteSection(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    x.DeleteSection(args1[1].AsString());
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_DeleteSection'", caller_script);
            }
            return 0;
        }

        public static int INI_DeleteKey(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 3) return 0;
            try
            {
                foreach (IniFile x in Program.m_ScriptINIFiles)
                {
                    if (x.m_ScrID != args1[0].AsInt32()) continue;
                    x.DeleteKey(args1[1].AsString(), args1[2].AsString());
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'INI_DeleteKey'", caller_script);
            }
            return 0;
        }
    }
}
