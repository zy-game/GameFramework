using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public interface IGame : IRefrence
    {
        /// <summary>
        /// 游戏名称
        /// </summary>
        /// <value></value>
        string name { get; }

        /// <summary>
        /// 当前实体数量
        /// </summary>
        /// <value></value>
        int EntityCount { get; }

        /// <summary>
        /// 主相机
        /// </summary>
        /// <value></value>
        Camera GameCamera { get; }

        /// <summary>
        /// UI相机
        /// </summary>
        /// <value></value>
        IUIManager UIManager { get; }

        /// <summary>
        /// 加载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void LoadScript<T>() where T : IGameScript;

        /// <summary>
        /// 卸载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void UnloadScript<T>() where T : IGameScript;

        /// <summary>
        /// 轮询
        /// </summary>
        void Update();

        /// <summary>
        /// 创建游戏实体
        /// </summary>
        /// <returns></returns>
        IEntity CreateEntity();

        /// <summary>
        /// 创建实体对象
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        IEntity CreateEntity(string guid);

        /// <summary>
        /// 获取游戏实体
        /// </summary>
        /// <param name="id">实体编号</param>
        /// <returns></returns>
        IEntity GetEntity(string id);

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="id">实体编号</param>
        void RemoveEntity(string id);
    }
}
