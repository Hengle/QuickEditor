namespace QuickEditor.Core
{
    using UnityEditor;
    using UnityEngine;

    public class QuickScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T mSetting;

        public static T Current
        {
            get
            {
                if (mSetting == null)
                {
                    mSetting = QuickEditorAssetStaticAPI.LoadOrCreateAssetFromFindAssets<T>(false);
                }
                return mSetting;
            }
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
    }
}
