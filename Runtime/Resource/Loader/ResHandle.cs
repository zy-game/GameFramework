using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源句柄
    /// </summary>
    public sealed class ResHandle : IRefrence
    {
        public static ResHandle GenerateHandler(IResourceLoaderHandler resourceLoaderHandler, object assetObject)
        {
            return default;
        }

        public void Release()
        {
            throw new System.NotImplementedException();
        }
    }
}
