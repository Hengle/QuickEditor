namespace QuickEditor.NGUIToolKit
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TexturePackerSetting))]
    public class TexturePackerSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            QEditorGUIStaticAPI.Space();

            var setting = (TexturePackerSetting)(serializedObject.targetObject);

            QEditorGUIStaticAPI.FileTextField("TPCommand", ref setting.TPCommand, "Select TexturePacker Path", "exe");
            QEditorGUIStaticAPI.Space();

            QEditorGUIStaticAPI.ToggleButton("IsSeperateRGBandAlpha", ref setting.IsSeperateRGBandAlpha);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}