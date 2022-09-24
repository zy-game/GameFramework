using System;
namespace GameFramework.Network
{
    /// <summary>
    /// 消息处理管道
    /// </summary>
    public interface IChannelHandler : IRefrence
    {
        /// <summary>
        /// 网络连接成功
        /// </summary>
        /// <param name="context"></param>
        void ChannelActive(IChannelContext context);

        /// <summary>
        /// 网络关闭
        /// </summary>
        /// <param name="context"></param>
        void ChannelInactive(IChannelContext context);

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stream"></param>
        void ChannelRead(IChannelContext context, DataStream stream);

        /// <summary>
        /// 网络错误
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        void ChannelError(IChannelContext context, Exception exception);
    }
}
