using System.Collections.Generic;
using System;

namespace GameFramework.Event
{
    /// <summary>
    /// ??????
    /// </summary>
    sealed class SubscribeData : IRefrence
    {
        public string eventId { get; private set; }
        private List<GameFrameworkAction<object>> events;
        private Dictionary<int, GameFrameworkAction<object>> map;

        public SubscribeData(string id)
        {
            eventId = id;
            events = new List<GameFrameworkAction<object>>();
            map = new Dictionary<int, GameFrameworkAction<object>>();
        }

        public void Release()
        {
            map.Clear();
            events.Clear();
            eventId = string.Empty;
        }

        /// <summary>
        /// ????
        /// </summary>
        /// <param name="hascode">???????</param>
        /// <param name="callback">????</param>
        public void Add(int hascode, GameFrameworkAction<object> callback)
        {
            Remove(hascode);
            events.Add(callback);
            map.Add(hascode, callback);
        }

        /// <summary>
        /// ????
        /// </summary>
        /// <param name="hascode">???????</param>
        public void Remove(int hascode)
        {
            if (!map.TryGetValue(hascode, out GameFrameworkAction<object> callback))
            {
                return;
            }
            map.Remove(hascode);
            events.Remove(callback);
        }

        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="hascode">???????</param>
        /// <returns></returns>
        public bool IsHaveCallback(int hascode)
        {
            return map.ContainsKey(hascode);
        }

        /// <summary>
        /// ????
        /// </summary>
        /// <param name="args">????</param>
        public void Executed(object args)
        {
            foreach (GameFrameworkAction<object> callback in events)
            {
                SafeRun(callback, args);
            }
        }

        /// <summary>
        /// ????
        /// </summary>
        /// <param name="call">????</param>
        /// <param name="args">????</param>
        private void SafeRun(GameFrameworkAction<object> call, object args)
        {
            try
            {
                call(args);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}