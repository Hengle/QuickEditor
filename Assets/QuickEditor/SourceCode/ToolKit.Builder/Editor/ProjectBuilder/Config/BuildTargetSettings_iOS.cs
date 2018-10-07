/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Builder
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class BuildTargetSettings_iOS : IBuildTargetSettings
    {
        public BuildTarget buildTarget { get { return BuildTarget.iOS; } }
        public Texture icon { get { return EditorGUIUtility.FindTexture("BuildSettings.WebGL.Small"); } }

        [Tooltip("Keystore file path.")]
        public ScriptingImplementation scriptingBackend;

        public iOSSdkVersion sdkVersion;
        public iOSTargetDevice targetDevice;

        [Tooltip("Target Minimum iOS Version.")]
        public string targetMinimumiOSVersion;

        //public
        public void Reset()
        {
        }

        public enum Architecture
        {
            X86,
            X64,
            Arm,
            Arm64
        }

        public void ApplySettings(ProjectBuildSetting builder)
        {
        }

        public void DrawSetting(SerializedObject serializedObject)
        {
        }
    }
}