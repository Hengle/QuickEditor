/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Builder
{
    using UnityEditor;
    using UnityEngine;

    public interface IBuildTargetSettings
    {
        BuildTarget buildTarget { get; }

        Texture icon { get; }

        void Reset();

        void ApplySettings(ProjectBuildSetting builder);

        void DrawSetting(SerializedObject serializedObject);
    }
}