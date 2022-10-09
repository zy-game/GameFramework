using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏实体
    /// </summary>
    public sealed class GameEntity : IEntity
    {
        private Dictionary<Type, IComponent> components;

        /// <summary>
        /// 实体唯一ID
        /// </summary>
        /// <value></value>
        public string guid { get; private set; }

        /// <summary>
        /// 所属游戏
        /// </summary>
        /// <value></value>
        public IGameWorld owner { get; private set; }

        public GameEntity()
        {
            components = new Dictionary<Type, IComponent>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : IComponent => (T)AddComponent(typeof(T));

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IComponent AddComponent(Type componentType)
        {
            if (components.TryGetValue(componentType, out IComponent component))
            {
                throw GameFrameworkException.Generate("the entity is already exsit component");
            }
            component = (IComponent)Loader.Generate(componentType);
            components.Add(componentType, component);
            GameWorld gameWorld = (GameWorld)owner;
            if (gameWorld != null)
            {
                gameWorld.INTERNAL_EntityComponentChange(this);
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        /// <returns></returns>
        public IComponent AddComponent(string componentTypeName) => AddComponent(Type.GetType(componentTypeName));

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : IComponent => (T)GetComponent(typeof(T));

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IComponent GetComponent(Type componentType)
        {
            if (components.TryGetValue(componentType, out IComponent component))
            {
                return component;
            }
            return default;
        }

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        /// <returns></returns>
        public IComponent GetComponent(string componentTypeName) => GetComponent(Type.GetType(componentTypeName));

        /// <summary>
        /// 获取当前实体上所有的组件
        /// </summary>
        /// <returns></returns>
        public IComponent[] GetComponents()
        {
            return components.Values.ToArray();
        }

        internal static GameEntity Generate(string guid, IGameWorld game)
        {
            GameEntity entity = Loader.Generate<GameEntity>();
            entity.guid = guid;
            entity.owner = game;
            return entity;
        }

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        public IComponent[] GetComponents(params Type[] componentTypes)
        {
            if (componentTypes == null || componentTypes.Length <= 0 || components.Count <= 0)
            {
                return Array.Empty<IComponent>();
            }
            List<IComponent> results = new List<IComponent>();
            for (var i = 0; i < componentTypes.Length; i++)
            {
                if (components.TryGetValue(componentTypes[i], out IComponent component))
                {
                    results.Add(component);
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypeNames"></param>
        /// <returns></returns>
        public IComponent[] GetComponents(params string[] componentTypeNames)
        {
            if (componentTypeNames == null || componentTypeNames.Length < 0 || components.Count <= 0)
            {
                return Array.Empty<IComponent>();
            }
            Type[] types = new Type[componentTypeNames.Length];
            for (var i = 0; i < componentTypeNames.Length; i++)
            {
                types[i] = Type.GetType(componentTypeNames[i]);
            }
            return GetComponents(types);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>() where T : IComponent => RemoveComponent(typeof(T));

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="componentType"></param>
        public void RemoveComponent(Type componentType)
        {
            if (components.TryGetValue(componentType, out IComponent component))
            {
                Loader.Release(component);
                components.Remove(componentType);
                GameWorld gameWorld = (GameWorld)owner;
                if (gameWorld == null)
                {
                    return;
                }
                gameWorld.INTERNAL_EntityComponentChange(this);
            }
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        public void RemoveComponent(string componentTypeName) => RemoveComponent(Type.GetType(componentTypeName));

        /// <summary>
        /// 回收实体
        /// </summary>
        public void Release()
        {
            foreach (KeyValuePair<Type, IComponent> item in components)
            {
                Loader.Release(item.Value);
            }
            components.Clear();
        }
    }
}
