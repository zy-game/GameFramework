using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GameFramework.Editor
{
    public class GameEditorMenu
    {
        [MenuItem("GameFramework/Resouce Generate %`")]
        public static void OpenResouceGenerateWindow()
        {
            EditorWindow.GetWindow(typeof(ResoueceEditor.ResourceGenerateEditorScript), false, "Resource Generate", true);
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
