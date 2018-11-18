/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Builder
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class ProjectBuildSetting : QScriptableObject<ProjectBuildSetting>
    {
        [Tooltip("Buid Application.")]
        public bool buildApplication = true;

        [Tooltip("Product Name.")]
        public string productName;

        [Tooltip("Company Name.")]
        public string companyName;

        [Tooltip("Bundle Identifier.")]
        public string applicationIdentifier;

        [Tooltip("Icon Asset Path.")]
        public string iconAssetPath;

        [Tooltip("Splash Screen Asset Path.")]
        public string splashScreenAssetPath;

        public Texture2D Icon { get { return QEditorAssetStaticAPI.LoadAsset<Texture2D>(iconAssetPath); } }
        public Texture2D SplashScreen { get { return QEditorAssetStaticAPI.LoadAsset<Texture2D>(splashScreenAssetPath); } }
        public string Version;
        public string BundleVersionCode;
        public string SplashScreenPath;
        public string IconPath;

        public string AndroidKeystoreName;
        public string AndroidKeystorePass;
        public string AndroidKeyaliasName;
        public string AndroidKeyaliasPass;
        public ScriptingImplementation AndroidScriptingBackend;
        public bool ForceSDCardPermission;
        public AndroidArchitecture AndroidTargetDevice;
        public AndroidSdkVersions AndroidMinSdkVersion;
        public AndroidPreferredInstallLocation AndroidPreferredInstallLocation;

        public ScriptingImplementation iOSScriptingBackend;
        public iOSSdkVersion iOSSdkVersion;
        public iOSTargetDevice iOSTargetDevice;

        public BuildTargetSettings_Android androidSettings = new BuildTargetSettings_Android();

        public void ApplyDefaults()
        {
            applicationIdentifier = string.Empty;
            Version = string.Empty;
            BundleVersionCode = string.Empty;
            AndroidKeystoreName = "Assets/_keystore/user.keystore";
            AndroidKeystorePass = "henry890307";
            AndroidKeyaliasName = string.Empty;
            AndroidKeyaliasPass = string.Empty;
            AndroidTargetDevice = AndroidArchitecture.ARMv7;
            AndroidMinSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
            AndroidPreferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            AndroidScriptingBackend = ScriptingImplementation.Mono2x;

            iOSScriptingBackend = ScriptingImplementation.IL2CPP;
            iOSSdkVersion = iOSSdkVersion.DeviceSDK;
            iOSTargetDevice = iOSTargetDevice.iPhoneAndiPad;
        }
    }
}
