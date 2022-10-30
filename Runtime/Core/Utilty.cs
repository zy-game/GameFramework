using System;
using UnityEngine;
using GameFramework.Game;
using System.Collections.Generic;
using GameFramework.Resource;
using System.Collections;

namespace GameFramework
{

    public sealed class Varable<T>
    {
        public T Value { get; private set; }
        private event GameFrameworkAction<T> OnValueChanged;
        public void SetValue(T value)
        {
            Value = value;
            OnValueChanged?.Invoke(Value);
        }

        public void RegisterChangeCallback(GameFrameworkAction<T> callback)
        {
            OnValueChanged += callback;
            OnValueChanged(Value);
        }

        public void UnregisterChangeCallback(GameFrameworkAction<T> callback)
        {
            OnValueChanged -= callback;
        }
    }
    public abstract class Singleton<T> where T : IRefrence
    {
        private static Lazy<T> instance = new Lazy<T>(() => Loader.Generate<T>());

        public static T Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }

    public abstract partial class SingletonBehaviour<T> where T : IRefrence
    {
        private static GameObject behaviourObject;
        private static Lazy<T> instance = new Lazy<T>(() =>
        {
            if (behaviourObject == null)
            {
                behaviourObject = GameObject.Find("Singleton");
                if (behaviourObject == null)
                {
                    behaviourObject = new GameObject("Singleton");
                }
                GameObject.DontDestroyOnLoad(behaviourObject);
            }
            return Loader.Generate<T>();
        });
        BehaviourAdpter component;
        public static T Instance
        {
            get
            {

                return instance.Value;
            }
        }

        public SingletonBehaviour()
        {
            component = behaviourObject.AddComponent<BehaviourAdpter>();
            component.update = Update;
            component.fixedUpdate = FixedUpdate;
            component.lateUpdate = LateUpdate;
            component.applictionQuit = ApplictionQuit;
        }

        protected virtual void Update() { }
        protected virtual void FixedUpdate() { }
        protected virtual void LateUpdate() { }
        protected virtual void ApplictionQuit() { }

        public void StartCoroutine(IEnumerator ie)
        {
            component.StartCoroutine(ie);
        }

        public void StartCoroutine(AsyncOperation ie)
        {
            component.StartCoroutine(INTERNAL_RunningIEnumerator(ie));
        }

        static IEnumerator INTERNAL_RunningIEnumerator(AsyncOperation ie)
        {
            yield return ie;
        }
    }

    public static partial class Utilty
    {
        public static Transform EmptyTransform = null;
        public static GameObject EmptyGameObject = null;
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
        /// 判断在基于位置的是否在前后
        /// </summary>
        /// <param name="baseTrans"></param>
        /// <param name="dir"></param>
        /// <returns>true:前；false:后</returns>
        public static bool IsForward(Transform baseTrans, Vector3 pos)
        {
            return GetIsForwardValue(baseTrans, pos) >= 0 ? true : false;
        }

        /// <summary>
        /// 判断在基于位置的是否在左右
        /// </summary>
        /// <param name="baseTrans"></param>
        /// <param name="dir"></param>
        /// <returns>true:右；false:左</returns>
        public static bool IsRight(Transform baseTrans, Vector3 pos)
        {
            return GetIsRightValue(baseTrans, pos) >= 0 ? true : false;
        }

        // 大于0，前
        // pos 为 0 ，结果会出错 (做好是相对位置)
        public static float GetIsForwardValue(Transform baseTrans, Vector3 pos)
        {
            return Vector3.Dot(baseTrans.forward, pos - baseTrans.position);
        }

        // 大于0，右边
        // pos 为 0 ，结果会出错(做好是相对位置)
        public static float GetIsRightValue(Transform baseTrans, Vector3 pos)
        {
            return Vector3.Cross(baseTrans.forward, pos - baseTrans.position).y;
        }

        public static void SetParent(this GameObject basic, GameObject parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.transform.SetParent(parent, position, rotation, scale);
        }

        public static void SetParent(this GameObject basic, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.transform.SetParent(parent, position, rotation, scale);
        }

        public static void SetParent(this Transform basic, GameObject parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            basic.SetParent(parent.transform, position, rotation, scale);
        }

        public static void SetParent(this Transform basic, Transform parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (basic == null || parent == null)
            {
                return;
            }
            basic.SetParent(parent);
            basic.localPosition = position;
            basic.localRotation = Quaternion.Euler(rotation);
            basic.localScale = scale;
        }

        public static void SetParent(this IEntity entity, IEntity parent)
        {
            entity.SetParent(parent, Vector3.zero);
        }

        public static void SetParent(this IEntity entity, IEntity parent, Vector3 position)
        {
            entity.SetParent(parent, position, Vector3.zero);
        }

        public static void SetParent(this IEntity entity, IEntity parent, Vector3 position, Vector3 rotation)
        {
            entity.SetParent(parent, position, rotation, Vector3.one);
        }

        public static void SetParent(this IEntity entity, IEntity parent, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            TransformComponent basicTransformComponent = entity.GetComponent<TransformComponent>();
            if (basicTransformComponent == null)
            {
                Debug.LogError("the entity is not have transform component");
                return;
            }
            if (parent == null)
            {
                basicTransformComponent.transform.SetParent(null);
                basicTransformComponent.transform.position = position;
                basicTransformComponent.localRotation = rotation;
                basicTransformComponent.localScale = scale;
            }
            else
            {
                TransformComponent parentTransformComponent = parent.GetComponent<TransformComponent>();
                if (parentTransformComponent == null)
                {
                    Debug.LogError("the parent entity is not have transform component");
                    return;
                }
                basicTransformComponent.transform.SetParent(parentTransformComponent.transform);
                basicTransformComponent.transform.position = position;
                basicTransformComponent.localRotation = rotation;
                basicTransformComponent.localScale = scale;
            }
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