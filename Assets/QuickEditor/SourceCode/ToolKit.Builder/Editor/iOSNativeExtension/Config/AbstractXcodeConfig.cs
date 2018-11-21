namespace QuickEditor.Builder
{
    using QuickEditor.Core;
    using UnityEditor;
    using UnityEngine;

    public abstract class AbstractXcodeConfig
    {
        [SerializeField]
        public string SaveName = "New Xcode Project Setting";

        [SerializeField]
        public bool Enabled = true;

        public abstract void DrawInnerGUI();

        protected void DrawFilterGUI()
        {
            EditorGUILayout.LabelField("Xcode Project Setting", EditorStyles.boldLabel);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), QuickEditorColors.DarkGrayX11);
            GUILayout.Space(3);

            SaveName = EditorGUILayout.TextField(new GUIContent("Name", "Only used for organisation"), SaveName);
        }
    }
}
