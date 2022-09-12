using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Resource
{
    /// <summary>
    /// ��Դ�����
    /// </summary>
    public sealed class BundleHandle : IRefrence
    {
        /// <summary>
        /// bundle��
        /// </summary>
        private AssetBundle bundle;
        /// <summary>
        /// ��Դ����
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
        /// ���ü���
        /// </summary>
        /// <value></value>
        public int refCount { get; private set; }

        /// <summary>
        /// ж�ؼ�ʱ
        /// </summary>
        private DateTime time;

        /// <summary>
        /// ��Դ��������캯��
        /// </summary>
        public BundleHandle()
        {
            handles = new Dictionary<string, ResHandle>();
        }

        /// <summary>
        /// ����
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
        /// ������Դ���
        /// </summary>
        /// <param name="name">��Դ��</param>
        /// <returns>��Դ���</returns>
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
        /// ������Դ���
        /// </summary>
        /// <param name="name">��Դ��</param>
        /// <returns>��Դ���</returns>
        internal ResHandle LoadHandleSync(string name)
        {
            Object asset = bundle.LoadAsset(name);
            ResHandle resHandle = ResHandle.GenerateResHandle(this, asset);
            return resHandle;
        }

        /// <summary>
        /// ������Դ��
        /// </summary>
        /// <param name="resourceManager">��Դ������</param>
        /// <param name="name">��Դ����</param>
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
        /// ������Դ��
        /// </summary>
        /// <param name="resourceManager">��Դ������</param>
        /// <param name="name">��Դ����</param>
        /// <returns>��������</returns>
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
        /// �Ƿ����ж����Դ
        /// </summary>
        /// <returns></returns>
        internal bool CanUnload()
        {
            return DateTime.Now - time >= TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// ������Դ
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
