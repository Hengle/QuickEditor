namespace QuickEditor.Toolkit
{
    using QuickEditor.Core;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 打开文件夹相关的实用函数。
    /// </summary>
    internal static class QEditorCatalogToolbar
    {
        private const int CatalogNodePriority = 800;
        private const string CatalogNodeName = QuickToolkitConstants.ToolkitRootNodeName + "Catalog Toolbar/";

        [MenuItem(CatalogNodeName + "Open Project Catalog Settings", false, CatalogNodePriority)]
        private static void OpenCatalogSettings()
        {
            QuickEditorAssetStaticAPI.LoadOrCreateAssetFromFindAssets<ProjectCatalogSettings>();
        }

        [MenuItem(CatalogNodeName + "Generate  Assets Template Directory", false, CatalogNodePriority)]
        private static void GenerateForAssets()
        {
            if (ProjectCatalogSettings.Current.AssetFolders != null && ProjectCatalogSettings.Current.AssetFolders.Count > 0)
            {
                ProjectCatalogSettings.Current.AssetFolders.ForEach(path => QuickEditorFileStaticAPI.MakeDir(path));
            }
        }

        [MenuItem(CatalogNodeName + "Generate  Resources Template Directory", false, CatalogNodePriority)]
        private static void GenerateForResources()
        {
            if (ProjectCatalogSettings.Current.ResourcesFolders != null && ProjectCatalogSettings.Current.ResourcesFolders.Count > 0)
            {
                ProjectCatalogSettings.Current.ResourcesFolders.ForEach(path => QuickEditorFileStaticAPI.MakeDir(Path.Combine("Resources", path)));
            }
        }

        /// <summary>
        /// 打开 Temporary Cache Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Temporary Cache Path", false, CatalogNodePriority + 100)]
        private static void OpenFolderTemporaryCachePath()
        {
            InternalOpenFolder(Application.temporaryCachePath);
        }

        /// <summary>
        /// 打开 Persistent Data Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Persistent Data Path", false, CatalogNodePriority + 101)]
        private static void OpenFolderPersistentDataPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.PersistentDataPath);
        }

        /// <summary>
        /// 打开 Streaming Assets Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Streaming Assets Path", false, CatalogNodePriority + 102)]
        private static void OpenFolderStreamingAssetsPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.StreamingAsstesPath);
        }

        /// <summary>
        /// 打开 Data Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Data Path", false, CatalogNodePriority + 103)]
        private static void OpenFolderDataPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.AssetsPath);
        }

        private static void InternalOpenFolder(string folder)
        {
            folder = string.Format("\"{0}\"", folder);
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                    break;

                case RuntimePlatform.OSXEditor:
                    Process.Start("open", folder);
                    break;

                default:
                    throw new QuickEditorException(string.Format("Not support open folder on '{0}' platform.", Application.platform.ToString()));
            }
        }
    }
}
