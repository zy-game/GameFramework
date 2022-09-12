using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// ��������
    /// </summary>
    public interface IChannelContext : IRefrence
    {
        /// <summary>
        /// ���Ӷ���
        /// </summary>
        /// <value></value>
        IChannel Channel { get; }

        /// <summary>
        /// �Ͽ�����
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// �첽д������
        /// </summary>
        /// <param name="stream">��Ҫ���͵�����</param>
        /// <returns></returns>
        Task WriteAsync(DataStream stream);

        /// <summary>
        /// ������������
        /// </summary>
        void Flush();

        /// <summary>
        /// ������д�����ӻ���������������������
        /// </summary>
        /// <param name="stream">��Ҫ���͵�����</param>
        /// <returns></returns>
        Task WriteAndFlushAsync(DataStream stream);
    }
}
