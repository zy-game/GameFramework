using System;
using System.Collections.Generic;

namespace GameFramework.Datable
{
    public sealed class DatableManager : IDatableManager
    {
        private Dictionary<Type, IGameDatable> datables = new Dictionary<Type, IGameDatable>();

        /// <summary>
        /// 生成游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        /// <returns>游戏数据表</returns>
        public IGameDatable GenerateGameDatable(Type gameDatableType)
        {
            IGameDatable datable = GetGameDatable(gameDatableType);
            if (datable != null)
            {
                return datable;
            }
            datable = (IGameDatable)Creater.Generate(gameDatableType);
            datables.Add(gameDatableType, datable);
            return datable;
        }
        /// <summary>
        /// 获取游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns>游戏数据表</returns>
        public IGameDatable GetGameDatable(Type gameDatableType)
        {
            gameDatableType.EnsureObjectRefrenceType<IGameDatable>();
            if (!datables.TryGetValue(gameDatableType, out IGameDatable datable))
            {
                return default;
            }
            return datable;
        }
        /// <summary>
        /// 是否存在游戏数据表
        /// </summary>
        /// <typeparam name="gameDatableType">数据表类型</typeparam>
        /// <returns></returns>
        public bool IsHaveGameDatable(Type gameDatableType)
        {
            gameDatableType.EnsureObjectRefrenceType<IGameDatable>();
            return datables.ContainsKey(gameDatableType);
        }

        public void Release()
        {
            
        }

        /// <summary>
        /// 移除游戏数据表
        /// </summary>
        /// <param name="gameDatableType">数据表类型</param>
        public void RemoveGameDatable(Type gameDatableType)
        {
            gameDatableType.EnsureObjectRefrenceType<IGameDatable>();
            if (!datables.TryGetValue(gameDatableType, out IGameDatable datable))
            {
                return;
            }
            Creater.Release(datable);
            datables.Remove(gameDatableType);
        }

        public void Update()
        {
        }
    }
}
