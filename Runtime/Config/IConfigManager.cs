using System.Linq;
using System;
using System.Threading.Tasks;

namespace GameFramework.Config
{
    /// <summary>
    /// 配置表管理器
    /// </summary>
    public interface IConfigManager : IGameModule
    {
        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="name"></param>
        void UnloadConfig<T>(string name) where T : IConfig;

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        void UnloadConfig<T>(int id) where T : IConfig;

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        void UnloadConfig(Type configType, string name);

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="id"></param>
        void UnloadConfig(Type configType, int id);

        /// <summary>
        /// 卸载所有相同类型的配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void UnloadAllConfig<T>();

        /// <summary>
        /// 卸载所有相同类型的配置表
        /// </summary>
        /// <param name="configType"></param>
        void UnloadAllConfig(Type configType);

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetConfig<T>(string name) where T : IConfig;

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetConfig<T>(int id) where T : IConfig;

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfig GetConfig(Type configType, string name);

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IConfig GetConfig(Type configType, int id);

        /// <summary>
        /// 清理所有配置表
        /// </summary>
        void Clear();
    }
}
