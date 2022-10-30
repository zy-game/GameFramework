using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;

namespace GameFramework.Resource
{
    sealed class HotfixBundleHandler : IBundleHandler
    {
        private int refCount;
        internal AssetBundle assetBundle;
        private bool isCanUnload = false;
        private Dictionary<string, ResHandle> resHandleCacheing;

        public string name { get; private set; }

        public HotfixBundleHandler()
        {
            refCount = 0;
            resHandleCacheing = new Dictionary<string, ResHandle>();
        }

        public ResHandle LoadAsset<T>(AssetData assetData) where T : UnityEngine.Object
        {
            GameFrameworkException.IsNull(assetData);
            if (resHandleCacheing.TryGetValue(assetData.name, out ResHandle resHandle))
            {
                return resHandle;
            }
            GameFrameworkException.IsNull(assetBundle);
            Object assetObject = assetBundle.LoadAsset<T>(assetData.name);//todo 在这里会有个问题，如果需要加载的是个sprite，但是因为没有指定类型，加载出来的将会是个texture2d
            resHandle = ResHandle.GenerateHandler(this, assetData.name, assetObject);
            resHandleCacheing.Add(assetData.name, resHandle);
            return resHandle;
        }

        public async Task<ResHandle> LoadAssetAsync<T>(AssetData assetData) where T : UnityEngine.Object
        {
            GameFrameworkException.IsNull(assetData);
            if (resHandleCacheing.TryGetValue(assetData.name, out ResHandle resHandle))
            {
                return resHandle;
            }
            GameFrameworkException.IsNull(assetBundle);
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetData.name);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetException(GameFrameworkException.Generate("load asset error:" + assetData.name));
                    return;
                }
                ResHandle handle = ResHandle.GenerateHandler(this, assetData.name, request.asset);
                resHandleCacheing.Add(assetData.name, handle);
                waiting.SetResult(handle);
            };
            return await waiting.Task;
        }

        public void Release()
        {
            refCount = 0;
            foreach (var item in resHandleCacheing.Values)
            {
                Loader.Release(item);
            }
            resHandleCacheing.Clear();
            Debug.Log("unload assetbundle:" + assetBundle.name);
            assetBundle.Unload(true);
            assetBundle = null;
            Resources.UnloadUnusedAssets();
        }

        public bool CanUnload()
        {
            return isCanUnload;
        }

        public static async Task<IBundleHandler> GenerateAsync(string fileName, IResourceStreamingHandler resourceStreamingHandler)
        {
            TaskCompletionSource<IBundleHandler> waiting = new TaskCompletionSource<IBundleHandler>();
            HotfixBundleHandler hotfixBundleHandler = Loader.Generate<HotfixBundleHandler>();
            DataStream stream = await resourceStreamingHandler.ReadPersistentDataAsync(fileName);
            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate("read file error:" + fileName);
            }
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(stream.bytes);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetResult(null);
                    return;
                }
                hotfixBundleHandler.name = fileName;
                hotfixBundleHandler.isCanUnload = false;
                hotfixBundleHandler.assetBundle = request.assetBundle;
                waiting.SetResult(hotfixBundleHandler);
            };
            return await waiting.Task;
        }

        public static IBundleHandler Generate(string fileName, IResourceStreamingHandler resourceStreamingHandler)
        {
            HotfixBundleHandler hotfixBundleHandler = Loader.Generate<HotfixBundleHandler>();
            DataStream stream = resourceStreamingHandler.ReadPersistentDataSync(fileName);
            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate("read file error:" + fileName);
            }
            hotfixBundleHandler.name = fileName;
            hotfixBundleHandler.isCanUnload = false;
            hotfixBundleHandler.assetBundle = AssetBundle.LoadFromMemory(stream.bytes);
            Loader.Release(stream);
            if (hotfixBundleHandler.assetBundle == null)
            {
                throw GameFrameworkException.Generate("read file error:" + fileName);
            }
            return hotfixBundleHandler;
        }

        public void SubRefrence()
        {
            refCount--;
            if (refCount <= 0)
            {
                isCanUnload = true;
            }
        }

        public void AddRefrence()
        {
            refCount++;
            isCanUnload = false;
        }
    }
}
