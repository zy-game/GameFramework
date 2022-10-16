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
            if (!ExistPersistentAsset(fileName))
            {
                return default;
            }
            return DataStream.Generate(await File.ReadAllBytesAsync(Path.Combine(Application.persistentDataPath, fileName)));
        }

        public DataStream ReadPersistentDataSync(string fileName)
        {
            if (!ExistPersistentAsset(fileName))
            {
                return default;
            }
            return DataStream.Generate(File.ReadAllBytes(Path.Combine(Application.persistentDataPath, fileName)));
        }

        public async Task<T> ReadResourceDataAsync<T>(string fileName) where T : Object
        {
            TaskCompletionSource<T> waiting = new TaskCompletionSource<T>();
            ResourceRequest request = Resources.LoadAsync<T>(fileName);
            request.completed += _ =>
            {
                if (!request.isDone)
                {
                    waiting.SetResult(default);
                    return;
                }
                waiting.SetResult((T)request.asset);
            };
            return await waiting.Task;
        }

        public T ReadResourceDataSync<T>(string fileName) where T : Object
        {
            return Resources.Load<T>(fileName);
        }

        public async Task<DataStream> ReadStreamingAssetDataAsync(string fileName)
        {
            if (!ExistStreamingAsset(fileName))
            {
                return default;
            }
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (Application.isEditor)
            {
                byte[] bytes = await File.ReadAllBytesAsync(filePath);
                return DataStream.Generate(bytes);
            }
            TaskCompletionSource<DataStream> waiting = new TaskCompletionSource<DataStream>();
            ResourceDownloadHandle resourceDownloadHandle = ResourceDownloadHandle.Generate(filePath, args =>
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
            if (!ExistStreamingAsset(fileName))
            {
                return default;
            }
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
