using GameFramework.Events;
using System;
using UnityEngine;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEngine.UI;

namespace GameFramework.Game
{
    public abstract class UIHandler : IUIFormHandler
    {
        public abstract int layer { get; }
        public abstract string name { get; }

        public GameObject this[string name]
        {
            get
            {
                return GetChild(name);
            }
        }

        internal GameObject gameObject { get; private set; }
        private Dictionary<string, RectTransform> childs;

        public UIHandler()
        {
            childs = new Dictionary<string, RectTransform>();
        }


        public GameObject GetChild(string name)
        {
            if (childs.TryGetValue(name, out RectTransform transform))
            {
                return transform.gameObject;
            }
            return default;
        }

        public void Release()
        {
            Debug.Log("release ui handle:" + this.gameObject.name);
            GameObject.DestroyImmediate(this.gameObject);
            childs.Clear();
            this.OnDestory();
        }

        public void Notify(EventData eventData)
        {
            this.OnEvent(eventData);
        }

        public void Awake()
        {
            ResHandle resHandle = ResourceManager.Instance.LoadAssetSync<GameObject>(name);
            this.gameObject = resHandle.Generate<GameObject>();
            RectTransform[] transforms = this.gameObject.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform item in transforms)
            {
                if (childs.ContainsKey(item.name))
                {
                    continue;
                }
                childs.Add(item.name, item);
            }
            Button[] buttons = this.gameObject.GetComponentsInChildren<Button>(true);
            foreach (Button item in buttons)
            {
                item.onClick.RemoveAllListeners();
                item.onClick.AddListener(() =>
                {
                    this.Notify(EventData.Generate(item.name, null));
                });
            }
            InputField[] inputs = this.gameObject.GetComponentsInChildren<InputField>(true);
            foreach (InputField item in inputs)
            {
                item.onEndEdit.RemoveAllListeners();
                item.onEndEdit.AddListener((args) =>
                {
                    this.Notify(EventData.Generate(item.name, args));
                });
            }

            Toggle[] toggles = this.gameObject.GetComponentsInChildren<Toggle>(true);
            foreach (Toggle item in toggles)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onValueChanged.AddListener((args) =>
                {
                    this.Notify(EventData.Generate(item.name, item.isOn));
                });
            }
            this.OnAwake();
        }

        public void Enable()
        {
            this.gameObject.SetActive(true);
            this.OnEnable();
        }

        public void Disable()
        {
            this.gameObject.SetActive(false);
            this.OnDisable();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnDestory() { }
        protected virtual void OnEvent(EventData eventData) { }
    }
}