using System.Threading.Tasks;
using Object = UnityEngine.Object;
namespace GameFramework.Resource
{
    public interface IResourceLoaderHandler : IRefrence
    {
        void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler);
        ResHandle LoadAsset<T>(string assetName) where T : UnityEngine.Object;
        Task<ResHandle> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object;
        void Update();

        /// <summary>
        /// 重新加载所有包列表数据
        /// </summary>
        void AddResourceModule(string moduleName);
    }
}
