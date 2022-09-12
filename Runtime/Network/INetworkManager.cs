using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Diagnostics;

namespace GameFramework.Network
{

    /// <summary>
    /// 网络管理器
    /// </summary>
    public interface INetworkManager : IGameModule
    {
        /// <summary>
        /// 连接远程地址
        /// </summary>
        /// <param name="name">连接名</param>
        /// <param name="addres">链接地址</param>
        /// <param name="port">连接端口</param>
        /// <typeparam name="TChannel">链接类型</typeparam>
        /// <returns>异步任务</returns>
        Task Connect<TChannel>(string name, string addres, ushort port) where TChannel : IChannel;

        /// <summary>
        /// 断开链接
        /// </summary>
        /// <param name="name">连接名</param>
        /// <returns></returns>
        Task Disconnect(string name);

        /// <summary>
        /// 将数据写入缓冲区并立即发送数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialize"></param>
        /// <returns></returns>
        public Task WriteAndFlushAsync(string name, ISerialize serialize)
        {
            Task writeTask = WriteAsync(name, serialize);
            Flush(name);
            return writeTask;
        }

        /// <summary>
        ///  将数据写入缓冲区
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serialize"></param>
        /// <returns></returns>
        Task WriteAsync(string name, ISerialize serialize);

        /// <summary>
        /// 立即发送数据
        /// </summary>
        /// <param name="name"></param>
        void Flush(string name);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        string Request(string url);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <returns>返回数据</returns>
        string Request(string url, Dictionary<string, string> header);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">参数</param>
        /// <returns>返回数据</returns>
        string Request(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        T Request<T>(string url);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        T Request<T>(string url, Dictionary<string, string> header);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">参数</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        T Request<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        string PostData(string url);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <returns>返回数据</returns>
        string PostData(string url, Dictionary<string, string> header);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <returns>返回数据</returns>
        string PostData(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        T PostData<T>(string url);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        T PostData<T>(string url, Dictionary<string, string> header);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">表单数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        T PostData<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        Task<string> RequestAsync(string url);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <returns>返回数据</returns>
        Task<string> RequestAsync(string url, Dictionary<string, string> header);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">参数</param>
        /// <returns>返回数据</returns>
        Task<string> RequestAsync(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        Task<T> RequestAsync<T>(string url);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        Task<T> RequestAsync<T>(string url, Dictionary<string, string> header);

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">参数</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        Task<T> RequestAsync<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        Task<string> PostDataAsync(string url);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <returns>返回数据</returns>
        Task<string> PostDataAsync(string url, Dictionary<string, string> header);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <returns>返回数据</returns>
        Task<string> PostDataAsync(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        Task<T> PostDataAsync<T>(string url);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        Task<T> PostDataAsync<T>(string url, Dictionary<string, string> header);

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">表单数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        Task<T> PostDataAsync<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data);

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>下载句柄</returns>
        IDownloadHandle Download(string url, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres);

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="form">起始偏移</param>
        /// <returns>下载句柄</returns>
        IDownloadHandle Download(string url, int form, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres);

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="form">起始偏移</param>
        /// <param name="to">结束位置</param>
        /// <returns>下载句柄</returns>
        IDownloadHandle Download(string url, int form, int to, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres);
    }
}
