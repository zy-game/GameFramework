using System.Collections.Generic;
using System;

namespace GameFramework.Event
{
    /// <summary>
    /// 事件数据
    /// </summary>
    public interface IEventData : IRefrence
    {

    }

    /// <summary>
    /// 事件管理器
    /// </summary>
    public interface IEventManager : IGameModule
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        void Subscribe(string eventId, GameFrameworkAction callback);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        /// <typeparam name="T">回调参数类型</typeparam>
        void Subscribe<T>(string eventId, GameFrameworkAction<T> callback);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        void Subscribe(string eventId, GameFrameworkAction<IEventData> callback);

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        void Executed(string eventId);

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        void Executed<T>(string eventId, T eventData);

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        void Executed(string eventId, IEventData eventData);

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        void Unsubscribe(string eventId);

        /// <summary>
        /// 清理所有订阅
        /// </summary>
        void Clear();
    }

    public sealed class EventManager : IEventManager
    {
        private Dictionary<string, List<GameFrameworkAction>> events;

        
        /// <summary>
        /// 清理所有订阅
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void Executed(string eventId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public void Executed<T>(string eventId, T eventData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        public void Executed(string eventId, IEventData eventData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        public void Subscribe(string eventId, GameFrameworkAction callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        /// <typeparam name="T">回调参数类型</typeparam>
        public void Subscribe<T>(string eventId, GameFrameworkAction<T> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        public void Subscribe(string eventId, GameFrameworkAction<IEventData> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void Unsubscribe(string eventId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 轮询事件管理器
        /// </summary>
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}