namespace QuickEditor.Toolkit
{
    using QuickEditor.Common.ReorderableList;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ProjectCatalogSettings))]
    public class ProjectCatalogSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty mAssetFoldersProperty;
        private SerializedProperty mResourcesFoldersProperty;

        protected void OnEnable()
        {
            mAssetFoldersProperty = serializedObject.FindProperty("AssetFolders");
            mResourcesFoldersProperty = serializedObject.FindProperty("ResourcesFolders");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ReorderableListGUI.Title("Asset Folders");
            ReorderableListGUI.ListField(mAssetFoldersProperty);
            ReorderableListGUI.Title("Resources Folders");
            ReorderableListGUI.ListField(mResourcesFoldersProperty);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
