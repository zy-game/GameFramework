using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    public sealed class WebSocketChannel<THandler> : IChannel where THandler : IChannelHandler
    {
        private WebSocketSharp.WebSocket webSocket;
        private Queue<Task> waitingExecuteSendBuffer = new Queue<Task>();
        private DefaultChannelContext defaultChannelContext;
        private THandler channelHandler;
        public bool Actived
        {
            get
            {
                return webSocket != null && webSocket.IsConnected;
            }
        }

        public Task Connect(string name, string addres, ushort port)
        {
            webSocket = new WebSocketSharp.WebSocket($"ws://{addres}:{port}/{name}");
            webSocket.OnOpen += (sender, e) =>
            {
                defaultChannelContext = Creater.Generate<DefaultChannelContext>();
                defaultChannelContext.Channel = this;
                channelHandler = Creater.Generate<THandler>();
                channelHandler.ChannelActive(defaultChannelContext);
            };

            webSocket.OnMessage += (sender, e) =>
            {
                var fmt = "[WebSocket Message] {0}";
                if (e.IsPing)
                {
                    UnityEngine.Debug.LogFormat(fmt, "ping");
                    webSocket.Ping();
                    return;
                }
                if(channelHandler!=null)
                {
                    
                }
            };

            webSocket.OnError += (sender, e) =>
            {
                var fmt = "[WebSocket Error] {0}";
                UnityEngine.Debug.LogFormat(fmt, e.Message);
                if (channelHandler != null)
                {
                    channelHandler.ChannelError(defaultChannelContext, GameFrameworkException.Generate(e.Message));
                }
            };

            webSocket.OnClose += (sender, e) =>
            {
                var fmt = "[WebSocket Close ({0})] {1}";
                UnityEngine.Debug.LogFormat(fmt, e.Code, e.Reason);
                if (channelHandler != null)
                {
                    channelHandler.ChannelInactive(defaultChannelContext);
                }
            };
            webSocket.Connect();
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            if (webSocket == null || !webSocket.IsConnected)
            {
                return Task.CompletedTask;
            }
            bool wailState = webSocket.IsConnected;
            webSocket.Close();
            if (wailState == webSocket.IsConnected)
            {
                return Task.FromException(GameFrameworkException.Generate("close the socket failur"));
            }
            return Task.CompletedTask;
        }

        public void Flush()
        {
            while (true)
            {
                if (!waitingExecuteSendBuffer.TryDequeue(out Task sender))
                {
                    return;
                }
                sender.Start();
            }
        }

        public void Release()
        {
            Disconnect();
            if (waitingExecuteSendBuffer.Count > 0)
            {
                while (true)
                {
                    if (!waitingExecuteSendBuffer.TryDequeue(out Task sender))
                    {
                        break;
                    }
                    sender.Dispose();
                }
                waitingExecuteSendBuffer.Clear();
            }
        }

        public Task WriteAsync(DataStream stream)
        {
            Task sendTask = new Task(() =>
            {
                if (!this.Actived)
                {
                    return;
                }
                webSocket.Send(stream.bytes);
            });
            waitingExecuteSendBuffer.Enqueue(sendTask);
            return sendTask;
        }
    }
}
