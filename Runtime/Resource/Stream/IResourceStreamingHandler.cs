using System.Threading.Tasks;

namespace GameFramework.Resource
{
    public interface IResourceStreamingHandler : IRefrence
    {
        DataStream ReadStreamingAssetDataSync(string fileName);
        Task<DataStream> ReadStreamingAssetDataAsync(string fileName);
        DataStream ReadPersistentDataSync(string fileName);
        Task<DataStream> ReadPersistentDataAsync(string fileName);
        void WriteSync(string fileName, DataStream stream);
        Task WriteAsync(string fileName, DataStream stream);
        bool ExistStreamingAsset(string fileName);
        bool ExistPersistentAsset(string fileName);
        void Delete(string fileName);
    }
}
