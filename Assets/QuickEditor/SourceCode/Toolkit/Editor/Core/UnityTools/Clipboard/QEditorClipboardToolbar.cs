namespace QuickEditor.Toolkit
{
    using QuickEditor.Common;
    using UnityEditor;
    using UnityEngine;

    public class QEditorClipboardToolbar
    {
        #region Hierarchy view extension

        [MenuItem("GameObject/ToolSet/Clipboard/Copy Path", true, 100)]
        public static bool CheckCopyHierarchyName()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/ToolSet/Clipboard/Copy Path", false, 100)]
        public static void CopyHierarchyName()
        {
            GameObject go = Selection.activeGameObject;
            string hierarchyName = GetNodePath(go.transform);
            CopyStringToPasteboard(hierarchyName);
            Debug.LogFormat(go, "Current selected gameObject hierarchy name :{0}", hierarchyName);
        }

        public static string GetNodePath(Transform trans)
        {
            if (null == trans) return string.Empty;
            if (null == trans.parent) return trans.name;
            return GetNodePath(trans.parent) + "/" + trans.name;
        }

        #endregion Hierarchy view extension

        #region Project view extension

        [MenuItem("Assets/ToolSet/Clipboard/Copy Resource Path to ClipBoard")]
        public static void CopyResourcePath()
        {
            Object obj = Selection.activeObject;
            Debug.Assert(obj != null, "must select a asset in project view to copy its resourcePath!");
            string assetPath = AssetDatabase.GetAssetPath(obj);
            Debug.LogFormat("assetPath is :{0}", assetPath);
            const string resourcesFolderName = "Resources";
            int startIndex = assetPath.LastIndexOf(resourcesFolderName);
            if (startIndex >= 0)
            {
                startIndex = startIndex + resourcesFolderName.Length + 1;
                int endIndex = assetPath.LastIndexOf(".");
                int length = endIndex - startIndex;
                if (length > 0)
                {
                    string resourcePath = assetPath.Substring(startIndex, length);
                    CopyStringToPasteboard(resourcePath);
                    Debug.LogFormat("resourcePath is :{0}", resourcePath);
                }
                else
                {
                    Debug.LogErrorFormat("invalid resource Path!");
                }
            }
            else
            {
                Debug.LogErrorFormat("selected asset is not within the Resources Folder!");
            }
        }

        [MenuItem("Assets/ToolSet/Clipboard/Copy Asset Path to ClipBoard", true)]
        public static bool CheckCopyAssetPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return Selection.activeObject != null && !path.IsNullOrEmpty();
        }

        [MenuItem("Assets/ToolSet/Clipboard/Copy Asset Path to ClipBoard", false)]
        public static void CopyAssetPath()
        {
            Object obj = Selection.activeObject;
            Debug.Assert(obj != null, "must select a asset in project view to copy its assetPath!");
            string assetPath = AssetDatabase.GetAssetPath(obj);
            Debug.LogFormat("assetPath is :{0}", assetPath);
            CopyStringToPasteboard(assetPath);
        }

        #endregion Project view extension

        #region other

        public static void CopyStringToPasteboard(string content)
        {
            GUIUtility.systemCopyBuffer = content;
        }

        #endregion other

        private static Component[] copiedComponents;

        [MenuItem("GameObject/ToolSet/Clipboard/Copy Current Components #&C")]
        private static void CopyCurrentComponents()
        {
            copiedComponents = Selection.activeGameObject.GetComponents<Component>();
        }

        [MenuItem("GameObject/ToolSet/Clipboard/Paste Current Components #&P")]
        private static void PasteCurrentComponents()
        {
            foreach (var targetGameObject in Selection.gameObjects)
            {
                if (!targetGameObject || copiedComponents == null) continue;
                foreach (var copiedComponent in copiedComponents)
                {
                    if (!copiedComponent) continue;
                    UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
                }
            }
        }
    }
}
