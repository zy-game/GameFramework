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
        /// 加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        /// <exception cref="NullReferenceException">不存在资源对象</exception>
        ResHandle LoadObject(string name);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="name">资源名</param>
        /// <returns>资源句柄</returns>
        /// <exception cref="NullReferenceException">不存在资源对象</exception>
        Task<ResHandle> LoadObjectAsync(string name);

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
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>数据流</returns>
        /// <exception cref="System.IO.FileNotFoundException">文件不存在</exception>
        T ReadFileSync<T>(string fileName);

        /// <summary>
        /// 读取文件数据
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>数据流</returns>
        /// <exception cref="System.IO.FileNotFoundException">文件不存在</exception>
        Task<T> ReadFileAsync<T>(string fileName);

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
        /// 下载需要更新的资源
        /// </summary>
        /// <param name="resourceUpdateDataed">资源更新数据</param>
        /// <param name="progres">进度回调</param>
        /// <param name="completed">完成回调</param>
        Task<bool> DownloadResourceUpdate(string url, GameFrameworkAction<float> progres);
    }
}
