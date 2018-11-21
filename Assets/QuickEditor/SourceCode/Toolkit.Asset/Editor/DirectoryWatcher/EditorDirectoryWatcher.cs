//using System.IO;
//using System.Threading;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using QuickEditor.Common;

//[InitializeOnLoad]
//public class SettingModuleEditor
//{
//    static SettingModuleEditor()
//    {
//        var path = SettingSourcePath;
//        if (Directory.Exists(path))
//        {
//            // watch files, when changed, compile settings
//            new EditorDirectoryWatcher(path, (o, args) =>
//            {
//                Debug.Log("[SettingModuleEditor]Watching File: " + args.FullPath + " ChangeType: " + args.ChangeType);
//                //if (_isPopUpConfirm) return;
//                EditorUtility.DisplayDialog("Excel Setting Changed!", "Ready to Recompile All!", "OK");
//                //_isPopUpConfirm = true;
//                //KEditorUtils.CallMainThread(() =>
//                //{
//                //    //                        EditorUtility.DisplayDialog("Excel Setting Changed!", "Ready to Recompile All!", "OK");
//                //    QuickCompileSettings();
//                //    _isPopUpConfirm = false;
//                //});
//            });
//            Debug.Log("[SettingModuleEditor]Watching directory: " + SettingSourcePath);
//        }
//    }

//    protected static string SettingSourcePath
//    {
//        get
//        {
//            string dirPath = Application.dataPath.Replace("Assets", "Product") + "/SettingSource/";
//            //dirPath = dirPath.Replace("/Assets", "");
//            //string dirPath = Application.dataPath + "/../Product/SettingSource/";
//            if (!System.IO.Directory.Exists(dirPath))
//            {
//                System.IO.Directory.CreateDirectory(dirPath);
//            }
//            GFileStaticAPI.CreateFolder(Path.Combine(GPathStaticAPI.ProjectPath, "Product/GBuilder"));
//            return dirPath;
//        }
//    }
//}

//public class EditorDirectoryWatcher
//{
//    private static readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

//    /// <summary>
//    /// 监视一个目录，如果有修改则触发事件函数, 包含其子目录！
//    /// <para>使用更大的buffer size确保及时触发事件</para>
//    /// <para>不用includesubdirect参数，使用自己的子目录扫描，更稳健</para>
//    /// </summary>
//    /// <param name="dirPath"></param>
//    /// <param name="handler"></param>
//    /// <returns></returns>
//    public EditorDirectoryWatcher(string dirPath, FileSystemEventHandler handler)
//    {
//        CreateWatch(dirPath, handler);
//    }

//    private void CreateWatch(string dirPath, FileSystemEventHandler handler)
//    {
//        if (_watchers.ContainsKey(dirPath))
//        {
//            _watchers[dirPath].Dispose();
//            _watchers[dirPath] = null;
//        }

//        if (!Directory.Exists(dirPath)) return;

//        var watcher = new FileSystemWatcher();
//        watcher.IncludeSubdirectories = false;//includeSubdirectories;
//        watcher.Path = dirPath;
//        watcher.NotifyFilter = NotifyFilters.LastWrite;
//        watcher.Filter = "*";
//        watcher.Changed += handler;
//        watcher.EnableRaisingEvents = true;
//        watcher.InternalBufferSize = 10240;
//        //return watcher;
//        _watchers[dirPath] = watcher;

//        foreach (var childDirPath in Directory.GetDirectories(dirPath))
//        {
//            CreateWatch(childDirPath, handler);
//        }
//    }

//    private void Waiting(string path)
//    {
//        try
//        {
//            FileInfo fi;
//            fi = new FileInfo(path);
//            long len1, len2;
//            len2 = fi.Length;
//            do
//            {
//                len1 = len2;
//                Thread.Sleep(1000);//等待1秒钟
//                fi.Refresh();//这个语句不能漏了
//                len2 = fi.Length;
//            } while (len1 < len2);
//        }
//        catch { }
//    }
//}