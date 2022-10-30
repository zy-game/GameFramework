using System.Text;
using System.IO;
using GameFramework.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

namespace GameFramework.Resource
{
    public enum ResourceUpdateState
    {
        Success,
        Failure,
    }

    sealed class DefaultResourceUpdateHandler : IResourceUpdateHandler
    {
        private BundleList persistentBundleList;
        private ResourceUpdateState resourceUpdateState;
        private List<BundleData> waitingUpdateBundleList;
        private IResourceStreamingHandler resourceStreamingHandler;
        private List<ResourceDownloadHandle> resourceDownloadHandles;
        private IResourceUpdateListenerHandler resourceUpdateListenerHandler;
        private List<ResourceDownloadHandle> resourceDownloadCompletedHandles;
        public DefaultResourceUpdateHandler()
        {
            waitingUpdateBundleList = new List<BundleData>();
            resourceDownloadHandles = new List<ResourceDownloadHandle>();
            resourceDownloadCompletedHandles = new List<ResourceDownloadHandle>();
        }

        private async Task<List<BundleData>> ReadStreamingAssetsBundleListDataAsync(string moduleName)
        {
            if (!resourceStreamingHandler.ExistStreamingAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                return default;
            }
            DataStream resourceDataStreaming = await resourceStreamingHandler.ReadStreamingAssetDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
            if (resourceDataStreaming == null || resourceDataStreaming.position <= 0)
            {
                throw GameFrameworkException.GenerateFormat("not find file:{0}", Path.Combine(Application.streamingAssetsPath, AppConfig.HOTFIX_FILE_LIST_NAME));
            }
            BundleList streamingBundleList = BundleList.Generate(resourceDataStreaming.ToString());
            Loader.Release(resourceDataStreaming);
            if (streamingBundleList == null || streamingBundleList.Count <= 0)
            {
                return default;
            }
            return streamingBundleList.GetBundleDatas(moduleName);
        }

        private async Task<List<BundleData>> ReadPersistentAssetsBundleListDataAsync(string moduleName)
        {
            if (!resourceStreamingHandler.ExistPersistentAsset(AppConfig.HOTFIX_FILE_LIST_NAME))
            {
                persistentBundleList = Loader.Generate<BundleList>();
            }
            else
            {
                DataStream resourceDataStreaming = await resourceStreamingHandler.ReadPersistentDataAsync(AppConfig.HOTFIX_FILE_LIST_NAME);
                if (resourceDataStreaming == null || resourceDataStreaming.position <= 0)
                {
                    throw GameFrameworkException.GenerateFormat("not find file:{0}", Path.Combine(Application.streamingAssetsPath, AppConfig.HOTFIX_FILE_LIST_NAME));
                }
                persistentBundleList = BundleList.Generate(resourceDataStreaming.ToString());
                Loader.Release(resourceDataStreaming);
                Debug.Log("读取本地资源列表");
                if (persistentBundleList == null)
                {
                    persistentBundleList = Loader.Generate<BundleList>();
                }
            }
            return persistentBundleList.GetBundleDatas(moduleName);
        }

        private void OnUpdateCompleted()
        {
            this.resourceUpdateListenerHandler.Progres(1f);
            this.resourceUpdateListenerHandler.Completed(ResourceUpdateState.Success);
        }

        public async void CheckoutStreamingAssetListUpdate(string moduleName, IResourceUpdateListenerHandler resourceUpdateListenerHandler)
        {
            this.resourceUpdateListenerHandler = resourceUpdateListenerHandler;
            List<BundleData> bundleDataList = await ReadStreamingAssetsBundleListDataAsync(moduleName);
            if (bundleDataList == default || bundleDataList.Count <= 0)
            {
                OnUpdateCompleted();
                return;
            }
            List<BundleData> persistenBundleList = await ReadPersistentAssetsBundleListDataAsync(moduleName);

            if (persistenBundleList == null || persistenBundleList.Count <= 0)
            {
                waitingUpdateBundleList.AddRange(bundleDataList);
            }
            else
            {
                for (int i = 0; i < bundleDataList.Count; i++)
                {
                    BundleData streamingBundleData = bundleDataList[i];
                    BundleData persistentBundleData = persistenBundleList.Find(x => x.name == streamingBundleData.name);
                    if (persistentBundleData == null || persistentBundleData.EqualsVersion(streamingBundleData))
                    {
                        waitingUpdateBundleList.Add(streamingBundleData);
                    }
                }
            }

            if (waitingUpdateBundleList.Count <= 0)
            {
                OnUpdateCompleted();
                return;
            }
            Debug.Log("需要更新资源");
            resourceUpdateState = ResourceUpdateState.Success;
            foreach (BundleData bundleData in waitingUpdateBundleList)
            {
                resourceDownloadHandles.Add(OnStartDownloadBundleData(AppConfig.STREAMING_FILE_PATH, bundleData));
            }
        }

