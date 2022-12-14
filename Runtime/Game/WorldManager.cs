using System.Collections.Generic;
using System;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public sealed class WorldManager : SingletonBehaviour<WorldManager>, IWorldManager
    {
        private Dictionary<Type, IGameWorld> games = new Dictionary<Type, IGameWorld>();

        /// <summary>
        /// 当前游戏
        /// </summary>
        /// <value></value>
        public IGameWorld current { get; private set; }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenWorld<T>(string resouceModuleName) where T : IGameWorld
        {
            return (T)OpenWorld(typeof(T), resouceModuleName);
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="worldType"></param>
        /// <returns></returns>
        public IGameWorld OpenWorld(Type worldType, string resouceModuleName)
        {
            if (!games.TryGetValue(worldType, out IGameWorld gameWorld))
            {
                gameWorld = (IGameWorld)Loader.Generate(worldType);
                ((AbstractGameWorld)gameWorld).resouceModuleName = resouceModuleName;
                games.Add(worldType, gameWorld);
            }
            current = gameWorld;
            current.Awake();
            return gameWorld;
        }

        /// <summary>
        /// 打开游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        /// <returns></returns>
        public IGameWorld OpenWorld(string gameTypeName, string resouceModuleName)
        {
            return OpenWorld(Type.GetType(gameTypeName), resouceModuleName);
        }

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetWorld<T>() where T : IGameWorld
        {
            return (T)GetWorld(typeof(T));
        }

        /// <summary>
        /// 获取指定的游戏
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public IGameWorld GetWorld(Type gameType)
        {
            if (games.TryGetValue(gameType, out IGameWorld game))
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
        public IGameWorld GetWorld(string gameTypeName)
        {
            return GetWorld(Type.GetType(gameTypeName));
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWorld<T>() where T : IGameWorld
        {
            CloseWorld(typeof(T));
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameType"></param>
        public void CloseWorld(Type gameType)
        {
            IGameWorld game = GetWorld(gameType);
            if (game == null)
            {
                return;
            }
            game.Disable();
            Loader.Release(game);
            games.Remove(gameType);
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="gameTypeName"></param>
        public void CloseWorld(string gameTypeName)
        {
            CloseWorld(Type.GetType(gameTypeName));
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            foreach (IGameWorld item in games.Values)
            {
                Loader.Release(item);
            }
            games.Clear();
        }

        protected override void Update()
        {
            if (current == null)
            {
                return;
            }
            current.Update();
        }
    }
}
