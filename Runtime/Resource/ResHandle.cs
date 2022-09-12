using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源句柄
    /// </summary>
    public sealed class ResHandle : IRefrence
    {
        private Object basic;
        private BundleHandle bundle;
        private Queue<Object> caches;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            while (caches.TryDequeue(out Object obj))
            {
                Object.DestroyImmediate(obj);
            }
            caches.Clear();
            basic = null;
            bundle = null;
        }

        /// <summary>
        /// 生成资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>资源对象</returns>
        public T Generate<T>() where T : Object
        {
            if (caches.Count > 0)
            {
                return (T)caches.Dequeue();
            }
            if (typeof(T) == typeof(GameObject))
            {
                return (T)GameObject.Instantiate(basic);
            }
            return (T)basic;
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        /// <param name="asset"></param>
        public void FreeObject(Object asset)
        {
            bundle.ReleaseObject();
            if (asset.GetType() != typeof(GameObject))
            {
                return;
            }
            GameObject obj = (GameObject)asset;
            caches.Enqueue(obj);
            obj.SetActive(false);
            GameObject pool = GameObject.Find("ObjectPool");
            if (pool == null)
            {
                pool = new GameObject("ObjectPool");
                GameObject.DontDestroyOnLoad(pool);
            }
            obj.transform.SetParent(pool.transform);
        }

        /// <summary>
        /// 创建资源句柄
        /// </summary>
        /// <param name="data">所属资源包</param>
        /// <param name="asset">资源对象</param>
        /// <returns>资源句柄</returns>
        internal static ResHandle GenerateResHandle(BundleHandle data, Object asset)
        {
            ResHandle handle = Creater.Generate<ResHandle>();
            handle.basic = asset;
            handle.bundle = data;
            handle.caches = new Queue<Object>();
            return handle;
        }
    }
}
