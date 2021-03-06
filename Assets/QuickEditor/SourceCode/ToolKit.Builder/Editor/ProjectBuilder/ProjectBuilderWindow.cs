﻿/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Builder
{
    using QuickEditor.Core;
    using UnityEditor;
    using UnityEngine;

    public class ProjectBuilderWindow : QEditorHorizontalSplitWindow
    {
        public static void Open()
        {
            WindowTitle = "Project Builder";
            GetEditorWindow<ProjectBuilderWindow>();
        }

        private SerializedObject mSerializedObject;
        private ProjectBuildSetting mAppBuildSetting;
        private Vector2 mScrollPos;
        private int mSelectedTabIndex = 0;
        private string[] mAssetTabsText = new string[] { "Model", "Texture", "Audio" };

        protected override void OnEnable()
        {
            mAppBuildSetting = ProjectBuildSetting.Current;
            mSerializedObject = new SerializedObject(mAppBuildSetting);
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected override void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        protected override void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnSelectionChanged()
        {
            Repaint();
        }

        private string path;

        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(mAppBuildSetting);
            }
        }

        protected override void DrawLeftRect()
        {
            base.DrawLeftRect();
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            QuickEditorGUIStaticAPI.Toolbar(ref mSelectedTabIndex, mAssetTabsText, EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();
        }

        protected override void DrawRightRect()
        {
            using (new QuickEditorGUILayout.ScrollViewBlock(ref mScrollPos))
            {
                DrawOnekeyPackGeneralSettings(mAppBuildSetting);
                using (new QuickEditorGUILayout.IndentBlock())
                {
                    DrawOnekeyPackAndroidSettings(mAppBuildSetting);
                }

                DrawOnekeyPackIOSSettings(mAppBuildSetting);
                DrawOnekeyPackMenus(mAppBuildSetting);
                mSerializedObject.ApplyModifiedProperties();
            }
        }

        private bool mGeneralSettingsFoldout;
        private bool mAndroidSettingsFoldout;
        private bool mIOSSettingsFoldout;

        private void DrawOnekeyPackGeneralSettings(ProjectBuildSetting settings)
        {
            QuickEditorGUIStaticAPI.DrawFoldableBlock(ref mGeneralSettingsFoldout, "Application Build Setting", () =>
            {
                QuickEditorGUIStaticAPI.DragAndDropTextField("name", ref settings.productName);

                QuickEditorGUIStaticAPI.TextField("Product Name", ref settings.productName);
                QuickEditorGUIStaticAPI.TextField("Company Name", ref settings.companyName);
                QuickEditorGUIStaticAPI.TextField("Bundle Identifier", ref settings.applicationIdentifier);

                QuickEditorGUIStaticAPI.TextField("Version", ref settings.Version);
                QuickEditorGUIStaticAPI.TextField("Bundle Version Code", ref settings.BundleVersionCode);
                QuickEditorGUIStaticAPI.FileTextField("splashScreenAssetPath", ref settings.splashScreenAssetPath, "选择文件", "png");
                QuickEditorGUIStaticAPI.FileTextField("iconAssetPath", ref settings.iconAssetPath, "选择文件", "png");
            });
        }

        private void DrawOnekeyPackAndroidSettings(ProjectBuildSetting settings)
        {
            QuickEditorGUIStaticAPI.DrawFoldableBlock(ref mAndroidSettingsFoldout, "Android Setting", () =>
            {
                QuickEditorGUIStaticAPI.FileTextField("KeystoreName", ref settings.AndroidKeystoreName, "选择文件", "keystore");

                QuickEditorGUIStaticAPI.TextField("KeystorePass", ref settings.AndroidKeystorePass);
                settings.AndroidKeystorePass = EditorGUILayout.TextField("KeystorePass", settings.AndroidKeystorePass);
                settings.AndroidKeyaliasName = EditorGUILayout.TextField("KeyaliasName", settings.AndroidKeyaliasName);
                settings.AndroidKeyaliasPass = EditorGUILayout.TextField("KeyaliasPass", settings.AndroidKeyaliasPass);
                settings.ForceSDCardPermission = EditorGUILayout.Toggle("ForceSDCardPermission", settings.ForceSDCardPermission);
                settings.AndroidTargetDevice = (AndroidArchitecture)EditorGUILayout.EnumPopup("Device Filter", settings.AndroidTargetDevice);
                settings.AndroidMinSdkVersion = (AndroidSdkVersions)EditorGUILayout.EnumPopup("Min API Level", settings.AndroidMinSdkVersion);
                settings.AndroidPreferredInstallLocation = (AndroidPreferredInstallLocation)EditorGUILayout.EnumPopup("Install Location", settings.AndroidPreferredInstallLocation);
                settings.AndroidScriptingBackend = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting Backend", settings.AndroidScriptingBackend);
            });
        }

        private void DrawOnekeyPackIOSSettings(ProjectBuildSetting settings)
        {
            QuickEditorGUIStaticAPI.DrawFoldableBlock(ref mIOSSettingsFoldout, "IOS Setting", () =>
            {
                settings.iOSScriptingBackend = (ScriptingImplementation)EditorGUILayout.EnumPopup("Scripting Backend", settings.iOSScriptingBackend);
                settings.iOSTargetDevice = (iOSTargetDevice)EditorGUILayout.EnumPopup("Target Device", settings.iOSTargetDevice);
                settings.iOSSdkVersion = (iOSSdkVersion)EditorGUILayout.EnumPopup("Target SDK", settings.iOSSdkVersion);
            });
        }

        private void DrawOnekeyPackMenus(ProjectBuildSetting mOnekeyPackSettings)
        {
            EditorGUILayout.Space();
            using (new QuickEditorGUILayout.HorizontalBlock())
            {
                //if (GUILayout.Button("Reset"))
                //{
                //    mOnekeyPackSettings.ApplyDefaults();
                //}
                QuickEditorGUIStaticAPI.Button(new GUIContent("Apply Setting", EditorGUIUtility.FindTexture("vcs_check")), mOnekeyPackSettings.ApplyDefaults);

                if (GUILayout.Button(new GUIContent("Apply Setting", EditorGUIUtility.FindTexture("vcs_check"))))
                {
                    mOnekeyPackSettings.ApplyDefaults();
                }
                if (GUILayout.Button(new GUIContent("Player Setting", EditorGUIUtility.FindTexture("EditorSettings Icon")), GUILayout.Height(21), GUILayout.Width(110)))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
                }

                //if (GUILayout.Button("Reset"))
                //{
                //    mOnekeyPackSettings.ApplyDefaults();
                //}
            }
        }
    }
}