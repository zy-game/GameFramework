using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using GameFramework.Network;
using System.Threading.Tasks;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源更新列表
    /// </summary>
    public sealed class UpdateAssetList : IRefrence
    {
        private string resourceDownloadUrl;
        private List<BundleData> needUpdateList;
        private List<IDownloadHandle> downloads;
        private List<IDownloadHandle> completeds;
        private IResourceManager resourceManager;
        private TaskCompletionSource taskCompletionSource;
        private GameFrameworkAction<float> progres;



        /// <summary>
        /// 需要更新的文件数量
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return needUpdateList.Count;
            }
        }

        /// <summary>
        /// 是否存在下载失败的资源
        /// </summary>
        /// <value></value>
        public bool isHaveFailur
        {
            get;
            private set;
        }

        /// <summary>
        /// 更新列表构造函数
        /// </summary>
        public UpdateAssetList()
        {
            needUpdateList = new List<BundleData>();
            downloads = new List<IDownloadHandle>();
            completeds = new List<IDownloadHandle>();
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Release()
        {
            downloads.ForEach(Loader.Release);
            completeds.ForEach(Loader.Release);
            progres = null;
            taskCompletionSource = null;
            needUpdateList.Clear();
        }

        public async Task<BundleList> CheckNeedUpdateBundle()
        {
            BundleList remoteBundleList = Runtime.GetGameModule<NetworkManager>().Request<BundleList>(resourceDownloadUrl + Runtime.HOTFIX_FILE_LIST_NAME);
            BundleList localBundleList = resourceManager.ReadFileSync<BundleList>(Runtime.HOTFIX_FILE_LIST_NAME);
            if (remoteBundleList == null)
            {
                throw GameFrameworkException.Generate("not find hotfix file,please check file is exsit form:" + resourceDownloadUrl);
            }
            needUpdateList = CheckUpdateList(remoteBundleList, localBundleList);
            if (needUpdateList.Count <= 0)
            {
                return remoteBundleList;
            }
            taskCompletionSource = new TaskCompletionSource();
            NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();
            for (int i = needUpdateList.Count - 1; i >= 0; i--)
            {
                string downloadUrl = Path.Combine(resourceDownloadUrl, needUpdateList[i].name).Replace("\\", "/");
                IDownloadHandle downloadHandle = networkManager.Download(downloadUrl, OnDownloadCompleted, OnUpdateDownloadProgress);
                downloads.Add(downloadHandle);
            }
            await taskCompletionSource.Task;
            foreach (IDownloadHandle download in completeds)
            {
                if (download.isError)
                {
                    remoteBundleList.Remove(download.name);
                }
            }

            byte[] bytes = UTF8Encoding.UTF8.GetBytes(remoteBundleList.ToString());
            DataStream stream = DataStream.Generate(bytes);
            await resourceManager.WriteFileAsync(Runtime.HOTFIX_FILE_LIST_NAME, stream);
            return remoteBundleList;
        }

        public static List<BundleData> CheckUpdateList(BundleList remoteBundleList, BundleList localBundleList)
        {
            List<BundleData> needUpdateList = new List<BundleData>();
            if (localBundleList == null)
            {
                needUpdateList.AddRange(remoteBundleList.bundles);
            }
            else
            {
                foreach (BundleData bundle in remoteBundleList.bundles)
                {
                    BundleData localBundleData = localBundleList.GetBundleData(bundle.name);
                    if (localBundleData == null || !localBundleData.Equals(bundle))
                    {
                        needUpdateList.Add(bundle);
                    }
                }
            }
            return needUpdateList;
        }

        /// <summary>
        /// 资源下载完成回调
        /// </summary>
        /// <param name="downloadHandle"></param>
        private async void OnDownloadCompleted(IDownloadHandle downloadHandle)
        {
            if (downloadHandle.isError)
            {
                isHaveFailur = true;
            }
            else
            {
                await resourceManager.WriteFileAsync(downloadHandle.name, downloadHandle.stream);
            }
            completeds.Add(downloadHandle);
            downloads.Remove(downloadHandle);
            if (downloads.Count > 0)
            {
                return;
            }
            if (taskCompletionSource != null)
            {
                taskCompletionSource.Complete();
            }
        }

        /// <summary>
        /// 更新下载进度回调
        /// </summary>
        /// <param name="progres"></param>
        private void OnUpdateDownloadProgress(float progres)
        {
            float p = (float)completeds.Count;
            downloads.ForEach(x => p += x.progres);
            p /= downloads.Count + completeds.Count;
            if (this.progres != null)
            {
                this.progres(p);
            }
        }

        internal static UpdateAssetList Generate(string url, IResourceManager resourceManager, GameFrameworkAction<float> progres)
        {
            UpdateAssetList updateAssetList = Loader.Generate<UpdateAssetList>();
            updateAssetList.resourceDownloadUrl = url;
            updateAssetList.progres = progres;
            return updateAssetList;
        }
    }
}
