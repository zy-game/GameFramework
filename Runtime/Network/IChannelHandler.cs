using System;
namespace GameFramework.Network
{
    /// <summary>
    /// ���Ӵ�����
    /// </summary>
    public interface IChannelHandler : IRefrence
    {
        /// <summary>
        /// ���Ӽ���
        /// </summary>
        /// <param name="context"></param>
        void ChannelActive(IChannelContext context);

        /// <summary>
        /// ����ʧ��
        /// </summary>
        /// <param name="context"></param>
        void ChannelInactive(IChannelContext context);

        /// <summary>
        /// �յ�����
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stream"></param>
        void ChannelRead(IChannelContext context, DataStream stream);

        /// <summary>
        /// ���ӳ��ִ���
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        void ChannelError(IChannelContext context, Exception exception);
    }
}
