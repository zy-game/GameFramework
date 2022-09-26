using System.Threading.Tasks;
namespace GameFramework.Resource
{
    public interface IResourceUpdateHandler : IRefrence
    {
        void SetResourceModel(ResouceModle modle);
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler);
        void SetResourceDownloadUrl(string url);
        void CheckoutResourceUpdate(GameFrameworkAction<float> progresCallback, GameFrameworkAction<ResourceUpdateState> compoleted);
        void CheckoutResourceUpdate<TResourceUpdateListenerHandler>() where TResourceUpdateListenerHandler : IResourceUpdateListenerHandler;
    }

    public interface IResourceUpdateListenerHandler : IRefrence
    {
        void Progres(float progres);
        void Completed(ResourceUpdateState state);
    }
}
