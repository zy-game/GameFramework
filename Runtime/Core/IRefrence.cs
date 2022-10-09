using System.Collections.Generic;
namespace GameFramework
{
    using System;
    using UnityEngine;
    /// <summary>
    /// 引用对象
    /// </summary>
    public interface IRefrence
    {
        void Release();
    }

    /// <summary>
    /// 游戏模块
    /// </summary>
    public interface IGameModule : IRefrence
    {
        /// <summary>
        /// 轮询模块
        /// </summary>
        void Update();
    }

    public interface ISerialize : IRefrence
    {
        void Serialize(DataStream stream);
        void Deserialize(DataStream stream);
    }

    /// <summary>
    /// 无参委托
    /// </summary>
    public delegate void GameFrameworkAction();

    /// <summary>
    /// 有参委托
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="args">参数</param>
    public delegate void GameFrameworkAction<T>(T args);

    /// <summary>
    /// 有参委托
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <typeparam name="T2">参数类型</typeparam>
    /// <param name="args">参数</param>
    /// <param name="args2">参数</param>
    public delegate void GameFrameworkAction<T, T2>(T args, T2 args2);

    /// <summary>
    /// 有参委托
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <typeparam name="T2">参数类型</typeparam>
    /// <typeparam name="T3">参数类型</typeparam>
    /// <param name="args">参数</param>
    /// <param name="args2">参数</param>
    /// <param name="args3">参数</param>
    public delegate void GameFrameworkAction<T, T2, T3>(T args, T2 args2, T3 args3);

    /// <summary>
    /// 有参委托
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <typeparam name="T2">参数类型</typeparam>
    /// <typeparam name="T3">参数类型</typeparam>
    /// <typeparam name="T4">参数类型</typeparam>
    /// <param name="args">参数</param>
    /// <param name="args2">参数</param>
    /// <param name="args3">参数</param>
    /// <param name="args4">参数</param>
    public delegate void GameFrameworkAction<T, T2, T3, T4>(T args, T2 args2, T3 args3, T4 args4);
}