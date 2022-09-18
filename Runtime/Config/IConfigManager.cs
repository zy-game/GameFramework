using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Config
{
    /// <summary>
    /// 配置表管理器
    /// </summary>
    public interface IConfigManager : IGameModule
    {
        ConfigTable LoadConfig(string name);
        void UnloadConfig(string name);
        ConfigTable GetConfigTable(string name);
        bool HasLoadConfig(string name);
        void Clear();
    }

    public sealed class ConfigTable : IRefrence
    {
        public string name { get; private set; }


        public IConfig GetConfig(int id)
        {

        }
        public IConfig GetConfig(string name)
        {

        }
        public bool HasConfig(int id)
        {

        }
        public bool HasConfig(string name)
        {

        }
        public void Add(IConfig config)
        {

        }
        public void Remove(int id)
        {

        }
        public void Remove(string name)
        {

        }
        public void Remove(IConfig config)
        {

        }

        public void Release()
        {

        }

        public void Load(string name)
        {

        }

        public void Save(string name)
        {

        }

        public void Delete()
        {

        }
    }

    public sealed class ConfigManager : IConfigManager
    {
        private Dictionary<string, ConfigTable> configs;
        private const string CONFIG_FILE_EXTENSION = ".cfg";

        public ConfigManager()
        {
            configs = new Dictionary<string, ConfigTable>();
        }

        public void Clear()
        {
            foreach (var item in configs.Values)
            {
                Loader.Release(item);
            }
        }

        public ConfigTable GetConfigTable(string name)
        {
            if (configs.TryGetValue(name, out ConfigTable table))
            {
                return table;
            }
            return default;
        }

        public bool HasLoadConfig(string name) => configs.ContainsKey(name);


        public ConfigTable LoadConfig(string name)
        {
            ConfigTable configTable = Loader.Generate<ConfigTable>();
            configTable.Load(name);
            return configTable;
        }

        public void Release()
        {
            Clear();
        }

        public void UnloadConfig(string name)
        {
            if (HasLoadConfig(name))
            {
                Loader.Release(configs[name]);
                configs.Remove(name);
            }
        }

        public void Update()
        {

        }
    }
}
