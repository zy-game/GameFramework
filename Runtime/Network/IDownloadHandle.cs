namespace GameFramework.Network
{
    /// <summary>
    /// 下载句柄
    /// </summary>
    public interface IDownloadHandle : IRefrence
    {
        /// 下载结束位置
        /// </summary>
        int to { get; }

        /// <summary>
        /// 下载起始位置
        /// </summary>
        int form { get; }

        /// <summary>
        /// 下载地址
        /// </summary>
        string url { get; }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        bool isDone { get; }

        /// <summary>
        /// 是否下载错误
        /// </summary>
        bool isError { get; }

        /// <summary>
        /// 下载进度
        /// </summary>
        float progres { get; }

        /// <summary>
        /// 下载数据
        /// </summary>
        DataStream stream { get; }
        /// <summary>
        /// 取消下载
        /// </summary>
        void Cancel();

        /// <summary>
        /// 尝试取消下载
        /// </summary>
        /// <returns></returns>
        bool TryCancel();

        /// <summary>
        /// 暂停下载
        /// </summary>
        void PauseDownload();

        /// <summary>
        /// 继续下载
        /// </summary>
        void ResumeDownload();
    }
}
