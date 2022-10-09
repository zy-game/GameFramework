using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源句柄
    /// </summary>
    public sealed class ResHandle : IRefrence
    {
        private Object _object;
        private IBundleHandler handler;
        internal static ResHandle GenerateHandler(IBundleHandler handler, Object assetObject)
        {
            ResHandle resHandle = Loader.Generate<ResHandle>();
            resHandle._object = assetObject;
            resHandle.handler = handler;
            return resHandle;
        }

        public T Generate<T>() where T : Object
        {
            handler.AddRefrence();
            if (typeof(T) == typeof(GameObject))
            {
                T result = (T)GameObject.Instantiate(_object);
                result.name = _object.name;
                return result;
            }
            return (T)_object;
        }

        public void Release()
        {
            if (_object == null)
            {
                return;
            }
            handler = null;
            _object = null;
        }
    }
}
