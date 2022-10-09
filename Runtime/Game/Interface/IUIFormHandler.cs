using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// UI实体
    /// </summary>
    public interface IUIFormHandler : IRefrence
    {
        int layer { get; }
        GameObject gameObject { get; }
        GameObject GetChild(string name);
    }
}
