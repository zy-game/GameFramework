using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Diagnostics;

namespace GameFramework.Network
{
    public interface IChannel : IRefrence
    {
        bool Actived { get; }
        Task Connect();
        Task Disconnect();
        Task WriteAsync(DataStream stream);
        void Flush();
    }

    /// <summary>
    /// socket异步操作
    /// </summary>
    internal sealed class SocketAsyncEventOperation : SocketAsyncEventArgs, IRefrence
    {
        private DataStream dataStream;
        private AbstractChannel Channel;
        private TaskCompletionSource _waiting;


        public Task GetSocketAsyncTask()
        {
            if (_waiting == null)
            {
                _waiting = new TaskCompletionSource();
            }
            return _waiting.Task;
        }

        /// <summary>
        /// 设置连接管道
        /// </summary>
        /// <param name="channel"></param>
        public void SetChannel(AbstractChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// 异步操作完成
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCompleted(SocketAsyncEventArgs e)
        {
            base.OnCompleted(e);
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    Channel.DoConnectFinished(this);
                    break;
                case SocketAsyncOperation.Receive:
                case SocketAsyncOperation.ReceiveFrom:
                    Channel.DoRecvieFinished(this);
                    break;
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    Channel.DoWriteFinished(this);
                    break;
                case SocketAsyncOperation.Disconnect:
                    Channel.DoShutdownFinished(this);
                    break;
            }
        }

        public bool EnsureAsyncOperationState()
        {
            Exception exception = null;
            if (SocketError != SocketError.Success)
            {
                exception = new SocketException((int)SocketError);
            }
            if (LastOperation == SocketAsyncOperation.Connect)
            {
                return exception != null;
            }
            if (BytesTransferred <= 0)
            {
                exception = GameFrameworkException.GenerateFormat("remote server is closed the channel connected");
            }
            if (exception != null)
            {
                _waiting?.SetException(exception);
            }
            return exception != null;
        }

        public void SetBuffer(DataStream stream)
        {
            dataStream = stream;
            SetBuffer(dataStream.bytes, 0, dataStream.position);
        }

        public DataStream GetDataStream()
        {
            return dataStream;
        }

        public void Release()
        {
            Channel = null;
            Creater.Release(dataStream);
            dataStream = null;
            _waiting = null;
        }

        internal void SetCompletedTask()
        {
            if (_waiting != null)
            {
                _waiting.Complete();
                _waiting = null;
            }
        }
    }
    public abstract class AbstractChannel : IChannel
    {
        public bool Actived { get; private set; }
        protected Socket mSocket;
        protected IPEndPoint remoteAddres;
        protected IChannelHandler channelHandler;
        private Queue<SocketAsyncEventOperation> waitingSender;

        public AbstractChannel()
        {
            waitingSender = new Queue<SocketAsyncEventOperation>();
        }

        public abstract Task Connect();

        internal void SetConnectAddres(IPEndPoint endPoint)
        {
            remoteAddres = endPoint;
        }
        internal void SetChannelHandler(IChannelHandler channelHandler)
        {
            this.channelHandler = channelHandler;
        }
        public Task Disconnect()
        {
            if (!Actived)
            {
                return Task.FromException(GameFrameworkException.Generate("the channel is not actived"));
            }
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetChannel(this);
            if (!mSocket.DisconnectAsync(operation))
            {
                DoShutdownFinished(operation);
            }
            return operation.GetSocketAsyncTask();
        }

        public void Flush()
        {
            if (!Actived)
            {
                throw GameFrameworkException.Generate("the channel is not actived");
            }
            while (waitingSender.Count > 0)
            {
                SocketAsyncEventOperation operation = waitingSender.Dequeue();
                if (!mSocket.SendAsync(operation))
                {
                    DoWriteFinished(operation);
                }
            }
        }

        public void Release()
        {
            Actived = false;
            while (waitingSender.Count > 0)
            {
                Creater.Release(waitingSender.Dequeue());
            }
        }

        public Task WriteAsync(DataStream stream)
        {
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetChannel(this);
            operation.SetBuffer(stream);
            waitingSender.Enqueue(operation);
            return operation.GetSocketAsyncTask();
        }

        internal abstract void DoConnectFinished(SocketAsyncEventOperation operation);
        internal abstract void DoRecvieFinished(SocketAsyncEventOperation operation);
        internal abstract void DoWriteFinished(SocketAsyncEventOperation operation);
        internal abstract void DoShutdownFinished(SocketAsyncEventOperation operation);
    }

