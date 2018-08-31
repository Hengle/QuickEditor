/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace QuickEditor.Common
{
    using UnityEditor;

    public class QEditorWindow : EditorWindow
    {

        protected static string Title;

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
