using System.Threading.Tasks;

namespace GameFramework.Network
{
    internal sealed class DefaultChannelContext : IChannelContext
    {
        public IChannel Channel { get; internal set; }

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
}
