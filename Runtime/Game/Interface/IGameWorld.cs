using System;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public interface IGameWorld : IRefrence
    {
        /// <summary>
        /// 游戏名称
        /// </summary>
        /// <value></value>S
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
        Camera MainCamera { get; }

        /// <summary>
        /// UI相机
        /// </summary>
        /// <value></value>
        IUIFormManager UIManager { get; }

        /// <summary>
        /// 音效管理器
        /// </summary>
        /// <value></value>
        ISoundManager SoundManager { get; }

        /// <summary>
        /// 加载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AddScriptble<T>() where T : IGameScript;

        /// <summary>
        /// 卸载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveScriptble<T>() where T : IGameScript;

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
