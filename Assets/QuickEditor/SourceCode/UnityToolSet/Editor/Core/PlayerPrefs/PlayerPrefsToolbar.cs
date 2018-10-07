namespace QuickEditor.UnityToolSet
{
    using UnityEditor;
    using UnityEngine;

    public class PlayerPrefsToolbar
    {
        private const int PlayerPrefsNodePriority = 6000;
        private const string PlayerPrefsNodeName = UnityToolSetConstants.UnityToolSetRootNodeName + "Player Prefs/";

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