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
            return File.Exists(AppConfig.HOTFIX_FILE_PATH + fileName);
        }

        public bool ExistStreamingAsset(string fileName)
        {
            return File.Exists(AppConfig.STREAMING_FILE_PATH + fileName);
        }

        public async Task<DataStream> ReadPersistentDataAsync(string fileName)
        {
            if (!ExistPersistentAsset(fileName))
            {
                return default;
            }
            return DataStream.Generate(await File.ReadAllBytesAsync(AppConfig.HOTFIX_FILE_PATH + fileName));
        }

        public DataStream ReadPersistentDataSync(string fileName)
        {
            if (!ExistPersistentAsset(fileName))
            {
                return default;
            }
            return DataStream.Generate(File.ReadAllBytes(AppConfig.HOTFIX_FILE_PATH + fileName));
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
            string filePath = AppConfig.STREAMING_FILE_PATH + fileName;
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
                return DataStream.Generate(File.ReadAllBytes(AppConfig.STREAMING_FILE_PATH + fileName));
            }

            ResourceDownloadHandle resourceDownloadHandle = ResourceDownloadHandle.Generate(AppConfig.STREAMING_FILE_PATH + fileName, null, null);
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


        public async Task WriteAsync(string fileName, DataStream stream)
        {
            if (File.Exists(AppConfig.HOTFIX_FILE_PATH + fileName))
            {
                File.Delete(AppConfig.HOTFIX_FILE_PATH + fileName);
            }
            using (FileStream fileStream = new FileStream(AppConfig.HOTFIX_FILE_PATH + fileName, FileMode.CreateNew, FileAccess.Write))
            {
                await fileStream.WriteAsync(stream.bytes, 0, stream.position);
                await fileStream.FlushAsync();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void WriteSync(string fileName, DataStream stream)
        {
            if (File.Exists(AppConfig.HOTFIX_FILE_PATH + fileName))
            {
                File.Delete(AppConfig.HOTFIX_FILE_PATH + fileName);
            }
            using (FileStream fileStream = new FileStream(AppConfig.HOTFIX_FILE_PATH + fileName, FileMode.CreateNew, FileAccess.Write))
            {
                fileStream.Write(stream.bytes, 0, stream.position);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
            }
        }
    }
}
