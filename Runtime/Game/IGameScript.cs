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
        /// <param name="game"></param>
        /// <param name="entity"></param>
        void Executed(IGame game);
    }
}
