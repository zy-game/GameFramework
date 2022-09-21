using System;

namespace GameFramework.Events
{
    /// <summary>
    /// 事件数据
    /// </summary>
    sealed class EventUnit : IRefrence
    {
        public string eventId { get; private set; }
        public object eventData { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public EventUnit(string id, object data)
        {
            eventId = id;
            eventData = data;
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Release()
        {
            eventId = string.Empty;
            eventData = null;
            GC.SuppressFinalize(this);
        }
    }
}