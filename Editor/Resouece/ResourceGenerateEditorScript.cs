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

namespace GameFramework.Editor.ResoueceEditor
{

    public class GUIList<T>
    {
        public void OnGui()
        {

        }
    }
    public class ResourceGenerateEditorScript : EditorWindow
    {
        ResourceBundleBuilder resourceBundleBuilder;
        HotfixBundleBuilder hotfixResourceBundleBuilder;
        StreamingBundleBuilder streamingResourceBundleBuilder;


        private void OnEnable()
        {
            resourceBundleBuilder = new ResourceBundleBuilder();
            resourceBundleBuilder.showCallback += state =>
            {
                if (!state)
                {
                    return;
                }
                hotfixResourceBundleBuilder.active = !state;
                streamingResourceBundleBuilder.active = !state;
            };
            hotfixResourceBundleBuilder = new HotfixBundleBuilder();
            hotfixResourceBundleBuilder.showCallback += state =>
            {
                if (!state)
                {
                    return;
                }
                resourceBundleBuilder.active = !state;
                streamingResourceBundleBuilder.active = !state;
            };
            streamingResourceBundleBuilder = new StreamingBundleBuilder();
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

            resourceBundleBuilder.OnGui();
            hotfixResourceBundleBuilder.OnGui();
            streamingResourceBundleBuilder.OnGui();
        }

        public interface IBundleBuilder : IRefrence
        {
            string name { get; }
            bool active { get; set; }
            event GameFrameworkAction<bool> showCallback;
            void OnGui();
        }

        abstract class AbstractBundleBuilder : IBundleBuilder
        {
            private Vector2 position;
            public abstract string name { get; }
            public bool active { get; set; }

            public event GameFrameworkAction<bool> showCallback;

            private bool apkBundle;
            protected BundleList bundle;
            protected string dicRoot = Application.streamingAssetsPath;
            protected Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

            public AbstractBundleBuilder(bool isApk)
            {
                apkBundle = isApk;
            }

