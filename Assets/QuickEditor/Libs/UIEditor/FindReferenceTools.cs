using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 用于处理寻找物体引用
/// 并显示出来
/// </summary>
public class FindReferenceTools : EditorWindow
{
    public enum FindMode
    {
        Reference,
        BeReference,
    }

    public class RefItem
    {
        private Object _object;
        public string _path;
        public List<RefItem> _refList = new List<RefItem>();
        public bool isShowChild = true;

        public void InitRef(string path)
        {
            _path = path;
            string[] paths = AssetDatabase.GetDependencies(_path, false);
            for (int i = 0; i < paths.Length; i++)
            {
                RefItem item = new RefItem();
                item.InitRef(paths[i]);
                _refList.Add(item);
            }
        }

        public Object GetObject()
        {
            if (_object == null)
                _object = AssetDatabase.LoadAssetAtPath(_path, typeof(Object));
            return _object;
        }
    }

    public class BeRefItem
    {
        private Object _object;
        public string _path;

        public Object GetObject()
        {
            if (_object == null)
                _object = AssetDatabase.LoadAssetAtPath(_path, typeof(Object));
            return _object;
        }
    }

    [MenuItem("Assets/Find References")]
    public static void FindRefernce()
    {
        _DoFind(FindMode.Reference);
    }

    [MenuItem("Assets/Find BeReferences")]
    public static void FindBeReferences()
    {
        _DoFind(FindMode.BeReference);
    }

    [MenuItem("Assets/Find BeReferences UI")]
    public static void FindBeReferences_UI()
    {
        _DoFind(FindMode.BeReference, true);
    }

    private static void _DoFind(FindMode mode, bool _justUI = false)
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

        FindReferenceTools win = EditorWindow.GetWindow<FindReferenceTools>("引用查询器");
        if (win == null)
            return;

        win.position = new Rect(300, 100, 500, 600);
        win.Show();

        win._findMode = mode;
        win._target = Selection.activeObject;
        win._needCalculate = true;
        win.justUI = _justUI;
    }

    private Object _target;

    private string[] _referenceList;

    private RefItem _refItem;

    private static List<BeRefItem> _hostList = new List<BeRefItem>();

    private FindMode _findMode = FindMode.Reference;

    private Vector2 _scroll;
    private bool _needCalculate = false;

    private bool justUI = false;

    private void OnDisable()
    {
        Resources.UnloadUnusedAssets();
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
        UnityEditor.AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        Object oj = EditorGUILayout.ObjectField("当前查询物体：", _target, typeof(Object), false);

        if (oj != _target)
        {
            _target = oj;
            _needCalculate = true;
        }

        EditorGUILayout.BeginHorizontal();

        FindMode fm = (FindMode)EditorGUILayout.EnumPopup("查找方式", _findMode, GUILayout.Height(25));
        if (fm != _findMode)
        {
            _findMode = fm;
            _needCalculate = true;
        }

        if (GUILayout.Button("计算引用", GUILayout.Height(25)))
        {
            _needCalculate = true;
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        _DoCalculate();

        switch (_findMode)
        {
            case FindMode.Reference:
                ShowReference();
                break;
            case FindMode.BeReference:
                ShowBeReferences();
                break;
            default:
                break;
        }
    }

    private void ShowReference()
    {
        if (_refItem == null)
            return;

        EditorGUILayout.BeginVertical("box");
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        _ShowReference(_refItem);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void _ShowReference(RefItem item)
    {
        if (_refItem == null)
            return;

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(new GUIContent(AssetDatabase.GetCachedIcon(item._path)), GUILayout.Height(22), GUILayout.Width(22));
        EditorGUILayout.LabelField(item._path);

        if (GUILayout.Button("定位"))
        {
            EditorGUIUtility.PingObject(item.GetObject());
        }

        EditorGUILayout.EndHorizontal();

        if (item._refList != null && item._refList.Count > 0)
        {
            if (item.isShowChild)
            {
                item.isShowChild = EditorGUILayout.Foldout(item.isShowChild, "收起");
            }
            else
            {
                item.isShowChild = EditorGUILayout.Foldout(item.isShowChild, "展开");
            }

            if (item.isShowChild)
            {
                for (int i = 0; i < item._refList.Count; ++i)
                {
                    _ShowReference(item._refList[i]);
                }
                GUILayout.Space(10);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void BeginFindReference()
    {
        if (_target == null)
            return;

        _refItem = new RefItem();
        _refItem.InitRef(AssetDatabase.GetAssetPath(_target));
    }

    private void BeginFindBeReferences()
    {
        _hostList.Clear();

        if (_target == null)
            return;

        string path = AssetDatabase.GetAssetPath(_target);
        string _guid = AssetDatabase.AssetPathToGUID(path);

        string[] allGuids = null;
        if (justUI)
        {
            string[] _f = new string[] { "Assets/Editor/BuildBundles/Resources/Prefabs" };
            allGuids = AssetDatabase.FindAssets("t:Prefab", _f);//"t:Material t:Prefab t:Scene t:Model"
        }
        else
        {
            allGuids = AssetDatabase.FindAssets("t:Material t:Prefab");//"t:Material t:Prefab t:Scene t:Model"
        }

        string file = null;

        for (int i = 0; i < allGuids.Length; ++i)
        {
            file = AssetDatabase.GUIDToAssetPath(allGuids[i]);

            EditorUtility.DisplayProgressBar("正在查询引用", file, (float)i / (float)allGuids.Length);

            if (Regex.IsMatch(File.ReadAllText(file), _guid))
            {
                BeRefItem item = new BeRefItem();
                item._path = file;
                _hostList.Add(item);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private void ShowBeReferences()
    {

        if (_hostList != null && _hostList.Count > 0)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            justUI = EditorGUILayout.Toggle("仅检查UI预制体", justUI);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < _hostList.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(new GUIContent(AssetDatabase.GetCachedIcon(_hostList[i]._path)), GUILayout.Height(22), GUILayout.Width(22));
                EditorGUILayout.LabelField(_hostList[i]._path);
                if (GUILayout.Button("定位"))
                {
                    EditorGUIUtility.PingObject(_hostList[i].GetObject());
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    private void _DoCalculate()
    {
        if (!_needCalculate)
            return;

        _needCalculate = false;

        switch (_findMode)
        {
            case FindMode.Reference:
                BeginFindReference();
                break;
            case FindMode.BeReference:
                BeginFindBeReferences();
                break;
            default:
                break;
        }
    }
}
