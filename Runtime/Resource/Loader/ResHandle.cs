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
        private string assetName;
        private IBundleHandler handler;
        internal static ResHandle GenerateHandler(IBundleHandler handler, string assetName, Object assetObject)
        {
            ResHandle resHandle = Loader.Generate<ResHandle>();
            resHandle._object = assetObject;
            resHandle.handler = handler;
            resHandle.assetName = assetName;
            return resHandle;
        }

        public void EnsueAssetLoadState()
        {
            if (_object != null)
            {
                return;
            }
            throw GameFrameworkException.Generate("load asset failur:" + assetName);
        }
        public T Generate<T>() where T : Object
        {
            if (handler != null)
            {
                handler.AddRefrence();
            }

            if (typeof(T) == typeof(GameObject))
            {
                T result = (T)GameObject.Instantiate(_object);
                result.name = _object.name;
                return result;
            }
            if (typeof(T) == typeof(Transform))
            {
                GameObject go = Generate<GameObject>();
                go.name = _object.name;
                return (T)System.Convert.ChangeType(go.transform, typeof(T));
            }
            return _object as T;
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
