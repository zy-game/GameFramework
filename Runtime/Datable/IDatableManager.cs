using System;

namespace GameFramework.Datable
{
    /// <summary>
    /// 数据管理器
    /// </summary>
    public interface IDatableManager : IGameModule
    {
        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        T CreateDatable<T>() where T : IGameDatable;

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        /// <returns>游戏数据表</returns>
        IGameDatable CreateDatable(Type gameDatableType);

        /// <summary>
        /// 是否存在游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns></returns>
        bool IsHaveGameDatable(string guid);

        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        T GetGameDatable<T>(string guid) where T : IGameDatable;

        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        IGameDatable GetGameDatable(string guid);

        /// <summary>
        /// 移除游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        void RemoveGameDatable(string guid);

        /// <summary>
        /// 清理所有数据表
        /// </summary>
        void Clear();
    }
}
