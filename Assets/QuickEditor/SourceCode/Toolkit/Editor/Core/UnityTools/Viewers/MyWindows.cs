using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MyWindows : EditorWindow
{

    private static List<GUIStyle> styles = null;

    [MenuItem("Window/styles")]
    public static void Test()
    {
        EditorWindow.GetWindow<MyWindows>("styles");

        styles = new List<GUIStyle>();
        foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            object o = fi.GetValue(null, null);
            if (o.GetType() == typeof(GUIStyle))
            {
                styles.Add(o as GUIStyle);
            }
        }
    }

    public Vector2 scrollPosition = Vector2.zero;

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < styles.Count; i++)
        {
            GUILayout.Label("EditorStyles." + styles[i].name, styles[i]);
        }
        GUILayout.EndScrollView();
    }

}
