namespace QuickEditor.Toolkit
{
    using QuickEditor.Core;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 打开文件夹相关的实用函数。
    /// </summary>
    internal static class QuickFolderToolbar
    {
        private const int CatalogNodePriority = 800;
        private const string CatalogNodeName = QuickToolkitConstants.ToolkitRootNodeName + "Quick Folder Opener/";

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
        /// 打开 Data Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Application.dataPath", false, CatalogNodePriority + 103)]
        private static void OpenFolderDataPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.AssetsPath);
        }

        /// <summary>
        /// 打开 Persistent Data Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Application.persistentDataPath", false, CatalogNodePriority + 101)]
        private static void OpenFolderPersistentDataPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.PersistentDataPath);
        }

        /// <summary>
        /// 打开 Streaming Assets Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Application.streamingAssetsPath", false, CatalogNodePriority + 102)]
        private static void OpenFolderStreamingAssetsPath()
        {
            InternalOpenFolder(QuickEditorPathStaticAPI.StreamingAsstesPath);
        }

        /// <summary>
        /// 打开 Temporary Cache Path 文件夹。
        /// </summary>
        [MenuItem(CatalogNodeName + "Application.temporaryCachePath", false, CatalogNodePriority + 100)]
        private static void OpenFolderTemporaryCachePath()
        {
            InternalOpenFolder(Application.temporaryCachePath);
        }

        [MenuItem(CatalogNodeName + "Asset Store Packages Folder", false, CatalogNodePriority + 200)]
        private static void OpenAssetStorePackagesFolder()
        {
            //http://answers.unity3d.com/questions/45050/where-unity-store-saves-the-packages.html
            //
#if UNITY_EDITOR_OSX
            string path = GetAssetStorePackagesPathOnMac();
#elif UNITY_EDITOR_WIN
            string path = GetAssetStorePackagesPathOnWindows();
#endif

            InternalOpenFolder(path);
        }

        [MenuItem(CatalogNodeName + "Editor Application Path", false, CatalogNodePriority + 200)]
        private static void OpenUnityEditorPath()
        {
            InternalOpenFolder(new FileInfo(EditorApplication.applicationPath).Directory.FullName);
        }

        [MenuItem(CatalogNodeName + "Editor Log Folder", false, CatalogNodePriority + 200)]
        private static void OpenEditorLogFolderPath()
        {
#if UNITY_EDITOR_OSX
			string rootFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine(rootFolderPath, "Library");
			var logsFolder = Path.Combine(libraryPath, "Logs");
			var UnityFolder = Path.Combine(logsFolder, "Unity");
			InternalOpenFolder(UnityFolder);
#elif UNITY_EDITOR_WIN
            var rootFolderPath = System.Environment.ExpandEnvironmentVariables("%localappdata%");
            var unityFolder = Path.Combine(rootFolderPath, "Unity");
            InternalOpenFolder(Path.Combine(unityFolder, "Editor"));
#endif
        }

        [MenuItem(CatalogNodeName + "Asset Backup Folder", false, CatalogNodePriority + 300)]
        public static void OpenAEBackupFolder()
        {
            var folder = Path.Combine(Application.persistentDataPath, "AEBackup");
            Directory.CreateDirectory(folder);
            InternalOpenFolder(folder);
        }

        private const string ASSET_STORE_FOLDER_NAME = "Asset Store";

        private static string GetAssetStorePackagesPathOnMac()
        {
            var rootFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(rootFolderPath, "Library");
            var unityFolder = Path.Combine(libraryPath, "Unity");
            return Path.Combine(unityFolder, ASSET_STORE_FOLDER_NAME);
        }

        private static string GetAssetStorePackagesPathOnWindows()
        {
            var rootFolderPath = System.Environment.ExpandEnvironmentVariables("%appdata%");
            var unityFolder = Path.Combine(rootFolderPath, "Unity");
            return Path.Combine(unityFolder, ASSET_STORE_FOLDER_NAME);
        }

        private static void InternalOpenFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                UnityEngine.Debug.LogWarning(string.Format("Folder '{0}' is not Exists", folder));
                return;
            }

            EditorUtility.RevealInFinder(folder);

            //folder = string.Format("\"{0}\"", folder);
            //switch (Application.platform)
            //{
            //    case RuntimePlatform.WindowsEditor:
            //        Process.Start("Explorer.exe", folder.Replace('/', '\\'));
            //        break;

            //    case RuntimePlatform.OSXEditor:
            //        Process.Start("open", folder);
            //        break;

            //    default:
            //        throw new QuickEditorException(string.Format("Not support open folder on '{0}' platform.", Application.platform.ToString()));
            //}
        }
    }
}