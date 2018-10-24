using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomerManagement.IO
{
    public class ConfigFile
    {
        private const string FilePath = "Config.json";
        private static Dictionary<string, string> ConfigDictionary { get; set; }

        public static void Load()
        {
            ConfigDictionary = new Dictionary<string, string>();

            if (!File.Exists(FilePath))
            {
                File.CreateText(FilePath).Write("[]");
                return;
            }

            ConfigDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(FilePath));
        }

        public static void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(ConfigDictionary));
        }

        public static string GetValue(string key) => ConfigDictionary.ContainsKey(key) ? ConfigDictionary[key] : "";

        public static string GetLastCorrectDatabaseType()
        {
            var last = GetValue("DatabaseType");

            if (PluginManager.GetPluginNames().Contains(last))
                return last;

            return PluginManager.GetPluginNames().Count != 0 ? PluginManager.Plugins[0].GetName() : "";
        }

        public static void Write(string key, string value)
        {
            if (!ConfigDictionary.ContainsKey(key))
            {
                ConfigDictionary.Add(key, value);
                return;
            }

            ConfigDictionary[key] = value;

        }

        public static void Reload()
        {
            Save();
            Load();
        }

    }
}
