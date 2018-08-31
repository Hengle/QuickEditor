#if UNITY_NGUI

namespace QuickEditor.Toolkit
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class GEditorBitmapFontImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                DoImportBitmapFont(str);
            }
            foreach (string str in deletedAssets)
            {
                DeleteBitmapFont(str);
            }
            for (var i = 0; i < movedAssets.Length; i++)
            {
                MoveBitmapFont(movedFromAssetPaths[i], movedAssets[i]);
            }
        }

        public static bool IsFnt(string path)
        {
            return path.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase);
        }

        private static void DeleteBitmapFont(string fntPath)
        {
            if (!IsFnt(fntPath)) return;
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "png"));
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "mat"));
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "prefab"));
        }

        private static void MoveBitmapFont(string oldFntPath, string nowFntPath)
        {
            if (!IsFnt(nowFntPath)) return;
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "png"), Path.ChangeExtension(nowFntPath, "png"));
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "mat"), Path.ChangeExtension(nowFntPath, "mat"));
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "prefab"), Path.ChangeExtension(nowFntPath, "prefab"));
        }

        public static void DoImportBitmapFont(string fntPath)
        {
            if (!IsFnt(fntPath)) { return; }
            try
            {
                EditorUtility.DisplayProgressBar("Build Bitmap Font", string.Format("正在进行 {0}", fntPath), 1);
                string pngPath = Path.ChangeExtension(fntPath, "png");
                string prefabPath = Path.ChangeExtension(fntPath, "prefab");
                string matPath = Path.ChangeExtension(fntPath, "mat");

                GameObject prefabAsset = GEditorNGUILoader.LoadAtlasPrefab(prefabPath);
                Material matAsset = GEditorNGUILoader.LoadTransparentMaterial(matPath);
                UIFont mUIFont = prefabAsset.GetComponent<UIFont>();
                if (mUIFont == null)
                    mUIFont = prefabAsset.AddComponent<UIFont>();
                if (matAsset == null || prefabAsset == null)
                {
                    Debug.LogError(string.Format("Build Bitmap Font -> AssetPath: {0}  Material或者预制体为空", fntPath));
                    return;
                }
                // 配置图集参数
                Texture2D pngAsset = GEditorNGUILoader.LoadTexture2D(pngPath);
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(fntPath) as TextAsset;
                if (pngAsset == null || data == null)
                {
                    Debug.LogError(string.Format("Build Bitmap Font -> AssetPath: {0}  配置文件缺失", fntPath));
                    return;
                }
                BMFontReader.Load(mUIFont.bmFont, NGUITools.GetHierarchy(mUIFont.gameObject), data.bytes);
                matAsset.SetTexture("_MainTex", pngAsset);
                mUIFont.material = matAsset;
                mUIFont.MarkAsChanged();
                EditorUtility.SetDirty(mUIFont);
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("异常", ex.ToString(), "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

#endif