using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏实体对象与游戏资源绑定
    /// </summary>
    sealed class GameContext : IRefrence
    {
        public string guid;
        public GameObject gameObject;
        public void Release()
        {
            GameObject.DestroyImmediate(gameObject);
            gameObject = null;
            guid = string.Empty;
        }
    }
}
