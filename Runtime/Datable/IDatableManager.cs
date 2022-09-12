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
        public T GenerateGameDatable<T>() where T : IGameDatable
        {
            return (T)GenerateGameDatable(typeof(T));
        }

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        /// <returns>游戏数据表</returns>
        IGameDatable GenerateGameDatable(Type gameDatableType);

        /// <summary>
        /// 是否存在游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns></returns>
        public void IsHaveGameDatable<T>() where T : IGameDatable
        {
            IsHaveGameDatable(typeof(T));
        }
        /// <summary>
        /// 是否存在游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns></returns>
        bool IsHaveGameDatable(Type gameDatableType);

        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        public T GetGameDatable<T>() where T : IGameDatable
        {
            return (T)GetGameDatable(typeof(T));
        }
        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        IGameDatable GetGameDatable(Type gameDatableType);

        /// <summary>
        /// 移除游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        public void RemoveGameDatable<T>() where T : IGameDatable
        {
            RemoveGameDatable(typeof(T));
        }

        /// <summary>
        /// 移除游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        void RemoveGameDatable(Type gameDatableType);
    }
}
