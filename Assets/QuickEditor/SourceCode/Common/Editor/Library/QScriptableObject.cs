/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Common
{
    using UnityEngine;

    public class QScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T mSetting;

        public static T Current
        {
            get
            {
                if (mSetting == null)
                {
                    mSetting = QEditorAssetStaticAPI.LoadOrCreateAssetFromFindAssets<T>(false);
                }
                return mSetting;
            }
        }
    }
}