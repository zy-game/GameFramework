using GameFramework.Events;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// UI实体
    /// </summary>
    public interface IUIFormHandler : IRefrence
    {
        int layer { get; }
        GameObject GetChild(string name);
        void Notify(EventData eventData);
        void Awake();
        void Enable();
        void Diable();
    }
}
