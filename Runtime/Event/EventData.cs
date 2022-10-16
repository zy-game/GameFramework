using System;

namespace GameFramework.Events
{
    /// <summary>
    /// 事件数据
    /// </summary>
    public sealed class EventData : IRefrence
    {
        public string eventId { get; private set; }
        public object eventData { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public static EventData Generate(string id, object data)
        {
            EventData eventUnit = Loader.Generate<EventData>();
            eventUnit.eventId = id;
            eventUnit.eventData = data;
            return eventUnit;
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