namespace QuickEditor.ToolKit.SVN
{
    using QuickEditor.Core;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    ///closeonend:0 不自动关闭对话框
    ///closeonend:1 如果没发生错误则自动关闭对话框
    ///closeonend:2 如果没发生错误和冲突则自动关闭对话框
    ///closeonend:3如果没有错误、冲突和合并，会自动关闭
    public static class QuickSVNStaticAPI
    {
        private const string ToolKitNodeName = "QuickEditor.SVN/";
        private const string SVNAssetNodeName = "Assets/QuickEditor.SVN/";

        [MenuItem(ToolKitNodeName + "SVN 提交选中", true, 100)]
        public static bool CheckSVNCommit()
        {
            return (Selection.assetGUIDs != null && Selection.assetGUIDs.Length >= 1);
        }

        [MenuItem(ToolKitNodeName + "SVN 提交选中", false, 100)]
        [MenuItem(SVNAssetNodeName + "SVN 提交选中", false, 9100)]
        private static void SVNCommit()
        {
            if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length < 1) { return; }

            ProcessCommand("TortoiseProc.exe", "/command:commit /path:" + SelectedProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 提交全部", false, 101)]
        [MenuItem(SVNAssetNodeName + "SVN 提交全部", false, 9101)]
        private static void SVNCommitAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:commit /path:" + WholeProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 更新选中", true, 200)]
        public static bool CheckSVNUpdate()
        {
            return (Selection.assetGUIDs != null && Selection.assetGUIDs.Length >= 1);
        }

        [MenuItem(ToolKitNodeName + "SVN 更新选中", false, 200)]
        [MenuItem(SVNAssetNodeName + "SVN 更新选中", false, 9200)]
        private static void SVNUpdate()
        {
            ProcessCommand("TortoiseProc.exe", "/command:update /path:" + SelectedProjectPath + " /closeonend:0");
        }

        [MenuItem(ToolKitNodeName + "SVN 更新全部", false, 201)]
        [MenuItem(SVNAssetNodeName + "SVN 更新全部", false, 201)]
        private static void SVNUpdateAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:update /path:" + WholeProjectPath + " /closeonend:0");
        }

        [MenuItem(ToolKitNodeName + "SVN 增加选中", true, 300)]
        public static bool CheckSVNAdd()
        {
            return (Selection.assetGUIDs != null && Selection.assetGUIDs.Length >= 1);
        }

        [MenuItem(ToolKitNodeName + "SVN 增加选中", false, 300)]
        [MenuItem(SVNAssetNodeName + "SVN 增加选中", false, 9300)]
        private static void SVNAdd()
        {
            ProcessCommand("TortoiseProc.exe", "/command:add /path:" + SelectedProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 增加全部", false, 301)]
        [MenuItem(SVNAssetNodeName + "SVN 增加全部", false, 9301)]
        private static void SVNAddAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:add /path:" + WholeProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 锁住选中 ", true, 400)]
        public static bool CheckSVNLock()
        {
            return (Selection.assetGUIDs != null && Selection.assetGUIDs.Length >= 1);
        }

        [MenuItem(ToolKitNodeName + "SVN 锁住选中 ", false, 400)]
        [MenuItem(SVNAssetNodeName + "SVN 锁住选中 ", false, 9400)]
        private static void SVNLock()
        {
            ProcessCommand("TortoiseProc.exe", "/command:lock /path:" + SelectedProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 锁住全部 ", false, 401)]
        [MenuItem(SVNAssetNodeName + "SVN 锁住全部 ", false, 9401)]
        private static void SVNLockAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:lock /path:" + SVNProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 解锁选中", true, 500)]
        public static bool CheckSVNUnLock()
        {
            return (Selection.assetGUIDs != null && Selection.assetGUIDs.Length >= 1);
        }

        [MenuItem(ToolKitNodeName + "SVN 解锁选中", false, 500)]
        [MenuItem(SVNAssetNodeName + "SVN 解锁选中", false, 9500)]
        private static void SVNUnLock()
        {
            ProcessCommand("TortoiseProc.exe", "/command:unlock /path:" + SelectedProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 解锁全部", false, 501)]
        [MenuItem(SVNAssetNodeName + "SVN 解锁全部", false, 9501)]
        private static void SVNUnLockAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:unlock /path:" + SVNProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 显示日志", false, 600)]
        [MenuItem(SVNAssetNodeName + "SVN 显示日志", false, 9600)]
        private static void SVNLog()
        {
            ProcessCommand("TortoiseProc.exe", "/command:log /path:" + SelectedProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 全部日志", false, 601)]
        [MenuItem(SVNAssetNodeName + "SVN 全部日志", false, 9601)]
        private static void SVNLogAll()
        {
            ProcessCommand("TortoiseProc.exe", "/command:log /path:" + SVNProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 清理副本", false, 602)]
        [MenuItem(SVNAssetNodeName + "SVN 清理副本", false, 9602)]
        private static void SVNCleanUp()
        {
            ProcessCommand("TortoiseProc.exe", "/command:cleanup /path:" + SVNProjectPath);
        }

        [MenuItem(ToolKitNodeName + "SVN 打开设置", false, 700)]
        [MenuItem(SVNAssetNodeName + "SVN 打开设置", false, 9700)]
        private static void SVNSetting()
        {
            ProcessCommand("TortoiseProc.exe", "/command:settings");
        }

        [MenuItem(ToolKitNodeName + "SVN 打开文档", false, 700)]
        private static void SVNHelp()
        {
            Application.OpenURL("https://tortoisesvn.net/docs/release/TortoiseSVN_zh_CN/tsvn-automation.html#tsvn-automation-basics");
        }

        private static string SelectedProjectPath
        {
            get
            {
                List<string> pathList = new List<string>();
                int length = Selection.assetGUIDs.Length;
                for (int i = 0; i < length; i++)
                {
                    pathList.Add(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
                }

                return string.Join("*", pathList.ToArray());
            }
        }

        private static string WholeProjectPath
        {
            get
            {
                List<string> pathList = new List<string>(){
                    SVNProjectPath + "/Assets",
                    SVNProjectPath + "/ProjectSettings"
                };
                return string.Join("*", pathList.ToArray());
            }
        }

        private static string SVNProjectPath
        {
            get
            {
                return QuickEditorPathStaticAPI.ProjectPath;
            }
        }

        public static void ProcessCommand(string command, string argument)
        {
            QuickProcessStaticAPI.ExecuteTortoiseSVNCommand(argument);
        }
    }
}