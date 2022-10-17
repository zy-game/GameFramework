using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.ResoueceEditor
{
    public class ResourceEditorScript : EditorWindow
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
        public static bool IsHaveAssetData(string assetNamme)
        {
            return resourceBundleBuilder.ContainsAsset(assetNamme) || hotfixResourceBundleBuilder.ContainsAsset(assetNamme) || streamingResourceBundleBuilder.ContainsAsset(assetNamme);
        }

        public static bool IsHaveBundleData(string bundleName)
        {
            return resourceBundleBuilder.ContainsBundle(bundleName) || hotfixResourceBundleBuilder.ContainsBundle(bundleName) || streamingResourceBundleBuilder.ContainsBundle(bundleName);
        }
    }
}