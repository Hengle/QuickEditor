namespace QuickEditor.Toolkit
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using QuickEditor.Common;

    public class GEditorEditRecentProjectsWindow : QEditorWindow
    {
        internal const int EditRecentProjectsNodePriority = 7000;
        internal const string EditRecentProjectsNodeName = QEditorDefines.ToolkitRootNodeName + "Unity Toolbar/";

        internal const string RECENTLY_USED_PROJECT_KEY = "RecentlyUsedProjectPaths-";

        private Dictionary<string, string> recentProjects = new Dictionary<string, string>();
        private Vector2 scrollPos;

        [MenuItem(EditRecentProjectsNodeName + "Edit Recent Projects", false, EditRecentProjectsNodePriority)]
        public static void ShowWindow()
        {
            var window = GetWindow<GEditorEditRecentProjectsWindow>(false, "Edit Recent Projects", true);
            window.minSize = new Vector2(700, 400);
        }

        #region lifecycle methods

        protected override void OnEnable()
        {
            LoadRecentProjects();
        }

        protected override void OnGUI()
        {
            GUILayout.Space(15);
            EditorGUILayout.HelpBox("Tired of long list of recent projects in \"File -> Open Project...\" popup?\n" +
                "Now you can remove unneeded projects with one click. Enjoy!", MessageType.None);
            if (recentProjects.Count > 0)
            {
                DrawRecentProjects();
                GUILayout.Space(15);
                DrawClearAllRecentProjects();
            }
            else
            {
                EditorGUILayout.LabelField("No Recent Projects", EditorStyles.boldLabel);
            }
        }

        #endregion lifecycle methods

        #region private methods

        private void DrawRecentProjects()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Recent Projects", EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(300));

            foreach (var prefKey in recentProjects.Keys.ToArray())
            {
                var recentProject = recentProjects[prefKey];

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);

                if (GUILayout.Button(new GUIContent("x", "Remove project"), EditorStyles.miniButtonMid, GUILayout.Width(20f), GUILayout.Height(17f)))
                {
                    RemoveRecentProject(prefKey);
                }

                EditorGUILayout.LabelField(new GUIContent(recentProject, recentProject));

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawClearAllRecentProjects()
        {
            var color = GUI.color;
            GUI.color = Color.red;

            if (GUILayout.Button(new GUIContent("Clear All Recent Projects", "WARNING: You can't undone this operation!")))
            {
                RemoveAllRecentProjects();
            }

            GUI.color = color;
        }

        private void LoadRecentProjects()
        {
            recentProjects.Clear();

            string path = null;
            int i = 0;

            while (true)
            {
                var prefKey = RECENTLY_USED_PROJECT_KEY + i;

                if (EditorPrefs.HasKey(prefKey))
                {
                    path = EditorPrefs.GetString(prefKey);
                    recentProjects[prefKey] = path;
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        private void RemoveRecentProject(string projectKeyToRemove)
        {
            foreach (var prefKey in recentProjects.Keys)
            {
                EditorPrefs.DeleteKey(prefKey);
            }

            recentProjects.Remove(projectKeyToRemove);

            var recentProjectsValues = recentProjects.Values.ToArray();
            for (int i = 0; i < recentProjectsValues.Length; i++)
            {
                EditorPrefs.SetString(RECENTLY_USED_PROJECT_KEY + i, recentProjectsValues[i]);
            }
        }

        private void RemoveAllRecentProjects()
        {
            foreach (var prefKey in recentProjects.Keys)
            {
                EditorPrefs.DeleteKey(prefKey);
            }
            recentProjects.Clear();
        }

        #endregion private methods
    }
}