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
        /// <param name="channelBuilder">链接参数</param>
        /// <returns>异步任务</returns>
        Task Connect<TChannel>(string name, string addres, ushort port) where TChannel : IChannel;

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
        public string Request(string url)
        {
            return Request(url);
        }

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <returns>返回数据</returns>
        public string Request(string url, Dictionary<string, string> header)
        {
            return Request(url, header);
        }

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
        public T Request<T>(string url)
        {
            return Request<T>(url, null);
        }

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public T Request<T>(string url, Dictionary<string, string> header)
        {
            return Request<T>(url, header, null);
        }

        /// <summary>
        /// 请求远端数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">参数</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public T Request<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            string responseData = Request(url, header, data);
            if (string.IsNullOrEmpty(responseData))
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(responseData);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        public string PostData(string url)
        {
            return PostData(url);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <returns>返回数据</returns>
        public string PostData(string url, Dictionary<string, string> header)
        {
            return PostData(url, header);
        }

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
        public T PostData<T>(string url)
        {
            return PostData<T>(url, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        public T PostData<T>(string url, Dictionary<string, string> header)
        {
            return PostData<T>(url, header, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">表单数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据对象</returns>
        public T PostData<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            string result = PostData(url, header, data);
            return CatJson.JsonParser.ParseJson<T>(result);
        }

        /// <summary>
        /// 获取链接长度
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>长度</returns>
        private long GetContentLength(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "HEAD";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            long length = response.ContentLength;
            request.Abort();
            response.Close();
            return length;
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>下载句柄</returns>
        public IDownloadHandle Download(string url, GameFrameworkAction completed, GameFrameworkAction<float> progres)
        {
            return Download(url, 0, completed, progres);
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="form">起始偏移</param>
        /// <returns>下载句柄</returns>
        public IDownloadHandle Download(string url, int form, GameFrameworkAction completed, GameFrameworkAction<float> progres)
        {
            return Download(url, form, (int)GetContentLength(url), completed, progres);
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="form">起始偏移</param>
        /// <param name="to">结束位置</param>
        /// <returns>下载句柄</returns>
        public IDownloadHandle Download(string url, int form, int to, GameFrameworkAction completed, GameFrameworkAction<float> progres);
    }
}
