using UnityEditor;
using UnityEngine;

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
