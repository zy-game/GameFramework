using System.Net;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// ���Ӷ���
    /// </summary>
    public interface IChannel : IRefrence
    {
        /// <summary>
        /// ������
        /// </summary>
        /// <value></value>
        string Name { get; }

        /// <summary>
        /// �Ƿ񼤻�
        /// </summary>
        /// <value></value>
        bool Actived { get; }

        /// <summary>
        /// ����Զ�̵�ַ
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="addres">Զ�̵�ַ</param>
        /// <param name="port">Զ�̶˿�</param>
        /// <returns></returns>
        Task Connect(string name, string addres, ushort port);

        /// <summary>
        /// �Ͽ�����
        /// </summary>
        /// <returns></returns>
        Task Disconnect();

        /// <summary>
        /// ������д�����ӻ��������ȴ�����
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task WriteAsync(DataStream stream);

        /// <summary>
        /// �����������������ݷ��͵�Զ��
        /// </summary>
        void Flush();
    }
}
