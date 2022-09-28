using System.Threading;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework.Resource
{
    sealed class DefaultResourceStreamingHandler : IResourceStreamingHandler
    {
        public void Delete(string fileName)
        {
        }

        public bool ExistPersistentAsset(string fileName)
        {
            return File.Exists(Path.Combine(Application.persistentDataPath, fileName));
        }

        public bool ExistStreamingAsset(string fileName)
        {
            return File.Exists(Path.Combine(Application.streamingAssetsPath, fileName));
        }

        public async Task<DataStream> ReadPersistentDataAsync(string fileName)
        {
            return DataStream.Generate(await File.ReadAllBytesAsync(Path.Combine(Application.persistentDataPath, fileName)));
        }

        public DataStream ReadPersistentDataSync(string fileName)
        {
            return DataStream.Generate(File.ReadAllBytes(Path.Combine(Application.persistentDataPath, fileName)));
        }

        public async Task<DataStream> ReadStreamingAssetDataAsync(string fileName)
        {
            if (Application.isEditor)
            {
                return DataStream.Generate(await File.ReadAllBytesAsync(Path.Combine(Application.streamingAssetsPath, fileName)));
            }
            TaskCompletionSource<DataStream> waiting = new TaskCompletionSource<DataStream>();
            ResourceDownloadHandle resourceDownloadHandle = ResourceDownloadHandle.Generate(Path.Combine(Application.streamingAssetsPath, fileName), args =>
            {
                if (args.state == ResourceUpdateState.Failure)
                {
                    waiting.SetException(GameFrameworkException.Generate(args.error));
                    return;
                }
                waiting.SetResult(DataStream.Generate(args.data));
            }, null);
            return await waiting.Task;
        }

        public DataStream ReadStreamingAssetDataSync(string fileName)
        {
            if (Application.isEditor)
            {
                return DataStream.Generate(File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, fileName)));
            }

            ResourceDownloadHandle resourceDownloadHandle = ResourceDownloadHandle.Generate(Path.Combine(Application.streamingAssetsPath, fileName), null, null);
            while (!resourceDownloadHandle.isDone)
            {
                Thread.Sleep(10);
            }
            if (resourceDownloadHandle.state == ResourceUpdateState.Failure)
            {
                return default;
            }
            return DataStream.Generate(resourceDownloadHandle.data);
        }

        public void Release()
        {

        }


        public Task WriteAsync(string fileName, DataStream stream)
        {
            return File.WriteAllBytesAsync(Path.Combine(Application.persistentDataPath, fileName), stream.bytes);
        }

        public void WriteSync(string fileName, DataStream stream)
        {
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName), stream.bytes);
        }
    }
}
