namespace QuickEditor.Builder
{
    using QuickEditor.Core;
    using UnityEditor;

    public class ProjectBuilderToolbar
    {
        internal const string AutoBuilderNodeName = "QuickEditor.Builder/";

        [MenuItem(AutoBuilderNodeName + "Project Build Setting", false, 1)]
        public static void OpenProjectBuildSetting()
        {
            ProjectBuilderWindow.Open();
        }

        [MenuItem(AutoBuilderNodeName + "iOS Native Extension", false, 2)]
        public static void OpenXcodeProjectSettings()
        {
            XcodeProjectSettingWindow.Open();
        }

        [MenuItem(AutoBuilderNodeName + "Build Windows Standalone", false, 100)]
        public static void BuildWindowsStandalone()
        {
            BuildMechine.NewPipeline()
                .AddActions(new BuildAction_Print("Start Build Mechine"))
                .AddActions(new BuildAction_ActiveBuildTargetChanged(BuildTarget.StandaloneWindows))
                .AddActions(new BuildAction_IncreaseBuildNum())
                .AddActions(new BuildAction_SaveAndRefresh())
                .AddActions(new BuildAction_SetBundleId("cn.test.test"))
                .AddActions(new BuildAction_BuildProjectWindowsStandalone("game", "exe", "Build/Windows/", x64: false))
                .Run();
        }

        [MenuItem(AutoBuilderNodeName + "Build Android", false, 101)]
        public static void BuildAndroid()
        {
            BuildMechine.NewPipeline()
                .AddActions(new BuildAction_Print("Start Build Mechine"))
                .AddActions(new BuildAction_ActiveBuildTargetChanged(BuildTarget.Android))
                .AddActions(new BuildAction_IncreaseBuildNum())
                .AddActions(new BuildAction_SaveAndRefresh())
                .AddActions(new BuildAction_SetBundleId("cn.test.test"))
                .AddActions(new BuildAction_BuildProjectAndroid("game", "Build/Android/"))
                .Run();
        }

        [MenuItem(AutoBuilderNodeName + "Build iOS", false, 102)]
        public static void BuildIOS()
        {
        }

        protected static void SetSplashScreens()
        {
            QuickEditorStaticAPI.SetAppIcon(BuildTargetGroup.iOS, ProjectBuildSetting.Current.Icon);
            QuickEditorStaticAPI.SetAppIcon(BuildTargetGroup.Android, ProjectBuildSetting.Current.Icon);
            QuickEditorStaticAPI.SetAppIcon(BuildTargetGroup.Standalone, ProjectBuildSetting.Current.Icon);
            QuickEditorStaticAPI.SetSplashScreen("androidSplashScreen", ProjectBuildSetting.Current.SplashScreen);
            QuickEditorStaticAPI.SetSplashScreen("iOSLaunchScreenPortrait", ProjectBuildSetting.Current.SplashScreen);
            QuickEditorStaticAPI.SetSplashScreen("iOSLaunchScreenLandscape", ProjectBuildSetting.Current.SplashScreen);
        }

        private static string ResolveExtension(BuildTarget a_target)
        {
            switch (a_target)
            {
                case BuildTarget.StandaloneOSXIntel64:
                    return ".app";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";

                case BuildTarget.iOS:
                    return ".xcode";

                case BuildTarget.Android:
                    return ".apk";

                case BuildTarget.WebGL:
                    return ".html";
            }

            return string.Empty;
        }
    }
}