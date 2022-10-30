using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;

namespace GameFramework.Resource
{
    sealed class ResourceBundleHandler : IBundleHandler
    {
        private uint refCount;
        private Dictionary<string, ResHandle> resHandleCacheing;
        public string name { get; internal set; }
        public ResourceBundleHandler()
        {
            resHandleCacheing = new Dictionary<string, ResHandle>();
        }

        public bool CanUnload()
        {
            return refCount <= 0;
        }

        public ResHandle LoadAsset<T>(AssetData assetData) where T : UnityEngine.Object
        {
            GameFrameworkException.IsNull(assetData);
            if (resHandleCacheing.TryGetValue(assetData.name, out ResHandle handle))
            {
                return handle;
            }
            Object assetObject = Resources.Load<T>("files/" + assetData.name);
            GameFrameworkException.IsNull(assetObject);
            handle = ResHandle.GenerateHandler(this, assetData.name, assetObject);
            resHandleCacheing.Add(assetData.name, handle);
            return handle;
        }

        public async Task<ResHandle> LoadAssetAsync<T>(AssetData assetData) where T : UnityEngine.Object
        {
            GameFrameworkException.IsNull(assetData);
            if (resHandleCacheing.TryGetValue(assetData.name, out ResHandle handle))
            {
                return handle;
            }
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            ResourceRequest request = Resources.LoadAsync<T>("files/" + assetData.name);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetResult(null);
                    return;
                }
                handle = ResHandle.GenerateHandler(this, assetData.name, request.asset);
                resHandleCacheing.Add(assetData.name, handle);
                waiting.SetResult(handle);
            };
            return await waiting.Task;
        }

        public void Release()
        {
            foreach (ResHandle handle in resHandleCacheing.Values)
            {
                Loader.Release(handle);
            }
            resHandleCacheing.Clear();
            Resources.UnloadUnusedAssets();
        }

        public void SubRefrence()
        {
            refCount--;
        }

        public void AddRefrence()
        {
            refCount++;
        }

        internal static IBundleHandler Generate(BundleData bundleData)
        {
            ResourceBundleHandler resourceBundleHandler = Loader.Generate<ResourceBundleHandler>();
            resourceBundleHandler.name = bundleData.name;
            return resourceBundleHandler;
        }
    }
}
