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
namespace GameFramework.Editor.ResoueceEditor
{
    public class ResourceGenerateEditorScript : EditorWindow
    {
        class EditData
        {
            public bool foldout;
            public bool edit;
            public bool selection;
            public Dictionary<string, Object> objects = new Dictionary<string, Object>();
        }
        private string searchText;
        private Vector2 localBundleListPosition;
        private Vector2 hotfixBundleListPosition;
        private BundleList currentLocalData;
        private BundleList currentHotfixData;
        private const string HOTFIX_RESOURCE_PATH_NAME = "resource_editor_path";
        private const string HOTFIX_ASSET_BUILD_OUTPUT_PATH = "asset_build_output_path";

        private Dictionary<int, EditData> editDatas = new Dictionary<int, EditData>();
        private Dictionary<string, bool> showDataList = new Dictionary<string, bool>();
        private List<string> blackList = new List<string>() { ".meta", ".cs" };
        public async void OnEnable()
        {
            if (currentHotfixData != null)
            {
                return;
            }
            editDatas = new Dictionary<int, EditData>();

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            string localSettingPath = Path.Combine(Application.streamingAssetsPath, "hotfix", Runtime.BASIC_FILE_LIST_NAME);
            if (!Directory.Exists(Path.GetDirectoryName(localSettingPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localSettingPath));
            }
            if (!File.Exists(localSettingPath))
            {
                File.WriteAllText(localSettingPath, "{}");
                AssetDatabase.Refresh();
            }
            currentLocalData = BundleList.Generate(File.ReadAllText(localSettingPath));


            //todo 加载资源列表
            string dataPath = EditorPrefs.GetString(HOTFIX_RESOURCE_PATH_NAME);
            if (string.IsNullOrEmpty(dataPath))
            {
                string defaultName = Path.GetFileNameWithoutExtension(Runtime.HOTFIX_FILE_LIST_NAME);
                string extension = Path.GetExtension(Runtime.HOTFIX_FILE_LIST_NAME).Replace(".", "");
                dataPath = EditorUtility.SaveFilePanel("选择资源配置保存路径", Application.dataPath, defaultName, extension);
                if (string.IsNullOrEmpty(dataPath))
                {
                    await Task.Delay(1);
                    this.Close();
                    return;
                }
                EditorPrefs.SetString(HOTFIX_RESOURCE_PATH_NAME, dataPath);
            }
            if (!File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, string.Empty);
                AssetDatabase.Refresh();
            }
            string data = File.ReadAllText(dataPath);
            if (string.IsNullOrEmpty(data))
            {
                currentHotfixData = BundleList.Generate("{}");
            }
            else
            {
                currentHotfixData = CatJson.JsonParser.ParseJson<BundleList>(data);
            }
        }

        public void OnGUI()
        {
            if (currentHotfixData == null)
            {
                return;
            }
            OnDrawBundleList();
        }

        private void DrawToolbarMeun()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.FlexibleSpace();


