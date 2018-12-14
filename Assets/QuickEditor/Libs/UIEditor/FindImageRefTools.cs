using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FindImageRefTools : EditorWindow
{
    [MenuItem("Assets/Find Common ImageRef")]
    static void FindImageRef()
    {
        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            EditorUtility.DisplayDialog("错误", "游戏正在运行或者暂定，请不要操作！", "确定");
            return;
        }

        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("错误", "游戏脚本正在编译，请不要操作！", "确定");
            return;
        }

        FindImageRefTools win = EditorWindow.GetWindow<FindImageRefTools>("引用查询器");
        if (win == null) return;
        win.Show();

        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        win._targetObjs = objs;
        win.InitPrefabInfo();
        win.CalRef();
    }

    private class RefObject
    {
        public string _path;
        public Dictionary<string, bool> _denpends = new Dictionary<string, bool>();
    }

    private Dictionary<string, RefObject> _prefabList;

    private UnityEngine.Object[] _targetObjs;
    private Dictionary<UnityEngine.Object, int> _targetCount;
    private Dictionary<string, RefObject> _resultObjs;

    private Vector2 _scroll;

    private FindImageRefTools()
    {
        _prefabList = new Dictionary<string, RefObject>();

        _targetCount = new Dictionary<Object, int>();
        _resultObjs = new Dictionary<string, RefObject>();
        
    }

    private void InitPrefabInfo()
    {
        if (_prefabList.Count > 0) return;

        string _f = "Assets/Editor/BuildBundles/Resources/Prefabs";
        string[] prefabPaths = Directory.GetFiles(_f, "*.prefab", SearchOption.AllDirectories);

        foreach(var prafabPath in prefabPaths)
        {
            RefObject refObj = new RefObject();
            string path = prafabPath.Replace("\\", "/");
            refObj._path = path.Substring(path.IndexOf("Assets/Editor"));
            var depends = AssetDatabase.GetDependencies(refObj._path);
            foreach(var depend in depends)
            {
                if(depend.ToLower().EndsWith(".png") || depend.ToLower().EndsWith(".jpg"))
                {
                    refObj._denpends.Add(depend, false);
                }
            }

            _prefabList.Add(refObj._path, refObj);
        }
    }

    private void CalRef()
    {
        _targetCount.Clear();
        _resultObjs.Clear();

        foreach(var prefab in _prefabList.Values)
        {
            string[] keys = new string[prefab._denpends.Count];
            prefab._denpends.Keys.CopyTo(keys, 0);
            foreach(var depend in keys)
            {Debug.Log(prefab._path);
                prefab._denpends[depend] = false;
            }
        }

        foreach(var obj in _targetObjs)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            int count = 0;
            foreach(var prefab in _prefabList.Values)
            {
                if(prefab._denpends.ContainsKey(path))
                {
                    if(!_resultObjs.ContainsKey(prefab._path))
                    {
                        _resultObjs.Add(prefab._path, prefab);
                    }

                    prefab._denpends[path] = true;
                    count++;
                }
            }
            _targetCount.Add(obj, count);
        }
    }

    private Vector2 _scrAtlas = Vector2.zero;
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("查询图片：", GUILayout.Width(100));

        if(GUILayout.Button("刷新", GUILayout.Width(100)))
        {
            _prefabList.Clear();
            InitPrefabInfo();
        }
        EditorGUILayout.EndHorizontal();
        
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        foreach (var obj in _targetObjs)
        {
            EditorGUILayout.ObjectField("引用："+ _targetCount[obj], obj, typeof(Object), false);
        }

        EditorGUILayout.LabelField("引用列表：");

        foreach (var result in _resultObjs.Values)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(result._path, GUILayout.Width(630));
            if (GUILayout.Button("定位", GUILayout.Width(100)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(result._path));
            }
            EditorGUILayout.EndHorizontal();

            foreach (var depend in result._denpends)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(new GUIContent(AssetDatabase.GetCachedIcon(depend.Key)), GUILayout.Height(22), GUILayout.Width(22));
                if (depend.Value) GUI.color = Color.red;
                EditorGUILayout.LabelField(depend.Value.ToString(), GUILayout.Width(100));
                if (depend.Value) GUI.color = Color.white;
                EditorGUILayout.LabelField(depend.Key, GUILayout.Width(500));
                if (GUILayout.Button("定位", GUILayout.Width(100)))
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(depend.Key));
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}