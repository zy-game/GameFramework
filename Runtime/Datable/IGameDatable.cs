namespace GameFramework.Datable
{
    /// <summary>
    /// 游戏数据表
    /// </summary>
    public interface IGameDatable : IRefrence
    {
        /// <summary>
        /// 数据表唯一ID
        /// </summary>
        /// <value></value>
        string guid { get; }
    }
}
