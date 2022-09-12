using System.Collections.Generic;
using System;

namespace GameFramework.Event
{
    /// <summary>
    /// �¼�����
    /// </summary>
    public interface IEventData : IRefrence
    {

    }

    /// <summary>
    /// �¼�������
    /// </summary>
    public interface IEventManager : IGameModule
    {
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        void Subscribe(string eventId, GameFrameworkAction callback);

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        /// <typeparam name="T">�ص���������</typeparam>
        void Subscribe<T>(string eventId, GameFrameworkAction<T> callback);

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        void Subscribe(string eventId, GameFrameworkAction<IEventData> callback);

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        void Executed(string eventId);

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="eventData">�¼�����</param>
        /// <typeparam name="T">�¼���������</typeparam>
        void Executed<T>(string eventId, T eventData);

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="eventData">�¼�����</param>
        void Executed(string eventId, IEventData eventData);

        /// <summary>
        /// ȡ�������¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        void Unsubscribe(string eventId);

        /// <summary>
        /// �������ж���
        /// </summary>
        void Clear();
    }

    public sealed class EventManager : IEventManager
    {
        private Dictionary<string, List<GameFrameworkAction>> events;

        
        /// <summary>
        /// �������ж���
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        public void Executed(string eventId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="eventData">�¼�����</param>
        /// <typeparam name="T">�¼���������</typeparam>
        public void Executed<T>(string eventId, T eventData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ִ���¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="eventData">�¼�����</param>
        public void Executed(string eventId, IEventData eventData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Release()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        public void Subscribe(string eventId, GameFrameworkAction callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        /// <typeparam name="T">�ص���������</typeparam>
        public void Subscribe<T>(string eventId, GameFrameworkAction<T> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        /// <param name="callback">�¼��ص�</param>
        public void Subscribe(string eventId, GameFrameworkAction<IEventData> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ȡ�������¼�
        /// </summary>
        /// <param name="eventId">�¼�ID</param>
        public void Unsubscribe(string eventId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ѯ�¼�������
        /// </summary>
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}