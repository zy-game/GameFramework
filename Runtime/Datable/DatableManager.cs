using System;
using System.Collections.Generic;

namespace GameFramework.Datable
{
    public sealed class DatableManager : IDatableManager
    {
        private Dictionary<string, IGameDatable> datables = new Dictionary<string, IGameDatable>();

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        public T CreateDatable<T>() where T : IGameDatable
        {
            return (T)CreateDatable(typeof(T));
        }

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        /// <returns>游戏数据表</returns>
        public IGameDatable CreateDatable(Type gameDatableType)
        {
            if (gameDatableType == null)
            {
                throw GameFrameworkException.Generate<NullReferenceException>();
            }
            if (gameDatableType.IsAbstract || gameDatableType.IsInterface)
            {
                throw GameFrameworkException.Generate("datable type cannot be interface or abstract");
            }
            if (!typeof(IGameDatable).IsAssignableFrom(gameDatableType))
            {
                throw GameFrameworkException.Generate("the datable is not impart IGameDatable");
            }
            return CreateDatable((IGameDatable)Loader.Generate(gameDatableType));
        }

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        /// <returns>游戏数据表</returns>
        public IGameDatable CreateDatable(IGameDatable gameDatable)
        {
            if (datables.TryGetValue(gameDatable.guid, out IGameDatable _))
            {
                throw GameFrameworkException.Generate("the datable is already exist");
            }
            datables.Add(gameDatable.guid, gameDatable);
            return gameDatable;
        }

        /// <summary>
        /// 是否存在游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns></returns>
        public bool IsHaveGameDatable(string guid) => datables.ContainsKey(guid);

        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        public T GetGameDatable<T>(string guid) where T : IGameDatable => (T)GetGameDatable(guid);

        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        public IGameDatable GetGameDatable(string guid)
        {
            if (datables.TryGetValue(guid, out IGameDatable datable))
            {
                return datable;
            }
            return default;
        }

        /// <summary>
        /// 移除游戏数据表
        /// </summary>
        /// <typeparam name="T">数据表类型</typeparam>
        public void RemoveGameDatable(string guid)
        {
            if (IsHaveGameDatable(guid))
            {
                Loader.Release(datables[guid]);
                datables.Remove(guid);
            }
        }

        /// <summary>
        /// 清理所有数据表
        /// </summary>
        public void Clear()
        {
            foreach (var item in datables.Values)
            {
                Loader.Release(item);
            }
            datables.Clear();
        }

        /// <summary>
        /// 轮询
        /// </summary>
        public void Update() { }

        /// <summary>
        /// 回收数据管理器
        /// </summary>
        public void Release()
        {
            Clear();
        }
    }
}
