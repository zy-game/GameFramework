using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 上下文管理器
    /// </summary>
    sealed class ContextManager : IRefrence
    {
        private List<GameContext> contexts;
        private static ContextManager _instance;
        public static ContextManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ContextManager();
                }
                return _instance;
            }
        }

        public ContextManager()
        {
            contexts = new List<GameContext>();
        }

        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GameContext CreateContext(string guid, GameObject gameObject)
        {
            GameContext context = Loader.Generate<GameContext>();
            context.guid = guid;
            context.gameObject = gameObject;
            contexts.Add(context);
            return context;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public GameContext GetGameContext(string guid)
        {
            return contexts.Find(x => x.guid == guid);
        }

        public void Release()
        {
            contexts.ForEach(Loader.Release);
            contexts.Clear();
        }
    }
}
