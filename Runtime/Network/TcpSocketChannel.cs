using System;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    public sealed class TcpSocketChannel : IChannel
    {
        public bool Actived => throw new NotImplementedException();

        public Task Connect(string name, string addres, ushort port)
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(DataStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
