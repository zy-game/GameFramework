using System.Threading.Tasks;
using Object = UnityEngine.Object;
namespace GameFramework.Resource
{
    public interface IResourceLoaderHandler : IRefrence
    {
        void SetResourceModel(ResourceModle modle);
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler);
        ResHandle LoadAsset(string assetName);
        Task<ResHandle> LoadAssetAsync(string assetName);
        void Update();
    }
}
