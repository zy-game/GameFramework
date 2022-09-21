using System;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 实体对象
    /// </summary>
    public interface IEntity : IRefrence
    {
        /// <summary>
        /// 实体唯一ID
        /// </summary>
        /// <value></value>
        string guid { get; }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T AddComponent<T>() where T : IComponent;

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        IComponent AddComponent(Type componentType);
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        /// <returns></returns>
        IComponent AddComponent(string componentTypeName);

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>() where T : IComponent;

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        IComponent GetComponent(Type componentType);

        /// <summary>
        /// 获取指定的组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        /// <returns></returns>
        IComponent GetComponent(string componentTypeName);

        /// <summary>
        /// 获取当前实体上所有的组件
        /// </summary>
        /// <returns></returns>
        IComponent[] GetComponents();

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        IComponent[] GetComponents(params Type[] componentTypes);

        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <param name="componentTypeNames"></param>
        /// <returns></returns>
        IComponent[] GetComponents(params string[] componentTypeNames);

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveComponent<T>() where T : IComponent;

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="componentType"></param>
        void RemoveComponent(Type componentType);

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="componentTypeName"></param>
        void RemoveComponent(string componentTypeName);


    }
}
