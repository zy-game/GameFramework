using UnityEditor;
using UnityEngine;
using GameFramework.Resource;

namespace GameFramework.Editor.ResoueceEditor
{
    public class ResourceGenerateEditorScript : EditorWindow
    {
        private BundleList currentResourceDetailedData;

        public void OnEnable()
        {
            if (currentResourceDetailedData == null)
            {
                //todo 加载资源列表
            }
        }

        public void OnGUI()
        {
            
        }
    }
}