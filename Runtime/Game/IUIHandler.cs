using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// UI实体
    /// </summary>
    public interface IUIHandler : IEntity
    {
        int layer { get; }
        GameObject GetChild(string name);
    }
}
