using System;
namespace GameFramework.Network
{
    /// <summary>
    /// 链接处理器
    /// </summary>
    public interface IChannelHandler : IRefrence
    {
        /// <summary>
        /// 链接激活
        /// </summary>
        /// <param name="context"></param>
        void ChannelActive(IChannelContext context);

        /// <summary>
        /// 连接失活
        /// </summary>
        /// <param name="context"></param>
        void ChannelInactive(IChannelContext context);

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stream"></param>
        void ChannelRead(IChannelContext context, DataStream stream);

        /// <summary>
        /// 连接出现错我
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        void ChannelError(IChannelContext context, Exception exception);
    }
}
