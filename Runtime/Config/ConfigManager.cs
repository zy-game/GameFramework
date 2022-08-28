using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameFramework.Resource;

namespace GameFramework.Config
{
    public sealed class ConfigManager : IConfigManager
    {
        private Dictionary<Type, List<IConfig>> _config;
        public ConfigManager()
        {
            _config = new Dictionary<Type, List<IConfig>>();
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="id">配置表ID</param>
        /// <returns>配置表</returns>
        public IConfig GetConfig(Type type, int id)
        {
            if (!_config.TryGetValue(type, out List<IConfig> configs))
            {
                throw GameFrameworkException.GenerateFormat("not find the config:{0}", id);
            }
            IConfig config = configs.Find(x => x.id == id);
            if (config == null)
            {
                throw GameFrameworkException.GenerateFormat("not find the config:{0}", id);
            }
            return config;
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="name">配置表名</param>
        /// <returns>配置表</returns>
        /// <exception cref="NullReferenceException"></exception>
        public IConfig GetConfig(Type type, string name)
        {
            if (!_config.TryGetValue(type, out List<IConfig> configs))
            {
                throw GameFrameworkException.GenerateFormat("not find the config:{0}", name);
            }
            IConfig config = configs.Find(x => x.name == name);
            if (config == null)
            {
                throw GameFrameworkException.GenerateFormat("not find the config:{0}", name);
            }
            return config;
        }

        /// <summary>
        /// 获取指定类型的所有配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <returns>相同类型的所有配置表数组</returns>
        public IConfig[] GetConfigs(Type type)
        {
            if (_config.TryGetValue(type, out List<IConfig> configs))
            {
                return configs.ToArray();
            }
            return default;
        }
        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <param name="id">配置表ID</param>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        public bool HasConfig(Type type, int id)
        {
            return GetConfig(type, id) != null;
        }
        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <param name="name">配置表名</param>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        public bool HasConfig(Type type, string name)
        {
            return GetConfig(type, name) != null;
        }
    
        /// <summary>
        /// 是否加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        public bool HasLoadConfig(Type type)
        {
            return _config.ContainsKey(type);
        }

        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="configName">配置表名</param>
        /// <returns></returns>
        public async Task LoadConfigAsync(Type type, string configName)
        {
            if (_config.TryGetValue(type, out List<IConfig> configs))
            {
                return;
            }
            IResourceManager resourceManager = Runtime.GetGameModule<IResourceManager>();
            DataStream dataStream = await resourceManager.ReadFileAsync(configName);
            IList list = (IList)CatJson.JsonParser.ParseJson(dataStream.ToString(), typeof(List<>).MakeGenericType(type));
            configs = new List<IConfig>();
            for (var i = 0; i < list.Count; i++)
            {
                configs.Add((IConfig)list[i]);
            }
            _config.Add(type, configs);
        }
        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="configName">配置表名</param>
        public async void LoadConfigSync(Type type, string configName)
        {
            await LoadConfigAsync(type, configName);
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            foreach (var item in _config.Values)
            {
                item.ForEach(Creater.Release);
            }
            _config.Clear();
        }

        public void Update()
        {

        }
    }
}
