using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    public sealed class NetworkManager : INetworkManager
    {
        private List<DefaultDownloadHandler> downloadHandles = new List<DefaultDownloadHandler>();
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();
        public async Task Connect<TChannel>(string name, string addres, ushort port) where TChannel : IChannel
        {
            TChannel channel = Creater.Generate<TChannel>();
            await channel.Connect(name, addres, port);
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
