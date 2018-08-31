namespace QuickEditor.Toolkit
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public class QEditorGUIStyleViewerWindow : QEditorWindow
    {
        internal const int GUIStyleViewerNodePriority = 100;
        internal const string GUIStyleViewerNodeName = QEditorDefines.ToolkitRootNodeName + "GUI Style Viewer";
        private Vector2 scrollPosition = new Vector2(0, 0);

        private string search = "";

        private int mSelectedTabIndex = 0;
        private string[] mAssetTabsText = new string[] { "Styles", "Icons" };

        [MenuItem(GUIStyleViewerNodeName, false, GUIStyleViewerNodePriority)]
        public static void Open()
        {
            Title = "GUIStyle Viewer";
            GetWindow<QEditorGUIStyleViewerWindow>(Title, true)
               .minSize(new Vector2(800, 600));
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            QEditorGUIStaticAPI.Toolbar(ref mSelectedTabIndex, mAssetTabsText, EditorStyles.toolbarButton);
            GUILayout.BeginHorizontal("HelpBox");

            GUILayout.Label("Click a Sample to copy its Name to your Clipboard", "MiniBoldLabel");

            GUILayout.FlexibleSpace();

            GUILayout.Label("Search:");

            search = EditorGUILayout.TextField(search);

            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (GUIStyle style in GUI.skin.customStyles)
            {

                if (style.name.ToLower().Contains(search.ToLower()))
                {

                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");

                    GUILayout.Space(7);

                    if (GUILayout.Button(style.name, style))
                    {

                        EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";

                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");

                    GUILayout.EndHorizontal();

                    GUILayout.Space(11);

                }

            }

            GUILayout.EndScrollView();

        }
    }
}
