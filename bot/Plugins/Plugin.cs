using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AMXWrapper;
using System.Threading.Tasks;
using System.Collections;

namespace dcamx.Plugins
{
    public class Plugin
    {
        public string _File;
        public Assembly m_Plugin = null;
        public object m_Instance = null;
        public Plugin(string _filename)
        {
            _File = _filename;
            try
            {
                m_Plugin = Assembly.LoadFile(Environment.CurrentDirectory + "\\" + _filename);
                m_Instance = Activator.CreateInstance(m_Plugin.GetExportedTypes()[0]);
                Program.m_Plugins.Add(this);
            }
            catch (Exception ex)
            {
                Utils.Log.Error("=========================\nPlugin " + _filename + " threw an error:\n" + ex.Message + ex.StackTrace + "\n        =========================\n");
            }

            object ret = m_Plugin.GetExportedTypes()[0].InvokeMember("GetNatives", BindingFlags.InvokeMethod, null, m_Instance, new object[] { });

            //Fetch all the natives we need to register from this plugin
            if (ret.GetType() == typeof(string[]))
            {
                string[] arr = ((IEnumerable)ret).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

                Program.m_PluginNatives.Add(new PluginNatives(this, arr));
            }

        }

        public void InvokeLoad()
        {
            object ret = m_Plugin.GetExportedTypes()[0].InvokeMember("OnLoad", BindingFlags.InvokeMethod, null, m_Instance, new object[] { });
        }

        public void Unload(int exitcode)
        {
            m_Plugin.GetExportedTypes()[0].InvokeMember("OnUnload", BindingFlags.InvokeMethod, null, m_Instance, new object[] { exitcode });



            m_Instance = null;
            m_Plugin = null;
            GC.Collect();

        }
    }
}
