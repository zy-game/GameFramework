using System.Threading;
using System.Net.Mime;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器运行模式
    /// </summary>
    public enum ResouceModle
    {
        /// <summary>
        /// 本地模式-
        /// </summary>
        Local,
        /// <summary>
        /// 热更新模式
        /// </summary>
        Hotfix,
    }
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

        private ResouceModle resouceModle;

        /// <summary>
        /// 资源管理器构造函数
        /// </summary>
        public ResourceManager()
        {
            bundleCacheList = new List<BundleHandle>();
            bundleHandlers = new List<BundleHandle>();
        }

        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="modle">资源模式</param>
        public void SetResourceModle(ResouceModle modle)
        {
            resouceModle = modle;
        }

        /// <summary>
        /// 下载需要更新的资源文件
        /// </summary>
        /// <param name="resourceUpdateDataed">更新列表</param>
        /// <param name="progres">更新进度回调</param>
        /// <param name="completed">更新完成回调</param>
        public async Task<bool> DownloadResourceUpdate(string url, GameFrameworkAction<float> progres)
        {
            //todo 将streamingAsset的资源拷贝到沙盒中
            BundleList streamingBundleList = await ReadFileAsync<BundleList>(Runtime.BASIC_FILE_LIST_NAME);
            BundleList sanboxBundleList = await ReadFileAsync<BundleList>(Runtime.BASIC_FILE_LIST_NAME);
            List<BundleData> needCopyBundleList = UpdateAssetList.CheckUpdateList(streamingBundleList, sanboxBundleList);
            if (needCopyBundleList.Count > 0)
            {
                foreach (BundleData bundleData in needCopyBundleList)
                {
                    DataStream stream = await ReadFileAsync(bundleData.name, true);
                    await WriteFileAsync(bundleData.name, stream);
                }
            }
            if (resouceModle != ResouceModle.Local)
            {
                UpdateAssetList updateAssetList = UpdateAssetList.Generate(url, this, progres);
                bundleList = await updateAssetList.CheckNeedUpdateBundle();
                return updateAssetList.isHaveFailur;
            }
            return false;
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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadFileSync<T>(string fileName, bool isStreamingAssets = false)
        {
            DataStream stream = ReadFileSync(fileName);
            if (stream == null || stream.position <= 0)
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(stream.ToString());
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ReadFileAsync<T>(string fileName, bool isStreamingAssets = false)
        {
            DataStream stream = await ReadFileAsync(fileName);
            if (stream == null || stream.position <= 0)
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(stream.ToString());
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件数据流</returns>
        public async Task<DataStream> ReadFileAsync(string fileName, bool isStreamingAssets = false)
        {
            if (File.Exists(GetFilePath(fileName)))
            {
                throw GameFrameworkException.Generate<FileNotFoundException>();
            }
            if (isStreamingAssets)
            {
                UnityWebRequest request = new UnityWebRequest(Path.Combine(Application.streamingAssetsPath));
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Delay(10);
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    return default;
                }
                DataStream stream = DataStream.Generate(request.downloadHandler.data);
                return stream;
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
        public DataStream ReadFileSync(string fileName, bool isStreamingAssets = false)
        {
            if (File.Exists(GetFilePath(fileName)))
            {
                throw GameFrameworkException.Generate<FileNotFoundException>();
            }
            if (isStreamingAssets)
            {
                UnityWebRequest request = new UnityWebRequest(Path.Combine(Application.streamingAssetsPath));
                request.SendWebRequest();
                while (!request.isDone)
                {
                    Thread.Sleep(10);
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    return default;
                }
                DataStream stream = DataStream.Generate(request.downloadHandler.data);
                return stream;
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
            return Path.Combine(Application.persistentDataPath, MD5Core.GetHashString(fileName));
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
