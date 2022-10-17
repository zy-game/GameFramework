using UnityEngine;

namespace GameFramework.Editor.ResoueceEditor
{
    public interface IBundleBuilder : IRefrence
    {
        bool active { get; set; }
        event GameFrameworkAction<bool> showCallback;
        void OnGUI(Rect position);
        bool ContainsBundle(string name);
        bool ContainsAsset(string name);
    }
}