using System.IO;
using System.Collections.Concurrent;
using GameFramework.Network;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace GameFramework.Resource
{
    public enum ResourceUpdateState
    {
        Success,
        Failure,
    }

    sealed class DefaultResourceUpdateHandler : IResourceUpdateHandler
    {
        private string rootUrl;
        private ResouceModle resouceModle;
        private List<IDownloadHandle> failures;
        private List<IDownloadHandle> downloads;
        private List<IDownloadHandle> completeds;
        private IResourceStreamingHandler resourceStreamingHandler;
        private IResourceUpdateListenerHandler resourceUpdateListenerHandler;

        public void CheckoutResourceUpdate(GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted)
        {
            resourceUpdateListenerHandler = DefaultResourceUpdateListenerHandle.Generate(progresCallback, compoleted);
            DownloadRemoteFileList();
        }

        public void CheckoutResourceUpdate<TResourceUpdateListenerHandler>() where TResourceUpdateListenerHandler : IResourceUpdateListenerHandler
        {
            resourceUpdateListenerHandler = Loader.Generate<TResourceUpdateListenerHandler>();
            DownloadRemoteFileList();
        }

        private async void DownloadRemoteFileList()
        {
            BundleList remoteBundleList = null;
            BundleList localBundleList = null;
            DataStream resourceDataStream = null;
            if (resouceModle != ResouceModle.Local)
            {
                //todo 从url获取资源列表文件
                NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();
                remoteBundleList = await networkManager.RequestAsync<BundleList>(Path.Combine(rootUrl, Runtime.HOTFIX_FILE_LIST_NAME));
            }
            else
            {
                //todo 本地模式，从StreamingAssetPaths中读取资源列表
                if (!resourceStreamingHandler.Exist(Runtime.BASIC_FILE_LIST_NAME))
                {
                    resourceUpdateListenerHandler.Completed(ResourceUpdateState.Failure);
                    return;
                }
                resourceDataStream = await resourceStreamingHandler.ReadStreamingAssetDataAsync(Runtime.BASIC_FILE_LIST_NAME);
                if (resourceDataStream == null || resourceDataStream.position <= 0)
                {
                    resourceUpdateListenerHandler.Completed(ResourceUpdateState.Failure);
                    return;
                }
                remoteBundleList = CatJson.JsonParser.ParseJson<BundleList>(resourceDataStream.ToString());
            }
            resourceDataStream = await resourceStreamingHandler.ReadPersistentDataAsync(Runtime.HOTFIX_FILE_LIST_NAME);
            localBundleList = CatJson.JsonParser.ParseJson<BundleList>(resourceDataStream.ToString());

            CheckoutNeedUpdateList(remoteBundleList, localBundleList);
        }

        private void CheckoutNeedUpdateList(BundleList remoteBundleList, BundleList localBundleList)
        {
            if (remoteBundleList == null || remoteBundleList.bundles.Count <= 0)
            {
                resourceUpdateListenerHandler.Completed(ResourceUpdateState.Failure);
                return;
            }
            List<BundleData> waitingDownloads = new List<BundleData>();
            if (localBundleList == null || localBundleList.bundles.Count <= 0)
            {
                waitingDownloads.AddRange(remoteBundleList.bundles);
            }
            else
            {
                foreach (BundleData bundle in remoteBundleList.bundles)
                {
                    BundleData localBundle = localBundleList.GetBundleData(bundle.name);
                    if (localBundle == null || !bundle.Equals(localBundle))
                    {
                        waitingDownloads.Add(bundle);
                    }
                }
            }
            StartUpdateBundleList(waitingDownloads);
        }

        private void StartUpdateBundleList(List<BundleData> waitingDownloads)
        {
            if (waitingDownloads == null || waitingDownloads.Count <= 0)
            {
                resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            failures = new List<IDownloadHandle>();
            downloads = new List<IDownloadHandle>();
            NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();
            for (var i = 0; i < waitingDownloads.Count; i++)
            {
                downloads.Add(networkManager.Download(Path.Combine(rootUrl, waitingDownloads[i].name), DownloadCompleted, UpdateProgress));
            }
        }

        private void UpdateProgress(float progres)
        {
            float newProgres = downloads.Sum(x => x.progres) + completeds.Count;
            resourceUpdateListenerHandler.Progres(newProgres / (downloads.Count + completeds.Count + failures.Count));
        }

        private void DownloadCompleted(IDownloadHandle handle)
        {
            if (handle.isError)
            {
                failures.Add(handle);
            }
            else
            {
                completeds.Add(handle);
                resourceStreamingHandler.WriteSync(handle.name, handle.stream);
            }
            downloads.Remove(handle);
            if (downloads.Count >= 0)
            {
                return;
            }
            resourceUpdateListenerHandler.Completed(failures.Count > 0 ? ResourceUpdateState.Failure : ResourceUpdateState.Success);
        }



        public void Release()
        {
            Loader.Release(this.resourceStreamingHandler);
            Loader.Release(this.resourceUpdateListenerHandler);
            this.resourceStreamingHandler = null;
            this.resourceUpdateListenerHandler = null;
        }

        public void SetResourceDownloadUrl(string url)
        {
            rootUrl = url;
        }

        public void SetResourceModel(ResouceModle modle)
        {
            resouceModle = modle;
        }

        public void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler)
        {
            this.resourceStreamingHandler = resourceReaderAndWriterHandler;
        }
    }

    sealed class DefaultResourceUpdateListenerHandle : IResourceUpdateListenerHandler
    {
        private GameFrameworkAction<float> progresCallback;
        private GameFrameworkAction<ResourceUpdateState> compoleted;
        public void Completed(ResourceUpdateState state)
        {
            if (compoleted == null)
            {
                return;
            }
            compoleted(state);
        }

        public void Progres(float progres)
        {
            if (progresCallback == null)
            {
                return;
            }
            progresCallback(progres);
        }

        public void Release()
        {
            compoleted = null;
            progresCallback = null;
        }



        public static DefaultResourceUpdateListenerHandle Generate(GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted)
        {
            DefaultResourceUpdateListenerHandle defaultResourceUpdateListenerHandle = Loader.Generate<DefaultResourceUpdateListenerHandle>();
            defaultResourceUpdateListenerHandle.progresCallback = progresCallback;
            defaultResourceUpdateListenerHandle.compoleted = compoleted;
            return defaultResourceUpdateListenerHandle;
        }
    }
}
