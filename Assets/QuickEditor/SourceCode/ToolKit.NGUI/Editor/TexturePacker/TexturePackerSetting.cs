namespace QuickEditor.NGUIToolKit
{
    using QuickEditor.Core;
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
                    settings = QuickEditorAssetStaticAPI.LoadOrCreateAssetFromFindAssets<TexturePackerSetting>(false);
                }
                return settings;
            }
        }
    }
}