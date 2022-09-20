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
        /// 加载配置表
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfigDatable<T> LoadConfig<T>(string name) where T : IConfig;

        /// <summary>
        /// 卸载配置表
        /// </summary>
        /// <param name="name"></param>
        void UnloadConfig(string name);

        /// <summary>
        /// 获取指定的配置表
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IConfigDatable<T> GetConfigTable<T>(string name) where T : IConfig;

        /// <summary>
        /// 查询是否已经加载指定的配置表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasLoadConfig(string name);

        /// <summary>
        /// 清理所有配置表
        /// </summary>
        void Clear();
    }
}
