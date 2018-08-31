#if UNITY_NGUI

namespace QuickEditor.Toolkit
{
    using System.IO;
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public class GEditorBitmapFontToolbar
    {
        protected const string BitmapFontNodeName = GEditorDefines.GEditorRootNodeName + "NGUI Toolbar/Bitmap Font Toolbar/";

        protected const string RebuildBitmapFontMenuName = BitmapFontNodeName + "Rebuild Bitmap Font";
        protected const string RebuildAllBitmapFontMenuName = BitmapFontNodeName + "Rebuild All Bitmap Font";

        protected const int RebuildBitmapFontMenuPriority = 140;
        protected const int RebuildAllBitmapFontMenuPriority = 141;

        [MenuItem(RebuildBitmapFontMenuName, true, RebuildBitmapFontMenuPriority)]
        public static bool CheckRebuildFont()
        {
            if (EditorApplication.isPlaying) return false;
            TextAsset selected = Selection.activeObject as TextAsset;
            if (selected == null) return false;
            return GEditorBitmapFontImporter.IsFnt(AssetDatabase.GetAssetPath(selected));
        }

        [MenuItem(RebuildBitmapFontMenuName, false, RebuildBitmapFontMenuPriority)]
        public static void RebuildBitmapFont()
        {
            TextAsset selected = Selection.activeObject as TextAsset;
            GEditorBitmapFontImporter.DoImportBitmapFont(AssetDatabase.GetAssetPath(selected));
        }

        [MenuItem(RebuildAllBitmapFontMenuName, true, RebuildAllBitmapFontMenuPriority)]
        public static bool CheckRebuildAllFont()
        {
            return !EditorApplication.isPlaying;
        }

        [MenuItem(RebuildAllBitmapFontMenuName, false, RebuildAllBitmapFontMenuPriority)]
        public static void RebuildAllFont()
        {
            int startPos = GPathStaticAPI.ProjectPath.Length;
            string[] files = Directory.GetFiles(Application.dataPath, "*.fnt", SearchOption.AllDirectories);
            if (files != null && files.Length > 0)
            {
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    GEditorBitmapFontImporter.DoImportBitmapFont(files[i].Substring(startPos));
                }
            }
        }
    }
}

#endif