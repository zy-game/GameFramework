using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;

namespace GameFramework.Resource
{
    sealed class BundleHandle : IRefrence
    {
        private uint refCount;
        private DateTime unloadTime;
        private AssetBundle assetBundle;
        private ResourceModle resouceModle;
        private Dictionary<string, ResHandle> resHandleCacheing;
        private IResourceStreamingHandler resourceStreamingHandler;

        public BundleHandle()
        {
            refCount = 0;
            resHandleCacheing = new Dictionary<string, ResHandle>();
        }

        private async Task<AssetBundle> LoadAssetBundleFormStreamAsync(DataStream stream)
        {
            TaskCompletionSource<AssetBundle> waiting = new TaskCompletionSource<AssetBundle>();
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(stream.bytes);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetResult(null);
                    return;
                }
                waiting.SetResult(request.assetBundle);
            };
            return await waiting.Task;
        }

        public ResHandle LoadAsset(string assetName)
        {
            if (resHandleCacheing.TryGetValue(assetName, out ResHandle resHandle))
            {
                return resHandle;
            }
            if (assetBundle == null)
            {
                throw GameFrameworkException.Generate("load asset error:" + assetName);
            }
            Object assetObject = assetBundle.LoadAsset(assetName);
            resHandle = ResHandle.GenerateHandler(this, assetObject);
            resHandleCacheing.Add(assetName, resHandle);
            return resHandle;
        }

        public async Task<ResHandle> LoadAssetAsync(string assetName)
        {
            if (resHandleCacheing.TryGetValue(assetName, out ResHandle resHandle))
            {
                return resHandle;
            }
            if (assetBundle == null)
            {
                throw GameFrameworkException.Generate("load asset error:" + assetName);
            }
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            AssetBundleRequest request = assetBundle.LoadAssetAsync(assetName);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetException(GameFrameworkException.Generate("load asset error:" + assetName));
                    return;
                }
                waiting.SetResult(ResHandle.GenerateHandler(this, request.asset));
            };
            return await waiting.Task;
        }

        public async Task<bool> LoadBundleAsync(string fileName)
        {
            unloadTime = DateTime.Now + TimeSpan.FromSeconds(60);
            DataStream stream = await ReadBundleStreamAsync(fileName);
            assetBundle = await LoadAssetBundleFormStreamAsync(stream);
            return assetBundle != null;
        }

        private async Task<DataStream> ReadBundleStreamAsync(string fileName)
        {
            DataStream stream = null;
            if (resouceModle == ResourceModle.Streaming)
            {
                stream = await this.resourceStreamingHandler.ReadStreamingAssetDataAsync(fileName);
            }
            else
            {
                stream = await this.resourceStreamingHandler.ReadPersistentDataAsync(fileName);
            }

            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate("read file error:" + fileName);
            }
            return stream;
        }

        public void Release()
        {
            refCount = 0;
            foreach (var item in resHandleCacheing.Values)
            {
                Loader.Release(item);
            }
            resHandleCacheing.Clear();
            assetBundle.Unload(true);
            assetBundle = null;
            resourceStreamingHandler = null;
        }

        public void UnloadAsset(Object assetObject)
        {
            if (!resHandleCacheing.TryGetValue(assetBundle.name, out ResHandle handle))
            {
                return;
            }
            refCount--;
            if (refCount <= 0)
            {
                unloadTime = DateTime.Now + TimeSpan.FromSeconds(60);
            }
        }

        internal void LoadBundleSync(string name)
        {
            unloadTime = DateTime.Now + TimeSpan.FromSeconds(60);
            DataStream stream = ReadBundleStreamSync(name);
            assetBundle = AssetBundle.LoadFromMemory(stream.bytes);
            if (assetBundle != null)
            {
                return;
            }
            throw GameFrameworkException.Generate("read file error:" + name);
        }

        private DataStream ReadBundleStreamSync(string name)
        {
            DataStream stream = null;
            if (resouceModle == ResourceModle.Streaming)
                stream = this.resourceStreamingHandler.ReadStreamingAssetDataSync(name);
            else
                stream = this.resourceStreamingHandler.ReadPersistentDataSync(name);
            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate("read file error:" + name);
            }
            return stream;
        }

        public bool CanUnload()
        {
            if (refCount > 0)
            {
                return false;
            }
            if (DateTime.Now > unloadTime)
            {
                return true;
            }
            return false;
        }
    }
}
