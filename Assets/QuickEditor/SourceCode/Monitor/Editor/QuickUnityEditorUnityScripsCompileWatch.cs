namespace QuickEditor.Monitor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal sealed partial class QuickUnityEditorUnityScripsCompileWatch
    {
        static QuickUnityEditorUnityScripsCompileWatch()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            QuickEditorAssetPostprocessorWatch.OnPostprocessAllAssetsEvent -= OnPostprocessAllAssets;
            QuickEditorAssetPostprocessorWatch.OnPostprocessAllAssetsEvent += OnPostprocessAllAssets;
        }

        private static void OnPostprocessAllAssets(String[] importedAssets, String[] deletedAssets, String[] movedAssets, String[] movedFromAssetPaths)
        {
            List<string> importedKeys = new List<string>() { "Assets/Scripts", "Editor" };
            for (int i = 0; i < importedAssets.Length; i++)
            {
                for (int j = 0; j < importedKeys.Count; j++)
                {
                    if (importedAssets[i].Contains(importedKeys[j]))
                    {
                        PlayerPrefs.SetInt("ImportScripts", 1);
                        return;
                    }
                }
            }
        }

        private static void Update()
        {
            bool importScripts = Convert.ToBoolean(PlayerPrefs.GetInt("ImportScripts", 1));
            if (importScripts && !EditorApplication.isCompiling)
            {
                OnUnityScripsCompilingCompleted();
                importScripts = false;
                PlayerPrefs.SetInt("ImportScripts", 0);
                EditorApplication.update -= Update;
            }
        }

        private static void OnUnityScripsCompilingCompleted()
        {
            Debug.Log("Unity Scrips Compiling Completed.");
        }
    }
}
