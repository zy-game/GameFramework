using System.Collections.Generic;
using System;
using GameFramework.Network;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源更新列表
    /// </summary>
    public sealed class UpdateAssetList : IRefrence
    {
        private List<string> urls;
        private bool isStart;
        private List<IDownloadHandle> downloads;
        private List<IDownloadHandle> completeds;
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
                return urls.Count;
            }
        }

        /// <summary>
        /// 更新列表构造函数
        /// </summary>
        public UpdateAssetList()
        {
            urls = new List<string>();
            downloads = new List<IDownloadHandle>();
            completeds = new List<IDownloadHandle>();
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Release()
        {
            downloads.ForEach(Creater.Release);
            completeds.ForEach(Creater.Release);
            progres = null;
            taskCompletionSource = null;
            urls.Clear();
            isStart = false;
        }

        /// <summary>
        /// 添加下载资源
        /// </summary>
        /// <param name="url"></param>
        public void Add(string url)
        {
            urls.Add(url);
        }

        /// <summary>
        /// 资源更新是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted()
        {
            return downloads.Count <= 0 && isStart;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="progres"></param>
        /// <param name="completed"></param>
        public void Start()
        {
            NetworkManager networkManager = Runtime.GetGameModule<NetworkManager>();

            for (int i = urls.Count - 1; i >= 0; i--)
            {
                downloads.Add(networkManager.Download(urls[i], OnDownloadCompleted, OnUpdateDownloadProgress));
            }
            isStart = true;
        }

        public List<BundleData> CheckNeedUpdateBundle(BundleList bundleList, BundleList remoteResourceDetailedData)
        {
            List<BundleData> bundles = new List<BundleData>();
            foreach (var bundle in remoteResourceDetailedData.bundles)
            {
                if (bundleList == null)
                {
                    urls.Add(bundle.url);
                    bundles.Add(bundle);
                    continue;
                }
                BundleData temp = bundleList.GetBundleData(bundle.name);
                if (temp != null && temp.crc32 == bundle.crc32)
                {
                    continue;
                }
                urls.Add(bundle.name);
                bundles.Add(bundle);
            }
            return bundles;
        }

        /// <summary>
        /// 资源下载完成回调
        /// </summary>
        /// <param name="downloadHandle"></param>
        private void OnDownloadCompleted(IDownloadHandle downloadHandle)
        {
            completeds.Add(downloadHandle);
            downloads.Remove(downloadHandle);
            if (!IsCompleted())
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
            float p = 0f;
            downloads.ForEach(x => p += x.progres);
            p /= downloads.Count + completeds.Count;
            if (this.progres != null)
            {
                this.progres(p);
            }
        }

        internal static UpdateAssetList Generate(TaskCompletionSource taskCompletionSource, GameFrameworkAction<float> progres)
        {
            UpdateAssetList updateAssetList = Creater.Generate<UpdateAssetList>();
            updateAssetList.taskCompletionSource = taskCompletionSource;
            updateAssetList.progres = progres;
            return updateAssetList;
        }
    }
}