                GUILayout.EndHorizontal();
            }
        }

        private void OnDrawSearchToolbar()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField);
                Rect layoutRect = GUILayoutUtility.GetLastRect();
                if ((Event.current.type == EventType.MouseDown && !layoutRect.Contains(Event.current.mousePosition)) || Event.current.keyCode == KeyCode.Return)
                {
                    GUI.FocusControl(null);
                    Repaint();
                }
                GUILayout.EndHorizontal();
            }
        }
        private bool localResourceGroupFoldout;
        private bool hotfixResourceGroupFoldout;
        private void OnDrawBundleList()
        {

            (localBundleListPosition, localResourceGroupFoldout) = OnDrawBundleGroup("Local Resource Group", false, localBundleListPosition, localResourceGroupFoldout, currentLocalData);
            hotfixResourceGroupFoldout = !localResourceGroupFoldout;
            (hotfixBundleListPosition, hotfixResourceGroupFoldout) = OnDrawBundleGroup("Hotfix Resource Group", true, hotfixBundleListPosition, hotfixResourceGroupFoldout, currentHotfixData);
            localResourceGroupFoldout = !hotfixResourceGroupFoldout;
        }

        private (Vector2, bool) OnDrawBundleGroup(string groupName, bool isLocal, Vector2 scroolPosition, bool foldout, BundleList list)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                {
                    foldout = EditorGUILayout.Foldout(foldout, groupName);
                    GUILayout.Space(90);
                    if (GUILayout.Button("+", EditorStyles.toolbarDropDown))
                    {
                        list.Add(new BundleData() { name = "bundle " + list.bundles.Count + ".assetbundle" });
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("All", EditorStyles.toolbarButton))
                    {
                        foreach (var item in list.bundles)
                        {
                            if (editDatas.TryGetValue(item.GetHashCode(), out EditData editData))
                            {
                                editData.selection = true;
                            }
                        }
                    }

                    if (GUILayout.Button("None", EditorStyles.toolbarButton))
                    {
                        foreach (var item in list.bundles)
                        {
                            if (editDatas.TryGetValue(item.GetHashCode(), out EditData editData))
                            {
                                editData.selection = false;
                            }
                        }
                    }

                    if (GUILayout.Button("Invert", EditorStyles.toolbarButton))
                    {
                        foreach (var item in list.bundles)
                        {
                            if (editDatas.TryGetValue(item.GetHashCode(), out EditData editData))
                            {
                                editData.selection = !editData.selection;
                            }
                        }
                    }
                    if (GUILayout.Button("Delete", EditorStyles.toolbarButton))
                    {
                        for (int i = list.bundles.Count - 1; i >= 0; i--)
                        {
                            BundleData assetData = list.bundles[i];
                            if (editDatas.TryGetValue(assetData.GetHashCode(), out EditData editData))
                            {
                                if (editData.selection)
                                {
                                    editDatas.Remove(assetData.GetHashCode());
                                    list.Remove(assetData.name);
                                }
                            }
                        }
                    }

                    if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                    {
                        SaveBundleList();
                    }

                    if (GUILayout.Button("Generate", EditorStyles.toolbarDropDown))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Build All"), false, () => { BuilAssetPackaged(list, true, isLocal); });
                        menu.AddItem(new GUIContent("Build Seletion"), false, () => { BuilAssetPackaged(list, false, isLocal); });
                        menu.ShowAsContext();
                    }
                    GUILayout.EndHorizontal();
                }

                if (foldout)
                {
                    OnDrawSearchToolbar();
                    scroolPosition = GUILayout.BeginScrollView(scroolPosition);
                    {
                        for (var i = 0; i < list.bundles.Count; i++)
                        {
                            BundleData bundleData = list.bundles[i];
                            OnDrawBundleData(list, bundleData);
                        }
                        GUILayout.EndScrollView();
                    }
                }

                GUILayout.EndVertical();
            }
            return (scroolPosition, foldout);
        }

        private void SaveBundleList()
        {
            string localSettingPath = Path.Combine(Application.streamingAssetsPath, "hotfix", Runtime.BASIC_FILE_LIST_NAME);
            if (!Directory.Exists(Path.GetDirectoryName(localSettingPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localSettingPath));
            }
            File.WriteAllText(localSettingPath, currentLocalData.ToString());

            string dataPath = EditorPrefs.GetString(HOTFIX_RESOURCE_PATH_NAME);
            if (string.IsNullOrEmpty(dataPath))
            {
                string defaultName = Path.GetFileNameWithoutExtension(Runtime.HOTFIX_FILE_LIST_NAME);
                string extension = Path.GetExtension(Runtime.HOTFIX_FILE_LIST_NAME);
                dataPath = EditorUtility.SaveFilePanel("选择资源配置保存路径", Application.dataPath, defaultName, extension);
                if (string.IsNullOrEmpty(dataPath))
                {
                    return;
                }
                EditorPrefs.SetString(HOTFIX_RESOURCE_PATH_NAME, dataPath);
            }
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
            File.WriteAllText(dataPath, currentHotfixData.ToString());
        }

        private void BuilAssetPackaged(BundleList bundleList, bool isAll, bool isHotfix)
        {
            string outputPath = string.Empty;
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            if (isHotfix)
            {
                outputPath = EditorPrefs.GetString(HOTFIX_ASSET_BUILD_OUTPUT_PATH);
                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = EditorUtility.OpenFolderPanel("选择保存路径", Application.dataPath, "");
                }
            }
            else
            {
                outputPath = Path.Combine(Application.streamingAssetsPath, "hotfix");

            }
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            List<BundleData> builds = new List<BundleData>();
            string deletePath = string.Empty;
            foreach (BundleData bundleData in bundleList.bundles)
            {
                if (editDatas.TryGetValue(bundleData.GetHashCode(), out EditData editData))
                {
                    if (isAll || editData.selection)
                    {
                        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                        assetBundleBuild.assetBundleName = bundleData.name;
                        assetBundleBuild.assetNames = new string[bundleData.assets.Count];
                        assetBundleBuilds.Add(assetBundleBuild);
                        int index = 0;
                        foreach (AssetData assetData in bundleData.assets)
                        {
                            assetBundleBuild.assetNames[index++] = AssetDatabase.GUIDToAssetPath(assetData.guid);
                        }
                        builds.Add(bundleData);
                        DeleteFile(Path.Combine(outputPath, bundleData.name));
                        DeleteFile(Path.Combine(outputPath, bundleData.name + ".meta"));
                        DeleteFile(Path.Combine(outputPath, bundleData.name + ".manifest"));
                        DeleteFile(Path.Combine(outputPath, bundleData.name + ".manifest.meta"));
                    }
                }
            }
            AssetDatabase.Refresh();

