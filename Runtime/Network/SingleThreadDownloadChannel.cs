using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework.Network
{
    public sealed class SingleThreadDownloadChannel : IRefrence
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

        private int recount;
        private bool isCancel;
        private bool isPause;
        private const int FILE_DOWNLOAD_MAX_RETRY_COUNT = 5;
        private const int FILE_DOWNLOAD_MAX_RETRY_INTERVAL = 1000;

        /// <summary>
        /// 回收下载器
        /// </summary>
        public void Release()
        {
            Loader.Release(stream);
            stream = null;
            url = string.Empty;
            form = 0;
            to = 0;
            isDone = false;
            isError = false;
            progres = 0;
            recount = 0;
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
            isCancel = true;
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
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            try
            {
                isDone = false;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Proxy = null;
                request.Timeout = 5000;
                request.KeepAlive = false;
                request.ServicePoint.Expect100Continue = false;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.ServicePoint.ConnectionLimit = 65500;
                request.AllowWriteStreamBuffering = false;
                request.Method = "GET";
                request.AddRange(form, to);
                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    ReadData(stream);
                }
            }
            catch
            {
                //是否超过最大重试次数
                if (recount > FILE_DOWNLOAD_MAX_RETRY_COUNT)
                {
                    isDone = true;
                    isError = true;
                    return;
                }
                await Task.Delay(FILE_DOWNLOAD_MAX_RETRY_INTERVAL);
                recount++;
                Debug.LogError("重试下载:" + url + " from:" + form + " to:" + to + " retryCount:" + recount);
                await Start();
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="stream"></param>
        private async void ReadData(Stream stream)
        {
            byte[] bytes = new byte[4096];
            int length = 0;
            int total = 0;
            while (!isCancel)
            {
                if (isPause)
                {
                    await Task.Delay(100);
                    continue;
                }
                length = await stream.ReadAsync(bytes, 0, bytes.Length);
                if (length <= 0)
                {
                    return;
                }
                stream.Write(bytes, 0, length);
                progres = (float)total / (to - form);
                total += length;
                form += length;
            }
            Debug.LogWarning("download:" + url + " from:" + form + " to:" + to + " total:" + total);
            isDone = true;
            isError = isCancel == true;
        }

        /// <summary>
        /// 生成下载器
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="form">下载起始位置</param>
        /// <param name="to">下载结束位置</param>
        /// <returns>下载器</returns>
        public static SingleThreadDownloadChannel Generate(string url, int form, int to)
        {
            SingleThreadDownloadChannel singleThreadDownloadChannel = Loader.Generate<SingleThreadDownloadChannel>();
            singleThreadDownloadChannel.to = to;
            singleThreadDownloadChannel.url = url;
            singleThreadDownloadChannel.form = form;
            singleThreadDownloadChannel.stream = DataStream.Generate(to - form);
            return singleThreadDownloadChannel;
        }
    }
}