        private ResourceDownloadHandle OnStartDownloadBundleData(string remoteUrl, BundleData bundleData)
        {
            return ResourceDownloadHandle.Generate(Path.Combine(remoteUrl, bundleData.name), async handle =>
            {
                if (handle.state == ResourceUpdateState.Failure)
                {
                    resourceUpdateState = handle.state;
                    return;
                }
                await resourceStreamingHandler.WriteAsync(bundleData.name, handle.stream);
                Debug.Log("write file completed:" + bundleData.name + " length:" + handle.stream.position + "  " + handle.stream.length);
                resourceDownloadHandles.Remove(handle);
                resourceDownloadCompletedHandles.Add(handle);
                BundleData completed = waitingUpdateBundleList.Find(x => x.name == handle.name);
                if (completed != null)
                {
                    persistentBundleList.Add(completed);
                }
                DataStream resourceDataStreaming = DataStream.Generate(UTF8Encoding.UTF8.GetBytes(persistentBundleList.ToString()));
                resourceStreamingHandler.WriteSync(AppConfig.HOTFIX_FILE_LIST_NAME, resourceDataStreaming);
                if (resourceDownloadHandles.Count > 0)
                {
                    return;
                }
                resourceUpdateListenerHandler.Completed(resourceUpdateState);
            }, progres =>
            {
                float p = resourceDownloadHandles.Sum(x => x.progres) / (float)(resourceDownloadHandles.Count + resourceDownloadCompletedHandles.Count);
                resourceUpdateListenerHandler.Progres(p);
            });
        }

        public async void ChekeoutHotfixResourceListUpdate(string moduleName, IResourceUpdateListenerHandler resourceUpdateListenerHandler)
        {
            if (string.IsNullOrEmpty(AppConfig.RESOURCE_SERVER_ADDRESS))
            {
                OnUpdateCompleted();
                return;
            }
            this.resourceUpdateListenerHandler = resourceUpdateListenerHandler;
            string fileListAddress = Path.Combine(AppConfig.RESOURCE_SERVER_ADDRESS, AppConfig.HOTFIX_FILE_LIST_NAME);
            BundleList hotfixBundleList = BundleList.Generate(NetworkManager.Instance.Request(fileListAddress));
            if (hotfixBundleList == null)
            {
                Debug.LogError("remote server is not find file:" + fileListAddress);
                OnUpdateCompleted();
                return;
            }
            List<BundleData> remoteModuleBundleList = hotfixBundleList.GetBundleDatas(moduleName);
            if (remoteModuleBundleList == null || remoteModuleBundleList.Count <= 0)
            {
                OnUpdateCompleted();
                return;
            }
            List<BundleData> persistentBundleList = await ReadPersistentAssetsBundleListDataAsync(moduleName);
            if (persistentBundleList == null || persistentBundleList.Count <= 0)
            {
                waitingUpdateBundleList.AddRange(remoteModuleBundleList);
            }
            else
            {
                for (int i = 0; i < remoteModuleBundleList.Count; i++)
                {
                    BundleData hotfixBundleData = remoteModuleBundleList[i];
                    BundleData persistentBundleData = persistentBundleList.Find(x => x.name == hotfixBundleData.name);
                    if (persistentBundleData == null || persistentBundleData.EqualsVersion(hotfixBundleData))
                    {
                        waitingUpdateBundleList.Add(hotfixBundleData);
                    }
                }
            }

            if (waitingUpdateBundleList.Count <= 0)
            {
                OnUpdateCompleted();
                return;
            }
            Debug.Log("需要更新资源");
            resourceUpdateState = ResourceUpdateState.Success;
            foreach (BundleData bundleData in waitingUpdateBundleList)
            {
                resourceDownloadHandles.Add(OnStartDownloadBundleData(AppConfig.RESOURCE_SERVER_ADDRESS, bundleData));
            }
        }

        public void Release()
        {
            this.resourceStreamingHandler = null;
            this.resourceUpdateListenerHandler = null;
            resourceDownloadHandles.Clear();
            waitingUpdateBundleList.Clear();
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
            Debug.Log(name + ":" + this.received);
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
                Debug.Log("============");
            }

            stream.Write(data, 0, dataLength);
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
            ResourceManager.Instance.StartCoroutine(StartDownloadResource(url, resourceDownloadHandle));
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
