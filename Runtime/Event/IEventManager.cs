namespace GameFramework.Events
{

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
        void UnsafeExecuted(string eventId);

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        /// <typeparam name="T">事件参数类型</typeparam>
        void UnsafeExecuted<T>(string eventId, T eventData);

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="eventData">事件参数</param>
        void UnsafeExecuted(string eventId, IEventData eventData);

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
        /// 取消所有订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        void Unsubscribe(string eventId);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        void Unsubscribe(GameFrameworkAction callback);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        /// <typeparam name="T">回调参数类型</typeparam>
        void Unsubscribe<T>(GameFrameworkAction<T> callback);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="callback">事件回调</param>
        void Unsubscribe(GameFrameworkAction<IEventData> callback);

        /// <summary>
        /// 清理所有订阅
        /// </summary>
        void Clear();
    }
}