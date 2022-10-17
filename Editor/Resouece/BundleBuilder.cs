using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameFramework.Resource;

namespace GameFramework.Editor.ResoueceEditor
{
    class BundleBuilder : IBundleBuilder
    {
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
            BundleList bundle = string.IsNullOrEmpty(bundleListData) ? new BundleList() : BundleList.Generate(bundleListData);
            bundleListGUI = new BundleListGUI(bundle, bundleType == BundleType.ResourcesAssets);
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
                    if (GUILayout.Button("Remove", EditorStyles.toolbarButton))
                    {
                        bundleListGUI.Remove();
                        window.Repaint();
                    }
                    if (GUILayout.Button("Build", EditorStyles.toolbarDropDown))
                    {
                        GenericMenu generic = new GenericMenu();
                        generic.AddItem(new GUIContent("Build All"), false, () => { Build(true); });
                        generic.AddItem(new GUIContent("Build Selete"), false, () => { Build(false); });
                        generic.ShowAsContext();
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
            File.WriteAllText(savePath, bundleListGUI.bundleList.ToString());
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
                filePath = AppConfig.PACKAGED_FILE_PATH + AppConfig.HOTFIX_FILE_LIST_NAME + ".txt";
            }
            return filePath;
        }

        protected virtual void Build(bool isAll)
        {
            string outputPath = string.Empty;
            switch (bundleType)
            {
                case BundleType.StreamingAssets:
                case BundleType.HotfixAssets:
                    outputPath = bundleType == BundleType.HotfixAssets ? AppConfig.HOTFIX_FILE_PATH : AppConfig.STREAMING_FILE_PATH;
                    List<AssetBundleBuild> bundleBuilds = new List<AssetBundleBuild>();
                    for (int i = 0; i < bundleListGUI.bundleList.Count; i++)
                    {
                        if (isAll || this.bundleListGUI.CheckoutSeletion(bundleListGUI.bundleList[i].name))
                        {
                            bundleBuilds.Add(new AssetBundleBuild()
                            {
                                assetBundleName = bundleListGUI.bundleList[i].name + AppConfig.BUNDLE_EXTENSION,
                                assetNames = bundleListGUI.bundleList[i].Paths
                            });
                        }
                    }
                    if (bundleBuilds.Count > 0)
                    {

                        BuildPipeline.BuildAssetBundles(outputPath, bundleBuilds.ToArray(), BuildAssetBundleOptions.None,
#if UNITY_ANDROID
                            BuildTarget.Android
#elif UNITY_IPHONE
                            BuildTarget.iOS
#else
                        BuildTarget.StandaloneWindows
#endif
                        );
                        Delete(Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath)));
                        Delete(Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".meta"));
                        Delete(Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".manifest"));
                        Delete(Path.Combine(outputPath, Utilty.GetLastDirectory(outputPath) + ".manifest.meta"));
                    }

                    break;
                case BundleType.ResourcesAssets:
                    //todo check asset not exsit form resource directory
                    //todo copy asset to resource directory
                    for (int i = 0; i < bundleListGUI.bundleList.Count; i++)
                    {
                        if (isAll || this.bundleListGUI.CheckoutSeletion(bundleListGUI.bundleList[i].name))
                        {
                            for (int j = 0; j < bundleListGUI.bundleList[i].Count; j++)
                            {
                                AssetDatabase.CopyAsset(bundleListGUI.bundleList[i][j].path, "Assets/Resources/files/" + Path.GetFileName(bundleListGUI.bundleList[i][j].path));
                            }
                        }
                    }
                    break;
            }
            AssetDatabase.Refresh();
        }

        private void Delete(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            File.Delete(path);
        }

        protected virtual void Clear()
        {
            bundleListGUI.bundleList.Clear();
        }

        public bool ContainsBundle(string name)
        {
            return bundleListGUI.bundleList.Contains(name);
        }

        public bool ContainsAsset(string name)
        {
            return bundleListGUI.bundleList.GetBundleDataWithAsset(name) != null;
        }
    }
}