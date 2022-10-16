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
    /// 资源管理器
    /// </summary>
    public sealed class ResourceManager : IResourceManager
    {


        /// <summary>
        /// 资源读写管道
        /// </summary>
        private IResourceStreamingHandler resourceStreamingHandler;

        /// <summary>
        /// 资源加载管道
        /// </summary>
        private IResourceLoaderHandler resourceLoaderHandler;

        /// <summary>
        /// 资源更新管道
        /// </summary>
        private IResourceUpdateHandler resourceUpdateHandler;

        /// <summary>
        /// 资源管理器构造函数
        /// </summary>
        public ResourceManager()
        {
            resourceStreamingHandler = Loader.Generate<DefaultResourceStreamingHandler>();
            resourceLoaderHandler = Loader.Generate<ResourceLoaderHandler>();
            resourceLoaderHandler.SetResourceStreamingHandler(resourceStreamingHandler);
            resourceUpdateHandler = Loader.Generate<DefaultResourceUpdateHandler>();
            resourceUpdateHandler.SetResourceStreamingHandler(resourceStreamingHandler);
        }



        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        public ResHandle LoadAssetSync(string name)
        {
            return resourceLoaderHandler.LoadAsset(name);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        public Task<ResHandle> LoadAssetAsync(string name)
        {
            return resourceLoaderHandler.LoadAssetAsync(name);
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件数据流</returns>
        public Task<DataStream> ReadFileAsync(string fileName)
        {
            return resourceStreamingHandler.ReadPersistentDataAsync(fileName);
        }

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件数据流</returns>
        public DataStream ReadFileSync(string fileName)
        {
            return resourceStreamingHandler.ReadPersistentDataSync(fileName);
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
            resourceStreamingHandler.Delete(fileName);
        }

        /// <summary>
        /// 回收资源管理器
        /// </summary>
        public void Release()
        {
            Loader.Release(resourceLoaderHandler);
            Loader.Release(resourceUpdateHandler);
            Loader.Release(resourceStreamingHandler);
            resourceLoaderHandler = null;
            resourceUpdateHandler = null;
            resourceStreamingHandler = null;
        }

        /// <summary>
        /// 轮询管理器
        /// </summary>
        public void Update()
        {
            if (resourceLoaderHandler == null)
            {
                return;
            }
            resourceLoaderHandler.Update();
        }

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">文件数据</param>
        /// <returns>任务</returns>
        public Task WriteFileAsync(string fileName, DataStream stream)
        {
            return resourceStreamingHandler.WriteAsync(fileName, stream);
        }

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">文件数据</param>
        public void WriteFileSync(string fileName, DataStream stream)
        {
            resourceStreamingHandler.WriteSync(fileName, stream);
        }
        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <param name="progresCallback"></param>
        /// <param name="compoleted"></param>
        public void CheckoutResourceUpdate(string url, GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted)
        {
            DefaultResourceUpdateListenerHandle defaultResourceUpdateListenerHandle = DefaultResourceUpdateListenerHandle.Generate(progresCallback, state =>
            {
                if (state == ResourceUpdateState.Failure)
                {
                    compoleted(state);
                    return;
                }
                defaultResourceUpdateListenerHandle = DefaultResourceUpdateListenerHandle.Generate(progresCallback, compoleted);
                resourceUpdateHandler.ChekeoutHotfixResourceListUpdate(url, defaultResourceUpdateListenerHandle);
            });
            resourceUpdateHandler.CheckoutStreamingAssetListUpdate(defaultResourceUpdateListenerHandle);
        }

        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <typeparam name="IResourceUpdateListenerHandler"></typeparam>
        public void CheckoutResourceUpdate<TResourceUpdateListenerHandler>(string url) where TResourceUpdateListenerHandler : IResourceUpdateListenerHandler
        {
            TResourceUpdateListenerHandler resourceUpdateListenerHandler = Loader.Generate<TResourceUpdateListenerHandler>();
            CheckoutResourceUpdate(url, resourceUpdateListenerHandler.Progres, state => 
            {
                if (state == ResourceUpdateState.Failure)
                {
                    resourceUpdateListenerHandler.Completed(state);
                    return;
                }
                resourceUpdateHandler.ChekeoutHotfixResourceListUpdate(url, resourceUpdateListenerHandler);
            });
            resourceUpdateHandler.CheckoutStreamingAssetListUpdate(resourceUpdateListenerHandler);
        }
    }
}
