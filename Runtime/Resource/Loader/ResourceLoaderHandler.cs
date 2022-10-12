using System.IO;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Linq;
using GameFramework.Game;

namespace GameFramework.Resource
{
    public static partial class Utilty
    {
        public static bool TryGetValue(this List<IBundleHandler> list, string name, out IBundleHandler handler)
        {
            handler = list.Find(x => x.name == name);
            return handler != null;
        }

        
    }
    sealed class ResourceLoaderHandler : IResourceLoaderHandler
    {
        /// <summary>
        /// 热更新资源文件列表
        /// </summary>
        private BundleList bundleList;

        private AutoResetEvent waiter;
        private HashSet<string> loading;
        private ResourceModle resouceModle;
        private List<IBundleHandler> bundles;
        private IResourceStreamingHandler resourceStreamingHandler;
        private List<IBundleHandler> waitingUnloadBundleHandler = new List<IBundleHandler>();

        public ResourceLoaderHandler()
        {
            loading = new HashSet<string>();
            waiter = new AutoResetEvent(false);
            bundles = new List<IBundleHandler>();
        }

        public ResHandle LoadAsset(string assetName)
        {
            LoadBundleList();
            BundleData bundleData = bundleList.GetBundleDataWithAsset(assetName);
            GameFrameworkException.IsNull(bundleData);
            AssetData assetData = bundleData.GetAssetData(assetName);
            if (bundles.TryGetValue(bundleData.name, out IBundleHandler handler))
            {
                return handler.LoadAsset(assetData);
            }
            if (bundleData.IsApk)
            {
                handler = ResourceBundleHandler.Generate(bundleData);
            }
            else
            {
                handler = HotfixBundleHandler.Generate(bundleData.name, resourceStreamingHandler);
            }
            bundles.Add(handler);
            return handler.LoadAsset(assetData);
        }

        public async Task<ResHandle> LoadAssetAsync(string assetName)
        {
            LoadBundleList();
            BundleData bundleData = bundleList.GetBundleDataWithAsset(assetName);
            GameFrameworkException.IsNull(bundleData);
            AssetData assetData = bundleData.GetAssetData(assetName);
            if (bundles.TryGetValue(bundleData.name, out IBundleHandler handler))
            {
                return await handler.LoadAssetAsync(assetData);
            }
            if (!loading.Contains(assetName))
            {
                waiter.WaitOne(TimeSpan.FromSeconds(10));
                return await LoadAssetAsync(assetName);
            }
            loading.Add(bundleData.name);
            if (bundleData.IsApk)
            {
                handler = ResourceBundleHandler.Generate(bundleData);
            }
            else
            {
                handler = await HotfixBundleHandler.GenerateAsync(bundleData.name, resourceStreamingHandler);
                if (handler == null)
                {
                    GameFrameworkException.IsNull(handler, bundleData.name);
                }
            }
            bundles.Add(handler);
            loading.Remove(bundleData.name);
            waiter.Set();
            return await handler.LoadAssetAsync(assetData);
        }

        private void LoadBundleList()
        {
            if (bundleList != null)
            {
                return;
            }
            bundleList = new BundleList();
            TextAsset resourceBundleListData = resourceStreamingHandler.ReadResourceDataSync<TextAsset>(Path.GetFileNameWithoutExtension(AppConfig.HOTFIX_FILE_LIST_NAME));
            if (resourceBundleListData != null)
            {
                BundleList resourceBundleList = BundleList.Generate(resourceBundleListData.text);
                if (resourceBundleList != null && resourceBundleList.Count > 0)
                {
                    bundleList.AddRange(resourceBundleList);
                }
            }
            DataStream stream = this.resourceStreamingHandler.ReadPersistentDataSync(AppConfig.HOTFIX_FILE_LIST_NAME);
            if (stream == null || stream.position <= 0)
            {
                throw GameFrameworkException.Generate("read file error:" + AppConfig.HOTFIX_FILE_LIST_NAME);
            }
            BundleList hotfixBundleList = BundleList.Generate(stream.ToString());
            if (hotfixBundleList != null && hotfixBundleList.Count > 0)
            {
                bundleList.AddRange(hotfixBundleList);
            }
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


        private List<string> canUnloadList = new List<string>();
        public void Update()
        {
            IBundleHandler handler = null;
            for (var i = 0; i < bundles.Count; i++)
            {
                handler = bundles[i];
                if (!bundles[i].CanUnload())
                {
                    continue;
                }
                Debug.Log("waiting unload:" + handler.name);
                bundles.Remove(handler);
                waitingUnloadBundleHandler.Add(handler);
                if (waitingUnloadBundleHandler.Count > AppConfig.MaxResourceBundleCacheCount)
                {
                    IBundleHandler bundleHandler = waitingUnloadBundleHandler.First();
                    waitingUnloadBundleHandler.Remove(bundleHandler);
                    Loader.Release(bundleHandler);
                }
            }
        }
    }
}
