namespace QuickEditor.Toolkit
{
    using QuickEditor.Core;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class AssetImportPresetsSettings : QuickScriptableObject<AssetImportPresetsSettings>
    {
        [SerializeField]
        [HideInInspector]
        public List<MeshImportSetting> MeshImportSettings = new List<MeshImportSetting>();

        [SerializeField]
        [HideInInspector]
        public List<AudioImportSetting> AudioImportSettings = new List<AudioImportSetting>();

        [SerializeField]
        [HideInInspector]
        public List<TextureImportSetting> TextureImportSettings = new List<TextureImportSetting>();
    }
}
