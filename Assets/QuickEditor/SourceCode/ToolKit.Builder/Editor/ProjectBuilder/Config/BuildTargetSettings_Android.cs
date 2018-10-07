/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Builder
{
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class BuildTargetSettings_Android : IBuildTargetSettings
    {
        public BuildTarget buildTarget { get { return BuildTarget.Android; } }
        public Texture icon { get { return EditorGUIUtility.FindTexture("BuildSettings.WebGL.Small"); } }

        [Tooltip("Keystore file path.")]
        public string keystoreFile = "";

        [Tooltip("Keystore password.")]
        public string keystorePassword = "";

        [Tooltip("Keystore alias name.")]
        public string keystoreAliasName = "";

        [Tooltip("Keystore alias password.")]
        public string keystoreAliasPassword = "";

        public void Reset()
        {
        }

        public void ApplySettings(ProjectBuildSetting builder)
        {
        }

        public void DrawSetting(SerializedObject serializedObject)
        {
        }
    }
}