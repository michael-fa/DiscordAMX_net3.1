using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Plugins
{
    
    public class PluginNatives
    {
        public Plugin m_SourcePlugin;
        public string[] m_Natives = null;

        public PluginNatives(Plugin m_pl, string[] _natives)
        {
            m_Natives = _natives;
            m_SourcePlugin = m_pl;
        }
        
    }
}
