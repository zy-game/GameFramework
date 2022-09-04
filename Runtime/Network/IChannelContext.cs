using System.Threading.Tasks;

namespace GameFramework.Network
{
    public interface IChannelContext : IRefrence
    {
        IChannel Channel { get; }
        Task Disconnect();
        Task WriteAsync(DataStream stream);
        void Flush();
        Task WriteAndFlushAsync(DataStream stream);
    }
}
