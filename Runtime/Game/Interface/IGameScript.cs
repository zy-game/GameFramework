namespace GameFramework.Game
{
    /// <summary>
    /// 游戏脚本
    /// </summary>
    public interface IGameScript : IRefrence
    {
        /// <summary>
        /// 执行逻辑
        /// </summary>
        /// <param name="world"></param>
        void Executed(IGameWorld world);
    }
}
