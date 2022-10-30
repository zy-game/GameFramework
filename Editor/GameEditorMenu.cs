using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFramework.Game;

namespace GameFramework.Editor
{
    public class GameEditorMenu
    {
        [MenuItem("GameFramework/Resouce Generate %`")]
        public static void OpenResouceGenerateWindow()
        {
            EditorWindow.GetWindow(typeof(ResoueceEditor.ResourceEditorScript), false, "Resource Generate", true);
        }

        [MenuItem("GameFramework/Game Map Generate %`")]
        public static void OpenMapGenerateWindow()
        {
            EditorWindow.GetWindow(typeof(MapEditor.MapEditorWindow), false, "Map Generate", true);
        }

        [MenuItem("GameFramework/Clear EditorPrefs")]
        public static void ClearEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }

        [MenuItem("GameFramework/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}

namespace GameFramework.Editor.MapEditor
{
    public sealed class MapEditorWindow : EditorWindow
    {
        private void OnEnable()
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("");
            
        }

        private void OnGUI()
        {
            
        }
    }
}