    public sealed class TcpSocketChannel : AbstractChannel
    {
        private IChannelContext context;
        public override Task Connect()
        {
            if (Actived)
            {
                return Task.FromException(GameFrameworkException.Generate("the channel is actived"));
            }
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Pup);
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetChannel(this);
            operation.RemoteEndPoint = remoteAddres;
            if (!mSocket.ConnectAsync(operation))
            {
                DoConnectFinished(operation);
            }
            return operation.GetSocketAsyncTask();
        }
        private void DoRecvie()
        {

        }
        internal override void DoConnectFinished(SocketAsyncEventOperation operation)
        {
            if (operation.EnsureAsyncOperationState())
            {
                return;
            }
            DoRecvie();
            operation.SetCompletedTask();
            Creater.Release(operation);
        }

        internal override void DoRecvieFinished(SocketAsyncEventOperation operation)
        {
            if (operation.EnsureAsyncOperationState())
            {
                return;
            }
            DataStream recvData = operation.GetDataStream();
            DoRecvie();
            Creater.Release(operation);
        }

        internal override void DoShutdownFinished(SocketAsyncEventOperation operation)
        {

        }

        internal override void DoWriteFinished(SocketAsyncEventOperation operation)
        {

        }
    }

    public sealed class WebSocketChannel : AbstractChannel
    {
        private WebSocketSharp.WebSocket webSocket;
        public override Task Connect()
        {
            webSocket = new WebSocketSharp.WebSocket("");
            webSocket.ConnectAsync();
            return Task.CompletedTask;
        }

        internal override void DoConnectFinished(SocketAsyncEventOperation operation)
        {
        }

        internal override void DoRecvieFinished(SocketAsyncEventOperation operation)
        {
        }

        internal override void DoShutdownFinished(SocketAsyncEventOperation operation)
        {
        }

        internal override void DoWriteFinished(SocketAsyncEventOperation operation)
        {
        }
    }

    public interface IChannelContext : IRefrence
    {
        IChannel Channel { get; }
        Task Disconnect();
        Task WriteAsync(DataStream stream);
        void Flush();
        Task WriteAndFlushAsync(DataStream stream);
    }

    internal sealed class DefaultChannelContext : IChannelContext
    {
        public IChannel Channel { get; private set; }

        public Task Disconnect()
        {
            return Channel.Disconnect();
        }

        public void Flush()
        {
            Channel.Flush();
        }

        public void Release()
        {
            Channel = null;
        }

        public Task WriteAndFlushAsync(DataStream stream)
        {
            Task waiting = Channel.WriteAsync(stream);
            Channel.Flush();
            return waiting;
        }

        public Task WriteAsync(DataStream stream)
        {
            return Channel.WriteAsync(stream);
        }

        public static DefaultChannelContext Generate(IChannel channel)
        {
            DefaultChannelContext defaultChannelContext = Creater.Generate<DefaultChannelContext>();
            defaultChannelContext.Channel = channel;
            return defaultChannelContext;
        }
    }

    public interface IChannelHandler : IRefrence
    {
        void ChannelActive(IChannelContext context);
        void ChannelInactive(IChannelContext context);
        void ChannelDisconnect(IChannelContext context);
        void ChannelRead(IChannelContext context, DataStream stream);
    }

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
        Task Connect<T>(string name, string addres, ushort port) where T : IChannelHandler;

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

    public sealed class NetworkManager : INetworkManager
    {
        private List<DefaultDownloadHandler> downloadHandles = new List<DefaultDownloadHandler>();
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();
        public async Task Connect<T>(string name, string addres, ushort port) where T : IChannelHandler
        {
            TcpSocketChannel channel = Creater.Generate<TcpSocketChannel>();
            channel.SetChannelHandler(Creater.Generate<T>());
            channel.SetConnectAddres(new IPEndPoint(IPAddress.Parse(addres), port));
            await channel.Connect();
            if (!channel.Actived)
            {
                return;
            }
            channels.Add(name, channel);
        }

        public IDownloadHandle Download(string url, int form, int to, GameFrameworkAction completed, GameFrameworkAction<float> progres)
        {
            DefaultDownloadHandler defaultDownloadHandler = DefaultDownloadHandler.Generate(url, form, to, completed, progres);
            defaultDownloadHandler.StartDownload();
            return defaultDownloadHandler;
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

        public string PostData(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "POST";
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

        public void Release()
        {
            downloadHandles.ForEach(Creater.Release);
            downloadHandles.Clear();
            foreach (var item in channels.Values)
            {
                item.Disconnect();
                Creater.Release(item);
            }
            channels.Clear();
        }

        public string Request(string url, Dictionary<string, string> header, Dictionary<string, object> data)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
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
            Creater.Release(serialize);
            return channel.WriteAsync(stream);
        }
    }
}
