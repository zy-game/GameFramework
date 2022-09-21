using System.Collections.Generic;
using System;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public sealed class GameManager : IGameManager
    {
        private Dictionary<Type, IGame> games = new Dictionary<Type, IGame>();

        /// <summary>
        /// 当前游戏
        /// </summary>
        /// <value></value>
        public IGame current { get; private set; }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenGame<T>() where T : IGame
        {
            return (T)OpenGame(typeof(T));
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public IGame OpenGame(Type gameType)
        {
            if (!games.TryGetValue(gameType, out IGame game))
            {
                game = (IGame)Loader.Generate(gameType);
            }
            current = game;
            return game;
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        public IGame OpenGame(string gameTypeName)
        {
            return OpenGame(Type.GetType(gameTypeName));
        }

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetGame<T>() where T : IGame
        {
            return (T)GetGame(typeof(T));
        }

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public IGame GetGame(Type gameType)
        {
            if (games.TryGetValue(gameType, out IGame game))
            {
                return game;
            }
            return default;
        }

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        public IGame GetGame(string gameTypeName)
        {
            return GetGame(Type.GetType(gameTypeName));
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseGame<T>() where T : IGame
        {
            CloseGame(typeof(T));
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameType"></param>
        public void CloseGame(Type gameType)
        {
            IGame game = GetGame(gameType);
            if (game == null)
            {
                return;
            }
            Loader.Release(game);
            games.Remove(gameType);
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        public void CloseGame(string gameTypeName)
        {
            CloseGame(Type.GetType(gameTypeName));
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            foreach (IGame item in games.Values)
            {
                Loader.Release(item);
            }
            games.Clear();
        }

        public void Update()
        {
            if (current == null)
            {
                return;
            }
            current.Update();
        }
    }
}