            public void OnGui()
            {
                Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    {
                        active = EditorGUILayout.Foldout(active, name);
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
                        position = GUILayout.BeginScrollView(position);
                        {
                            OnGUI();
                            GUILayout.EndScrollView();
                        }

                        if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        }
                        if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
                        {
                            for (int i = 0; i < DragAndDrop.paths.Length; i++)
                            {
                                if (Path.HasExtension(DragAndDrop.paths[i]))
                                {
                                    continue;
                                }
                                AddPackage(DragAndDrop.paths[i]);
                            }
                            DragAndDrop.AcceptDrag();
                            Event.current.Use();
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

            }

            protected virtual void Build()
            {
            }

            protected virtual void Clear()
            {
            }


            protected void OnGUI()
            {
                for (int i = bundle.Count - 1; i >= 0; i--)
                {
                    BundleData bundleData = bundle[i];
                    Rect rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        if (!foldouts.TryGetValue(bundleData.name, out bool state))
                        {
                            foldouts.Add(bundleData.name, false);
                        }
                        foldouts[bundleData.name] = EditorGUILayout.Foldout(state, bundleData.name);
                        if (foldouts[bundleData.name])
                        {
                            foreach (AssetData item in bundleData.assets)
                            {
                                Rect itemRect = EditorGUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(item.name, GUILayout.Width(200));
                                    GUILayout.Label(item.path, GUILayout.Width(400));
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                        if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
                        {
                            foreach (string seletionPath in DragAndDrop.paths)
                            {
                                if (!Path.HasExtension(seletionPath))
                                {
                                    continue;
                                }
                                AssetData assetData = Loader.Generate<AssetData>();
                                assetData.path = seletionPath;
                                assetData.name = Path.GetFileNameWithoutExtension(seletionPath);
                                assetData.guid = AssetDatabase.AssetPathToGUID(seletionPath);
                                bundleData.Add(assetData);
                            }
                            Event.current.Use();
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
            }
            protected void AddPackage(string path)
            {
                BundleData bundleData = Loader.Generate<BundleData>();
                DirectoryInfo directory = new DirectoryInfo(path);
                bundleData.name = directory.Name;
                bundleData.IsApk = apkBundle;
                bundleData.owner = SystemInfo.deviceName;
                bundleData.time = Utilty.GetTimeStampSeconds(DateTime.Now);
                bundleData.version = 0;
                bundleData.crc32 = 0;
                bundle.Add(bundleData);
            }

            protected void RemovePackage(string name)
            {
                bundle.Remove(name);
            }
        }

        sealed class ResourceBundleBuilder : AbstractBundleBuilder
        {
            public override string name => "ResourceBundle";
            public ResourceBundleBuilder() : base(true)
            {
                dicRoot = Application.dataPath + "/Resources";
                TextAsset text = Resources.Load<TextAsset>(AppConfig.HOTFIX_FILE_LIST_NAME);
                if (text == null)
                {
                    bundle = new BundleList();
                    return;
                }
                bundle = BundleList.Generate(text.text);
            }
            protected override void Save()
            {
                if (!Directory.Exists(dicRoot))
                {
                    Directory.CreateDirectory(dicRoot);
                }
                string data = bundle.ToString();
                File.WriteAllText(Path.Combine(dicRoot, AppConfig.HOTFIX_FILE_LIST_NAME + ".txt"), data);
                AssetDatabase.Refresh();
            }



            protected override void Build()
            {
                //todo 将资源不在resource文件夹的拷贝到resource文件夹中
                for (int i = bundle.Count - 1; i >= 0; i--)
                {
                    BundleData bundleData = bundle[i];
                    foreach (AssetData item in bundleData.assets)
                    {

                        string extension = Path.GetExtension(item.path);
                        string targetPath = "Assets/Resources/";
                        targetPath += extension switch
                        {
                            ".prefab" => "Prefab/" + Path.GetFileName(item.path),
                            ".mp3" => "Sound/" + Path.GetFileName(item.path),
                            ".wav" => "Sound/" + Path.GetFileName(item.path),
                            ".png" => "Texture/" + Path.GetFileName(item.path),
                            ".jpg" => "Texture/" + Path.GetFileName(item.path),
                            ".ttf" => "Font/" + Path.GetFileName(item.path),
                            ".mat" => "Material/" + Path.GetFileName(item.path),
                            _ => throw GameFrameworkException.Generate(""),
                        };
                        DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(targetPath));
                        if (!Directory.Exists(Application.dataPath + "/Resources/" + directory.Name))
                        {
                            AssetDatabase.CreateFolder("Assets/Resources", directory.Name);
                        }
                        if (!AssetDatabase.CopyAsset(item.path, targetPath))
                        {
                            //todo 拷贝失败，说明目标路径可能存在相同的资源名
                            Debug.Log("拷贝失败，说明目标路径可能存在相同的资源名:" + targetPath);
                        }
                    }
                }
                AssetDatabase.Refresh();
            }

            protected override void Clear()
            {
                bundle.Clear();
            }
        }

        sealed class StreamingBundleBuilder : AbstractBundleBuilder
        {
            public override string name => "StreamingBundle";

            public StreamingBundleBuilder() : base(false)
            {
                dicRoot = Application.streamingAssetsPath;
                string path = Path.Combine(dicRoot, AppConfig.HOTFIX_FILE_LIST_NAME);
                if (!File.Exists(path))
                {
                    bundle = new BundleList();
                    return;
                }
                string text = File.ReadAllText(path);
                if (string.IsNullOrEmpty(text))
                {
                    bundle = new BundleList();
                    return;
                }
                bundle = BundleList.Generate(text);
            }

            protected override void Save()
            {
                if (!Directory.Exists(dicRoot))
                {
                    Directory.CreateDirectory(dicRoot);
                }
                string data = bundle.ToString();
                File.WriteAllText(Path.Combine(dicRoot, AppConfig.HOTFIX_FILE_LIST_NAME), data);
                AssetDatabase.Refresh();
            }
        }

        sealed class HotfixBundleBuilder : AbstractBundleBuilder
        {
            public override string name => "HotfixBundle";
            public HotfixBundleBuilder() : base(false)
            {
                dicRoot = Application.dataPath + "/../hotfix";
                string path = Path.Combine(dicRoot, AppConfig.HOTFIX_FILE_LIST_NAME);
                if (!File.Exists(path))
                {
                    bundle = new BundleList();
                    return;
                }
                string text = File.ReadAllText(path);
                if (string.IsNullOrEmpty(text))
                {
                    bundle = new BundleList();
                    return;
                }
                bundle = BundleList.Generate(text);
            }

            protected override void Save()
            {
                if (!Directory.Exists(dicRoot))
                {
                    Directory.CreateDirectory(dicRoot);
                }
                string data = bundle.ToString();
                File.WriteAllText(Path.Combine(dicRoot, AppConfig.HOTFIX_FILE_LIST_NAME), data);
                AssetDatabase.Refresh();
            }
        }
    }
}