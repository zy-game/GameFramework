using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源包句柄
    /// </summary>
    public sealed class BundleHandle : IRefrence
    {
        /// <summary>
        /// bundle包
        /// </summary>
        private AssetBundle bundle;
        /// <summary>
        /// 资源缓存
        /// </summary>
        private Dictionary<string, ResHandle> handles;
        public string name
        {
            get
            {
                if (null == bundle)
                {
                    return string.Empty;
                }
                return bundle.name;
            }
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        /// <value></value>
        public int refCount { get; private set; }

        /// <summary>
        /// 卸载计时
        /// </summary>
        private DateTime time;

        /// <summary>
        /// 资源包句柄构造函数
        /// </summary>
        public BundleHandle()
        {
            handles = new Dictionary<string, ResHandle>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            foreach (var item in handles.Values)
            {
                Creater.Release(item);
            }
            handles.Clear();
            bundle.Unload(true);
        }

        /// <summary>
        /// 加载资源句柄
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        internal Task<ResHandle> LoadHandleAsync(string name)
        {
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            AssetBundleRequest request = bundle.LoadAssetAsync(name);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetException(new KeyNotFoundException());
                }
                ResHandle resHandle = ResHandle.GenerateResHandle(this, request.asset);
                waiting.TrySetResult(resHandle);
            };
            return waiting.Task;
        }

        /// <summary>
        /// 加载资源句柄
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        internal ResHandle LoadHandleSync(string name)
        {
            Object asset = bundle.LoadAsset(name);
            ResHandle resHandle = ResHandle.GenerateResHandle(this, asset);
            return resHandle;
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        /// <param name="name">资源包名</param>
        internal void LoadBundleSync(ResourceManager resourceManager, string name)
        {
            DataStream stream = resourceManager.ReadFileSync(name);
            if (stream == null || stream.position <= 0)
            {
                return;
            }
            bundle = AssetBundle.LoadFromMemory(stream.bytes);
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        /// <param name="name">资源包名</param>
        /// <returns>加载任务</returns>
        internal async Task LoadBundleAsync(ResourceManager resourceManager, string name)
        {
            DataStream stream = await resourceManager.ReadFileAsync(name);
            if (stream == null || stream.position <= 0)
            {
                return;
            }
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(stream.bytes);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    taskCompletionSource.SetException(GameFrameworkException.Generate("load bundle failur"));
                    return;
                }
                bundle = request.assetBundle;
                taskCompletionSource.Complete();
            };
            await taskCompletionSource.Task;
        }

        /// <summary>
        /// 是否可以卸载资源
        /// </summary>
        /// <returns></returns>
        internal bool CanUnload()
        {
            return DateTime.Now - time >= TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        internal void ReleaseObject()
        {
            refCount--;
            if (refCount <= 0)
            {
                time = DateTime.Now;
            }
        }
    }
}
