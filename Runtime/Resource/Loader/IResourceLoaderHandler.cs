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
        /// ���¼������а��б�����
        /// </summary>
        void AddResourceModule(string moduleName);
    }
}
