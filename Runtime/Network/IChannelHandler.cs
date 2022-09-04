using System;
namespace GameFramework.Network
{
    public interface IChannelHandler : IRefrence
    {
        void ChannelActive(IChannelContext context);
        void ChannelInactive(IChannelContext context);
        void ChannelDisconnect(IChannelContext context);
        void ChannelRead(IChannelContext context, DataStream stream);
        void ChannelError(IChannelContext context, Exception exception);
    }
}
