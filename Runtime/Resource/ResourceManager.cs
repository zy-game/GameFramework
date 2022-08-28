using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Resource
{
    public sealed class BundleHandle : IRefrence
    {
        public string Name { get; set; }
        public void Release()
        {
        }
    }
    public sealed class FileHandle : IRefrence
    {
        public void Release()
        {
        }
    }
    public sealed class ResourceManager : IResourceManager
    {
        public void DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void DownloadResourceUpdate(ResourceUpdateDataed resourceUpdateDataed, GameFrameworkAction<float> progres, GameFrameworkAction completed)
        {
            throw new NotImplementedException();
        }

        public ResourceUpdateDataed EnsureResourceDelatiledStated(ResourceDetailed remoteResourceDetailedData)
        {
            throw new NotImplementedException();
        }

        public ResHandle LoadObject(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ResHandle> LoadObjectAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<DataStream> ReadFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public DataStream ReadFileSync(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public Task WriteFileAsync(string fileName, DataStream stream)
        {
            throw new NotImplementedException();
        }

        public void WriteFileSync(string fileName, DataStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
