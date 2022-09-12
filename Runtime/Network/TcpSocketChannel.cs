using System.IO;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameFramework.Network
{
    public sealed class TcpSocketChannel<THandler> : IChannel where THandler : IChannelHandler
    {
        private Socket mSocket;
        private THandler channelHandler;
        private DataStream mRecvieStream;
        private IChannelContext channelContext;
        private Queue<SocketAsyncEventOperation> waitingWritedOperations;
        public string Name { get; private set; }
        public bool Actived
        {
            get
            {
                if (mSocket == null || mSocket.Connected == false)
                {
                    return false;
                }
                return true;
            }
        }

        public Task Connect(string name, string addres, ushort port)
        {
            Name = name;
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(addres), port);
            operation.SetCompletionCallback(OnConnectCompletionCallback);
            if (!mSocket.ConnectAsync(operation))
            {
                OnConnectCompletionCallback(operation);
            }
            return operation.GetSocketAsyncTask();
        }

        public Task Disconnect()
        {
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetCompletionCallback(OnDisconnectCompletionCallback);
            if (!mSocket.DisconnectAsync(operation))
            {
                OnDisconnectCompletionCallback(operation);
            }
            return operation.GetSocketAsyncTask();
        }

        private void OnRecvied()
        {
            DataStream stream = DataStream.Generate();
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetBuffer(stream);
            operation.SetCompletionCallback(OnRecvieCompletionCallback);
            if (!mSocket.ReceiveAsync(operation))
            {
                OnRecvieCompletionCallback(operation);
            }
        }

        public void Flush()
        {
            while (true)
            {
                if (!waitingWritedOperations.TryDequeue(out SocketAsyncEventOperation operation))
                {
                    break;
                }
                if (!mSocket.SendAsync(operation))
                {
                    OnWriteCompletionCallback(operation);
                }
            }
            waitingWritedOperations.Clear();
        }

        public void Release()
        {
            mSocket = null;
            while (true)
            {
                if (!waitingWritedOperations.TryDequeue(out SocketAsyncEventOperation operation))
                {
                    break;
                }
                Creater.Release(operation);
            }
            waitingWritedOperations.Clear();
            channelHandler.ChannelInactive(channelContext);
        }

        public Task WriteAsync(DataStream stream)
        {
            SocketAsyncEventOperation operation = Creater.Generate<SocketAsyncEventOperation>();
            operation.SetBuffer(stream);
            operation.SetCompletionCallback(OnWriteCompletionCallback);
            waitingWritedOperations.Enqueue(operation);
            return operation.GetSocketAsyncTask();
        }

        private void OnConnectCompletionCallback(SocketAsyncEventOperation operation)
        {
            if (operation.SocketError != SocketError.Success)
            {
                return;
            }
            mRecvieStream = DataStream.Generate();
            channelHandler = Creater.Generate<THandler>();
            channelContext = Creater.Generate<DefaultChannelContext>();
            waitingWritedOperations = new Queue<SocketAsyncEventOperation>();
            channelHandler.ChannelActive(channelContext);
            OnRecvied();
        }


        private void OnDisconnectCompletionCallback(SocketAsyncEventOperation operation)
        {
            Runtime.GetGameModule<NetworkManager>().RemoveChannel(this.Name);
        }

        private async void OnWriteCompletionCallback(SocketAsyncEventOperation operation)
        {
            if (SocketError.Success == operation.SocketError || operation.BytesTransferred > 0)
            {
                Runtime.GetGameModule<NetworkManager>().RemoveChannel(this.Name);
                return;
            }
            await Runtime.GetGameModule<NetworkManager>().Disconnect(this.Name);
        }

        private void OnRecvieCompletionCallback(SocketAsyncEventOperation operation)
        {
            if (SocketError.Success != operation.SocketError || operation.BytesTransferred <= 0)
            {
                Runtime.GetGameModule<NetworkManager>().RemoveChannel(this.Name);
                return;
            }
            DataStream recviedFragment = operation.GetDataStream();
            mRecvieStream.Write(recviedFragment);
            EnsureSplitNetworkPackagd();
        }

        private void EnsureSplitNetworkPackagd()
        {
            int packageLength = mRecvieStream.GetShort(mRecvieStream.position);
            if (mRecvieStream.position < packageLength)
            {
                return;
            }
            DataStream stream = mRecvieStream.Read(mRecvieStream.position + sizeof(short), packageLength);
            mRecvieStream.Trim(0, packageLength + sizeof(short));
            channelHandler.ChannelRead(channelContext, stream);
        }
    }
}
