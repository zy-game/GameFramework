using System;
using UnityEngine;
using GameFramework.Game;
using System.Collections.Generic;
using GameFramework.Resource;

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

        public static void SetParent(this GameObject basic, Transform parent)
        {
            basic.SetParent(parent, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, Transform parent, Vector3 position)
        {
            basic.SetParent(parent, position, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, Transform parent, Vector3 position, Vector3 rotation)
        {
            basic.SetParent(parent, position, rotation, Vector3.one);
        }

        public static void SetParent(this GameObject basic, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (basic == null)
            {
                return;
            }
            basic.transform.SetParent(parent);
            basic.transform.localPosition = position;
            basic.transform.localRotation = Quaternion.Euler(rotation);
            basic.transform.localScale = scale;
        }

        public static void SetParent(this GameObject basic, GameObject parent)
        {
            basic.SetParent(parent, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, GameObject parent, Vector3 position)
        {
            basic.SetParent(parent, position, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, GameObject parent, Vector3 position, Vector3 rotation)
        {
            basic.SetParent(parent, position, rotation, Vector3.one);
        }

        public static void SetParent(this GameObject basic, GameObject parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.SetParent(parent.transform, position, rotation, scale);
        }

        public static void SetParent(this GameObject basic, Component parent)
        {
            basic.SetParent(parent, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, Component parent, Vector3 position)
        {
            basic.SetParent(parent, position, Vector3.zero);
        }

        public static void SetParent(this GameObject basic, Component parent, Vector3 position, Vector3 rotation)
        {
            basic.SetParent(parent, position, rotation, Vector3.one);
        }

        public static void SetParent(this GameObject basic, Component parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.SetParent(parent.transform, position, rotation, scale);
        }

        public static void SetParent(this Component basic, Component parent)
        {
            basic.SetParent(parent, Vector3.zero);
        }

        public static void SetParent(this Component basic, Component parent, Vector3 position)
        {
            basic.SetParent(parent, position, Vector3.zero);
        }

        public static void SetParent(this Component basic, Component parent, Vector3 position, Vector3 rotation)
        {
            basic.SetParent(parent, position, rotation, Vector3.one);
        }

        public static void SetParent(this Component basic, Component parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.gameObject.SetParent(parent.transform, position, rotation, scale);
        }
    }
}