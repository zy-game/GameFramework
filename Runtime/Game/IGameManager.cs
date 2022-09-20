using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public interface IGame : IRefrence
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        /// <value></value>
        bool activeSelf { get; }

        /// <summary>
        /// 当前实体数量
        /// </summary>
        /// <value></value>
        int EntityCount { get; }

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
        GameEntity GenerateGameEntity();

        /// <summary>
        /// 获取游戏实体
        /// </summary>
        /// <param name="id">实体编号</param>
        /// <returns></returns>
        GameEntity GetGameEntity(string id);

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="id">实体编号</param>
        void RemoveGameEntity(int id);
    }

    /// <summary>
    /// 实体组件
    /// </summary>
    public interface IComponent : IRefrence
    {

    }

    /// <summary>
    /// 游戏脚本
    /// </summary>
    public interface IGameScript : IRefrence
    {

    }
    /// <summary>
    /// 游戏实体
    /// </summary>
    public sealed class GameEntity : IRefrence
    {
        public void Release()
        {
        }
    }

    /// <summary>
    /// 游戏管理器
    /// </summary>
    public interface IGameManager : IGameModule
    {
    }

    /// <summary>
    /// 游戏管理器
    /// </summary>
    public sealed class GameManager : IGameManager
    {
        public void Release()
        {
            
        }

        public void Update()
        {
        }
    }
}
