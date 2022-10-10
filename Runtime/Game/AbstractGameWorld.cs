using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Jobs;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏世界
    /// </summary>
    public abstract class AbstractGameWorld : IGameWorld
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
        public Camera MainCamera
        {
            get;
        }

        /// <summary>
        /// UI相机
        /// </summary>
        /// <value></value>
        public IUIFormManager UIManager
        {
            get;
            private set;
        }

        /// <summary>
        /// 音效管理器
        /// </summary>
        /// <value></value>
        public ISoundManager SoundManager
        {
            get;
            private set;
        }

        public AbstractGameWorld()
        {
            Resource.ResHandle handle = Runtime.GetGameModule<Resource.ResourceManager>().LoadAssetSync("MainCamera");
            MainCamera = handle.Generate<GameObject>().GetComponent<Camera>();
            if (MainCamera == null)
            {
                throw GameFrameworkException.Generate("Resource not find MainCamera");
            }
            scripts = new List<IGameScript>();
            entitys = new Dictionary<string, IEntity>();
            UIManager = DefaultUIFormManager.Generate(this);
            SoundManager = DefaultSoundManager.Generate(this);
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
            DefaultGameEntity gameEntity = DefaultGameEntity.Generate(guid, this);
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
        public void AddScriptble<T>() where T : IGameScript
        {
            RemoveScriptble<T>();
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
            GameObject.DestroyImmediate(MainCamera.gameObject);
            Debug.Log("release world");
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
        public void RemoveScriptble<T>() where T : IGameScript
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
        public void Update()
        {
            for (int i = scripts.Count - 1; i >= 0; i--)
            {
                SafeRun(scripts[i].Executed);
            }
        }

        private void SafeRun(GameFrameworkAction<IGameWorld> runner)
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

    public sealed class Context
    {
        class Chunk : IRefrence
        {
            private List<Type> componentTypes;
            private List<IEntity> entities = new List<IEntity>();
            public void Release()
            {
                entities.Clear();
            }

            public void SetTags(Type[] componentTypes)
            {
                this.componentTypes = componentTypes.ToList();
            }

            public void Add(IEntity entity)
            {
                entities.Add(entity);
            }
            public void Remove(IEntity entity)
            {
                entities.Remove(entity);
            }
            public bool Contains(Type[] componentTypes)
            {
                for (var i = 0; i < componentTypes.Length; i++)
                {
                    if (!this.componentTypes.Contains(componentTypes[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public IEntity[] GetEntities()
            {
                return entities.ToArray();
            }
        }
        private static List<Chunk> chunks = new List<Chunk>();
        internal static void INTERNAL_EntityComponentChange(IEntity entity, Type[] componentTypes)
        {

            chunks.ForEach(x => x.Remove(entity));
            Chunk chunk = chunks.Find(x => x.Contains(componentTypes));
            if (chunk == null)
            {
                chunk = Loader.Generate<Chunk>();
                chunk.SetTags(componentTypes);
                chunks.Add(chunk);
            }
            chunk.Add(entity);
        }

        public static IEntity[] GetEntities(params string[] componentTypeNames)
        {
            if (componentTypeNames == null || componentTypeNames.Length <= 0)
            {
                return default;
            }

            Type[] componentTypes = new Type[componentTypeNames.Length];
            for (var i = 0; i < componentTypeNames.Length; i++)
            {
                componentTypes[i] = Type.GetType(componentTypeNames[i]);
            }
            return GetEntities(componentTypes);
        }

        public static IEntity[] GetEntities(params Type[] componentTypes)
        {
            Chunk chunk = chunks.Find(x => x.Contains(componentTypes));
            if (chunk == null)
            {
                return Array.Empty<IEntity>();
            }
            return chunk.GetEntities();
        }
    }
}
