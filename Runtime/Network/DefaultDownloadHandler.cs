using System;

namespace GameFramework.Network
{
    internal sealed class DefaultDownloadHandler : IDownloadHandle
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


        private bool isCancel;
        private bool isPauseDownload;
        private GameFrameworkAction<float> progresCallback;
        private GameFrameworkAction<IDownloadHandle> completedCallback;
        private MultiThreadDownloadChannel multiThreadDownloadChannel;

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            if (isCancel)
            {
                throw GameFrameworkException.GenerateFormat("cannot cancel a  canceled download task:{0}", url);
            }
            if (isDone)
            {
                throw GameFrameworkException.GenerateFormat("cannot cancel a completed download task:{0}", url);
            }
            isCancel = true;
            if (multiThreadDownloadChannel != null)
            {
                multiThreadDownloadChannel.Cancel();
            }
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void PauseDownload()
        {
            if (isPauseDownload)
            {
                return;
            }
            if (multiThreadDownloadChannel != null)
            {
                multiThreadDownloadChannel.Pause();
            }
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public void ResumeDownload()
        {
            if (!isPauseDownload)
            {
                return;
            }
            if (multiThreadDownloadChannel != null)
            {
                multiThreadDownloadChannel.Resume();
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Release()
        {
            progresCallback = null;
            completedCallback = null;
            Creater.Release(multiThreadDownloadChannel);
            multiThreadDownloadChannel = null;
            Creater.Release(stream);
            stream = null;
            isCancel = false;
            isDone = false;
            isError = false;
            isPauseDownload = false;
            url = string.Empty;
            form = 0;
            to = 0;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 尝试取消下载
        /// </summary>
        /// <returns></returns>
        public bool TryCancel()
        {
            if (isCancel)
            {
                return false;
            }
            Cancel();
            return isCancel;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public async void StartDownload()
        {
            multiThreadDownloadChannel = MultiThreadDownloadChannel.Generate(url, form, to);
            await multiThreadDownloadChannel.Start();
        }

        /// <summary>
        /// 轮询下载句柄
        /// </summary>
        public void Update()
        {
            if (isCancel)
            {
                return;
            }
            if (multiThreadDownloadChannel == null)
            {
                return;
            }
            if (multiThreadDownloadChannel.isDone)
            {
                return;
            }
            isDone = multiThreadDownloadChannel.isDone;
            isError = multiThreadDownloadChannel.isError;
            multiThreadDownloadChannel.FixedUpdate();
            progres = multiThreadDownloadChannel.progres;
            if (multiThreadDownloadChannel.isDone)
            {
                if (progresCallback != null)
                {
                    progresCallback(1);
                }
                if (completedCallback != null)
                {
                    completedCallback(this);
                }
                return;
            }
            if (progresCallback != null)
            {
                progresCallback(progres);
            }
        }

        /// <summary>
        /// 生成一个下载句柄
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="form">下载起始位置</param>
        /// <param name="to">下载结束位置</param>
        /// <param name="completed">下载完成回调</param>
        /// <param name="progres">下载进度更新回调</param>
        /// <returns>下载句柄</returns>
        public static DefaultDownloadHandler Generate(string url, int form, int to, GameFrameworkAction<IDownloadHandle> completed, GameFrameworkAction<float> progres)
        {
            DefaultDownloadHandler defaultDownloadHandler = Creater.Generate<DefaultDownloadHandler>();
            defaultDownloadHandler.to = to;
            defaultDownloadHandler.url = url;
            defaultDownloadHandler.form = form;
            defaultDownloadHandler.isDone = false;
            defaultDownloadHandler.isCancel = false;
            defaultDownloadHandler.progresCallback = progres;
            defaultDownloadHandler.completedCallback = completed;
            return defaultDownloadHandler;
        }
    }
}
