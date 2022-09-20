using System.Collections.Generic;

namespace GameFramework.Config
{
    /// <summary>
    /// 配置表管理器
    /// </summary>
    public sealed class ConfigManager : IConfigManager
    {
        private Dictionary<string, IRefrence> configs;
        private const string CONFIG_FILE_EXTENSION = ".cfg";

        public ConfigManager()
        {
            configs = new Dictionary<string, IRefrence>();
        }

        /// <summary>
        /// 清理所有配置表
        /// </summary>
        public void Clear()
        {
            foreach (var item in configs.Values)
            {
                Loader.Release(item);
            }
        }

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <param name="name">配置表名称</param>
        /// <typeparam name="T">配置表</typeparam>
        /// <returns></returns>
        public IConfigDatable<T> GetConfigTable<T>(string name) where T : IConfig
        {
            if (configs.TryGetValue(name, out IRefrence table))
            {
                return (IConfigDatable<T>)table;
            }
            return default;
        }

        /// <summary>
        /// 查询是否已经加载指定的配置表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasLoadConfig(string name) => configs.ContainsKey(name);

        /// <summary>
        /// 加载指定的配置表
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IConfigDatable<T> LoadConfig<T>(string name) where T : IConfig
        {
            IConfigDatable<T> configTable = Loader.Generate<IConfigDatable<T>>();
            configTable.Load(name);
            return configTable;
        }

        /// <summary>
        /// 回收配置表
        /// </summary>
        public void Release()
        {
            Clear();
        }

        /// <summary>
        /// 卸载指定的配置表
        /// </summary>
        /// <param name="name"></param>
        public void UnloadConfig(string name)
        {
            if (HasLoadConfig(name))
            {
                Loader.Release(configs[name]);
                configs.Remove(name);
            }
        }

        /// <summary>
        /// 轮询
        /// </summary>
        public void Update()
        {

        }
    }
}
