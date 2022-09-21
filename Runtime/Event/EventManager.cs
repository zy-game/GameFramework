using System.Collections.Generic;

namespace GameFramework.Events
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public sealed class EventManager : IEventManager
    {
        private Queue<EventUnit> eventUnits;
        private List<SubscribeData> subscribes;

        public EventManager()
        {
            eventUnits = new Queue<EventUnit>();
            subscribes = new List<SubscribeData>();
        }

        /// <summary>
        /// 清理所有订阅
        /// </summary>
        public void Clear()
        {
            foreach (SubscribeData item in subscribes)
            {
                Loader.Release(item);
            }
            subscribes.Clear();
        }

        /// <summary>
        /// 获取指定的事件订阅集
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <returns>事件订阅集</returns>
        private SubscribeData GetSubscribeData(string eventId)
        {
            return subscribes.Find(x => x.eventId == eventId);
        }

        /// <summary>
        /// 以不安全的方式立即执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void UnsafeExecuted(string eventId)
        {
            SubscribeData subscribe = GetSubscribeData(eventId);
            if (subscribe == null)
            {
                return;
            }
            subscribe.Executed(null);
        }

        /// <summary>
        /// 以不安全的方式立即执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件数据</param>
        /// <typeparam name="T">数据类型</typeparam>
        public void UnsafeExecuted<T>(string eventId, T eventData)
        {
            SubscribeData subscribe = GetSubscribeData(eventId);
            if (subscribe == null)
            {
                return;
            }
            subscribe.Executed(eventData);
        }

        /// <summary>
        /// 以不安全的方式立即执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件数据</param>
        public void UnsafeExecuted(string eventId, IEventData eventData)
        {
            SubscribeData subscribe = GetSubscribeData(eventId);
            if (subscribe == null)
            {
                return;
            }
            subscribe.Executed(eventData);
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void Executed(string eventId)
        {
            eventUnits.Enqueue(new EventUnit(eventId, null));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        public void Executed<T>(string eventId, T eventData)
        {
            eventUnits.Enqueue(new EventUnit(eventId, eventData));
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        public void Executed(string eventId, IEventData eventData)
        {
            eventUnits.Enqueue(new EventUnit(eventId, eventData));
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            Clear();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        public void Subscribe(string eventId, GameFrameworkAction callback)
        {
            SubscribeData subscribeData = GetSubscribeData(eventId);
            if (subscribeData == null)
            {
                subscribeData = new SubscribeData(eventId);
            }
            subscribeData.Add(callback.GetHashCode(), args => { callback(); });
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        /// <typeparam name="T">回调参数类型</typeparam>
        public void Subscribe<T>(string eventId, GameFrameworkAction<T> callback)
        {
            SubscribeData subscribeData = GetSubscribeData(eventId);
            if (subscribeData == null)
            {
                subscribeData = new SubscribeData(eventId);
            }
            subscribeData.Add(callback.GetHashCode(), args => { callback((T)args); });
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        public void Subscribe(string eventId, GameFrameworkAction<IEventData> callback)
        {
            SubscribeData subscribeData = GetSubscribeData(eventId);
            if (subscribeData == null)
            {
                subscribeData = new SubscribeData(eventId);
            }
            subscribeData.Add(callback.GetHashCode(), args => { callback((IEventData)args); });
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void Unsubscribe(string eventId)
        {
            SubscribeData subscribeData = GetSubscribeData(eventId);
            if (subscribeData == null)
            {
                return;
            }
            Loader.Release(subscribeData);
            subscribes.Remove(subscribeData);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="callback">订阅回调</param>
        public void Unsubscribe(GameFrameworkAction callback)
        {
            UnsubscribeWithHasCode(callback.GetHashCode());
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="callback">订阅回调</param>
        /// <typeparam name="T">回调参数类型</typeparam>
        public void Unsubscribe<T>(GameFrameworkAction<T> callback)
        {
            UnsubscribeWithHasCode(callback.GetHashCode());
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="callback">订阅回调</param>
        public void Unsubscribe(GameFrameworkAction<IEventData> callback)
        {
            UnsubscribeWithHasCode(callback.GetHashCode());
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="hasCode">回调哈希值</param>
        private void UnsubscribeWithHasCode(int hasCode)
        {
            SubscribeData subscribeData = subscribes.Find(x => x.IsHaveCallback(hasCode));
            if (subscribeData == null)
            {
                return;
            }
            subscribeData.Remove(hasCode);
        }

        /// <summary>
        /// 轮询事件管理器
        /// </summary>
        public void Update()
        {
            while (eventUnits.TryDequeue(out EventUnit eventUnit))
            {
                SubscribeData subscribeData = GetSubscribeData(eventUnit.eventId);
                if (subscribeData == null)
                {
                    continue;
                }
                subscribeData.Executed(eventUnit.eventData);
            }
        }
    }
}