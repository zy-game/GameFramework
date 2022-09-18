using System.Net.Mime;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public sealed class ResourceManager : IResourceManager
    {
        /// <summary>
        /// 本地资源列表
        /// </summary>
        private BundleList bundleList;

        /// <summary>
        /// 资源缓存列表
        /// </summary>
        private List<BundleHandle> bundleCacheList;

        /// <summary>
        /// 资源加载列表
        /// </summary>
        private List<BundleHandle> bundleHandlers;

        /// <summary>
        /// 资源管理器构造函数
        /// </summary>
        public ResourceManager()
        {
            bundleCacheList = new List<BundleHandle>();
            bundleHandlers = new List<BundleHandle>();
        }

        /// <summary>
        /// 下载需要更新的资源文件
        /// </summary>
        /// <param name="resourceUpdateDataed">更新列表</param>
        /// <param name="progres">更新进度回调</param>
        /// <param name="completed">更新完成回调</param>
        public async Task DownloadResourceUpdate(string url, GameFrameworkAction<float> progres)
        {
            string remoteBundleList = await Runtime.GetGameModule<Network.NetworkManager>().RequestAsync(url);
            if (string.IsNullOrEmpty(remoteBundleList))
            {
                throw GameFrameworkException.Generate("download remote bundle list failur");
            }
            BundleList remoteResourceDetailedData = BundleList.Generate(remoteBundleList);
            DataStream stream = await ReadFileAsync("BundleList.ini");
            if (stream != null)
            {
                bundleList = BundleList.Generate(stream.ToString());
            }

            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
            UpdateAssetList updateAssetList = UpdateAssetList.Generate(taskCompletionSource, progres);
            List<BundleData> needUpdateList = updateAssetList.CheckNeedUpdateBundle(bundleList, remoteResourceDetailedData);
            if (updateAssetList.Count <= 0)
            {
                return;
            }
            updateAssetList.Start();
            await taskCompletionSource.Task;
            if (bundleList == null)
            {
                bundleList = remoteResourceDetailedData;
            }
            else
            {
                needUpdateList.ForEach(x =>
                {
                    bundleList.Remove(x.name);
                    bundleList.Add(x);
                });
            }
            stream = DataStream.Generate();
            stream.Write(UTF8Encoding.UTF8.GetBytes(bundleList.ToString()));
            await WriteFileAsync("BundleList.ini", stream);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        public ResHandle LoadObject(string name)
        {
            BundleData bundleData = bundleList.GetBundleDataWithAsset(name);
            if (bundleData == null)
            {
                return null;
            }
            BundleHandle handle = bundleHandlers.Find(x => x.name == bundleData.name);
            if (handle != null)
            {
                return handle.LoadHandleSync(name);
            }
            handle = Loader.Generate<BundleHandle>();
            handle.LoadBundleSync(this, bundleData.name);
            bundleHandlers.Add(handle);
            return handle.LoadHandleSync(name);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        public async Task<ResHandle> LoadObjectAsync(string name)
        {
            BundleData bundleData = bundleList.GetBundleDataWithAsset(name);
            if (bundleData == null)
            {
                return null;
            }
            BundleHandle handle = bundleHandlers.Find(x => x.name == bundleData.name);
            if (handle != null)
            {
                return await handle.LoadHandleAsync(name);
            }
            handle = Loader.Generate<BundleHandle>();
            await handle.LoadBundleAsync(this, bundleData.name);
            bundleHandlers.Add(handle);
            return await handle.LoadHandleAsync(name);
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件数据流</returns>
        public async Task<DataStream> ReadFileAsync(string fileName)
        {
            if (File.Exists(GetFilePath(fileName)))
            {
                throw GameFrameworkException.Generate<FileNotFoundException>();
            }
            using (FileStream fileStream = new FileStream(GetFilePath(fileName), FileMode.Open, FileAccess.Read))
            {
                int length = 0;
                DataStream stream = DataStream.Generate((int)fileStream.Length);
                byte[] bytes = new byte[4096];
                while ((length = await fileStream.ReadAsync(bytes)) > 0)
                {
                    stream.Write(bytes);
                }
                return stream;
            }
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件数据流</returns>
        public DataStream ReadFileSync(string fileName)
        {
            if (File.Exists(GetFilePath(fileName)))
            {
                throw GameFrameworkException.Generate<FileNotFoundException>();
            }
            using (FileStream fileStream = new FileStream(GetFilePath(fileName), FileMode.Open, FileAccess.Read))
            {
                int length = 0;
                DataStream stream = DataStream.Generate((int)fileStream.Length);
                byte[] bytes = new byte[4096];
                while ((length = fileStream.Read(bytes)) > 0)
                {
                    stream.Write(bytes);
                }
                return stream;
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件实际路径</returns>
        public string GetFilePath(string fileName)
        {
            if (Application.isEditor)
            {
                return Application.dataPath + "/../Hotfix/" + MD5Core.GetHashString(fileName);
            }
            return Application.persistentDataPath + "/" + MD5Core.GetHashString(fileName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void DeleteFile(string fileName)
        {
            if (!File.Exists(GetFilePath(fileName)))
            {
                return;
            }
            File.Delete(GetFilePath(fileName));
        }

        /// <summary>
        /// 回收资源管理器
        /// </summary>
        public void Release()
        {
            Loader.Release(bundleList);
            bundleList = null;
            foreach (var item in bundleHandlers)
            {
                Loader.Release(item);
            }
            bundleHandlers.Clear();
        }

        /// <summary>
        /// 轮询管理器
        /// </summary>
        public void Update()
        {
            for (int i = bundleHandlers.Count - 1; i >= 0; i--)
            {
                if (bundleHandlers[i].refCount > 0)
                {
                    continue;
                }
                bundleCacheList.Add(bundleHandlers[i]);
                bundleHandlers.Remove(bundleHandlers[i]);
            }

            for (int i = bundleCacheList.Count - 1; i >= 0; i--)
            {
                if (!bundleCacheList[i].CanUnload())
                {
                    continue;
                }
                Loader.Release(bundleCacheList[i]);
                bundleCacheList.Remove(bundleCacheList[i]);
            }
        }

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">文件数据</param>
        /// <returns>任务</returns>
        public async Task WriteFileAsync(string fileName, DataStream stream)
        {
            string filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                await fileStream.WriteAsync(stream.bytes, 0, stream.position);
            }
        }

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">文件数据</param>
        public void WriteFileSync(string fileName, DataStream stream)
        {
            string filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                fileStream.Write(stream.bytes, 0, stream.position);
            }
        }
    }
}
