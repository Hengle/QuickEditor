#if UNITY_NGUI
namespace QuickEditor.NGUIToolKit
{
    using System;
    using System.IO;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class TexturePackerImporter : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                DoRebuildNGUIAtlas(importedAsset);
            }
            foreach (string str in deletedAssets)
            {
                DeleteNGUIAtlas(str);
            }
            for (var i = 0; i < movedAssets.Length; i++)
            {
                MoveNGUIAtlas(movedFromAssetPaths[i], movedAssets[i]);
            }
        }

        private static void DeleteNGUIAtlas(string fntPath)
        {
            if (!IsAtlasConfig(fntPath)) return;
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "png"));
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "mat"));
            AssetDatabase.DeleteAsset(Path.ChangeExtension(fntPath, "prefab"));
        }

        private static void MoveNGUIAtlas(string oldFntPath, string nowFntPath)
        {
            if (!IsAtlasConfig(nowFntPath)) return;
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "png"), Path.ChangeExtension(nowFntPath, "png"));
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "mat"), Path.ChangeExtension(nowFntPath, "mat"));
            AssetDatabase.MoveAsset(Path.ChangeExtension(oldFntPath, "prefab"), Path.ChangeExtension(nowFntPath, "prefab"));
        }

        public static bool IsAtlasConfig(string path)
        {
            return path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) && path.Replace("\\", "/").Contains("UIAtlas");
        }

        public static void DoRebuildNGUIAtlas(string assetPath)
        {
            if (!IsAtlasConfig(assetPath)) { return; }
            if (TexturePackerSetting.Current.IsSeperateRGBandAlpha)
            {
                RebuildSeperateRGBandAlphaNGUIAtlas(assetPath);
            }
            else
            {
                RebuildNGUIAtlas(assetPath);
            }

            AssetDatabase.Refresh();
            Resources.UnloadUnusedAssets();
            System.GC.GetTotalMemory(true);
            System.GC.Collect();
        }

        private static void RebuildNGUIAtlas(string assetPath)
        {
            try
            {
                EditorUtility.DisplayProgressBar("设置NGUI图集", string.Format("正在进行 {0}", assetPath), 1);

                string pngFile = Path.ChangeExtension(assetPath, "tga");
                string prefabFile = Path.ChangeExtension(assetPath, "prefab");
                string matFile = Path.ChangeExtension(assetPath, "mat");

                GameObject prefabAsset = GEditorNGUILoader.LoadAtlasPrefab(prefabFile);
                Material matAsset = GEditorNGUILoader.LoadTransparentMaterial(matFile);
                if (matAsset == null || prefabAsset == null)
                {
                    Debug.LogError(string.Format("AssetPath: {0}  Material或者预制体为空", assetPath));
                    return;
                }
                UIAtlas uiAtlas = prefabAsset.GetComponent<UIAtlas>();
                if (uiAtlas == null)
                    uiAtlas = prefabAsset.AddComponent<UIAtlas>();

                // 配置图集参数
                Texture2D pngAsset = GEditorNGUILoader.LoadTexture2D(pngFile);
                TextAsset dataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath) as TextAsset;
                if (pngAsset == null || dataAsset == null)
                {
                    Debug.LogError(string.Format("AssetPath: {0}  图集所需图片或者配置文件不存在", assetPath));
                    return;
                }
                matAsset.SetTexture("_MainTex", pngAsset);
                uiAtlas.spriteMaterial = matAsset;
                NGUIJson.LoadSpriteData(uiAtlas, dataAsset);
                uiAtlas.MarkAsChanged();
                EditorUtility.SetDirty(uiAtlas);
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error " + ex.ToString());
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        protected static void RebuildSeperateRGBandAlphaNGUIAtlas(string assetPath)
        {
            try
            {
                EditorUtility.DisplayProgressBar("设置NGUI图集", string.Format("正在进行 {0}", assetPath), 1);
                string pngFile = Path.ChangeExtension(assetPath, "png");
                string prefabFile = Path.ChangeExtension(assetPath, "prefab");
                string matFile = Path.ChangeExtension(assetPath, "mat");

                GameObject prefabAsset = GEditorNGUILoader.LoadAtlasPrefab(prefabFile);
                Material matAsset = GEditorNGUILoader.LoadETCMaterial(matFile);
                if (matAsset == null || prefabAsset == null)
                {
                    Debug.LogError(string.Format("AssetPath: {0}  Material或者预制体为空", assetPath));
                    return;
                }
                UIAtlas uiAtlas = prefabAsset.GetComponent<UIAtlas>();
                if (uiAtlas == null)
                    uiAtlas = prefabAsset.AddComponent<UIAtlas>();

                // 配置图集参数

                Texture2D _mainTex = GEditorNGUILoader.LoadTexture2D(pngFile.Replace(".png", "_RGB.png"));
                Texture2D _alphaTex = GEditorNGUILoader.LoadTexture2D(pngFile.Replace(".png", "_Alpha.png"));

                TextAsset dataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath) as TextAsset;
                if (_mainTex == null || _alphaTex == null || dataAsset == null)
                {
                    Debug.LogError(string.Format("AssetPath: {0}  图集所需图片或者配置文件不存在", assetPath));
                    return;
                }
                matAsset.SetTexture("_MainTex", _mainTex);
                matAsset.SetTexture("_MainTex_A", _alphaTex);
                uiAtlas.spriteMaterial = matAsset;
                NGUIJson.LoadSpriteData(uiAtlas, dataAsset);
                uiAtlas.MarkAsChanged();
                EditorUtility.SetDirty(uiAtlas);
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error " + ex.ToString());
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static bool IsTextureFile(string _path)
        {
            string path = _path.ToLower();
            return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".dds") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
        }

        public static bool IsPngFile(string path)
        {
            return path.ToLower().EndsWith(".png");
        }

        public static bool IsTextureConverted(string path)
        {
            return path.Contains("_RGB.") || path.Contains("_Alpha.");
        }

        /// <summary>
        /// 获取texture的原始文件尺寸
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void GetOriginalSize(TextureImporter importer, out int width, out int height)
        {
            object[] args = new object[2] { 0, 0 };
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(importer, args);
            width = (int)args[0];
            height = (int)args[1];
        }
    }
}
#endif