#if UNITY_STANDALONE
            BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#endif
#if UNITY_ANDROID
            BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);
#endif
#if UNITY_IPHONE
            BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.iOS);
#endif
            AssetDatabase.Refresh();
            byte[] bytes = null;
            foreach (BundleData bundleBuild in builds)
            {
                string filePath = Path.Combine(outputPath, bundleBuild.name);
                if (!File.Exists(filePath))
                {
                    Debug.Log("not find file:" + filePath);
                    continue;
                }
                bundleBuild.version++;
                bundleBuild.owner = SystemInfo.deviceName;
                bundleBuild.time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                bytes = File.ReadAllBytes(filePath);
                bundleBuild.crc32 = CRC32.GetCRC32Byte(bytes, bundleBuild.version);
            }
            SaveBundleList();
            DirectoryInfo directoryInfo = new DirectoryInfo(outputPath);
            DeleteFile(Path.Combine(outputPath, directoryInfo.Name));
            DeleteFile(Path.Combine(outputPath, directoryInfo.Name + ".meta"));
            DeleteFile(Path.Combine(outputPath, directoryInfo.Name + ".manifest"));
            DeleteFile(Path.Combine(outputPath, directoryInfo.Name + ".manifest.meta"));
            AssetDatabase.Refresh();
        }

        private void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void OnDrawBundleData(BundleList list, BundleData bundleData)
        {
            Rect layoutRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                string search = searchText?.ToLower();
                if (!string.IsNullOrEmpty(searchText) && !bundleData.name.ToLower().Contains(search) && bundleData.assets.Find(x => x.name.ToLower().Contains(search)) == null)
                {
                    return;
                }
                if (!editDatas.TryGetValue(bundleData.GetHashCode(), out EditData editData))
                {
                    editDatas.Add(bundleData.GetHashCode(), editData = new EditData());
                }
                OnDrawFoldoutGUI(bundleData, editData);
                OnDrawBundleAssetList(bundleData, editData);
                if (Event.current.type == EventType.DragUpdated && layoutRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                if (Event.current.type == EventType.DragPerform)
                {
                    if (layoutRect.Contains(Event.current.mousePosition))
                    {
                        foreach (var item in DragAndDrop.paths)
                        {
                            GetFileList(item, bundleData);
                        }
                        DragAndDrop.AcceptDrag();
                    }
                }
                EditorGUILayout.EndVertical();
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
                string assetPath = path.Replace(Application.dataPath, "");
                bundle.Add(new AssetData()
                {
                    name = Path.GetFileNameWithoutExtension(path),
                    guid = AssetDatabase.AssetPathToGUID(assetPath),
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

        private void OnDrawBundleAssetList(BundleData bundleData, EditData editData)
        {
            if (!editData.foldout)
            {
                return;
            }
            for (var i = 0; i < bundleData.assets.Count; i++)
            {
                AssetData assetData = bundleData.assets[i];
                string search = searchText?.ToLower();
                if (!string.IsNullOrEmpty(searchText) && !assetData.name.ToLower().Contains(search))
                {
                    continue;
                }
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
                        assetData.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(editData.objects[assetData.name]));
                    }
                    GUILayout.Label(AssetDatabase.GUIDToAssetPath(assetData.guid));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-"))
                    {
                        bundleData.Remove(assetData.name);
                        Repaint();
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        private Rect OnDrawFoldoutGUI(BundleData bundleData, EditData editData)
        {
            Rect layoutRect = EditorGUILayout.BeginHorizontal();
            {
                editData.selection = GUILayout.Toggle(editData.selection, "", GUILayout.Width(18));
                if (editData.edit)
                {
                    bundleData.name = EditorGUILayout.TextField("", bundleData.name, GUILayout.Width(200));
                }
                else
                {
                    editData.foldout = EditorGUILayout.Foldout(editData.foldout, bundleData.name);
                }
                if (GUI.changed)
                {
                    if (!bundleData.name.EndsWith(".assetbundle"))
                    {
                        bundleData.name += ".assetbundle";
                    }
                }
                Rect rect = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.MouseDown)
                {
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        editData.edit = true;
                    }
                    else
                    {
                        editData.edit = false;
                    }
                }
            }
            GUILayout.EndHorizontal();
            return layoutRect;
        }
    }
}