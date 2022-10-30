using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameFramework.Resource;
using Object = UnityEngine.Object;

namespace GameFramework.Editor.ResoueceEditor
{
    class BundleListGUI
    {
        private static AssetData seletion;
        private static List<string> moduleNames = new List<string>() { "none" };
        private bool IsPackaged;
        public BundleList bundleList { get; }

        public int Count
        {
            get
            {
                return bundleList.Count;
            }
        }
        private Dictionary<string, Object> objectList;
        private Dictionary<string, bool> selectionBundles;
        private Dictionary<string, bool> foldoutBundles;
        public BundleListGUI(BundleList bundleList, bool IsPackaged)
        {
            this.IsPackaged = IsPackaged;
            this.bundleList = bundleList;
            objectList = new Dictionary<string, Object>();
            foldoutBundles = new Dictionary<string, bool>();
            selectionBundles = new Dictionary<string, bool>();
        }

        public void OnGUI(EditorWindow window)
        {
            Rect backgroundRect = EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(5);
                if (bundleList.Count <= 0)
                {
                    GUILayout.Label(GameEditorStyle.s_ListIsEmpty);
                }
                else
                {

                    for (int i = 0; i < bundleList.Count; i++)
                    {
                        GUILayout.BeginHorizontal(GameEditorStyle.headerBackground, GUILayout.Height(30));
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Space(100);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.Space(-21);
                        GUILayout.BeginHorizontal();
                        {
                            if (!selectionBundles.TryGetValue(bundleList[i].name, out bool state))
                            {
                                selectionBundles.Add(bundleList[i].name, state = false);
                            }
                            selectionBundles[bundleList[i].name] = GUILayout.Toggle(state, "");

                            if (!foldoutBundles.TryGetValue(bundleList[i].name, out bool foldout))
                            {
                                foldoutBundles.Add(bundleList[i].name, foldout = false);
                            }
                            foldoutBundles[bundleList[i].name] = EditorGUILayout.Foldout(foldout, bundleList[i].name);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                        if (foldoutBundles[bundleList[i].name])
                        {
                            Rect rect = EditorGUILayout.BeginVertical(GameEditorStyle.boxBackground);
                            {
                                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label("Module");
                                    BundleData bundleData = bundleList[i];
                                    bundleData.module = GUILayout.TextField(bundleData.module, "TextFieldDropDownText", GUILayout.Width(200));

                                    Rect last = GUILayoutUtility.GetLastRect();
                                    if ((Event.current.type == EventType.MouseDown && !last.Contains(Event.current.mousePosition))
                                        || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.None))
                                    {
                                        if (!moduleNames.Contains(bundleData.module))
                                        {
                                            moduleNames.Add(bundleData.module);
                                        }
                                        GUI.FocusControl("");
                                        window.Repaint();
                                    }
                                    GUILayout.Space(-3);
                                    if (GUILayout.Button("", "TextFieldDropDown"))
                                    {
                                        GenericMenu generic = new GenericMenu();
                                        for (int j = 0; j < moduleNames.Count; j++)
                                        {
                                            string name = moduleNames[j];
                                            generic.AddItem(new GUIContent(name), name == bundleData.module, () =>
                                            {
                                                bundleData.module = name;
                                            });
                                        }
                                        generic.ShowAsContext();
                                    }

                                    GUILayout.Space(2);
                                    GUILayout.EndHorizontal();
                                }
                                DrawBundleDataListGUI(bundleList[i], window);
                                if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                    foreach (var item in DragAndDrop.paths)
                                    {
                                        GetFileList(item, bundleList[i]);
                                    }
                                    DragAndDrop.AcceptDrag();
                                    Event.current.Use();
                                    window.Repaint();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            DrawBottomButtonGUI(bundleList[i], window);
                        }
                    }
                }
                if (Event.current.type == EventType.DragPerform && backgroundRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    foreach (var item in DragAndDrop.paths)
                    {
                        if (Path.HasExtension(item))
                        {
                            continue;
                        }
                        string fileName = Path.GetFileName(item);
                        if (ResourceEditorScript.IsHaveBundleData(fileName))
                        {
                            Debug.LogErrorFormat("重复资源包：", fileName);
                            continue;
                        }
                        BundleData bundleData = new BundleData() { name = (fileName + AppConfig.BUNDLE_EXTENSION).ToLower(), IsApk = IsPackaged };
                        GetFileList(item, bundleData);
                        bundleList.Add(bundleData);
                    }
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                    window.Repaint();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBottomButtonGUI(BundleData bundleData, EditorWindow window)
        {
            GUILayout.Space(-1);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Box("", GameEditorStyle.boxBackground, GUILayout.Width(60), GUILayout.Height(20));
                GUILayout.Space(-50);
                if (GUILayout.Button(GameEditorStyle.iconToolbarPlus, GameEditorStyle.preButton))
                {

                }
                GUILayout.Space(10);
                if (GUILayout.Button(GameEditorStyle.iconToolbarMinus, GameEditorStyle.preButton))
                {
                    if (seletion != null)
                    {
                        bundleData.Remove(seletion.name);
                    }
                    else
                    {
                        AssetData last = bundleData.Last();
                        if (last != null)
                        {
                            bundleData.Remove(last.name);
                        }
                    }
                }
                GUILayout.Space(30);
                GUILayout.EndHorizontal();
            }
        }

        private void DrawBundleDataListGUI(BundleData bundleData, EditorWindow window)
        {
            for (int i = 0; i < bundleData.Count; i++)
            {
                Rect rect = EditorGUILayout.BeginHorizontal(seletion != null && bundleData.Contains(seletion) && seletion.Equals(bundleData[i]) ? GameEditorStyle.selectionItem : GUIStyle.none);
                {
                    GUILayout.Space(5);
                    GUILayout.Label(bundleData[i].name);
                    GUILayout.Label(bundleData[i].path);
                    if (!objectList.TryGetValue(bundleData[i].name, out Object assetObject))
                    {
                        assetObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(bundleData[i].guid), typeof(Object));
                        objectList.Add(bundleData[i].name, assetObject);
                    }
                    objectList[bundleData[i].name] = EditorGUILayout.ObjectField("", assetObject, typeof(Object), false);
                    GUILayout.Space(5);
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (rect.Contains(Event.current.mousePosition))
                        {
                            seletion = bundleData[i];
                            window.Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public void GetFileList(string path, BundleData bundle)
        {
            if (Path.HasExtension(path))
            {
                if (path.EndsWith(".meta") || path.EndsWith(".cs"))
                {
                    return;
                }
                if (!path.StartsWith("Assets"))
                {
                    path = path.Replace(Application.dataPath, "Assets");
                }
                string assetName = Path.GetFileNameWithoutExtension(path);
                if (ResourceEditorScript.IsHaveAssetData(assetName))
                {
                    Debug.LogErrorFormat("存在重复资源：{0}", assetName);
                    return;
                }
                bundle.Add(new AssetData()
                {
                    name = assetName,
                    guid = AssetDatabase.AssetPathToGUID(path),
                    path = path.Replace("\\", "/")
                });
                return;
            }
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            path = Application.dataPath.Replace("Assets", "") + path;
            Debug.Log(path);
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                GetFileList(file, bundle);
            }
        }

        public bool CheckoutSeletion(string name)
        {
            if (selectionBundles.TryGetValue(name, out bool state))
            {
                return state;
            }
            return false;
        }

        public List<BundleData> GetSelectionBundleData()
        {
            List<BundleData> datas = new List<BundleData>();
            foreach (var item in selectionBundles)
            {
                if (item.Value)
                {
                    datas.Add(bundleList.GetBundleData(item.Key));
                }
            }
            return datas;
        }

        public void Remove()
        {
            List<BundleData> selections = GetSelectionBundleData();
            if (selections.Count <= 0)
            {
                bundleList.Remove(bundleList.Last());
            }
            else
            {
                foreach (var item in selections)
                {
                    bundleList.Remove(item);
                }
            }
        }
    }
}