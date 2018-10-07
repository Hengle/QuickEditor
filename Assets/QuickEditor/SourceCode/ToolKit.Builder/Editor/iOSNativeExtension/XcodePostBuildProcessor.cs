namespace QuickEditor.Builder
{
#if UNITY_5_6_OR_NEWER

    using UnityEditor.iOS.Xcode;
    using UnityEditor.Build;

#else

    using UnityEditor.Callbacks;
    using UnityEditor.iOS.Xcode.Custom;

#endif

    using UnityEngine;
    using UnityEditor;

    using System.IO;

#if UNITY_5_6_OR_NEWER

    public class XcodePostBuildProcessor : IPostprocessBuild
    {
        int IOrderedCallback.callbackOrder
        {
            get { return 0; }
        }

        void IPostprocessBuild.OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            iOSNativeExtension.ProcessNativeExtension(path);
        }
    }

#else

    public class XcodePostBuildProcessor
    {
        [PostProcessBuild(1)]
        private static void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return;

            iOSNativeExtension.ProcessNativeExtension(path);
        }
    }

#endif

    public static class iOSNativeExtension
    {
        private static XcodeProjectSetting projectSetting;

        public static void ProcessNativeExtension(string buildPath)
        {
            projectSetting = XcodeProjectSetting.Current;
            if (projectSetting == null)
            {
                Debug.LogError("Xcode Project Setting is null");
                return;
            }
            ModifyProject(buildPath);
            SetInfoPlist(buildPath);
        }

        private static void ModifyProject(string buildPath)
        {
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            var settings = projectSetting.XcodeSettingsList.FindAll(p => p.Enabled);
            foreach (var setting in settings)
            {
                setting.Frameworks.ForEach(framework => proj.AddFrameworkToProject(targetGuid, framework.Path, framework.IsWeak));

                setting.Librarys.ForEach(lib => AddLibToProject(proj, targetGuid, lib.File));
                setting.DependentLibrarys.ForEach(lib => AddLibToProject(proj, targetGuid, lib.File));

                foreach (var buildProperty in setting.BuildPropertys)
                {
                    if (string.IsNullOrEmpty(buildProperty.Name) || string.IsNullOrEmpty(buildProperty.Value))
                        continue;

                    proj.AddBuildProperty(targetGuid, buildProperty.Name, buildProperty.Value);
                }
            }
            proj.SetBuildProperty(targetGuid, XcodeProjectConst.ENABLE_BITCODE_KEY, projectSetting.EnableBitCode ? "YES" : "NO");

            File.WriteAllText(projPath, proj.WriteToString());
        }

        private static void AddFrameworkToProject(PBXProject proj, string targetGuid, string framework, bool isWeak)
        {
            string fileGuid = proj.AddFile("System/Library/Frameworks/" + framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            //proj.AddFrameworkToProjectByFileGuid(targetGuid, fileGuid, isWeak);
        }

        private static void AddLibToProject(PBXProject proj, string targetGuid, string lib)
        {
            string fileGuid = proj.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
            proj.AddFileToBuild(targetGuid, fileGuid);
        }

        private static void SetInfoPlist(string buildPath)
        {
            CInfoPlist.SetATS(buildPath, projectSetting.NSAppTransportSecurity);
            CInfoPlist.SetStatusBar(buildPath, projectSetting.EnableUIStatusBar);
            var settings = projectSetting.XcodeSettingsList.FindAll(p => p.Enabled);
            foreach (var setting in settings)
            {
                CInfoPlist.SetApplicationQueriesSchemes(buildPath, setting.ApplicationQueriesSchemes);
            }

            //插入代码
            //读取UnityAppController.mm文件
            string unityAppControllerPath = buildPath + "/Classes/UnityAppController.mm";
            CXClass UnityAppController = new CXClass(unityAppControllerPath);
            UnityAppController.Load();
            //在指定代码后面增加一行代码
            UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import <UMSocialCore/UMSocialCore.h>");

            string newCode = "\n" +
                       "    [[UMSocialManager defaultManager] openLog:YES];\n" +
                       "    [UMSocialGlobal shareInstance].type = @\"u3d\";\n" +
                       "    [[UMSocialManager defaultManager] setUmSocialAppkey:@\"" + "\"];\n" +
                       "    [[UMSocialManager defaultManager] setPlaform:UMSocialPlatformType_WechatSession appKey:@\"" + "\" appSecret:@\"" + "\" redirectURL:@\"http://mobile.umeng.com/social\"];\n" +
                       "    \n"
                       ;
            //在指定代码后面增加一大行代码
            UnityAppController.WriteBelow("// if you wont use keyboard you may comment it out at save some memory", newCode);
            UnityAppController.Write();
        }
    }
}
