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
        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="configName">配置表名</param>
        void LoadConfigSync(Type type, string configName);

        /// <summary>
        /// 加载指定的配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="configName">配置表名</param>
        public void LoadConfigSync<T>(string configName) where T : IConfig
        {
            LoadConfigAsync(typeof(T), configName);
        }

        /// <summary>
        /// 加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <param name="configName">配置表名</param>
        /// <returns></returns>
        Task LoadConfigAsync(Type type, string configName);

        /// <summary>
        /// 加载指定的配置表
        /// </summary>
        /// <param name="configName">配置表名</param>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <returns></returns>
        public Task LoadConfigAsync<T>(string configName) where T : IConfig
        {
            return LoadConfigAsync(typeof(T), configName);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="id">配置表ID</param>
        /// <param name="type">配置表类型</param>
        /// <returns>配置表对象</returns>
        /// <exception cref="NullReferenceException"/>
        IConfig GetConfig(Type type, int id);

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <param name="name">配置表名</param>
        /// <param name="type">配置表类型</param>
        /// <returns>配置表对象</returns>
        /// <exception cref="NullReferenceException"/>
        IConfig GetConfig(Type type, string name);

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="id">配置表ID</param>
        /// <returns>配置表对象</returns>
        /// <exception cref="NullReferenceException"/>
        public T GetConfig<T>(int id) where T : IConfig
        {
            return (T)GetConfig(typeof(T), id);
        }

        /// <summary>
        /// 获取配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="name">配置表名</param>
        /// <returns>配置表对象</returns>
        /// <exception cref="NullReferenceException"/>
        public T GetConfig<T>(string name) where T : IConfig
        {
            return (T)GetConfig(typeof(T), name);
        }

        /// <summary>
        /// 获取指定类型的所有配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <returns>相同类型的所有配置表数组</returns>
        public T[] GetConfigs<T>() where T : IConfig
        {
            return GetConfigs(typeof(T)).Cast<T>().ToArray();
        }

        /// <summary>
        /// 获取指定类型的所有配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <returns>相同类型的所有配置表数组</returns>
        IConfig[] GetConfigs(Type type);

        /// <summary>
        /// 是否加载配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <returns></returns>
        public bool HasLoadConfig<T>() where T : IConfig
        {
            return HasLoadConfig(typeof(T));
        }

        /// <summary>
        /// 是否加载配置表
        /// </summary>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        bool HasLoadConfig(Type type);

        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <param name="id">配置表ID</param>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        bool HasConfig(Type type, int id);

        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <param name="name">配置表名</param>
        /// <param name="type">配置表类型</param>
        /// <returns></returns>
        bool HasConfig(Type type, string name);

        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="name">配置表名</param>
        /// <returns></returns>
        public bool HasConfig<T>(string name) where T : IConfig
        {
            return HasConfig(typeof(T), name);
        }

        /// <summary>
        /// 是否存在配置表
        /// </summary>
        /// <typeparam name="T">配置表类型</typeparam>
        /// <param name="id">配置表ID</param>
        /// <returns></returns>
        public bool HasConfig<T>(int id) where T : IConfig
        {
            return HasConfig(typeof(T), id);
        }
    }
}
