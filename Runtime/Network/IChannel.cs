using System.Net;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// 网络连接
    /// </summary>
    public interface IChannel : IRefrence
    {
        /// <summary>
        /// 连接名
        /// </summary>
        /// <value></value>
        string Name { get; }

        /// <summary>
        /// 是否激活
        /// </summary>
        /// <value></value>
        bool Actived { get; }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="name">连接名</param>
        /// <param name="addres">地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        Task Connect(string name, string addres, ushort port);

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// 将数据写入缓冲区等待发送
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task WriteAsync(DataStream stream);

        /// <summary>
        /// 立即将缓冲区中的数据发送到远端
        /// </summary>
        void Flush();
    }
}
