using System.IO;
using System.Collections.Concurrent;
using GameFramework.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

        public async void CheckoutStreamingAssetListUpdate(IResourceUpdateListenerHandler resourceUpdateListenerHandler)
        {
            this.resourceUpdateListenerHandler = resourceUpdateListenerHandler;
            BundleList streamingBundleList = null;
            BundleList persistentBundleList = null;
            DataStream resourceDataStreaming = null;
            if (resourceStreamingHandler.ExistStreamingAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                resourceDataStreaming = await resourceStreamingHandler.ReadStreamingAssetDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                if (resourceDataStreaming == null || resourceDataStreaming.position <= 0)
                {
                    throw GameFrameworkException.GenerateFormat("not find file:{0}", Path.Combine(Application.streamingAssetsPath, AppConfig.HOTFIX_FILE_LIST_NAME));
                }
                streamingBundleList = CatJson.JsonParser.ParseJson<BundleList>(resourceDataStreaming.ToString());
                Loader.Release(resourceDataStreaming);
            }
            if (resourceStreamingHandler.ExistPersistentAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                resourceDataStreaming = await resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                if (resourceDataStreaming != null || resourceDataStreaming.position > 0)
                {
                    persistentBundleList = CatJson.JsonParser.ParseJson<BundleList>(resourceDataStreaming.ToString());
                    Loader.Release(resourceDataStreaming);
                }
            }
            if (streamingBundleList == null || streamingBundleList.bundles.Count <= 0)
            {
                this.resourceUpdateListenerHandler.Progres(1f);
                this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            List<BundleData> needUpdateList = new List<BundleData>();
            if (persistentBundleList == null || persistentBundleList.bundles.Count <= 0)
            {
                needUpdateList.AddRange(streamingBundleList.bundles);
            }
            else
            {
                foreach (BundleData streamingBundleData in streamingBundleList.bundles)
                {
                    BundleData persistentBundleData = persistentBundleList.GetBundleData(streamingBundleData.name);
                    if (persistentBundleData == null || !streamingBundleData.Equals(persistentBundleData))
                    {
                        needUpdateList.Add(streamingBundleData);
                    }
                }
            }

            if (needUpdateList.Count <= 0)
            {
                this.resourceUpdateListenerHandler.Progres(1f);
                this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            //todo copy asset
        }

        public async void ChekeoutHotfixResourceListUpdate(string url, IResourceUpdateListenerHandler resourceUpdateListenerHandler)
        {
            this.resourceUpdateListenerHandler = resourceUpdateListenerHandler;
            NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();
            BundleList hotfixBundleList = networkManager.Request<BundleList>(Path.Combine(url, AppConfig.HOTFIX_FILE_LIST_NAME));
            if (hotfixBundleList == null)
            {
                throw GameFrameworkException.GenerateFormat("remote server is not find file:" + Path.Combine(url, AppConfig.HOTFIX_FILE_LIST_NAME));
            }
            DataStream resourceDataStreaming = await resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
            BundleList persistentBundleList = null;
            if (resourceDataStreaming != null && resourceDataStreaming.position > 0)
            {
                persistentBundleList = CatJson.JsonParser.ParseJson<BundleList>(resourceDataStreaming.ToString());
            }
            List<BundleData> needUpdateList = new List<BundleData>();
            if (persistentBundleList == null || persistentBundleList.bundles.Count <= 0)
            {
                needUpdateList.AddRange(hotfixBundleList.bundles);
            }
            else
            {
                foreach (BundleData streamingBundleData in hotfixBundleList.bundles)
                {
                    BundleData persistentBundleData = persistentBundleList.GetBundleData(streamingBundleData.name);
                    if (persistentBundleData == null || !streamingBundleData.Equals(persistentBundleData))
                    {
                        needUpdateList.Add(streamingBundleData);
                    }
                }
            }
            if (needUpdateList.Count <= 0)
            {
                this.resourceUpdateListenerHandler.Progres(1f);
                this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }

            //todo download asset
        }

        public void Release()
        {
            this.resourceStreamingHandler = null;
            this.resourceUpdateListenerHandler = null;
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

    sealed class ResourceDownloadHandle : DownloadHandlerScript
    {
        private string name;
        private int received;
        private int targetLength;
        private GameFrameworkAction<float> progresCallback;
        private GameFrameworkAction<ResourceUpdateState> completed;

        protected override async void CompleteContent()
        {
            if (completed == null)
            {
                return;
            }
            await File.WriteAllBytesAsync(Path.Combine(Application.persistentDataPath, name), data);
            completed(ResourceUpdateState.Success);
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            this.targetLength = (int)contentLength;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            this.received += dataLength;
            if (progresCallback != null)
            {
                progresCallback(received / (float)this.targetLength);
            }
            return base.ReceiveData(data, dataLength);
        }

        public static ResourceDownloadHandle Generate(string url, string fileName, GameFrameworkAction<ResourceUpdateState> completed, GameFrameworkAction<float> progresCallback)
        {
            UnityWebRequest request = new UnityWebRequest(url);
            ResourceDownloadHandle resourceDownloadHandle = new ResourceDownloadHandle();
            resourceDownloadHandle.name = fileName;
            resourceDownloadHandle.completed = completed;
            resourceDownloadHandle.progresCallback = progresCallback;
            request.downloadHandler = resourceDownloadHandle;
            Runtime.StartCoroutine(request.SendWebRequest());
            return resourceDownloadHandle;
        }
    }
}
