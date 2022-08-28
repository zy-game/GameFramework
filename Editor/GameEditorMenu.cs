using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GameFramework.Editor
{
    public class GameEditorMenu
    {
        [MenuItem("GameFramework/Resouce Generate %1")]
        public static void OpenResouceGenerateWindow()
        {
            EditorWindow.GetWindow(typeof(ResoueceEditor.ResourceGenerateEditorScript), false, "Resource Generate", true);
        }
    }

}
