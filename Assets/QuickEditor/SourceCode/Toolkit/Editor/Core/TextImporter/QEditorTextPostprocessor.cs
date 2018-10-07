namespace QuickEditor.Toolkit
{
    using System.IO;
    using UnityEditor;

    public class QEditorTextPostprocessor : UnityEditor.AssetPostprocessor
    {
        private const string TextEncodingNodeName = QToolkitConstants.ToolkitRootNodeName + "Text Encoding Toolbar/";

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string dic = Directory.GetCurrentDirectory();
            foreach (string importedAsset in importedAssets)
            {
                if (QEditorTextStaticAPI.IsFile(importedAsset))
                {
                    string file = dic + "/" + importedAsset;
                    QEditorTextStaticAPI.SetFileFormatToUTF8_BOM(file);
                }
            }
        }

        [MenuItem(TextEncodingNodeName + "UTF-8 + BOM All", false, 150)]
        public static void SetAllScriptsToUTF8_BOM()
        {
            string folder = Directory.GetCurrentDirectory() + "/Assets/";
            QEditorTextStaticAPI.SetFolderFormatToUTF8_BOM(folder);
        }
    }
}