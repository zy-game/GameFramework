using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏世界
    /// </summary>
    public abstract class GameWorld : IGame
    {
        private List<IGameScript> scripts;
        private Dictionary<string, IEntity> entitys;
        public abstract string name { get; }

        /// <summary>
        /// 实体数量
        /// </summary>
        /// <value></value>
        public int EntityCount
        {
            get
            {
                return entitys.Count;
            }
        }

        /// <summary>
        /// 游戏相机
        /// </summary>
        /// <value></value>
        public Camera GameCamera
        {
            get;
        }

        /// <summary>
        /// UI相机
        /// </summary>
        /// <value></value>
        public IUIManager UIManager
        {
            get;
            private set;
        }

        public GameWorld()
        {
            UIManager = new DefaultUIManager(this);
            GameCamera = GameObject.Instantiate(Resources.Load<Camera>("MainCamera"));
            if (GameCamera == null)
            {
                throw GameFrameworkException.Generate("Resource not find MainCamera");
            }
            scripts = new List<IGameScript>();
            entitys = new Dictionary<string, IEntity>();
        }

        /// <summary>
        /// 创建游戏实体
        /// </summary>
        /// <returns></returns>
        public IEntity CreateEntity()
        {
            return CreateEntity(Guid.NewGuid().ToString().ToLower());
        }

        /// <summary>
        /// 创建游戏实体
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IEntity CreateEntity(string guid)
        {
            if (entitys.TryGetValue(guid, out IEntity entity))
            {
                throw GameFrameworkException.GenerateFormat("the guid is already exsit:{0}", guid);
            }
            GameEntity gameEntity = GameEntity.Generate(guid, this);
            entitys.Add(guid, gameEntity);
            return gameEntity;
        }

        /// <summary>
        /// 获取指定的实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEntity GetEntity(string id)
        {
            if (entitys.TryGetValue(id, out IEntity entity))
            {
                return entity;
            }
            return default;
        }

        /// <summary>
        /// 加载游戏逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void LoadScript<T>() where T : IGameScript
        {
            UnloadScript<T>();
            T script = Loader.Generate<T>();
            scripts.Add(script);
        }

        /// <summary>
        /// 回收游戏
        /// </summary>
        public virtual void Release()
        {
            foreach (var item in entitys.Values)
            {
                Loader.Release(item);
            }
            entitys.Clear();
            foreach (var item in scripts)
            {
                Loader.Release(item);
            }
            scripts.Clear();
            Loader.Release(UIManager);
            UIManager = null;
        }

        /// <summary>
        /// 移除实体对象
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEntity(string id)
        {
            IEntity entity = GetEntity(id);
            if (entity == null)
            {
                return;
            }
            entitys.Remove(id);
            Loader.Release(entity);
        }

        /// <summary>
        /// 卸载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UnloadScript<T>() where T : IGameScript
        {
            IGameScript script = scripts.Find(x => x.GetType() == typeof(T));
            if (script == null)
            {
                return;
            }
            scripts.Remove(script);
        }

        /// <summary>
        /// 轮询
        /// </summary>
        public virtual void Update()
        {
            for (int i = scripts.Count - 1; i >= 0; i--)
            {
                SafeRun(scripts[i].Executed);
            }
        }

        private void SafeRun(GameFrameworkAction<IGame> runner)
        {
            try
            {
                runner(this);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
