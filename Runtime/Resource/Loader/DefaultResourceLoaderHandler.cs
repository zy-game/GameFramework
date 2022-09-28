using System.Threading;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Concurrent;

namespace GameFramework.Resource
{
    sealed class DefaultResourceLoaderHandler : IResourceLoaderHandler
    {
        private BundleList bundleList;
        private ResouceModle resouceModle;

        private Dictionary<string, AssetBundle> bundles;
        private IResourceStreamingHandler resourceStreamingHandler;
        private HashSet<string> loading;
        private AutoResetEvent locker;

        public DefaultResourceLoaderHandler()
        {
            locker = new AutoResetEvent(false);
            loading = new HashSet<string>();
            bundles = new Dictionary<string, AssetBundle>();
        }

        public ResHandle LoadAsset(string assetName)
        {
            LoadBundleList();
            if (bundleList == null)
            {
                throw GameFrameworkException.Generate("loading bundlelist error");
            }
            BundleData bundleData = bundleList.GetBundleDataWithAsset(assetName);
            if (bundleData == null)
            {
                throw GameFrameworkException.Generate("The resource does not exist in the resource list. Please check whether the resource has been added to the resource list and packaged");
            }
            ResHandle resHandle = null;
            if (resouceModle == ResouceModle.Resource)
            {
                AssetData assetData = bundleData.GetAssetData(assetName);
                resHandle = ResHandle.GenerateHandler(this, Resources.Load(assetData.path));
                return resHandle;
            }

            if (!bundles.TryGetValue(bundleData.name, out AssetBundle bundle))
            {
                DataStream stream = null;
                if (resouceModle == ResouceModle.Streaming)
                    stream = this.resourceStreamingHandler.ReadStreamingAssetDataSync(bundleData.name);
                else
                    stream = this.resourceStreamingHandler.ReadPersistentDataSync(bundleData.name);

                bundle = AssetBundle.LoadFromMemory(stream.bytes);
                if (bundle == null)
                {
                    return default;
                }
                bundles.Add(bundleData.name, bundle);
            }
            resHandle = ResHandle.GenerateHandler(this, bundle.LoadAsset(assetName));
            return resHandle;
        }

        public async Task<ResHandle> LoadAssetAsync(string assetName)
        {
            LoadBundleList();
            if (bundleList == null)
            {
                throw GameFrameworkException.Generate("loading bundlelist error");
            }
            BundleData bundleData = bundleList.GetBundleDataWithAsset(assetName);
            if (bundleData == null)
            {
                throw GameFrameworkException.Generate("The resource does not exist in the resource list. Please check whether the resource has been added to the resource list and packaged");
            }
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            if (resouceModle == ResouceModle.Resource)
            {
                AssetData assetData = bundleData.GetAssetData(assetName);
                ResourceRequest request = Resources.LoadAsync(assetData.path);
                request.completed += _ =>
                {
                    if (!request.isDone)
                    {
                        waiting.SetException(GameFrameworkException.Generate("load asset error"));
                        return;
                    }
                    waiting.SetResult(ResHandle.GenerateHandler(this, request.asset));
                };
                return await waiting.Task;
            }
            if (loading.Contains(bundleData.name))
            {
                locker.WaitOne();
            }
            TaskCompletionSource waitingAssetBundle = null;
            if (!bundles.TryGetValue(bundleData.name, out AssetBundle bundle))
            {
                waitingAssetBundle = new TaskCompletionSource();
                loading.Add(bundleData.name);
                DataStream stream = null;
                
                if (resouceModle == ResouceModle.Streaming)
                    stream = await this.resourceStreamingHandler.ReadStreamingAssetDataAsync(bundleData.name);
                else
                    stream = await this.resourceStreamingHandler.ReadPersistentDataAsync(bundleData.name);

                AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(stream.bytes);
                request.completed += _ =>
                {
                    if (!request.isDone)
                    {
                        return;
                    }
                    bundles.Add(bundleData.name, bundle);
                    loading.Remove(bundleData.name);
                    locker.Set();
                    waitingAssetBundle.Complete();
                };
            }
            if (waitingAssetBundle != null)
            {
                await waitingAssetBundle.Task;
            }
            AssetBundleRequest request1 = bundle.LoadAssetAsync(assetName);
            request1.completed += _ =>
            {
                if (!request1.isDone)
                {
                    waiting.SetException(GameFrameworkException.Generate("load asset error:" + assetName));
                    return;
                }
                waiting.SetResult(ResHandle.GenerateHandler(this, request1.asset));
            };
            return await waiting.Task;
        }

        private void LoadBundleList()
        {
            if (bundleList == null)
            {

            }
        }

        public void Release()
        {
        }

        public void SetResourceModel(ResouceModle modle)
        {
            resouceModle = modle;
        }

        public void SetResourceStreamingHandler(IResourceStreamingHandler resourceStreamingHandler)
        {
            this.resourceStreamingHandler = resourceStreamingHandler;
        }

        public void UnloadAsset(Object assetObject)
        {
        }

        public void Update()
        {
        }
    }
}
