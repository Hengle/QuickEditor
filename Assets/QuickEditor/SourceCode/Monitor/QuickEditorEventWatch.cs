namespace QuickEditor.Monitor
{
    using System;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    [InitializeOnLoad]
    internal sealed partial class QuickEditorEventWatch
    {
        private static EditorApplication.CallbackFunction mHierarchyWindowChangedEvent;

        public static event EditorApplication.CallbackFunction OnHierarchyWindowChangedEvent
        {
            add { mHierarchyWindowChangedEvent += value; }
            remove { mHierarchyWindowChangedEvent -= value; }
        }

        private static EditorApplication.ProjectWindowItemCallback mProjectWindowItemCallback;

        public static event EditorApplication.ProjectWindowItemCallback OnProjectWindowItemCallback
        {
            add { mProjectWindowItemCallback += value; }
            remove { mProjectWindowItemCallback -= value; }
        }

        private static EditorApplication.CallbackFunction mOnEditorUpdateEvent;

        public static event EditorApplication.CallbackFunction OnEditorUpdateEvent
        {
            add { mOnEditorUpdateEvent += value; }
            remove { mOnEditorUpdateEvent -= value; }
        }

        /// <summary>
        /// 将要播放游戏事件
        /// </summary>
        private static EditorApplication.CallbackFunction mOnWillPlayEvent;

        public static event EditorApplication.CallbackFunction OnWillPlayEvent
        {
            add { mOnEditorUpdateEvent += value; }
            remove { mOnEditorUpdateEvent -= value; }
        }

        /// <summary>
        /// 进入播放时刻事件
        /// </summary>
        private static EditorApplication.CallbackFunction mOnBeginPlayEvent;

        public static event EditorApplication.CallbackFunction OnBeginPlayEvent
        {
            add { mOnEditorUpdateEvent += value; }
            remove { mOnEditorUpdateEvent -= value; }
        }

        /// <summary>
        /// 将要停止游戏 (不包括暂停哦)
        /// </summary>
        private static EditorApplication.CallbackFunction mOnWillStopEvent;

        public static event EditorApplication.CallbackFunction OnWillStopEvent
        {
            add { mOnEditorUpdateEvent += value; }
            remove { mOnEditorUpdateEvent -= value; }
        }

        /// <summary>
        /// 编译前事件，比较特殊的处理，配合了PostBuildProcess和PostBuildScene
        /// </summary>
        private static Action mOnBeforeBuildPlayerEvent;

        public static event Action OnBeforeBuildPlayerEvent
        {
            add { mOnBeforeBuildPlayerEvent += value; }
            remove { mOnBeforeBuildPlayerEvent -= value; }
        }

        /// <summary>
        /// 编译完成后事件
        /// </summary>
        private static Action<BuildTarget, string> mOnPostBuildPlayerEvent;

        public static event Action<BuildTarget, string> OnPostBuildPlayerEvent
        {
            add { mOnPostBuildPlayerEvent += value; }
            remove { mOnPostBuildPlayerEvent -= value; }
        }

        /// <summary>
        /// 程序集锁定事件，事件中可以进行DLL的注入修改
        /// </summary>
        private static Action mOnLockingAssembly;

        public static event Action OnLockingAssembly
        {
            add { mOnLockingAssembly += value; }
            remove { mOnLockingAssembly -= value; }
        }

        /// <summary>
        /// 是否静态构造完成
        /// </summary>
        public static bool IsInited { get; private set; }

        static QuickEditorEventWatch()
        {
            EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;

            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

            EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;

            EditorApplication.projectWindowChanged -= OnProjectWindowChanged;
            EditorApplication.projectWindowChanged += OnProjectWindowChanged;

            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

            EditorApplication.searchChanged -= OnSearchChanged;
            EditorApplication.searchChanged += OnSearchChanged;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            EditorUserBuildSettings.activeBuildTargetChanged -= OnActiveBuildTargetChanged;
            EditorUserBuildSettings.activeBuildTargetChanged += OnActiveBuildTargetChanged;

            SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
            SceneView.onSceneGUIDelegate += OnSceneViewGUI;

            PrefabUtility.prefabInstanceUpdated -= OnPrefabInstanceUpdated;
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated;

            if (IsInited && mOnLockingAssembly != null)
            {
                EditorApplication.LockReloadAssemblies();
                mOnLockingAssembly();
                EditorApplication.UnlockReloadAssemblies();
            }

            IsInited = true;
        }

        private static void OnHierarchyWindowChanged()
        {
            if (IsInited && mHierarchyWindowChangedEvent != null)
            {
                mHierarchyWindowChangedEvent();
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {

        }

        private static void OnPlayModeStateChanged()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (!EditorApplication.isPlaying)
                {
                    if (IsInited && mOnWillPlayEvent != null)
                    {
                        mOnWillPlayEvent();
                    }
                }
                else
                {
                    if (IsInited && mOnBeginPlayEvent != null)
                    {
                        mOnBeginPlayEvent();
                    }
                }
            }
            else
            {
                if (EditorApplication.isPlaying)
                {
                    if (IsInited && mOnWillStopEvent != null)
                    {
                        mOnWillStopEvent();
                    }
                }
            }
        }

        private static void OnProjectWindowChanged()
        {

        }

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (mProjectWindowItemCallback != null)
            {
                mProjectWindowItemCallback(guid, selectionRect);
            }
        }

        private static void OnSearchChanged()
        {

        }

        private static void OnEditorUpdate()
        {
            CheckComplie();
            if (IsInited && mOnEditorUpdateEvent != null)
            {
                mOnEditorUpdateEvent();
            }
        }

        private static void OnActiveBuildTargetChanged()
        {
            UnityEngine.Debug.Log("当前平台为: " + EditorUserBuildSettings.activeBuildTarget);
        }

        private static void OnSceneViewGUI(SceneView sceneview)
        {
            Event e = Event.current;
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        UnityEngine.Object handleObj = DragAndDrop.objectReferences[i];
                        if (handleObj != null)
                        {
                            Debug.LogError(handleObj.GetType());
                        }
                    }
                }
            }
            CheckComplie();
        }

        private static void OnPrefabInstanceUpdated(GameObject instance)
        {
            GameObject go = null;
            if (Selection.activeTransform)
            {
                go = Selection.activeGameObject;
            }
            AssetDatabase.SaveAssets();
            if (go)
            {
                EditorApplication.delayCall = delegate
                {
                    Selection.activeGameObject = go;
                };
            }
        }

        private static void CheckComplie()
        {
            // 检查编译中，立刻暂停游戏！
            if (EditorApplication.isCompiling)
            {
                if (EditorApplication.isPlaying)
                {
                    UnityEngine.Debug.Log("Force Stop Play, Because of Compiling.");
                    EditorApplication.isPlaying = false;
                }
            }
        }

        private static bool mBeforeBuildFlag = false;

        [PostProcessScene]
        private static void OnProcessScene()
        {
            if (!mBeforeBuildFlag && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                mBeforeBuildFlag = true;
                if (IsInited && mOnBeforeBuildPlayerEvent != null)
                    mOnBeforeBuildPlayerEvent();
            }
        }

        /// <summary>
        /// Unity标准Build后处理函数
        /// </summary>
        [PostProcessBuild()]
        private static void OnPostBuildPlayer(BuildTarget target, string pathToBuiltProject)
        {
            if (IsInited && mOnPostBuildPlayerEvent != null)
            {
                mOnPostBuildPlayerEvent(target, pathToBuiltProject);
            }

            UnityEngine.Debug.Log(string.Format("Success build ({0}) : {1}", target, pathToBuiltProject));
        }
    }

    public enum PlayModeState
    {
        Playing,
        Paused,
        Stop,
        PlayingOrWillChangePlaymode
    }

}
