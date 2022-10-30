using System;
using UnityEngine;

namespace GameFramework.Game
{

    public interface IMapData : IRefrence
    {
        int id { get; }
        Vector3 position { get; }
        Vector4 size { get; }
        bool IsObstacle { get; }
    }
    /// <summary>
    /// 游戏入口
    /// </summary>
    public interface IGameWorld : IRefrence
    {
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
        /// 激活
        /// </summary>
        void Awake();

        /// <summary>
        /// 显示
        /// </summary>
        void Enable();

        /// <summary>
        /// 隐藏
        /// </summary>
        void Disable();

        /// <summary>
        /// 加载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T AddScriptble<T>() where T : IGameScriptSystem;

        /// <summary>
        /// 卸载逻辑单元
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveScriptble<T>() where T : IGameScriptSystem;

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

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        IEntity[] GetEntities(params Type[] componentTypes);

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="componentTypeNames"></param>
        /// <returns></returns>
        IEntity[] GetEntities(params string[] componentTypeNames);

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="componentAttribute"></param>
        /// <returns></returns>
        IEntity[] GetEntities(params int[] componentAttribute);

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="componentAttribute"></param>
        /// <returns></returns>
        IEntity[] GetEntities(int componentAttribute);
    }
}
