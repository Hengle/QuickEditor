namespace QuickEditor.UnityToolSet
{
    using QuickEditor.Common;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class UnityBuiltInResourcesViewerWindow : QEditorWindow
    {
        internal const int BuiltInResourcesViewerNodePriority = 100;
        internal const string BuiltInResourcesViewerNodeName = UnityToolSetConstants.UnityToolSetRootNodeName + "Built-in Resources Viewer";

        protected int IconColumn = 8;
        protected float mRectWidth;
        protected string mSearchContent = "";
        protected Vector2 mScrollPosition = new Vector2(0, 0);
        protected int mSelectedTabIndex = 0;
        protected string[] mTabsText = new string[] { "Styles", "Icons" };

        [MenuItem(BuiltInResourcesViewerNodeName, false, BuiltInResourcesViewerNodePriority)]
        public static void Open()
        {
            WindowTitle = "Built-in Resources Viewer";
            GetEditorWindow<UnityBuiltInResourcesViewerWindow>();
        }

        protected List<string> mIconNames = null;
        protected List<GUIStyle> mStyles = null;

        private void Awake()
        {
            mIconNames = new List<string>(); ;
            Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D x in t)
            {
                Debug.unityLogger.logEnabled = false;
                GUIContent gc = EditorGUIUtility.IconContent(x.name);
                Debug.unityLogger.logEnabled = true;
                if (gc != null && gc.image != null)
                {
                    mIconNames.Add(x.name);
                }
            }
            mIconNames.Sort((iconA, iconB) => string.Compare(iconA, iconB, System.StringComparison.OrdinalIgnoreCase));

            mStyles = new List<GUIStyle>();
            foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object o = fi.GetValue(null, null);
                if (o.GetType() == typeof(GUIStyle))
                {
                    mStyles.Add(o as GUIStyle);
                }
            }
        }

        protected override void OnGUI()
        {
            QEditorGUIStaticAPI.Toolbar(ref mSelectedTabIndex, mTabsText, EditorStyles.toolbarButton);
            GUILayout.BeginHorizontal("HelpBox");

            GUILayout.Label("Click a Sample to copy its Name to your Clipboard", "MiniBoldLabel");

            GUILayout.FlexibleSpace();

            GUILayout.Label("Search:");

            mSearchContent = EditorGUILayout.TextField(mSearchContent);

            GUILayout.EndHorizontal();

            switch (mSelectedTabIndex)
            {
                case 0:
                    DrawStyles();
                    break;

                case 1:
                    DrawIcons();
                    break;

                default:
                    break;
            }
        }

        private void DrawStyles()
        {
            //if (mStyles == null)
            //{
            //    mStyles = new List<GUIStyle>();
            //    foreach (GUIStyle style in GUI.skin.customStyles)
            //    {
            //        mStyles.Add(style);
            //    }
            //}

            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);
            if (mStyles == null || mStyles.Count < 1) { return; }
            foreach (GUIStyle style in mStyles)
            {
                if (style.name.ToLower().Contains(mSearchContent.ToLower()))
                {
                    DrawStyleItem(style);
                }
            }
            GUILayout.EndScrollView();
        }

        private void DrawStyleItem(GUIStyle style)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box");
            GUILayout.Label("EditorStyles." + style.name, style);
            if (GUILayout.Button("复制到剪贴板"))
            {
                CopyText(style.name);
                Debug.Log("Style name: " + style.name + "  [Copied]");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawIcons()
        {
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);
            mRectWidth = WindowRect.x / IconColumn;
            for (int i = 0; i < mIconNames.Count; i += IconColumn)
            {
                GUILayout.BeginHorizontal();
                //if (style.name.ToLower().Contains(search.ToLower()))

                for (int j = 0; j < IconColumn; j++)
                {
                    int index = i + j;
                    if (index < mIconNames.Count)
                    {
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button(EditorGUIUtility.IconContent(mIconNames[index]),
                            GUILayout.Width(mRectWidth), GUILayout.Height(50)))
                        {
                            CopyText(mIconNames[i]);
                            Debug.Log("Icon name: " + mIconNames[i] + "  [Copied]");
                        }
                        GUILayout.TextField(mIconNames[i], GUILayout.Width(mRectWidth), GUILayout.Height(20));
                        EditorGUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private static void CopyText(string text)
        {
            TextEditor mTextEditor = new TextEditor();
            mTextEditor.text = text;
            mTextEditor.SelectAll();
            mTextEditor.Copy();
        }
    }
}