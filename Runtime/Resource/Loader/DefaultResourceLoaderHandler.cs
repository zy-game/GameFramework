using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
namespace GameFramework.Resource
{
    sealed class DefaultResourceLoaderHandler : IResourceLoaderHandler
    {
        public ResHandle LoadAsset(string assetName)
        {
            throw new NotImplementedException();
        }

        public Task<ResHandle> LoadAssetAsync(string assetName)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void SetResourceModel(ResouceModle modle)
        {
            throw new NotImplementedException();
        }

        public void SetResourceStreamingHandler(IResourceStreamingHandler resourceReaderAndWriterHandler)
        {
            throw new NotImplementedException();
        }

        public void UnloadAsset(Object assetObject)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
