using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public interface IResourceManager : IGameModule
    {
        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="modle">资源模式</param>
        void SetResourceModle(ResouceModle modle);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        /// <exception cref="NullReferenceException">不存在资源对象</exception>
        ResHandle LoadAssetSync(string name);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        /// <exception cref="NullReferenceException">不存在资源对象</exception>
        Task<ResHandle> LoadAssetAsync(string name);

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>数据流</returns>
        /// <exception cref="System.IO.FileNotFoundException">文件不存在</exception>
        DataStream ReadFileSync(string fileName);

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>数据流</returns>
        /// <exception cref="System.IO.FileNotFoundException">文件不存在</exception>
        Task<DataStream> ReadFileAsync(string fileName);

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">数据流</param>
        void WriteFileSync(string fileName, DataStream stream);

        /// <summary>
        /// 写入文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">数据流</param>
        /// <returns>异步任务对象</returns>
        Task WriteFileAsync(string fileName, DataStream stream);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <exception cref="System.IO.FileNotFoundException">文件不存在</exception>
        void DeleteFile(string fileName);

        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <param name="progresCallback"></param>
        /// <param name="compoleted"></param>
        void CheckoutResourceUpdate(string url, GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted);

        /// <summary>
        /// 检查资源更新
        /// </summary>
        /// <typeparam name="IResourceUpdateListenerHandler"></typeparam>
        void CheckoutResourceUpdate<TResourceUpdateListenerHandler>(string url) where TResourceUpdateListenerHandler : IResourceUpdateListenerHandler;
    }
}
