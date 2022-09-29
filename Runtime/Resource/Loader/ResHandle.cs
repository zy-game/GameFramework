using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源句柄
    /// </summary>
    public sealed class ResHandle : IRefrence
    {
        internal static ResHandle GenerateHandler(BundleHandle handler, object assetObject)
        {
            return default;
        }

        internal static ResHandle GenerateHandler(IResourceLoaderHandler handler, object assetObject)
        {
            return default;
        }

        public bool CanUnload()
        {
            return false;
        }

        public void Free()
        {

        }

        public void Release()
        {
            throw new System.NotImplementedException();
        }
    }
}
