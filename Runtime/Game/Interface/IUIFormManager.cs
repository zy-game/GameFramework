using System;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public interface IUIFormManager : IRefrence
    {
        Camera UICamera { get; }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T OpenUI<T>() where T : IUIFormHandler;

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        IUIFormHandler OpenUI(Type uiType);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        /// <returns></returns>
        IUIFormHandler OpenUI(string uiTypeName);

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetUIHandler<T>() where T : IUIFormHandler;

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        IUIFormHandler GetUIHandler(Type uiType);

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        /// <returns></returns>
        IUIFormHandler GetUIHandler(string uiTypeName);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void CloseUI<T>(bool isUnload = false) where T : IUIFormHandler;

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiType"></param>
        void CloseUI(Type uiType, bool isUnload = false);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="uiTypeName"></param>
        void CloseUI(string uiTypeName, bool isUnload = false);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        void ToLayer(IUIFormHandler handler, int layer);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        void ToLayer(IUIFormHandler handler, int layer, Vector3 position);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        void ToLayer(IUIFormHandler handler, int layer, Vector3 position, Vector3 rotation);

        /// <summary>
        /// 将UI设置到指定的层级
        /// </summary>
        void ToLayer(IUIFormHandler handler, int layer, Vector3 position, Vector3 rotation, Vector3 scale);
    }
}
