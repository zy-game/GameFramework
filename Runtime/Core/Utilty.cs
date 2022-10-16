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


        /// <summary>
        /// 【秒级】获取时间（北京时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetDateTimeSeconds(long timestamp)
        {
            long begtime = timestamp * 10000000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【秒级】生成10位时间戳（北京时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetTimeStampSeconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }

        /// <summary>
        /// 【毫秒级】获取时间（北京时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetDateTimeMilliseconds(long timestamp)
        {
            long begtime = timestamp * 10000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【毫秒级】生成13位时间戳（北京时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetTimeStampMilliseconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        }

        /// <summary>
        /// 【秒级】获取时间（格林威治时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetUnixDateTimeSeconds(long timestamp)
        {
            long begtime = timestamp * 10000000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【秒级】生成10位时间戳（格林威治时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetUnixTimeStampSeconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }

        /// <summary>
        /// 【毫秒级】获取时间（格林威治时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetUnixDateTimeMilliseconds(long timestamp)
        {
            long begtime = timestamp * 10000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【毫秒级】生成13位时间戳（格林威治时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetUnixTimeStampMilliseconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        }

        public static string GetLastDirectory(string path)
        {
            return new System.IO.DirectoryInfo(path).Name;
        }
    }
}