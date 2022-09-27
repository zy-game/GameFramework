namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器运行模式
    /// </summary>
    public enum ResouceModle : byte
    {
        /// <summary>
        /// 本地模式-
        /// </summary>
        Resource,
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
