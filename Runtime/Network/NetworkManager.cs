using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    public sealed class NetworkManager : INetworkManager
    {
        private Dictionary<string, IChannel> channels;
        private List<DefaultDownloadHandler> downloadHandles;

        public NetworkManager()
        {
            channels = new Dictionary<string, IChannel>();
            downloadHandles = new List<DefaultDownloadHandler>();
        }

        public async Task Connect<TChannel>(string name, string addres, ushort port) where TChannel : IChannel
        {
            TChannel channel = Loader.Generate<TChannel>();
            await channel.Connect(name, addres, port);
            if (!channel.Actived)
            {
                return;
            }
            channels.Add(name, channel);
        }

        internal async void RemoveChannel(string name)
        {
            if (!channels.TryGetValue(name, out IChannel channel))
            {
                return;
            }
            if (channel.Actived)
            {
                await Disconnect(name);
                return;
            }
            channels.Remove(name);
        }

        public Task Disconnect(string name)
        {
            if (!channels.TryGetValue(name, out IChannel channel))
            {
                return Task.CompletedTask;
            }
            if (!channel.Actived)
            {
                RemoveChannel(name);
                return Task.CompletedTask;
            }
            return channel.Disconnect();
        }

        public IDownloadHandle Download(string url, int form, int to, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres)
        {
            DefaultDownloadHandler defaultDownloadHandler = DefaultDownloadHandler.Generate(url, form, to, completed, progres);
            defaultDownloadHandler.StartDownload();
            return defaultDownloadHandler;
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>下载句柄</returns>
        public IDownloadHandle Download(string url, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres)
        {
            return Download(url, 0, completed, progres);
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="form">起始偏移</param>
        /// <returns>下载句柄</returns>
        public IDownloadHandle Download(string url, int form, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres)
        {
            return Download(url, form, (int)GetContentLength(url), completed, progres);
        }

        public void Flush(string name)
        {
            if (!channels.TryGetValue(name, out IChannel channel))
            {
                throw GameFrameworkException.GenerateFormat("not find the channel:{0}", name);
            }
            if (!channel.Actived)
            {
                throw GameFrameworkException.GenerateFormat("the channel is not actived:{0}", name);
            }
            channel.Flush();
        }

        public string PostData(string url)
        {
            return PostData(url, null);
        }

        public string PostData(string url, Dictionary<string, string> header)
        {
            return PostData(url, header, null);
        }

        public string PostData(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            return GenerateHttpWebReponse(url, "POST", header, data);
        }

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

        public void Release()
        {
            downloadHandles.ForEach(Loader.Release);
            downloadHandles.Clear();
            foreach (var item in channels.Values)
            {
                item.Disconnect();
                Loader.Release(item);
            }
            channels.Clear();
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        public string Request(string url)
        {
            return Request(url, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <returns>返回数据</returns>
        public string Request(string url, Dictionary<string, string> header)
        {
            return Request(url, header, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <returns>返回数据</returns>
        public string Request(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            return GenerateHttpWebReponse(url, "GET", header, data);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public T Request<T>(string url)
        {
            return Request<T>(url, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public T Request<T>(string url, Dictionary<string, string> header)
        {
            return Request<T>(url, header, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public T Request<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            string result = Request(url, header, data);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(result);
        }

        /// <summary>
        /// 轮询管理器
        /// </summary>
        public void Update()
        {
            for (int i = downloadHandles.Count - 1; i >= 0; i--)
            {
                if (downloadHandles[i].isDone)
                {
                    downloadHandles.Remove(downloadHandles[i]);
                    continue;
                }
                downloadHandles[i].Update();
            }
        }

        /// <summary>
        /// 异步写入链接缓冲区，等待发送
        /// </summary>
        /// <param name="name">连接管道名</param>
        /// <param name="serialize">序列数据</param>
        /// <returns>异步任务</returns>
        public Task WriteAsync(string name, ISerialize serialize)
        {
            if (!channels.TryGetValue(name, out IChannel channel))
            {
                throw GameFrameworkException.GenerateFormat("not find the channel:{0}", name);
            }
            if (!channel.Actived)
            {
                throw GameFrameworkException.GenerateFormat("the channel is not actived:{0}", name);
            }
            DataStream stream = DataStream.Generate();
            serialize.Serialize(stream);
            Loader.Release(serialize);
            return channel.WriteAsync(stream);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        public Task<string> RequestAsync(string url)
        {
            return RequestAsync(url, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <returns>返回数据</returns>
        public Task<string> RequestAsync(string url, Dictionary<string, string> header)
        {
            return RequestAsync(url, header, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <returns>返回数据</returns>
        public Task<string> RequestAsync(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            return GenerateHttpWebReponseAsync(url, "GET", header, data);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public Task<T> RequestAsync<T>(string url)
        {
            return RequestAsync<T>(url, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public Task<T> RequestAsync<T>(string url, Dictionary<string, string> header)
        {
            return RequestAsync<T>(url, header, null);
        }

        /// <summary>
        /// 请求远程web数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头数据</param>
        /// <param name="data">提交数据</param>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <returns>返回数据</returns>
        public async Task<T> RequestAsync<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            string result = await RequestAsync(url, header, data);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(result);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns>返回数据</returns>
        public Task<string> PostDataAsync(string url)
        {
            return PostDataAsync(url, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <returns>响应数据</returns>
        public Task<string> PostDataAsync(string url, Dictionary<string, string> header)
        {
            return PostDataAsync(url, header, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">数据</param>
        /// <returns>响应数据</returns>
        public Task<string> PostDataAsync(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            return GenerateHttpWebReponseAsync(url, "POST", header, data);
        }

        /// <summary>
        /// 生成同步web请求对象
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="method">操作类型</param>
        /// <param name="header">表头</param>
        /// <param name="data">数据</param>
        /// <returns>响应数据</returns>
        private string GenerateHttpWebReponse(string url, string method, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.Proxy = null;
            request.Timeout = 5000;
            request.KeepAlive = false;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.ConnectionLimit = 65500;
            request.AllowWriteStreamBuffering = false;
            if (header != null && header.Count > 0)
            {
                foreach (var item in header)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            if (data != null && data.Count > 0)
            {
                string postData = CatJson.JsonParser.ToJson(data);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(UTF8Encoding.UTF8.GetBytes(postData));
                }
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw GameFrameworkException.Generate(response.StatusCode);
                }
                using (Stream resposeStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resposeStream))
                    {
                        string result = reader.ReadToEnd();
                        request.Abort();
                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// 生成异步web请求对象
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="method">操作类型</param>
        /// <param name="header">表头</param>
        /// <param name="data">数据</param>
        /// <returns>响应数据</returns>
        private async Task<string> GenerateHttpWebReponseAsync(string url, string method, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.Proxy = null;
            request.Timeout = 5000;
            request.KeepAlive = false;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.ConnectionLimit = 65500;
            request.AllowWriteStreamBuffering = false;
            if (header != null && header.Count > 0)
            {
                foreach (var item in header)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            if (data != null && data.Count > 0)
            {
                string postData = CatJson.JsonParser.ToJson(data);
                using (Stream stream = request.GetRequestStream())
                {
                    await stream.WriteAsync(UTF8Encoding.UTF8.GetBytes(postData));
                }
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw GameFrameworkException.Generate(response.StatusCode);
                }
                using (Stream resposeStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resposeStream))
                    {
                        string result = await reader.ReadToEndAsync();
                        request.Abort();
                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <returns>响应数据</returns>
        public Task<T> PostDataAsync<T>(string url)
        {
            return PostDataAsync<T>(url, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <returns>响应数据</returns>
        public Task<T> PostDataAsync<T>(string url, Dictionary<string, string> header)
        {
            return PostDataAsync<T>(url, header, null);
        }

        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="header">表头</param>
        /// <param name="data">数据</param>
        /// <typeparam name="T">响应数据类型</typeparam>
        /// <returns>响应数据</returns>
        public async Task<T> PostDataAsync<T>(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            string result = await PostDataAsync(url, header, data);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return CatJson.JsonParser.ParseJson<T>(result);
        }
    }
}
