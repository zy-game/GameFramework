using System;


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
    }
}