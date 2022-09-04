using System.Net.Sockets;
using System;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// socket异步操作
    /// </summary>
    internal sealed class SocketAsyncEventOperation : SocketAsyncEventArgs, IRefrence
    {
        private DataStream dataStream;
        private IChannel Channel;
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
        public void SetChannel(IChannel channel)
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
                    // Channel.DoConnectFinished(this);
                    break;
                case SocketAsyncOperation.Receive:
                case SocketAsyncOperation.ReceiveFrom:
                    // Channel.DoRecvieFinished(this);
                    break;
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    // Channel.DoWriteFinished(this);
                    break;
                case SocketAsyncOperation.Disconnect:
                    // Channel.DoShutdownFinished(this);
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
}
