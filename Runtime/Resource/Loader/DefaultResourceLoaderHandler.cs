using System.IO;
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
        private AutoResetEvent waiter;
        private HashSet<string> loading;
        private ResourceModle resouceModle;
        private Dictionary<string, BundleHandle> bundles;
        private IResourceStreamingHandler resourceStreamingHandler;
        private Dictionary<string, ResHandle> resourceResHandlerCacheing;

        public DefaultResourceLoaderHandler()
        {
            loading = new HashSet<string>();
            waiter = new AutoResetEvent(false);
            bundles = new Dictionary<string, BundleHandle>();
            resourceResHandlerCacheing = new Dictionary<string, ResHandle>();
        }

        public ResHandle LoadAsset(string assetName)
        {
            BundleData bundleData = GetBundleData(assetName);
            if (resouceModle == ResourceModle.Resource)
            {
                if (resourceResHandlerCacheing.TryGetValue(assetName, out ResHandle resHandler))
                {
                    return resHandler;
                }
                AssetData assetData = bundleData.GetAssetData(assetName);
                if (assetData == null)
                {
                    throw GameFrameworkException.Generate("not find asset:" + assetName);
                }
                Object assetObject = LoadAseetObjectFormResourceSync(assetData.path);
                resHandler = ResHandle.GenerateHandler(this, assetObject);
                resourceResHandlerCacheing.Add(assetName, resHandler);
                return resHandler;
            }

            if (!bundles.TryGetValue(bundleData.name, out BundleHandle handler))
            {
                handler = Loader.Generate<BundleHandle>();
                handler.LoadBundleSync(bundleData.name);
                bundles.Add(bundleData.name, handler);
            }
            return handler.LoadAsset(assetName);
        }

        private Object LoadAseetObjectFormResourceSync(string assetName)
        {
            Object assetObject = Resources.Load(assetName);
            if (assetObject == null)
            {
                throw GameFrameworkException.Generate("load asset error:" + assetName);
            }
            return assetObject;
        }

        private Task<ResHandle> LoadAseetObjectFormResourceAsync(string assetName)
        {
            loading.Add(assetName);
            TaskCompletionSource<ResHandle> waiting = new TaskCompletionSource<ResHandle>();
            ResourceRequest request = Resources.LoadAsync(assetName);
            request.completed += _ =>
            {
                ResHandle resHandle = null;
                if (request.isDone)
                {
                    resHandle = ResHandle.GenerateHandler(this, request.asset);
                }

                waiting.SetResult(resHandle);
            };
            return waiting.Task;
        }

        private BundleData GetBundleData(string assetName)
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
            return bundleData;
        }
        public async Task<ResHandle> LoadAssetAsync(string assetName)
        {
            BundleData bundleData = GetBundleData(assetName);
            bool isWatingOhterThreadLoadingCompleted = false;
            if (resouceModle == ResourceModle.Resource)
            {
                if (!loading.Contains(assetName))
                {
                    isWatingOhterThreadLoadingCompleted = true;
                    waiter.WaitOne(TimeSpan.FromSeconds(10));
                }
                if (resourceResHandlerCacheing.TryGetValue(assetName, out ResHandle resHandler))
                {
                    return resHandler;
                }
                if (isWatingOhterThreadLoadingCompleted)
                {
                    return null;
                }
                AssetData assetData = bundleData.GetAssetData(assetName);
                resHandler = await LoadAseetObjectFormResourceAsync(assetData.path);
                if (resHandler != null)
                {
                    resourceResHandlerCacheing.Add(assetName, resHandler);
                }
                loading.Remove(assetName);
                waiter.Set();
                return resHandler;
            }

            if (!loading.Contains(assetName))
            {
                isWatingOhterThreadLoadingCompleted = true;
                waiter.WaitOne(TimeSpan.FromSeconds(10));
            }
            if (!bundles.TryGetValue(bundleData.name, out BundleHandle handler))
            {
                if (isWatingOhterThreadLoadingCompleted)
                {
                    return null;
                }
                loading.Add(bundleData.name);
                handler = Loader.Generate<BundleHandle>();
                bool state = await handler.LoadBundleAsync(bundleData.name);
                if (state)
                {
                    bundles.Add(bundleData.name, handler);
                }
                loading.Remove(bundleData.name);
                waiter.Set();
            }
            return await handler.LoadAssetAsync(assetName);
        }

        private async void LoadBundleList()
        {
            if (bundleList != null)
            {
                return;
            }
            string bundleListString = string.Empty;
            if (resouceModle == ResourceModle.Resource)
            {
                TextAsset textAsset = await resourceStreamingHandler.ReadResourceDataAsync<TextAsset>(Path.GetFileNameWithoutExtension(AppConfig.HOTFIX_FILE_LIST_NAME));
                bundleListString = textAsset.text;
            }
            else
            {
                DataStream stream = null;
                if (resouceModle == ResourceModle.Streaming)
                {
                    stream = await this.resourceStreamingHandler.ReadStreamingAssetDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                }
                else
                {
                    stream = await this.resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                }

                if (stream == null || stream.position <= 0)
                {
                    throw GameFrameworkException.Generate("read file error:" + AppConfig.HOTFIX_FILE_LIST_NAME);
                }
                bundleListString = stream.ToString();
            }
            bundleList = CatJson.JsonParser.ParseJson<BundleList>(bundleListString);
            if (bundleList != null)
            {
                return;
            }
            throw GameFrameworkException.Generate("load bundle list error");
        }

        public void Release()
        {
            Loader.Release(this.bundleList);
            this.resourceStreamingHandler = null;

            bundles.Clear();
        }

        public void SetResourceModel(ResourceModle modle)
        {
            resouceModle = modle;
        }

        public void SetResourceStreamingHandler(IResourceStreamingHandler resourceStreamingHandler)
        {
            this.resourceStreamingHandler = resourceStreamingHandler;
        }

        public void UnloadAsset(Object assetObject)
        {
            if (resourceResHandlerCacheing.TryGetValue(assetObject.name, out ResHandle handle))
            {
                handle.Free();
                return;
            }
            if (bundleList == null)
            {
                return;
            }
            BundleData bundleData = bundleList.GetBundleDataWithAsset(assetObject.name);
            if (bundleData == null)
            {
                return;
            }
            if (bundles.TryGetValue(bundleData.name, out BundleHandle handler))
            {
                handler.UnloadAsset(assetObject);
            }
        }
        List<string> waitingUnloadResHandle = new List<string>();
        List<string> waitingUnloadBundleHandler = new List<string>();
        public void Update()
        {
            foreach (var item in resourceResHandlerCacheing)
            {
                if (item.Value.CanUnload())
                {
                    waitingUnloadResHandle.Add(item.Key);
                }
            }

            foreach (var item in bundles)
            {
                if (item.Value.CanUnload())
                {
                    waitingUnloadBundleHandler.Add(item.Key);
                }
            }

            if (waitingUnloadResHandle.Count > 0)
            {
                for (int i = waitingUnloadResHandle.Count - 1; i >= 0; i--)
                {
                    Loader.Release(resourceResHandlerCacheing[waitingUnloadResHandle[i]]);
                    resourceResHandlerCacheing.Remove(waitingUnloadResHandle[i]);
                }
                waitingUnloadResHandle.Clear();
            }

            if (waitingUnloadBundleHandler.Count > 0)
            {
                for (int i = waitingUnloadBundleHandler.Count - 1; i >= 0; i--)
                {
                    Loader.Release(bundles[waitingUnloadBundleHandler[i]]);
                    bundles.Remove(waitingUnloadBundleHandler[i]);
                }
                waitingUnloadBundleHandler.Clear();
            }
        }
    }
}
