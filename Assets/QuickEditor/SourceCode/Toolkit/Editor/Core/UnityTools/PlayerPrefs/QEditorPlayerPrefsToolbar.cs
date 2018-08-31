namespace QuickEditor.Toolkit
{
    using UnityEditor;
    using UnityEngine;

    public class QEditorPlayerPrefsToolbar
    {
        private const int PlayerPrefsNodePriority = 6000;
        private const string PlayerPrefsNodeName = QEditorDefines.ToolkitRootNodeName + "Unity Toolbar/Player Prefs/";

        [MenuItem(PlayerPrefsNodeName + "CleanPlayerPrefs", false, PlayerPrefsNodePriority)]
        public static void CleanPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem(PlayerPrefsNodeName + "CleanEditorPrefs", false, PlayerPrefsNodePriority)]
        public static void CleanEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }
    }
}