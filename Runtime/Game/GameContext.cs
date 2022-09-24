using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 游戏实体对象与游戏资源绑定
    /// </summary>
    sealed class GameContext : MonoBehaviour
    {
        private static Dictionary<string, GameContext> contexts = new Dictionary<string, GameContext>();
        private IEntity _entity;
        public IEntity entity
        {
            get
            {
                return _entity;
            }
            set
            {
                if (_entity != null)
                {
                    throw GameFrameworkException.Generate("cannot bind entity again");
                }
                _entity = value;
                if (contexts.TryGetValue(_entity.guid, out GameContext context))
                {
                    throw GameFrameworkException.Generate("cannot bind entity again");
                }
                contexts.Add(_entity.guid, this);
            }
        }


        private void OnDestroy()
        {
            string guid = _entity.guid;
            entity.owner.RemoveEntity(guid);
            contexts.Remove(guid);
            _entity = null;
        }

        public static GameObject GetObject(string guid)
        {
            if (!contexts.TryGetValue(guid, out GameContext context))
            {
                return default;
            }
            return context.gameObject;
        }
    }
}
