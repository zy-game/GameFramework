using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 默认UI管理器
    /// </summary>
    sealed class DefaultUIFormManager : IUIFormManager
    {
        public Camera UICamera { get; private set; }
        private Dictionary<int, Canvas> layers;
        private Dictionary<Type, IUIFormHandler> handlers;
        private Dictionary<Type, IUIFormHandler> cacheings;

        public static IUIFormManager Generate(IGameWorld game)
        {
            DefaultUIFormManager worldUIManager = Loader.Generate<DefaultUIFormManager>();
            Resource.ResHandle handle = Runtime.GetGameModule<Resource.ResourceManager>().LoadAssetSync("UICamera");
            worldUIManager.UICamera = handle.Generate<GameObject>().GetComponent<Camera>();
            if (worldUIManager.UICamera == null)
            {
                throw GameFrameworkException.Generate("Resources Folder not find UICamera");
            }
            worldUIManager.layers = new Dictionary<int, Canvas>();
            worldUIManager.handlers = new Dictionary<Type, IUIFormHandler>();
            worldUIManager.cacheings = new Dictionary<Type, IUIFormHandler>();
            return worldUIManager;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenUI<T>() where T : IUIFormHandler => (T)OpenUI(typeof(T));

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public IUIFormHandler OpenUI(Type uiType)
        {
            if (cacheings.TryGetValue(uiType, out IUIFormHandler handler))
            {
                cacheings.Remove(uiType);
                handlers.Add(uiType, handler);
                return handler;
            }
            handler = GetUIHandler(uiType);
            if (handler != null)
            {
                return handler;
            }
            handler = (IUIFormHandler)Loader.Generate(uiType);
            ToLayer(handler, handler.layer);
            handlers.Add(uiType, handler);
            return default;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        /// <returns></returns>
        public IUIFormHandler OpenUI(string uiTypeName) => OpenUI(Type.GetType(uiTypeName));

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIHandler<T>() where T : IUIFormHandler => (T)GetUIHandler(typeof(T));

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public IUIFormHandler GetUIHandler(Type uiType)
        {
            if (handlers.TryGetValue(uiType, out IUIFormHandler handler))
            {
                return handler;
            }
            return default;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        /// <returns></returns>
        public IUIFormHandler GetUIHandler(string uiTypeName) => GetUIHandler(Type.GetType(uiTypeName));

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseUI<T>(bool isUnload = false) where T : IUIFormHandler => CloseUI(typeof(T), isUnload);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiType"></param>
        public void CloseUI(Type uiType, bool isUnload = false)
        {
            IUIFormHandler handler = GetUIHandler(uiType);
            if (handler == null)
            {
                return;
            }
            if (isUnload == false)
            {
                cacheings.Add(uiType, handler);
                return;
            }
            Loader.Release(handler);
            handlers.Remove(uiType);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        public void CloseUI(string uiTypeName, bool isUnload = false) => CloseUI(Type.GetType(uiTypeName), isUnload);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        /// <param name="handler"></param>
        public void ToLayer(IUIFormHandler handler, int layer) => ToLayer(handler, layer, Vector3.zero);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIFormHandler handler, int layer, Vector3 position) => ToLayer(handler, layer, position, Vector3.zero);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIFormHandler handler, int layer, Vector3 position, Vector3 rotation) => ToLayer(handler, layer, position, rotation, Vector3.one);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIFormHandler handler, int layer, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (handler == null)
            {
                return;
            }
            if (!layers.TryGetValue(layer, out Canvas canvas))
            {
                Resource.ResHandle handle = Runtime.GetGameModule<Resource.ResourceManager>().LoadAssetSync("Canvas");
                canvas = handle.Generate<GameObject>().GetComponent<Canvas>();
                if (canvas == null)
                {
                    throw GameFrameworkException.Generate("Resources not find Canvas");
                }
                canvas.sortingOrder = layer;
                canvas.worldCamera = UICamera;
            }
            handler.gameObject.SetParent(canvas, position, rotation, scale);
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            foreach (IUIFormHandler item in handlers.Values)
            {
                Loader.Release(item);
            }
            handlers.Clear();

            foreach (Canvas item in layers.Values)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
            layers.Clear();
            GameObject.DestroyImmediate(UICamera.gameObject);
        }
    }
}
