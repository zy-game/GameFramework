using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameFramework.Resource;
using System;
using Object = UnityEngine.Object;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEditorInternal;
using System.Linq;
using static PlasticGui.LaunchDiffParameters;
using NUnit.Framework;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using UnityEditor.VersionControl;
using UnityEngine.Assertions;
using static UnityEditor.ShaderData;

namespace GameFramework.Editor
{
    public sealed class GameEditorStyle
    {
        public static readonly string draggingHandle = "RL DragHandle";
        public static readonly string headerBackground = "RL Header";
        public static readonly string emptyHeaderBackground = "RL Empty Header";
        public static readonly string footerBackground = "RL Footer";
        public static readonly string boxBackground = "RL Background";
        public static readonly string preButton = "RL FooterButton";
        public static readonly string elementBackground = "RL Element";
        public static readonly string selectionItem = "SelectionRect";
        public static readonly GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to the list");
        public static readonly GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to the list");
        public static readonly GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from the list");
        public static readonly GUIContent s_ListIsEmpty = EditorGUIUtility.TrTextContent("List is Empty");
    }
}
namespace GameFramework.Editor.ResoueceEditor
{
    public class ResourceGenerateEditorScript : EditorWindow
    {
        static BundleBuilder resourceBundleBuilder;
        static BundleBuilder hotfixResourceBundleBuilder;
        static BundleBuilder streamingResourceBundleBuilder;


        private void OnEnable()
        {
            resourceBundleBuilder = new BundleBuilder(BundleType.ResourcesAssets, this);
            resourceBundleBuilder.showCallback += state =>
            {
                if (!state)
                {
                    return;
                }
                hotfixResourceBundleBuilder.active = !state;
                streamingResourceBundleBuilder.active = !state;
            };
            hotfixResourceBundleBuilder = new BundleBuilder(BundleType.HotfixAssets, this);
            hotfixResourceBundleBuilder.showCallback += state =>
            {
                if (!state)
                {
                    return;
                }
                resourceBundleBuilder.active = !state;
                streamingResourceBundleBuilder.active = !state;
            };
            streamingResourceBundleBuilder = new BundleBuilder(BundleType.StreamingAssets, this);
            streamingResourceBundleBuilder.showCallback += state =>
            {
                if (!state)
                {
                    return;
                }
                resourceBundleBuilder.active = !state;
                hotfixResourceBundleBuilder.active = !state;
            };
            resourceBundleBuilder.active = true;
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }
            resourceBundleBuilder.OnGUI(position);
            hotfixResourceBundleBuilder.OnGUI(position);
            streamingResourceBundleBuilder.OnGUI(position);
        }
        private static bool IsHaveAssetData(string assetNamme)
        {
            return resourceBundleBuilder.ContainsAsset(assetNamme) || hotfixResourceBundleBuilder.ContainsAsset(assetNamme) || streamingResourceBundleBuilder.ContainsAsset(assetNamme);
        }

        private static bool IsHaveBundleData(string bundleName)
        {
            return resourceBundleBuilder.ContainsBundle(bundleName) || hotfixResourceBundleBuilder.ContainsBundle(bundleName) || streamingResourceBundleBuilder.ContainsBundle(bundleName);
        }
        public interface IBundleBuilder : IRefrence
        {
            bool active { get; set; }
            event GameFrameworkAction<bool> showCallback;
            void OnGUI(Rect position);
            bool ContainsBundle(string name);
            bool ContainsAsset(string name);
        }

        public enum BundleType
        {
            HotfixAssets,
            StreamingAssets,
            ResourcesAssets,
        }
        class BundleListGUI
        {
            private static AssetData seletion;

            private BundleList bundleList;
            private Dictionary<string, Object> objectList;
            public BundleListGUI(BundleList bundleList)
            {
                this.bundleList = bundleList;
                objectList = new Dictionary<string, Object>();
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
                            GUILayout.BeginHorizontal(GameEditorStyle.headerBackground, GUILayout.Height(20));
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Space(100);
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.Space(-20);
                            GUILayout.Label(bundleList[i].name);
                            Rect rect = EditorGUILayout.BeginVertical(GameEditorStyle.boxBackground);
                            {
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
                            if (IsHaveBundleData(fileName))
                            {
                                Debug.LogErrorFormat("重复资源包：", fileName);
                                continue;
                            }
                            BundleData bundleData = new BundleData() { name = fileName };
                            GetFileList(item, bundleData);
                            bundleList.Add(bundleData);
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                        window.Repaint();
                    }
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
                    Rect rect = EditorGUILayout.BeginHorizontal(seletion != null && bundleData.Contains(seletion) ? GameEditorStyle.selectionItem : GUIStyle.none);
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
                    if (IsHaveAssetData(assetName))
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
        }

