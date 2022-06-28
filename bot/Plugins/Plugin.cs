using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Plugins
{
    public class Plugin
    {
        public Plugin(string _filename)
        {
            var DLL = Assembly.LoadFile(_filename);

            foreach (Type type in DLL.GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                //type.InvokeMember("Output", BindingFlags.InvokeMethod, null, c, new object[] { @"Hello" });
            }

        }
    }
}
