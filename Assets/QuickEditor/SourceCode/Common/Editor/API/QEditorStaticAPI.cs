namespace QuickEditor.Common
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class QEditorStaticAPI
    {
        public static void SetAppIcon(BuildTargetGroup group, Texture2D texture)
        {
            var count = PlayerSettings.GetIconSizesForTargetGroup(group).Length;

            var textures = new List<Texture2D>();
            for (int i = 0; i < count; i++)
            {
                textures.Add(texture);
            }
            PlayerSettings.SetIconsForTargetGroup(group, textures.ToArray());
        }

        public static bool SetSplashScreen(string name, Texture2D texture)
        {
            if (texture == null) { return false; }
            var property = typeof(PlayerSettings).Invoke("FindProperty", name) as SerializedProperty;
            property.serializedObject.Update();
            property.objectReferenceValue = texture;
            property.serializedObject.ApplyModifiedProperties();
            return texture != null;
        }
    }
}