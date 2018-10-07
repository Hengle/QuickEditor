namespace QuickEditor.NGUIToolKit
{
    using QuickEditor.Common;
    using UnityEngine;

    public class TexturePackerSetting : ScriptableObject
    {
        public string TPCommand;

        public bool IsSeperateRGBandAlpha = true;

        internal static TexturePackerSetting settings;

        public static TexturePackerSetting Current
        {
            get
            {
                if (settings == null)
                {
                    settings = QEditorAssetStaticAPI.LoadOrCreateAssetFromFindAssets<TexturePackerSetting>(false);
                }
                return settings;
            }
        }
    }
}