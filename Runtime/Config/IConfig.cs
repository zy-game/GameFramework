namespace GameFramework.Config
{
    /// <summary>
    /// 配置表对象
    /// </summary>
    public interface IConfig : IRefrence
    {
        /// <summary>
        /// 配置表ID
        /// </summary>
        int id { get; }

        /// <summary>
        /// 配置表名
        /// </summary>
        string name { get; }
    }
}
