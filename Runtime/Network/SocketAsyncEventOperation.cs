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
        private GameFrameworkAction<SocketAsyncEventOperation> callback;


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
            callback(this);
            _waiting.TryComplete();
            Creater.Release(this);
        }

        internal void SetCompletionCallback(GameFrameworkAction<SocketAsyncEventOperation> completionCallback)
        {
            callback = completionCallback;
        }

        public void SetBuffer(DataStream stream)
        {
            dataStream = stream;
            SetBuffer(dataStream.bytes, 0, dataStream.length);
        }

        public DataStream GetDataStream()
        {
            return dataStream;
        }

        public void Release()
        {
            callback = null;
            Channel = null;
            Creater.Release(dataStream);
            dataStream = null;
            _waiting = null;
        }
    }
}
