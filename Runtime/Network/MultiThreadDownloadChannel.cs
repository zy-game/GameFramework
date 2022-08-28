using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework.Network
{
    /// <summary>
    /// 多线程下载器
    /// </summary>
    public sealed class MultiThreadDownloadChannel : IRefrence
    {
        /// <summary>
        /// 下载结束位置
        /// </summary>
        public int to { get; private set; }

        /// <summary>
        /// 下载起始位置
        /// </summary>
        public int form { get; private set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; private set; }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        public bool isDone { get; private set; }

        /// <summary>
        /// 是否下载错误
        /// </summary>
        public bool isError { get; private set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        public float progres { get; private set; }

        /// <summary>
        /// 下载数据
        /// </summary>
        public DataStream stream { get; private set; }


        private const int MULTI_THREAD_DOWNLOAD_SIZE = 1024 * 1024;
        private bool isCancel = false;
        private bool isPause = false;
        private List<SingleThreadDownloadChannel> singleThreadDownloadChannels = new List<SingleThreadDownloadChannel>();

        /// <summary>
        /// 回收下载器
        /// </summary>
        public void Release()
        {
            foreach (var singleThreadDownloadChannel in singleThreadDownloadChannels)
            {
                Creater.Release(singleThreadDownloadChannel);
            }
            singleThreadDownloadChannels.Clear();
            Creater.Release(stream);
            stream = null;
            url = null;
            form = 0;
            to = 0;
            isDone = false;
            isError = false;
            progres = 0;
            isCancel = false;
            isPause = false;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            if (isCancel)
            {
                return;
            }
            singleThreadDownloadChannels.ForEach(x => x.Cancel());
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            if (isPause)
            {
                return;
            }
            isPause = true;
            singleThreadDownloadChannels.ForEach(x => x.Pause());
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public void Resume()
        {
            if (!isPause)
            {
                return;
            }
            isPause = false;
            singleThreadDownloadChannels.ForEach(x => x.Resume());
        }

        /// <summary>
        /// 轮询下载
        /// </summary>
        public void FixedUpdate()
        {
            if (isDone)
            {
                return;
            }
            progres = 0;
            foreach (var item in singleThreadDownloadChannels)
            {
                progres += item.progres;
            }
            progres /= singleThreadDownloadChannels.Count;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            int total = to - form;
            isDone = false;
            stream = DataStream.Generate(total);
            if (MULTI_THREAD_DOWNLOAD_SIZE > to - form)
            {
                SingleThreadDownloadChannel singleThreadDownloadChannel = SingleThreadDownloadChannel.Generate(url, form, to);
                singleThreadDownloadChannels.Add(singleThreadDownloadChannel);
                await singleThreadDownloadChannel.Start();
                singleThreadDownloadChannel.stream.CopyTo(stream);
                isDone = true;
                isError = singleThreadDownloadChannel.isError;
                return;
            }
            int count = total / MULTI_THREAD_DOWNLOAD_SIZE;
            if (total % MULTI_THREAD_DOWNLOAD_SIZE != 0)
            {
                count++;
            }
            Task[] tasks = new Task[count];
            for (int i = 0; i < count; i++)
            {
                int offset = i * MULTI_THREAD_DOWNLOAD_SIZE;
                int end = offset + MULTI_THREAD_DOWNLOAD_SIZE - 1;
                if (end > total)
                {
                    end = total;
                }
                SingleThreadDownloadChannel singleThreadDownloadChannel = SingleThreadDownloadChannel.Generate(url, offset, end);
                singleThreadDownloadChannels.Add(singleThreadDownloadChannel);
                tasks[i] = singleThreadDownloadChannel.Start();
            }
            await Task.WhenAll(tasks);
            foreach (var item in singleThreadDownloadChannels)
            {
                if (item.isError)
                {
                    isError = true;
                    Creater.Release(item);
                }
            }
            if (!isError)
            {
                foreach (var singleThreadDownloadChannel in singleThreadDownloadChannels)
                {
                    singleThreadDownloadChannel.stream.CopyTo(stream);
                }
            }
            isDone = true;
        }

        /// <summary>
        /// 生成多线程下载器
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="offset">下载起始位置</param>
        /// <param name="end">下载结束位置</param>
        /// <returns>多线程下载器</returns>
        public static MultiThreadDownloadChannel Generate(string url, int offset, int end)
        {
            MultiThreadDownloadChannel multiThreadDownloadChannel = Creater.Generate<MultiThreadDownloadChannel>();
            multiThreadDownloadChannel.url = url;
            multiThreadDownloadChannel.form = offset;
            multiThreadDownloadChannel.to = end;
            return multiThreadDownloadChannel;
        }
    }
}
