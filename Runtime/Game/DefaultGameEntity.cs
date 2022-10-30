using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏实体
    /// </summary>
    public sealed class DefaultGameEntity : IEntity
    {
        /// <summary>
        /// 实体唯一ID
        /// </summary>
        /// <value></value>
        public string guid { get; private set; }

        internal int tag { get; set; }

        /// <summary>
        /// 所属游戏
        /// </summary>
        /// <value></value>
        public IGameWorld owner
        {
            get
            {
                return gameWorld;
            }
        }

        private AbstractGameWorld gameWorld;

        public DefaultGameEntity()
        {
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
            return gameWorld.INTERNAL_EntityAddComponent(this, componentType);
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
            IComponent[] components = gameWorld.INTERNAL_GetEntityComponents(this, componentType);
            if (components == null || components.Length <= 0)
            {
                return default;
            }
            return components[0];
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
            return gameWorld.INTERNAL_GetEntityComponents(this);
        }

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        public IComponent[] GetComponents(params Type[] componentTypes)
        {
            return gameWorld.INTERNAL_GetEntityComponents(this, componentTypes);
        }

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypeNames"></param>
        /// <returns></returns>
        public IComponent[] GetComponents(params string[] componentTypeNames)
        {
            if (componentTypeNames == null || componentTypeNames.Length < 0)
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
            gameWorld.INTERNAL_RemoveEntityComponent(this, componentType);
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
            gameWorld = null;
        }

        public T[] GetComponents<T>() where T : IComponent
        {
            return GetComponents(typeof(T)).Cast<T>().ToArray();
        }

        public IComponent[] GetComponents(Type componentType)
        {
            return gameWorld.INTERNAL_GetEntityComponents(this, componentType);
        }

        internal static DefaultGameEntity Generate(string guid, IGameWorld game)
        {
            DefaultGameEntity entity = Loader.Generate<DefaultGameEntity>();
            entity.guid = guid;
            entity.gameWorld = (AbstractGameWorld)game;
            return entity;
        }
    }
}
