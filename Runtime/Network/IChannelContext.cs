using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// 连接上下文
    /// </summary>
    public interface IChannelContext : IRefrence
    {
        /// <summary>
        /// 网络连接
        /// </summary>
        /// <value></value>
        IChannel Channel { get; }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <returns></returns>
        Task WriteAsync(DataStream stream);

        /// <summary>
        /// 立即发送数据
        /// </summary>
        void Flush();

        /// <summary>
        /// 写入数据并立即发送
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <returns></returns>
        Task WriteAndFlushAsync(DataStream stream);
    }
}
