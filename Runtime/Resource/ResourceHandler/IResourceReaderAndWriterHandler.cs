using System.Threading.Tasks;

namespace GameFramework.Resource
{
    public interface IResourceStreamingHandler : IRefrence
    {
        void SetResourceModle(ResouceModle modle);
        DataStream ReadStreamingAssetDataSync(string fileName);
        Task<DataStream> ReadStreamingAssetDataAsync(string fileName);
        DataStream ReadPersistentDataSync(string fileName);
        Task<DataStream> ReadPersistentDataAsync(string fileName);
        void WriteSync(string fileName, DataStream stream);
        Task WriteAsync(string fileName, DataStream stream);
        bool Exist(string fileName);
        void Delete(string fileName);
    }

    sealed class DefaultResourceStreamingHandler : IResourceStreamingHandler
    {
        public void Delete(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public bool Exist(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public Task<DataStream> ReadPersistentDataAsync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public DataStream ReadPersistentDataSync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public Task<DataStream> ReadStreamingAssetDataAsync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public DataStream ReadStreamingAssetDataSync(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public void Release()
        {
            throw new System.NotImplementedException();
        }

        public void SetResourceModle(ResouceModle modle)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteAsync(string fileName, DataStream stream)
        {
            throw new System.NotImplementedException();
        }

        public void WriteSync(string fileName, DataStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
