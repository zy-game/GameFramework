using System;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Game
{

    /// <summary>
    /// 游戏管理器
    /// </summary>
    public interface IWorldManager : IGameModule
    {
        /// <summary>
        /// 当前游戏
        /// </summary>
        /// <value></value>
        IGameWorld current { get; }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T OpenWorld<T>() where T : IGameWorld;

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        IGameWorld OpenWorld(Type gameType);

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        IGameWorld OpenWorld(string gameTypeName);

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetWorld<T>() where T : IGameWorld;

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        IGameWorld GetWorld(Type gameType);

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        IGameWorld GetWorld(string gameTypeName);

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseWorld<T>() where T : IGameWorld;

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameType"></param>
        void CloseWorld(Type gameType);

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        void CloseWorld(string gameTypeName);
    }
}
