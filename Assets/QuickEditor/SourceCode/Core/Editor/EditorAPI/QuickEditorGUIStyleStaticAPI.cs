namespace QuickEditor.Core
{
    using UnityEditor;
    using UnityEngine;

    public static class QuickEditorGUIStyleStaticAPI
    {
        public static GUIStyle GetEditorStyle(string style)
        {
            return EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector).GetStyle(style);
        }

        public static GUIStyle GetColoredTextStyle(Color color, FontStyle fontStyle)
        {
            return new GUIStyle
            {
                normal = { textColor = color },
                fontStyle = fontStyle
            };
        }
    }
}