﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickEditor.Builder
{
    public class BuildAction_BuildProjectAndroid : BuildAction
    {
        public string KeyAliasPass;
        public string KeyStorePass;
        public string BuildPath;
        public string ProjectName;

        public BuildAction_BuildProjectAndroid(string projectName, string buildPath, string keyAliasPass = "", string keyStorePass = "")
        {
            ProjectName = projectName;
            KeyAliasPass = keyAliasPass;
            KeyStorePass = keyStorePass;
            BuildPath = buildPath;
        }

        public override BuildState OnUpdate()
        {
            //PlayerSettings.keyaliasPass = KeyAliasPass;
            //PlayerSettings.keystorePass = KeyStorePass;

            var listScene = BuildHelper.GetAllScenesInBuild();

            // projectName_yyyyMMddHHmm
            var apkName = string.Format("{0}_build{1}_{2:yyyyMMddHHmm}.apk",
                ProjectName,
                BuildHelper.GetBuildNum(),
                DateTime.Now);

            BuildHelper.AddBuildNum();

            var path = Path.Combine(BuildPath, apkName);
            Debug.Log(path);
            var dir = Path.GetDirectoryName(path);
            Debug.Log(dir);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            EditorPrefs.SetString("BuildMechine.ProjectPath", path);

            var res = BuildPipeline.BuildPlayer(listScene.ToArray(), path, BuildTarget.Android, BuildOptions.None);

            if (string.IsNullOrEmpty(res) == false)
            {
                throw new Exception("Build Fail : " + res);
            }

            Debug.LogFormat("打包至 {0} 结果 {1}", path, res);

            Context.Set("BuildPath", path);
            //        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //        buildPlayerOptions.locationPathName = string.Format("{0}\\{3}\\{2}\\{3}_{2}_{1}{4}",
            //Path.GetFullPath(Application.dataPath + "/../" + prefs.BuildFolder),
            //prefs.ProjectName,
            //prefs.ProjectVersion,
            //platformName,
            //ResolveExtension(a_platform)

            return BuildState.Success;
        }

        public override BuildProgress GetProgress()
        {
            return null;
        }
    }
}