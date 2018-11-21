namespace QuickEditor.Core
{
    using UnityEditor;
    using UnityEngine;

    public class QEditorHorizontalSplitWindow : QuickEditorWindow
    {
        private QuickEditorGUILayout.SplitViewBlock horizontalSplitView = new QuickEditorGUILayout.SplitViewBlock(QuickEditorGUILayout.SplitViewBlock.Direction.Horizontal);

        protected override void OnGUI()
        {
            base.OnGUI();
            horizontalSplitView.BeginSplitView();
            DrawLeftRect();
            horizontalSplitView.Split();
            DrawRightRect();
            horizontalSplitView.EndSplitView();
        }

        protected virtual void DrawRightRect()
        {
        }

        protected virtual void DrawLeftRect()
        {
        }
    }

    public class QuickEditorWindow : EditorWindow
    {
        protected static string WindowTitle;
        protected static Vector2 WindowRect = new Vector2(800, 600);

        public static T GetEditorWindow<T>() where T : EditorWindow
        {
            return GetWindow<T>(WindowTitle, true)
                    .minSize(WindowRect) as T;
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// 绘制窗口时调用
        /// </summary>
        protected virtual void OnGUI()
        {
        }

        /// <summary>
        /// 获得焦点时调用一次
        /// </summary>
        protected virtual void OnFocus()
        {
        }

        /// <summary>
        /// 丢失焦点时调用一次
        /// </summary>
        protected virtual void OnLostFocus()
        {
        }

        /// <summary>
        /// Hierarchy视图中的任何对象发生改变时调用一次
        /// </summary>
        protected virtual void OnHierarchyChange()
        {
        }

        /// <summary>
        /// Project视图中的资源发生改变时调用一次
        /// </summary>
        protected virtual void OnProjectChange()
        {
        }

        /// <summary>
        /// 面板的更新
        /// </summary>
        protected virtual void OnInspectorUpdate()
        {
            this.Repaint();
        }

        /// <summary>
        /// 处于开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        /// </summary>
        protected virtual void OnSelectionChange()
        {
        }

        /// <summary>
        /// 关闭时调用
        /// </summary>
        protected virtual void OnDestroy()
        {
        }
    }
}