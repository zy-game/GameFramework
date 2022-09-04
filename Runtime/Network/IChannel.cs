using System.Threading.Tasks;

namespace GameFramework.Network
{
    public interface IChannel : IRefrence
    {
        bool Actived { get; }
        Task Connect(string name, string addres, ushort port);
        Task Disconnect();
        Task WriteAsync(DataStream stream);
        void Flush();
    }
}
