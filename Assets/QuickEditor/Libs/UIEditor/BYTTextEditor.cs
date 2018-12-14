using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BYXText))]
public class BYTTextEditor : UnityEditor.UI.TextEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BYXText byxText = target as BYXText;

        byxText.isSync = EditorGUILayout.Toggle("isSyncLoad", byxText.isSync);
    }
}