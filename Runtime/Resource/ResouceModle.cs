namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器运行模式
    /// </summary>
    public enum ResourceModle : byte
    {
        /// <summary>
        /// 本地模式-
        /// </summary>
        Streaming,
        /// <summary>
        /// 热更新模式
        /// </summary>
        Hotfix,
    }
}
