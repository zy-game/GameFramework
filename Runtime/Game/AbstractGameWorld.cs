using GameFramework.Config;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏世界
    /// </summary>
    public abstract class AbstractGameWorld : IGameWorld
    {
        private bool active;
        private List<IGameScriptSystem> scripts;
        private List<Chunk> chunks = new List<Chunk>();

        internal string resouceModuleName
        {
            get;
            set;
        }

        /// <summary>
        /// 游戏相机
        /// </summary>
        /// <value></value>
        public Camera MainCamera
        {
            get;
            private set;
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

        }

        public void Awake()
        {
            ResHandle handle = ResourceManager.Instance.LoadAssetSync<GameObject>("MainCamera");
            MainCamera = handle.Generate<GameObject>().GetComponent<Camera>();
            if (MainCamera == null)
            {
                throw GameFrameworkException.Generate("Resource not find MainCamera");
            }
            MainCamera.name = this.GetType().Name + "_WorldCamera";
            scripts = new List<IGameScriptSystem>();
            UIManager = DefaultUIFormManager.Generate(this);
            SoundManager = DefaultSoundManager.Generate(this);
            //todo 干脆在这里做模块化资源更新算了
            UIHandle_CommonLoading commonLoading = UIManager.OpenUI<UIHandle_CommonLoading>();
            ResourceManager.Instance.CheckResourceModuleUpdate(resouceModuleName, progres =>
            {
                commonLoading.SetProgresText($"正在更新资源...({(int)(progres * 100)}%)");
                commonLoading.SetLoadingProgres(progres);
            },
            state =>
            {
                UIManager.CloseUI<UIHandle_CommonLoading>();
                OnAwake();
                Enable();
            });
        }

        protected virtual void OnAwake() { }

        public void Enable()
        {
            active = true;
            OnEnable();
        }

        protected virtual void OnEnable() { }

        public void Disable()
        {
            active = false;
            OnDisable();
        }

        protected virtual void OnDisable() { }

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
            if (TryGetEntity(guid, out IEntity entity))
            {
                throw GameFrameworkException.GenerateFormat("the guid is already exsit:{0}", guid);
            }
            DefaultGameEntity gameEntity = DefaultGameEntity.Generate(guid, this);
            //Chunk chunk = chunks.Find(x => x.Contains(gameEntity.tag));
            //if (chunk == null)
            //{
            //    Debug.Log("create entity ");
            //    chunks.Add(chunk = Chunk.Generate(gameEntity.tag));
            //}
            //chunk.Add(gameEntity);
            return gameEntity;
        }

        private bool TryGetEntity(string guid, out IEntity entity)
        {
            Chunk chunk = chunks.Find(x => x.HaveEntity(guid));
            entity = chunk == null ? null : chunk.GetEntity(guid);
            return entity != null;
        }

        /// <summary>
        /// 获取指定的实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEntity GetEntity(string id)
        {
            if (TryGetEntity(id, out IEntity entity))
            {
                return entity;
            }
            return default;
        }

        /// <summary>
        /// 加载游戏逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T AddScriptble<T>() where T : IGameScriptSystem
        {
            RemoveScriptble<T>();
            T script = Loader.Generate<T>();
            scripts.Add(script);
            return script;
        }

        /// <summary>
        /// 回收游戏
        /// </summary>
        public virtual void Release()
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                IEntity[] entities = chunks[i].GetEntities();
                for (int j = 0; j < entities.Length; j++)
                {
                    Loader.Release(entities[j]);
                    IComponent[] components = chunks[i].GetComponents(entities[j]);
                    for (int k = 0; k < components.Length; k++)
                    {
                        Loader.Release(components[k]);
                    }
                }
                Loader.Release(chunks[i]);
            }
            chunks.Clear();
            foreach (var item in scripts)
            {
                Loader.Release(item);
            }
            scripts.Clear();
            Loader.Release(UIManager);
            Loader.Release(SoundManager);
            UIManager = null;
            SoundManager = null;
            GameObject.DestroyImmediate(MainCamera.gameObject);
            Debug.Log($"release {this.GetType().Name} world");
        }

        /// <summary>
        /// 移除实体对象
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEntity(string guid)
        {
            Chunk chunk = chunks.Find(x => x.HaveEntity(guid));
            if (chunk == null)
            {
                return;
            }
            IEntity entity = chunk.GetEntity(guid);
            IComponent[] components = chunk.GetComponents(entity);
            Array.ForEach(components, Loader.Release);
            chunk.Remove(entity);
            Loader.Release(entity);
        }

        /// <summary>
        /// 卸载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveScriptble<T>() where T : IGameScriptSystem
        {
            IGameScriptSystem script = scripts.Find(x => x.GetType() == typeof(T));
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
            if (!active)
            {
                return;
            }
            for (int i = scripts.Count - 1; i >= 0; i--)
            {
                SafeRun(scripts[i].Running);
            }
        }

        private void SafeRun(GameFrameworkAction runner)
        {
            try
            {
                runner();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw e;
            }
        }


        internal IComponent INTERNAL_EntityAddComponent(IEntity entity, Type commponentType)
        {
            DefaultGameEntity defaultGameEntity = (DefaultGameEntity)entity;
            if (defaultGameEntity == null)
            {
                return default;
            }
            ComponentAttribute attribute = commponentType.GetCustomAttribute<ComponentAttribute>();
            if (attribute == null)
            {
                Debug.LogError($"the component:{commponentType.Name} not added component attribute");
                return default;
            }
            IComponent component = (IComponent)Loader.Generate(commponentType);
            IComponent[] components = null;
            Chunk chunk = chunks.Find(x => x.Contains(defaultGameEntity.tag));
            if (chunk != null)
            {
                components = chunk.GetComponents(defaultGameEntity);
                chunk.Remove(entity);
            }

            defaultGameEntity.tag = defaultGameEntity.tag | attribute.tag;
            chunk = chunks.Find(x => x.tag == defaultGameEntity.tag);
            if (chunk == null)
            {
                chunks.Add(chunk = Chunk.Generate(defaultGameEntity.tag));
            }
            chunk.Add(entity);
            chunk.AddComponent(entity, components);
            chunk.AddComponent(entity, component);
            return component;
        }

        internal IComponent[] INTERNAL_GetEntityComponents(IEntity entity, params Type[] componentTypes)
        {
            DefaultGameEntity defaultGameEntity = (DefaultGameEntity)entity;
            if (defaultGameEntity == null)
            {
                return default;
            }
            Chunk chunk = chunks.Find(x => x.tag == defaultGameEntity.tag);
            if (chunk == null)
            {
                return default;
            }
            return chunk.GetComponents(defaultGameEntity, componentTypes);
        }

        internal bool INTERNAL_RemoveEntityComponent(IEntity entity, Type componentType)
        {
            DefaultGameEntity defaultGameEntity = (DefaultGameEntity)entity;
            if (defaultGameEntity == null)
            {
                return default;
            }
            ComponentAttribute attribute = componentType.GetCustomAttribute<ComponentAttribute>();
            if (attribute == null)
            {
                Debug.LogError($"the component:{componentType.Name} not added component attribute");
                return default;
            }
            Chunk chunk = chunks.Find(x => x.Contains(defaultGameEntity.tag));
            if (chunk == null)
            {
                return false;
            }
            chunk.RemoveComponent(entity, componentType);
            IComponent[] components = chunk.GetComponents(entity);
            chunk.Remove(entity);
            defaultGameEntity.tag = defaultGameEntity.tag & ~attribute.tag;
            chunk = chunks.Find(x => x.Contains(defaultGameEntity.tag));
            if (chunk == null)
            {
                Debug.Log("remove component");
                chunks.Add(chunk = Chunk.Generate(defaultGameEntity.tag));
            }
            chunk.Add(entity);
            chunk.AddComponent(entity, components);
            return true;
        }

        public IEntity[] GetEntities(params string[] componentTypeNames)
        {
            if (componentTypeNames == null || componentTypeNames.Length <= 0)
            {
                return default;
            }

            int componentTags = 0;
            for (var i = 0; i < componentTypeNames.Length; i++)
            {
                Type componentType = Type.GetType(componentTypeNames[i]);
                if (componentType == null)
                {
                    Debug.LogError("not find the component type:" + componentTypeNames[i]);
                    return default;
                }
                ComponentAttribute attribute = componentType.GetCustomAttribute<ComponentAttribute>();
                if (attribute == null)
                {
                    Debug.LogError($"the component:{componentTypeNames[i]} not added component attribute");
                    return default;
                }
                componentTags = componentTags | attribute.tag;
            }
            return GetEntities(componentTags);
        }

        public IEntity[] GetEntities(params Type[] componentTypes)
        {
            int componentTags = 0;
            for (var i = 0; i < componentTypes.Length; i++)
            {
                ComponentAttribute attribute = componentTypes[i].GetCustomAttribute<ComponentAttribute>();
                if (attribute == null)
                {
                    Debug.LogError($"the component:{componentTypes[i].Name} not added component attribute");
                    return default;
                }
                componentTags = componentTags | attribute.tag;
            }
            return GetEntities(componentTags);
        }

        public IEntity[] GetEntities(params int[] componentAttribute)
        {
            int componentTags = 0;
            for (int i = 0; i < componentAttribute.Length; i++)
            {
                componentTags = componentTags | componentAttribute[i];
            }
            return GetEntities(componentTags);
        }

        public IEntity[] GetEntities(int componentAttribute)
        {
            Chunk chunk = chunks.Find(x => x.Contains(componentAttribute));
            if (chunk == null)
            {
                return Array.Empty<IEntity>();
            }
            return chunk.GetEntities();
        }

        class Chunk : IRefrence
        {
            public int tag;
            private Dictionary<string, IEntity> entitys;
            private Dictionary<string, List<IComponent>> components;

            public Chunk()
            {
                entitys = new Dictionary<string, IEntity>();
                components = new Dictionary<string, List<IComponent>>();
            }

            public int Count
            {
                get
                {
                    return entitys.Count;
                }
            }
            public void Release()
            {
                entitys.Clear();
                components.Clear();
            }

            public bool Contains(int tag)
            {
                return (this.tag & tag) != 0;
            }

            public void Add(IEntity entity)
            {
                if (entitys.TryGetValue(entity.guid, out _))
                {
                    Debug.LogError("the entity is already exist");
                    return;
                }
                entitys.Add(entity.guid, entity);
            }
            public void Remove(IEntity entity)
            {
                if (entitys.TryGetValue(entity.guid, out _))
                {
                    entitys.Remove(entity.guid);
                    components.Remove(entity.guid);
                }
            }

            public IEntity[] GetEntities()
            {
                return entitys.Values.ToArray();
            }

            public IEntity GetEntity(string guid)
            {
                if (entitys.TryGetValue(guid, out IEntity entity))
                {
                    return entity;
                }
                return default;
            }

            public bool HaveEntity(string guid)
            {
                return entitys.ContainsKey(guid);
            }

            public static Chunk Generate(int tag)
            {
                Chunk chunk = Loader.Generate<Chunk>();
                chunk.tag = tag;
                return chunk;
            }

            internal IComponent[] GetComponents(IEntity entity, params Type[] componentTypes)
            {
                if (!components.TryGetValue(entity.guid, out List<IComponent> list))
                {
                    return default;
                }
                if (componentTypes == null || componentTypes.Length <= 0)
                {
                    return list.ToArray();
                }
                IComponent[] result = new IComponent[componentTypes.Length];
                for (int i = 0; i < componentTypes.Length; i++)
                {
                    result[i] = list.Find(x => x.GetType() == componentTypes[i]);
                    if (result[i] == null)
                    {
                        return default;
                    }
                }
                return result;
            }

            internal bool RemoveComponent(IEntity entity, params Type[] componentType)
            {
                if (!components.TryGetValue(entity.guid, out List<IComponent> list))
                {
                    return default;
                }
                if (componentType == null || componentType.Length <= 0)
                {
                    return false;
                }
                for (int i = 0; i < componentType.Length; i++)
                {
                    IComponent component = list.Find(x => x.GetType() == componentType[i]);
                    if (component == null)
                    {
                        continue;
                    }
                    list.Remove(component);
                }
                return true;
            }

            internal void AddComponent(IEntity entity, params IComponent[] component)
            {
                if (component == null || component.Length <= 0)
                {
                    return;
                }
                if (!components.TryGetValue(entity.guid, out List<IComponent> list))
                {
                    components.Add(entity.guid, list = new List<IComponent>());
                }
                Array.ForEach(component, args => list.Add(args));
            }
        }
    }

    public sealed class ComponentAttribute : Attribute
    {
        public int tag;
        public ComponentAttribute(int tag)
        {
            this.tag = 1 << tag;
        }
    }

    [Component(1)]
    public sealed class TransformComponent : IComponent
    {
        public Transform transform { get; private set; }
        public Vector3 localPosition
        {
            get
            {
                return transform.localPosition;
            }
            set
            {
                transform.localPosition = value;
            }
        }
        public Vector3 localRotation
        {
            get
            {
                return transform.localRotation.eulerAngles;
            }
            set
            {
                transform.localRotation = Quaternion.Euler(value);
            }
        }
        public Vector3 localScale
        {
            get
            {
                return transform.localScale;
            }
            set
            {
                transform.localScale = value;
            }
        }
        public Vector3 worldPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public static TransformComponent GenerateTransform(IEntity entity, string assetName)
        {
            TransformComponent component = entity.AddComponent<TransformComponent>();
            ResHandle handle = ResourceManager.Instance.LoadAssetSync<GameObject>(assetName);
            handle.EnsueAssetLoadState();
            component.transform = handle.Generate<Transform>();
            return component;
        }

        public void Release()
        {
            GameObject.DestroyImmediate(transform.gameObject);
        }
    }
}
