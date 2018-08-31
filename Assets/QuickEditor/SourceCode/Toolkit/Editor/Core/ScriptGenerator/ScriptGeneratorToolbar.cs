namespace QuickEditor.Toolkit
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class ScriptGeneratorToolbar
    {
        private const int ScriptGeneratorNodePriority = 5000;
        private const string ScriptGeneratorNodeName = QEditorDefines.ToolkitRootNodeName + "Script Generator Toolbar/";

        [MenuItem(ScriptGeneratorNodeName + "New Script...", false, ScriptGeneratorNodePriority)]
        public static void OpenFromComponentMenu()
        {
            NewScriptWindow.OpenFromComponentMenu();
        }

        [MenuItem(ScriptGeneratorNodeName + "New Script...", true, ScriptGeneratorNodePriority)]
        public static bool OpenFromComponentMenuValidation()
        {
            return NewScriptWindow.OpenFromComponentMenuValidation();
        }

        [MenuItem(ScriptGeneratorNodeName + "Script...", false, ScriptGeneratorNodePriority + 100)]
        public static void OpenFromAssetsMenu()
        {
            NewScriptWindow.OpenFromAssetsMenu();
        }
    }
}