        class BundleBuilder : IBundleBuilder
        {
            private BundleList bundle;
            private Vector2 _position;
            private EditorWindow window;
            private BundleType bundleType;
            private BundleListGUI bundleListGUI;
            public event GameFrameworkAction<bool> showCallback;
            public bool active { get; set; }
            public BundleBuilder(BundleType bundleType, EditorWindow window)
            {
                this.window = window;
                this.bundleType = bundleType;
                string bundleListFilePath = GetBundleListPath();
                string dire = Path.GetDirectoryName(bundleListFilePath);
                if (!Directory.Exists(dire))
                {
                    Directory.CreateDirectory(dire);
                }
                string bundleListData = string.Empty;
                if (File.Exists(bundleListFilePath))
                {
                    bundleListData = File.ReadAllText(bundleListFilePath);
                }
                bundle = string.IsNullOrEmpty(bundleListData) ? new BundleList() : BundleList.Generate(bundleListData);
                bundleListGUI = new BundleListGUI(bundle);
            }
            public void OnGUI(Rect position)
            {
                Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    {
                        active = EditorGUILayout.Foldout(active, bundleType.ToString());
                        if (GUI.changed)
                        {
                            showCallback?.Invoke(active);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                        {
                            Clear();
                        }
                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            Save();
                        }
                        if (GUILayout.Button("Build", EditorStyles.toolbarButton))
                        {
                            Build();
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (active)
                    {
                        _position = GUILayout.BeginScrollView(_position, false, true);
                        {
                            bundleListGUI.OnGUI(this.window);
                            GUILayout.EndScrollView();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            public virtual void Release()
            {
            }

            protected virtual void Save()
            {
                string savePath = GetBundleListPath();
                if (File.Exists(Path.Combine(savePath)))
                {
                    File.Delete(savePath);
                }
                string dire = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(dire))
                {
                    Directory.CreateDirectory(dire);
                }
                File.WriteAllText(savePath, bundle.ToString());
                AssetDatabase.Refresh();
            }

            private string GetBundleListPath()
            {
                string filePath = string.Empty;
                if (bundleType == BundleType.HotfixAssets)
                {
                    filePath = AppConfig.HOTFIX_FILE_PATH + AppConfig.HOTFIX_FILE_LIST_NAME;
                }
                if (bundleType == BundleType.StreamingAssets)
                {
                    filePath = AppConfig.STREAMING_FILE_PATH + AppConfig.HOTFIX_FILE_LIST_NAME;
                }
                if (bundleType == BundleType.ResourcesAssets)
                {
                    filePath = AppConfig.PACKAGED_FILE_PATH + AppConfig.HOTFIX_FILE_LIST_NAME;
                }
                return filePath;
            }

            protected virtual void Build()
            {
                string outputPath = string.Empty;
                switch (bundleType)
                {
                    case BundleType.StreamingAssets:
                    case BundleType.HotfixAssets:
                        outputPath = bundleType == BundleType.HotfixAssets ? AppConfig.HOTFIX_FILE_PATH : AppConfig.STREAMING_FILE_PATH;
                        AssetBundleBuild[] bundleBuilds = new AssetBundleBuild[bundle.Count];
                        for (int i = 0; i < bundle.Count; i++)
                        {
                            bundleBuilds[i] = new AssetBundleBuild();
                            bundleBuilds[i].assetBundleName = bundle[i].name + AppConfig.BUNDLE_EXTENSION;
                            bundleBuilds[i].assetNames = bundle[i].Paths;
                        }

                        BuildPipeline.BuildAssetBundles(outputPath, bundleBuilds, BuildAssetBundleOptions.None,
#if UNITY_ANDROID
                        BuildTarget.Android
#elif UNITY_IPHONE
                        BuildTarget.iOS
#else
                        BuildTarget.StandaloneWindows
#endif
                            );
                        break;
                    case BundleType.ResourcesAssets:

                        break;
                }

                string maniBundle = Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath));
                string maniBundleMeta = Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".meta");
                string manifest = Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".manifest");
                string manifestmeta = Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".manifest.meta");
                if (File.Exists(manifest))
                {
                    File.Delete(manifest);
                }
                if (File.Exists(manifestmeta))
                {
                    File.Delete(manifestmeta);
                }

                if (File.Exists(maniBundle))
                {
                    File.Delete(maniBundle);
                }
                if (File.Exists(maniBundleMeta))
                {
                    File.Delete(maniBundleMeta);
                }
                AssetDatabase.Refresh();
            }

            protected virtual void Clear()
            {
                bundle.Clear();
            }

            public bool ContainsBundle(string name)
            {
                return bundle.Contains(name);
            }

            public bool ContainsAsset(string name)
            {
                return bundle.GetBundleDataWithAsset(name) != null;
            }
        }
    }
}