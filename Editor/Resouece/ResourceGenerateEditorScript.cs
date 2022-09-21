using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameFramework.Resource;

namespace GameFramework.Editor.ResoueceEditor
{
    public class ResourceGenerateEditorScript : EditorWindow
    {
        class EditData
        {
            public bool foldout;
            public bool edit;
            public bool selection;
            public Dictionary<string, UnityEngine.Object> objects = new Dictionary<string, Object>();
        }
        private Vector2 bundleListPosition;
        private string searchText;
        private Dictionary<string, bool> showDataList = new Dictionary<string, bool>();
        private BundleList currentResourceDetailedData;
        private Dictionary<int, EditData> editDatas;
        private List<string> blackList = new List<string>() { ".meta", ".cs" };
        public void OnEnable()
        {
            if (currentResourceDetailedData != null)
            {
                return;
            }
            editDatas = new Dictionary<int, EditData>();
            //todo 加载资源列表
            string dataPath = EditorPrefs.GetString("resource_editor_path");
            if (string.IsNullOrEmpty(dataPath))
            {
                dataPath = EditorUtility.SaveFilePanel("选择资源配置保存路径", EditorPrefs.GetString("resource_editor_path", Application.dataPath), "BundleList", "ini");
                EditorPrefs.SetString("resource_editor_path", dataPath);
            }
            if (!File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, string.Empty);
            }
            string data = File.ReadAllText(dataPath);
            if (string.IsNullOrEmpty(data))
            {
                currentResourceDetailedData = new BundleList();
            }
            else
            {
                currentResourceDetailedData = CatJson.JsonParser.ParseJson<BundleList>(data);
            }
        }

        public void OnGUI()
        {
            if (currentResourceDetailedData == null)
            {
                return;
            }
            DrawToolbarMeun();
            OnDrawSearchToolbar();
            OnDrawBundleList();
        }

        private void DrawToolbarMeun()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("+", EditorStyles.toolbarDropDown))
                {
                    currentResourceDetailedData.Add(new BundleData() { name = "bundle " + currentResourceDetailedData.bundles.Count });
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Generate", EditorStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Build All"), false, () => { });
                    menu.AddItem(new GUIContent("Build Seletion"), false, () => { });
                    menu.ShowAsContext();
                }
                GUILayout.EndHorizontal();
            }
        }

        private void OnDrawSearchToolbar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField);
                if (Event.current.type == EventType.MouseDown)
                {
                    GUI.FocusControl(null);
                    Repaint();
                }
                GUILayout.EndHorizontal();
            }
        }

        private void OnDrawBundleList()
        {
            if (currentResourceDetailedData == null || currentResourceDetailedData.bundles.Count <= 0)
            {
                return;
            }
            bundleListPosition = GUILayout.BeginScrollView(bundleListPosition);
            {
                GUILayout.BeginVertical();
                for (int i = currentResourceDetailedData.bundles.Count - 1; i >= 0; i--)
                {
                    BundleData bundleData = currentResourceDetailedData.bundles[i];
                    OnDrawBundleData(bundleData);
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }

        private BundleData seletion;

        private void OnDrawBundleData(BundleData bundleData)
        {
            if (!editDatas.TryGetValue(bundleData.GetHashCode(), out EditData editData))
            {
                editData = new EditData();
                editDatas.Add(bundleData.GetHashCode(), editData);
            }
            Rect layoutRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                OnDrawFoldoutGUI(ref bundleData.name, ref editData.foldout, ref editData.edit, ref editData.selection);
                if (editData.foldout)
                {
                    for (int i = bundleData.assets.Count - 1; i >= 0; i--)
                    {
                        OnDrawBundleAssetList(bundleData, editData, bundleData.assets[i]);
                    }
                }
                if (Event.current.type == EventType.DragUpdated)
                {
                    if (layoutRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    }
                }
                if (Event.current.type == EventType.DragPerform && layoutRect.Contains(Event.current.mousePosition))
                {
                    foreach (var item in DragAndDrop.paths)
                    {
                        GetFileList(item, bundleData);
                    }
                    DragAndDrop.AcceptDrag();
                    Repaint();
                }
                if (Event.current.type == EventType.MouseDown)
                {
                    if (layoutRect.Contains(Event.current.mousePosition))
                    {
                        seletion = bundleData;
                        Debug.Log("---------" + bundleData.name);
                        Event.current.Use();
                    }
                    else
                    {
                        seletion = null;
                        Debug.Log("---------");
                    }
                }

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete && seletion != null)
                {
                    currentResourceDetailedData.Remove(bundleData.name);
                    bundleData = null;
                    Event.current.Use();
                }
                GUILayout.EndVertical();
            }
        }

        public void GetFileList(string path, BundleData bundle)
        {
            string extension = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(extension))
            {
                if (blackList.Contains(extension))
                {
                    return;
                }
                bundle.assets.Add(new AssetData()
                {
                    name = Path.GetFileNameWithoutExtension(path),
                    guid = AssetDatabase.AssetPathToGUID("Assets" + path.Replace(Application.dataPath, "")),
                });
                return;
            }
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            path = Application.dataPath.Replace("Assets", "") + path;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                GetFileList(file, bundle);
            }
        }

        private void OnDrawBundleAssetList(BundleData bundleData, EditData editData, AssetData assetData)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(assetData.name);
                if (!editData.objects.TryGetValue(assetData.name, out Object target))
                {
                    if (!string.IsNullOrEmpty(assetData.guid))
                    {
                        target = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetData.guid), typeof(UnityEngine.Object));
                    }
                    editData.objects.Add(assetData.name, target);
                }

                editData.objects[assetData.name] = EditorGUILayout.ObjectField(editData.objects[assetData.name], typeof(UnityEngine.Object), false);
                if (GUI.changed)
                {
                    assetData.name = editData.objects[assetData.name].name;
                    assetData.guid = AssetDatabase.GetAssetPath(editData.objects[assetData.name]);
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    bundleData.Remove(assetData.name);
                }
                GUILayout.EndHorizontal();
            }
        }

        public void OnDrawFoldoutGUI(ref string name, ref bool foldout, ref bool edit, ref bool selection)
        {
            Rect click = EditorGUILayout.BeginHorizontal();
            {
                selection = GUILayout.Toggle(selection, "", GUILayout.Width(18));
                GUIContent content = new GUIContent(name);
                if (edit)
                {
                    name = EditorGUILayout.TextField("", name, GUILayout.Width(200));
                }
                else
                {
                    foldout = EditorGUILayout.Foldout(foldout, content);
                }
                Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.foldoutHeader, GUILayout.Width(600));
                if (Event.current.type == EventType.MouseDown)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        edit = true;
                    }
                    else
                    {
                        edit = false;
                    }
                    Repaint();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }
}