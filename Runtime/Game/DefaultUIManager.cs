using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 默认UI管理器
    /// </summary>
    sealed class DefaultUIManager : IUIManager
    {
        public Camera UICamera { get; }
        private Dictionary<int, Canvas> layers;
        private Dictionary<Type, IUIHandler> handlers;


        public DefaultUIManager(IGame game)
        {
            UICamera = GameObject.Instantiate(Resources.Load<Camera>("UICamera"));
            if (UICamera == null)
            {
                throw GameFrameworkException.Generate("Resources Folder not find UICamera");
            }
            layers = new Dictionary<int, Canvas>();
            handlers = new Dictionary<Type, IUIHandler>();
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenUI<T>() where T : IUIHandler => (T)OpenUI(typeof(T));

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public IUIHandler OpenUI(Type uiType)
        {
            IUIHandler handler = GetUIHandler(uiType);
            if (handler != null)
            {
                return handler;
            }
            handler = (IUIHandler)Loader.Generate(uiType);
            ToLayer(handler, handler.layer);
            handlers.Add(uiType, handler);
            return default;
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        /// <returns></returns>
        public IUIHandler OpenUI(string uiTypeName) => OpenUI(Type.GetType(uiTypeName));

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIHandler<T>() where T : IUIHandler => (T)GetUIHandler(typeof(T));

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public IUIHandler GetUIHandler(Type uiType)
        {
            if (handlers.TryGetValue(uiType, out IUIHandler handler))
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
        public IUIHandler GetUIHandler(string uiTypeName) => GetUIHandler(Type.GetType(uiTypeName));

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseUI<T>() where T : IUIHandler => CloseUI(typeof(T));

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiType"></param>
        public void CloseUI(Type uiType)
        {
            IUIHandler handler = GetUIHandler(uiType);
            if (handler == null)
            {
                return;
            }

            Loader.Release(handler);
            handlers.Remove(uiType);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        public void CloseUI(string uiTypeName) => CloseUI(Type.GetType(uiTypeName));

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        /// <param name="handler"></param>
        public void ToLayer(IUIHandler handler, int layer) => ToLayer(handler, layer, Vector3.zero);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIHandler handler, int layer, Vector3 position) => ToLayer(handler, layer, position, Vector3.zero);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIHandler handler, int layer, Vector3 position, Vector3 rotation) => ToLayer(handler, layer, position, rotation, Vector3.one);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        public void ToLayer(IUIHandler handler, int layer, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (!layers.TryGetValue(layer, out Canvas canvas))
            {
                canvas = GameObject.Instantiate(Resources.Load<Canvas>("Canvas"));
                if (canvas == null)
                {
                    throw GameFrameworkException.Generate("Resources not find Canvas");
                }
                canvas.sortingOrder = layer;
                canvas.worldCamera = UICamera;
            }

            GameObject gameObject = handler.GetObject();
            gameObject.transform.SetParent(canvas.transform);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = Quaternion.Euler(rotation);
            gameObject.transform.localScale = scale;
        }

        /// <summary>
        /// 回收管理器
        /// </summary>
        public void Release()
        {
            foreach (IUIHandler item in handlers.Values)
            {
                Loader.Release(item);
            }
            handlers.Clear();

            foreach (Canvas item in layers.Values)
            {
                GameObject.DestroyImmediate(item);
            }
            layers.Clear();
        }
    }
}
