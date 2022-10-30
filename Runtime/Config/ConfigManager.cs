using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Config
{
    /// <summary>
    /// 配置表管理器
    /// </summary>
    public sealed class ConfigManager : Singleton<ConfigManager>, IConfigManager
    {
        private Dictionary<Type, List<IConfig>> configs;

        public ConfigManager()
        {
            configs = new Dictionary<Type, List<IConfig>>();
        }

        /// <summary>
        /// 清理所有配置
        /// </summary>
        public void Clear()
        {
            foreach (var item in configs.Values)
            {
                foreach (var config in item)
                {
                    Loader.Release(config);
                }
            }
        }

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public void UnloadConfig<T>(string name) where T : IConfig
        {
            UnloadConfig(typeof(T), name);
        }

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public void UnloadConfig<T>(int id) where T : IConfig
        {
            UnloadConfig(typeof(T), id);
        }

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        public void UnloadConfig(Type configType, string name)
        {
            if (!configs.TryGetValue(configType, out List<IConfig> configList))
            {
                return;
            }
            IConfig config = configList.Find(x => x.name == name);
            if (config == null)
            {
                return;
            }
            configList.Remove(config);
        }

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="id"></param>
        public void UnloadConfig(Type configType, int id)
        {
            if (!configs.TryGetValue(configType, out List<IConfig> configList))
            {
                return;
            }
            IConfig config = configList.Find(x => x.id == id);
            if (config == null)
            {
                return;
            }
            configList.Remove(config);
        }

        /// <summary>
        /// 卸载所有相同类型的配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UnloadAllConfig<T>()
        {
            UnloadAllConfig(typeof(T));
        }

        /// <summary>
        /// 卸载所有相同类型的配置表
        /// </summary>
        /// <param name="configType"></param>
        public void UnloadAllConfig(Type configType)
        {
            if (!configs.TryGetValue(configType, out List<IConfig> configList))
            {
                return;
            }
            foreach (var item in configList)
            {
                Loader.Release(item);
            }
            configList.Clear();
            configs.Remove(configType);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetConfig<T>(string name) where T : IConfig
        {
            return (T)GetConfig(typeof(T), name);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetConfig<T>(int id) where T : IConfig
        {
            return (T)GetConfig(typeof(T), id);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IConfig GetConfig(Type configType, string name)
        {
            if (!configs.TryGetValue(configType, out List<IConfig> configList))
            {
                configs.Add(configType, configList = LoadConfig(configType));
            }
            return configList.Find(x => x.name == name);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IConfig GetConfig(Type configType, int id)
        {
            if (!configs.TryGetValue(configType, out List<IConfig> configList))
            {
                configs.Add(configType, configList = LoadConfig(configType));
            }
            return configList.Find(x => x.id == id);
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            Clear();
        }

        private List<IConfig> LoadConfig(Type configType)
        {
            ResHandle handle = ResourceManager.Instance.LoadAssetSync<TextAsset>(configType.Name);
            handle.EnsueAssetLoadState();
            TextAsset textAsset = handle.Generate<TextAsset>();
            GameFrameworkException.IsNull(textAsset);
            return (List<IConfig>)CatJson.JsonParser.ParseJson(textAsset.text, typeof(List<>).MakeGenericType(configType));
        }
    }
}
