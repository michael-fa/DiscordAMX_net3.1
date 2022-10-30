using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Utils
{
    public static class PluginTools
    {
        public static int LoadAllPlugins()
        {
            foreach(Plugins.Plugin pl in Program.m_Plugins)
            {
                pl.InvokeLoad();
            }
            return Program.m_Plugins.Count;
        }

        public static void RegisterNatives_Late()
        {
            foreach(Plugins.Plugin plugin in Program.m_Plugins)
            {
                try
                {
                    object ret = plugin.m_Instance.GetType().GetMethod("GetNatives", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                    //Fetch all the natives we need to register from this plugin
                    IEnumerable enumerable = ret as IEnumerable;
                    plugin.m_ExportedNatives = new List<string>();

                    if (enumerable != null)
                    {
                        foreach (object element in enumerable)
                        {
                            plugin.m_ExportedNatives.Add((string)element);
                            foreach (dcamx.Scripting.Script scr in Program.m_Scripts)
                            {
                                scr.m_Amx.Register((string)element, (amx1, args1) =>
                                {
                                    plugin.m_Plugin.GetExportedTypes()[0].InvokeMember((string)element, BindingFlags.InvokeMethod, null, plugin.m_Instance, new object[] { amx1, args1 });
                                    return 1;
                                });
                                Utils.Log.Debug("Registering native " + (string)element + " for plugin: " + plugin.m_Plugin);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    Utils.Log.Error("=========================\nPlugin " + plugin.m_Plugin.FullName + " threw an error:\n" + ex.Message + "\nSOURE: \n" + ex.Source + ex.InnerException + ex.StackTrace + "\n        =========================\n");
                }
            }
            
        }
    }
}
