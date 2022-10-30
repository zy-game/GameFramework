namespace GameFramework.Game
{
    /// <summary>
    /// 游戏脚本
    /// </summary>
    public interface IGameScriptSystem : IRefrence
    {
        /// <summary>
        /// 执行逻辑
        /// </summary>
        /// <param name="world"></param>
        void Running();
    }
}
