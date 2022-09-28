using System.Threading.Tasks;
namespace GameFramework.Resource
{
    public interface IResourceUpdateHandler : IRefrence
    {
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceStreamingHandler);
        void CheckoutStreamingAssetListUpdate(IResourceUpdateListenerHandler resourceUpdateListenerHandler);
        void ChekeoutHotfixResourceListUpdate(string url, IResourceUpdateListenerHandler resourceUpdateListenerHandler);
    }
}
