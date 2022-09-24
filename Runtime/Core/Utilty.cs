using System;
using UnityEngine;
using GameFramework.Game;

namespace GameFramework
{
    public static partial class Utilty
    {
        /// <summary>
        /// 检查type类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public static void EnsureObjectRefrenceType<T>(this Type type)
        {
            if (type == null)
            {
                throw GameFrameworkException.Generate("Parameter cannot be empty");
            }
            if (type.IsValueType)
            {
                throw GameFrameworkException.Generate("Cannot instantiate an value object");
            }
            if (type.IsAbstract)
            {
                throw GameFrameworkException.Generate("Cannot instantiate an abstract object");
            }
            if (type.IsInterface)
            {
                throw GameFrameworkException.Generate("Cannot instantiate an interface object");
            }
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw GameFrameworkException.Generate("This object is an implementation IRefrence interface");
            }
        }

        /// <summary>
        /// 获取与实体绑定的游戏物体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static GameObject GetObject(this IEntity entity)
        {
            return GameContext.GetObject(entity.guid);
        }

        /// <summary>
        /// 删除当前实体
        /// </summary>
        /// <param name="entity"></param>
        public static void DestoryEntity(this IEntity entity)
        {
            GameObject gameObject = entity.GetObject();
            if (gameObject != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            entity.owner.RemoveEntity(entity.guid);
        }
    }
}