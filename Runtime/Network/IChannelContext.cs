using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// 网络连接
    /// </summary>
    public interface IChannelContext : IRefrence
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        /// <value></value>
        IChannel Channel { get; }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// 异步写入数据
        /// </summary>
        /// <param name="stream">需要发送的数据</param>
        /// <returns></returns>
        Task WriteAsync(DataStream stream);

        /// <summary>
        /// 立即发送数据
        /// </summary>
        void Flush();

        /// <summary>
        /// 将数据写入连接缓冲区，并立即发送数据
        /// </summary>
        /// <param name="stream">需要发送的数据</param>
        /// <returns></returns>
        Task WriteAndFlushAsync(DataStream stream);
    }
}
