#if UNITY_NGUI
namespace QuickEditor.Toolkit
{
    using System.IO;
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public class GEditorTexturePackerToolbar
    {
        internal const int TexturePackerNodePriority = 200;
        internal const string TexturePackerNodeName = GEditorDefines.GEditorRootNodeName + "NGUI Toolbar/TexturePacker Toobar/";

        /// <summary>
        /// 自动构建图集菜单命令
        /// </summary>
        [MenuItem(TexturePackerNodeName + "Open TexturePackerSetting", false, TexturePackerNodePriority)]
        public static void BuildTexturePacker1()
        {
            GEditorStaticAPI.LoadOrCreateAssetFromFindAssets<TexturePackerSetting>();
        }

        /// <summary>
        /// 自动构建图集菜单命令
        /// </summary>
        [MenuItem(TexturePackerNodeName + "Build TexturePacker", false, TexturePackerNodePriority + 20)]
        public static void BuildTexturePacker()
        {
            if (EditorApplication.isPlaying) return;
            GEditorTexturePackerAPI.BuildTexturePacker();
        }

        [MenuItem(TexturePackerNodeName + "Rebuild NGUI UIAtlas", true, TexturePackerNodePriority + 40)]
        public static bool CheckRebuildUIAtlas()
        {
            if (EditorApplication.isPlaying) return false;
            TextAsset selected = Selection.activeObject as TextAsset;
            if (selected == null) return false;
            return GEditorTexturePackerImporter.IsAtlasConfig(AssetDatabase.GetAssetPath(selected));
        }

        [MenuItem(TexturePackerNodeName + "Rebuild NGUI UIAtlas", false, TexturePackerNodePriority + 41)]
        public static void RebuildUIAtlas()
        {
            TextAsset selected = Selection.activeObject as TextAsset;
            GEditorTexturePackerImporter.DoRebuildNGUIAtlas(AssetDatabase.GetAssetPath(selected));
        }

        [MenuItem(TexturePackerNodeName + "Rebuild All NGUI UIAtlas", true, TexturePackerNodePriority + 42)]
        public static bool CheckRebuildAllUIAtlas()
        {
            return !EditorApplication.isPlaying;
        }

        [MenuItem(TexturePackerNodeName + "Rebuild All NGUI UIAtlas", false, TexturePackerNodePriority + 43)]
        public static void RebuildAllUIAtlas()
        {
            int startPos = GPathStaticAPI.ProjectPath.Length;
            string[] files = Directory.GetFiles(Application.dataPath, "*.fnt", SearchOption.AllDirectories);
            if (files != null && files.Length > 0)
            {
                for (int i = 0, length = files.Length; i < length; i++)
                {
                    GEditorTexturePackerImporter.DoRebuildNGUIAtlas(files[i].Substring(startPos));
                }
            }
        }
    }
}
#endif