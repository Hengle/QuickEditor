namespace QuickEditor.NGUIToolKit
{
    using QuickEditor.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TexturePackerSetting))]
    public class TexturePackerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            QuickEditorGUIStaticAPI.Space();

            var setting = (TexturePackerSetting)(serializedObject.targetObject);

            QuickEditorGUIStaticAPI.FileTextField("TPCommand", ref setting.TPCommand, "Select TexturePacker Path", "exe");
            QuickEditorGUIStaticAPI.Space();

            QuickEditorGUIStaticAPI.ToggleButton("IsSeperateRGBandAlpha", ref setting.IsSeperateRGBandAlpha);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}