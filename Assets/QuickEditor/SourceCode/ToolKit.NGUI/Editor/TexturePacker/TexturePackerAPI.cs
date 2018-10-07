#if UNITY_NGUI
namespace QuickEditor.NGUIToolKit
{
    using System.IO;
    using System.Text;
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public class TexturePackerAPI
    {
        protected static string tpCommand
        {
            get { return TexturePackerSetting.Current.TPCommand; }
        }

        //选择并设置TP命令行的参数和参数值
        protected static string args = "--sheet {0}.{1} --data {2}.txt --format json --trim-mode None --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT --force-squared  --disable-rotation --scale 1 {3}";

        protected static string args_rgb = "--sheet {0}.{1} --data {2}.txt --format json --trim-mode None --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT --opt RGB888 --force-squared  --disable-rotation --scale 1 {3}";
        protected static string args_alpha = "--sheet {0}.{1} --data {2}.txt --format json --trim-mode None --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT --opt ALPHA_INTENSITY --force-squared  --disable-rotation --scale 1 {3}";

        protected static string atlasDir
        {
            get { return GPathStaticAPI.GetDataPath("GAssets/UIAtlas"); }
        }

        protected static string tpProjectsDir
        {
            get { return GPathStaticAPI.GetProjectPath("Product/ResourcesSource/TexturePacker"); }
        }

        public static void BuildTexturePacker()
        {
            if (string.IsNullOrEmpty(tpCommand) || !tpCommand.Contains("TexturePacker"))
            {
                EditorUtility.DisplayDialog("错误信息", "未配置TexturePacker命令路径", "确定");
                GEditorStaticAPI.LoadOrCreateAssetFromFindAssets<TexturePackerSetting>();
                return;
            }
            SetupTpAtlas();
            Debug.Log("Build TexturePacker Finished");
        }

        /// <summary>
        /// 配置TexturePacker对Unity图集的导出
        /// </summary>
        private static void SetupTpAtlas()
        {
            try
            {
                string[] tpsFiles = Directory.GetDirectories(tpProjectsDir);
                for (int i = 0; i < tpsFiles.Length; i++)
                {
                    string path = tpsFiles[i];
                    EditorUtility.DisplayProgressBar("Build TP_Atlas", string.Format("正在进行{0}", path), i / tpsFiles.Length);
                    StringBuilder sb = new StringBuilder("");
                    string[] files = Directory.GetFiles(path);
                    GetImageName(files, ref sb);
                    string sheetPath = Path.Combine(atlasDir, Path.GetFileName(path));//用TP打包好的图集存放目录
                    if (TexturePackerSetting.Current.IsSeperateRGBandAlpha)
                    {
                        GCMDStaticAPI.ExecuteCommand(tpCommand, string.Format(args_rgb, sheetPath + "_RGB", "png", sheetPath, sb.ToString()));
                        GCMDStaticAPI.ExecuteCommand(tpCommand, string.Format(args_alpha, sheetPath + "_Alpha", "png", sheetPath, sb.ToString()));
                    }
                    else
                    {
                        GCMDStaticAPI.ExecuteCommand(tpCommand, string.Format(args, sheetPath, "tga", sheetPath, sb.ToString()));
                    }
                    Debug.Log("Build TexturePacker Atlas: " + sheetPath);
                }
                AssetDatabase.Refresh();
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

        private static StringBuilder GetImageName(string[] fileName, ref StringBuilder sb)
        {
            if (fileName != null && fileName.Length > 0)
            {
                for (int j = 0; j < fileName.Length; j++)
                {
                    if (GEditorTexturePackerImporter.IsTextureFile(fileName[j]))
                    {
                        if (!RegexUtility.IsHasCHZN(Path.GetFileName(fileName[j])))
                        {
                            sb.Append(fileName[j]);
                            sb.Append(" ");
                        }
                    }
                }
            }
            return sb;
        }
    }
}
#endif