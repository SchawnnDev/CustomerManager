using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomerManagement.Enums;
using CustomerManagement.Interfaces;

namespace CustomerManagement.IO
{
    public class PluginManager
    {

        private static IDatabasePlugin ChosenPlugin { get; set; }
        public static List<IDatabasePlugin> Plugins { get; private set; }

        public static void LoadPlugins()
        {
            Plugins = new List<IDatabasePlugin>();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.Substring(6) + "\\Plugins";

            if (!Directory.Exists(path)) return;

            var assemblies = new List<Assembly>();
            assemblies.AddRange(Directory.GetFiles(path, "*.dll").Select(dllFile => Assembly.Load(AssemblyName.GetAssemblyName(dllFile))));

            var pluginTypes = (from assembly in assemblies
                               where assembly != null
                               from type in assembly.GetTypes()
                               where !type.IsInterface && !type.IsAbstract && type.GetInterface(typeof(IDatabasePlugin).FullName) != null
                               select type).ToList();

            // Add plugins to list

            foreach (var type in pluginTypes)
                Plugins.Add((IDatabasePlugin)Activator.CreateInstance(type));

        }

        public static bool ChoosePlugin(string name)
        {

            foreach (var plugin in Plugins)
            {
                if (!plugin.GetName().Equals(name)) continue;
                ChosenPlugin = plugin;
                return true;
            }

            return false;

        }

        public static int GetIndexFromName(string name) => GetPluginNames().FindIndex(x => x.StartsWith(name));

        public static List<string> GetPluginNames() => Plugins.Select(plugin => plugin.GetName()).ToList();

        public static IDatabasePlugin GetPluginFromName(string name)
        {
            foreach (var plugin in Plugins)
                if (plugin.GetName() == name)
                    return plugin;

            return null;
        }

        public static IDatabasePlugin GetActivePlugin()
        {
            return ChosenPlugin;
        }

    }
}
