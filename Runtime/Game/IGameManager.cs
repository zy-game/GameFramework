using System;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Game
{

    /// <summary>
    /// 游戏管理器
    /// </summary>
    public interface IGameManager : IGameModule
    {
        /// <summary>
        /// 当前游戏
        /// </summary>
        /// <value></value>
        IGame current { get; }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T OpenGame<T>() where T : IGame;

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        IGame OpenGame(Type gameType);

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        IGame OpenGame(string gameTypeName);

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetGame<T>() where T : IGame;

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        IGame GetGame(Type gameType);

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        IGame GetGame(string gameTypeName);

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseGame<T>() where T : IGame;

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameType"></param>
        void CloseGame(Type gameType);

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        void CloseGame(string gameTypeName);
    }
}
