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
        /// 将游戏物体与实体绑定
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="entity"></param>
        public static void Content(this GameObject gameObject, IEntity entity)
        {
            ContextManager.instance.CreateContext(entity.guid, gameObject);
        }

        /// <summary>
        /// 将实体与游戏物体绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="gameObject"></param>
        public static void Content(this IEntity entity, GameObject gameObject)
        {
            ContextManager.instance.CreateContext(entity.guid, gameObject);
        }

        /// <summary>
        /// 获取与实体绑定的游戏物体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static GameObject GetObject(this IEntity entity)
        {
            GameContext context = ContextManager.instance.GetGameContext(entity.guid);
            if (context == null)
            {
                return default;
            }
            return context.gameObject;
        }
    }
}