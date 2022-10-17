using System.Text;
using System.IO;
using GameFramework.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;

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
            if (!resourceStreamingHandler.ExistStreamingAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                this.resourceUpdateListenerHandler.Progres(1f);
                this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            resourceDataStreaming = await resourceStreamingHandler.ReadStreamingAssetDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
            if (resourceDataStreaming == null || resourceDataStreaming.position <= 0)
            {
                throw GameFrameworkException.GenerateFormat("not find file:{0}", Path.Combine(Application.streamingAssetsPath, AppConfig.HOTFIX_FILE_LIST_NAME));
            }
            streamingBundleList = BundleList.Generate(resourceDataStreaming.ToString());
            Loader.Release(resourceDataStreaming);
            if (streamingBundleList == null || streamingBundleList.Count <= 0)
            {
                this.resourceUpdateListenerHandler.Progres(1f);
                this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            if (resourceStreamingHandler.ExistPersistentAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                resourceDataStreaming = await resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                if (resourceDataStreaming != null || resourceDataStreaming.position > 0)
                {
                    persistentBundleList = BundleList.Generate(resourceDataStreaming.ToString());
                    Loader.Release(resourceDataStreaming);
                }
            }
            List<BundleData> needUpdateList = new List<BundleData>();
            if (persistentBundleList == null || persistentBundleList.Count <= 0)
            {
                persistentBundleList = Loader.Generate<BundleList>();
                needUpdateList.AddRange(streamingBundleList.GetBundles());
            }
            else
            {
                for (int i = 0; i < streamingBundleList.Count; i++)
                {
                    BundleData streamingBundleData = streamingBundleList[i];
                    BundleData persistentBundleData = persistentBundleList.GetBundleData(streamingBundleData.name);
                    if (persistentBundleData == null || streamingBundleData.EqualsVersion(persistentBundleData))
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
            List<ResourceDownloadHandle> resourceDownloadHandles = new List<ResourceDownloadHandle>();
            List<ResourceDownloadHandle> completedResourceDoanloadHandles = new List<ResourceDownloadHandle>();
            ResourceUpdateState resourceUpdateState = ResourceUpdateState.Success;
            //todo copy asset
            foreach (BundleData bundleData in needUpdateList)
            {
                ResourceDownloadHandle downloadHandle = ResourceDownloadHandle.Generate(Path.Combine(Application.streamingAssetsPath, bundleData.name), async handle =>
                {
                    if (handle.state == ResourceUpdateState.Failure)
                    {
                        resourceUpdateState = handle.state;
                        return;
                    }
                    await resourceStreamingHandler.WriteAsync(bundleData.name, handle.stream);
                    Debug.Log("write file completed:" + bundleData.name);
                    resourceDownloadHandles.Remove(handle);
                    completedResourceDoanloadHandles.Add(handle);

                    BundleData completed = needUpdateList.Find(x => x.name == handle.name);
                    if (completed != null)
                    {
                        persistentBundleList.Remove(completed.name);
                        persistentBundleList.Add(completed);
                    }
                    resourceDataStreaming = DataStream.Generate(UTF8Encoding.UTF8.GetBytes(persistentBundleList.ToString()));
                    await resourceStreamingHandler.WriteAsync(AppConfig.HOTFIX_FILE_LIST_NAME, resourceDataStreaming);
                    if (resourceDownloadHandles.Count > 0)
                    {
                        return;
                    }
                    resourceUpdateListenerHandler.Completed(resourceUpdateState);
                }, progres =>
                {
                    float p = resourceDownloadHandles.Sum(x => x.progres) + completedResourceDoanloadHandles.Count;
                    p /= resourceDownloadHandles.Count + completedResourceDoanloadHandles.Count;
                    resourceUpdateListenerHandler.Progres(p);
                });
                resourceDownloadHandles.Add(downloadHandle);
            }
        }

        public async void ChekeoutHotfixResourceListUpdate(string url, IResourceUpdateListenerHandler resourceUpdateListenerHandler)
        {
            this.resourceUpdateListenerHandler = resourceUpdateListenerHandler;
            NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();
            BundleList hotfixBundleList = BundleList.Generate(networkManager.Request(Path.Combine(url, AppConfig.HOTFIX_FILE_LIST_NAME)));
            BundleList persistentBundleList = null;
            if (hotfixBundleList == null)
            {
                Debug.LogError("remote server is not find file:" + Path.Combine(url, AppConfig.HOTFIX_FILE_LIST_NAME));
                resourceUpdateListenerHandler.Progres(1);
                resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
                return;
            }
            DataStream resourceDataStreaming = await resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);

            if (resourceDataStreaming != null && resourceDataStreaming.position > 0)
            {
                persistentBundleList = BundleList.Generate(resourceDataStreaming.ToString());
            }
            List<BundleData> needUpdateList = new List<BundleData>();
            if (persistentBundleList == null || persistentBundleList.Count <= 0)
            {
                persistentBundleList = Loader.Generate<BundleList>();
                needUpdateList.AddRange(hotfixBundleList.GetBundles());
            }
            else
            {
                for (int i = 0; i < hotfixBundleList.Count; i++)
                {
                    BundleData streamingBundleData = hotfixBundleList[i];
                    BundleData persistentBundleData = persistentBundleList.GetBundleData(streamingBundleData.name);
                    if (persistentBundleData == null || !streamingBundleData.EqualsVersion(persistentBundleData))
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
            Debug.Log("need update list:" + CatJson.JsonParser.ToJson(needUpdateList));
            //todo download asset
            List<ResourceDownloadHandle> resourceDownloadHandles = new List<ResourceDownloadHandle>();
            List<ResourceDownloadHandle> completedResourceDoanloadHandles = new List<ResourceDownloadHandle>();
            ResourceUpdateState resourceUpdateState = ResourceUpdateState.Success;
            foreach (BundleData bundleData in needUpdateList)
            {
                ResourceDownloadHandle downloadHandle = ResourceDownloadHandle.Generate(Path.Combine(url, bundleData.name), async handle =>
                {
                    if (handle.state == ResourceUpdateState.Failure)
                    {
                        resourceUpdateState = handle.state;
                        return;
                    }
                    await resourceStreamingHandler.WriteAsync(bundleData.name, handle.stream);
                    Debug.Log("write file completed:" + bundleData.name);
                    resourceDownloadHandles.Remove(handle);
                    completedResourceDoanloadHandles.Add(handle);

                    BundleData completed = needUpdateList.Find(x => x.name == handle.name);
                    if (completed != null)
                    {
                        persistentBundleList.Add(completed);
                    }
                    resourceDataStreaming = DataStream.Generate(UTF8Encoding.UTF8.GetBytes(persistentBundleList.ToString()));
                    await resourceStreamingHandler.WriteAsync(AppConfig.HOTFIX_FILE_LIST_NAME, resourceDataStreaming);
                    if (resourceDownloadHandles.Count > 0)
                    {
                        return;
                    }
                    resourceUpdateListenerHandler.Completed(resourceUpdateState);
                }, progres =>
                {
                    float p = resourceDownloadHandles.Sum(x => x.progres) + completedResourceDoanloadHandles.Count;
                    p /= resourceDownloadHandles.Count + completedResourceDoanloadHandles.Count;
                    resourceUpdateListenerHandler.Progres(p);
                });
                resourceDownloadHandles.Add(downloadHandle);
            }
        }

        public void Release()
        {
            this.resourceStreamingHandler = null;
            this.resourceUpdateListenerHandler = null;
        }

        public void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler)
        {
            this.resourceStreamingHandler = resourceReaderAndWriterHandler;
        }
    }

    sealed class ResourceDownloadHandle : DownloadHandlerScript
    {
        public string name { get; private set; }
        private int received;
        private int targetLength;
        private GameFrameworkAction<float> progresCallback;
        private GameFrameworkAction<ResourceDownloadHandle> completed;

        public ResourceUpdateState state { get; private set; }
        public DataStream stream;
        public float progres { get; private set; }

        protected override void CompleteContent()
        {
            if (completed == null)
            {
                return;
            }
            state = ResourceUpdateState.Success;
            completed(this);
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            this.targetLength = (int)contentLength;
            if (stream == null)
            {
                stream = DataStream.Generate(this.targetLength);
            }
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            this.received += dataLength;
            this.progres = (float)this.received / this.targetLength;
            if (stream == null)
            {
                stream = DataStream.Generate(dataLength);
            }
            stream.Write(data);
            if (progresCallback != null)
            {
                progresCallback(received / (float)this.targetLength);
            }
            return true;
        }

        public static ResourceDownloadHandle Generate(string url, GameFrameworkAction<ResourceDownloadHandle> completed, GameFrameworkAction<float> progresCallback)
        {
            ResourceDownloadHandle resourceDownloadHandle = new ResourceDownloadHandle();
            resourceDownloadHandle.name = Path.GetFileName(url);
            resourceDownloadHandle.completed = completed;
            resourceDownloadHandle.progresCallback = progresCallback;
            Runtime.StartCoroutine(StartDownloadResource(url, resourceDownloadHandle));
            return resourceDownloadHandle;
        }
        private static IEnumerator StartDownloadResource(string url, ResourceDownloadHandle resourceDownloadHandle)
        {
            UnityWebRequest request = new UnityWebRequest(url.Replace("\\", "/"));
            request.downloadHandler = resourceDownloadHandle;
            request.timeout = 5;
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                resourceDownloadHandle.state = ResourceUpdateState.Failure;
            }
        }
    }
}
