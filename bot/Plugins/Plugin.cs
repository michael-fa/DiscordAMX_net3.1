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
        public List<string> m_ExportedNatives = null;
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
                //assume that theres no natives to be loaded.
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
