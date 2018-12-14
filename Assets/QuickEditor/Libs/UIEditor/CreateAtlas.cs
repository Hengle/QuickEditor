using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.U2D;
using UnityEditor.U2D;
#pragma warning disable 0649, 0414, 0618, 0219, 0162, 0168

public class AtlasTool : EditorWindow
{
    [MenuItem("Tools/UIEditor")]
    static public void CreateAtlas()
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
        EditorWindow window = EditorWindow.GetWindowWithRect<AtlasTool>(new Rect(100f, 100f, 800f, 1000f), true, "UIEditor", true);
    }

    [MenuItem("Assets/Generate To Assets")]
    static public void GenerateAssets()
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

        string _path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (_path.Contains(_uiPrefabPath))
        {
            string _name = _path.Replace(_uiPrefabPath, "");
            BedepnedGenPrefab(_name);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("UIEditor", "预制生成完成", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("UIEditor", string.Format("错误路径{0}\n只能生成{1}路径下预制体", _path, _uiPrefabPath), "确定");
        }
    }

    public class SpriteInfo
    {
        public string sName = string.Empty;
        public string sPath = string.Empty;
        public UnityEngine.Object target;
        public int refCount = 0;
        public int width;
        public int height;
    }

    public class AtlasInfo
    {
        public bool isSel = false;
        public bool isShowDetails = false;
        public bool isPrefabExist = false;
        public string sName = string.Empty;
        public int newCount = 0;
        public UIAtlas uiAtlas;
        public UnityEngine.GameObject target;
        public List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
        public List<TextureImporter> imps = new List<TextureImporter>();
    }

    public class AtlasGroupInfo
    {
        public string name = "";
        public bool isOpen = false;
        public bool isAllSel = false;
        public int newCount = 0;
        public List<AtlasInfo> atlasInfos = new List<AtlasInfo>();
    }

    public class UIPrefabInfo
    {
        public bool isSel = false;
        public bool isShowDetail = false;
        public string prefabName = null;
        public string prefabPath = null;
        public Dictionary<string, List<string>> atlass = new Dictionary<string, List<string>>();
        public List<string> textures = new List<string>();
        public List<string> others = new List<string>();
        public List<string> noAtlas = new List<string>();
        public List<string> fonts = new List<string>();
    }

    static private string _jpg = ".jpg";
    static private string _png = ".png";
    static private string _tga = ".tga";
    static private string _ttf = ".ttf";

    static private bool AllowTightWhenTagged = true;
    static private bool AllowRotationFlipping = false;
    static private string TagPrefix = "[TIGHT]";
    static private string invalidName = "(unnamed)";

    static private string _uiAtlasOutPath = "Assets/Editor/BuildBundles/Resources/Atlas/";
    static private string _uiAtlasTextureOutPath = "Assets/Res/UIAtlas/";
    static private string _uiAssetsPath = "Assets/Res/UIImage/";
    static private string _uiPrefabPath = "Assets/Editor/BuildBundles/Resources/Prefabs/";
    static private string _uiPrefabOutPath = "Assets/Editor/BuildBundles/Resources/UI/";
    static private string _uiTextureOutPath = "Assets/Editor/BuildBundles/Resources/Texture/";

    private string _uiAssetsFullPath = null;    
    private string _uiAtlasTextureOutFullPath = null;    
    private string _uiAtlasOutFullPath = null;
    private string _uiPrefabFullPath = null;
    private string _uiPrefabOutFullPath = null;
    private string _uiTextureOutFullPath = null;

    private Dictionary<string, AtlasInfo> _atlasInfoDic = new Dictionary<string, AtlasInfo>();
    private List<AtlasInfo> _atlasInfoList = new List<AtlasInfo>();

    private Dictionary<string, AtlasGroupInfo> _atlasGroupList = new Dictionary<string, AtlasGroupInfo>();

    private UIPrefabInfo[] _prefabInfos = null;

    private string[] _excessPrefab = null;
    private string[] _excessAtlas = null;

    private bool _hasAtlasData = false;
    private bool _hasPrefabData = false;
    private Vector2 _scrAtlas = Vector2.zero;
    private Vector2 _scrPrefab = Vector2.zero;

    private string[] _titles =
    {
        "精简模式",
        "预制",
        "图集",        
    };

    private int _currentTitle = 0;

    public void OnEnable()
    {
        _uiAssetsFullPath = Path.GetFullPath(_uiAssetsPath);
        _uiAtlasTextureOutFullPath = Path.GetFullPath(_uiAtlasTextureOutPath);
        _uiAtlasOutFullPath = Path.GetFullPath(_uiAtlasOutPath);

        _uiPrefabFullPath = Path.GetFullPath(_uiPrefabPath);
        _uiPrefabOutFullPath = Path.GetFullPath(_uiPrefabOutPath);

        _uiTextureOutFullPath = Path.GetFullPath(_uiTextureOutPath);

        if (!Directory.Exists(_uiAtlasTextureOutFullPath)) Directory.CreateDirectory(_uiAtlasTextureOutFullPath);
        if (!Directory.Exists(_uiAtlasOutFullPath)) Directory.CreateDirectory(_uiAtlasOutFullPath);

        if (!Directory.Exists(_uiPrefabOutFullPath)) Directory.CreateDirectory(_uiPrefabOutFullPath);

        //if (!Directory.Exists(_uiTextureOutFullPath)) Directory.CreateDirectory(_uiTextureOutFullPath);
    }

    public void OnDisable()
    {
        _refInfos.Clear();
        _atlasInfoDic.Clear();
        _atlasInfoList.Clear();

        _prefabInfos = null;

        Resources.UnloadUnusedAssets();
        EditorUtility.UnloadUnusedAssets();
        System.GC.Collect();
        UnityEditor.AssetDatabase.Refresh();
    }

    public void Update()
    {
        if (Application.isPlaying || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling)
        {
            Close();
            return;
        }
    }

    public void OnGUI()
    {
        _currentTitle = GUILayout.SelectionGrid(_currentTitle, _titles, _titles.Length);
        GUILayout.Space(10);
        switch (_currentTitle)
        {
            case 0:
                ShowEasy();
                break;
            case 1:
                ShowPrefabList();
                break;
            case 2:
                ShowAtlasList();
                break;
        }
        GUILayout.Space(10);
    }

    private void ShowEasy()
    {
        GUILayout.Label("小图资源路径：" + _uiAssetsFullPath);
        GUILayout.Label("图集纹理输出路径：" + _uiAtlasTextureOutFullPath);
        GUILayout.Label("图集信息输出路径：" + _uiAtlasOutFullPath);
        GUILayout.Label("预制体制作路径：" + _uiPrefabFullPath);
        GUILayout.Label("预制体输出路径：" + _uiPrefabOutFullPath);

        if (GUILayout.Button("一键生成(全部的预制和图集重新生成)"))
        {
            if (!_hasPrefabData)
            {
                ParsePrefab();
            }
            if (!_hasAtlasData)
            {
                ParseAtlas();
            }
            GenAllPrefab();
            ClearPrefab();
            GenAllAtlas();
            ClearAtlas();
            EditorUtility.DisplayDialog("UIEditor", "预制图集生成完成", "ok");
        }

//         if (GUILayout.Button("图集名转小写"))
//             PackTagToLower();
    }

    private void ParsePrefab()
    {
        string[] files = Directory.GetFiles(_uiPrefabFullPath, "*.prefab", SearchOption.AllDirectories);
        _prefabInfos = new UIPrefabInfo[files.Length];
        for (int i = 0; i < files.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("UI预制解析", files[i], (float)i / (float)files.Length);
            string path = files[i].Replace(_uiPrefabFullPath, "");
            string name = path.Replace("\\", "/");
            _prefabInfos[i] = GetPrefabAtlasInfo(name);
        }
        _hasPrefabData = true;
        EditorUtility.ClearProgressBar();
    }

    static private List<UIPrefabInfo> GenPrefabPreSort(List<UIPrefabInfo> tempPrefabList)
    {
        Dictionary<string, int> prefabPos = new Dictionary<string, int>();
        List<UIPrefabInfo> resultPrefabList = new List<UIPrefabInfo>();

        for (int i = 0; i < tempPrefabList.Count; i++)
        {
            prefabPos.Add(tempPrefabList[i].prefabPath, i);
            resultPrefabList.Add(null);
        }

        for (int i = 0; i < tempPrefabList.Count; i++)
        {
            var prefabPath = tempPrefabList[i].prefabPath;
            var depends = AssetDatabase.GetDependencies(prefabPath);
            var curIndex = prefabPos[prefabPath];
            foreach (var depend in depends)
            {
                if (!depend.EndsWith(".prefab")) continue;
                if (!depend.Contains("Assets/Editor/BuildBundles/Resources/Prefabs/")) continue;
                if (!prefabPos.ContainsKey(depend)) continue;

                var targetIndex = prefabPos[depend];

                if(targetIndex > curIndex)
                {//交换
                    prefabPos[prefabPath] = targetIndex;
                    prefabPos[depend] = curIndex;
                    curIndex = targetIndex;

                    //Debug.Log("swap\n" + prefabPath + "\n" + depend);
                }

            }
        }

        for (int i = 0; i < tempPrefabList.Count; i++)
        {
            //Debug.Log(prefabPos[tempPrefabList[i].prefabPath] +" "+ tempPrefabList[i].prefabPath);
            resultPrefabList[prefabPos[tempPrefabList[i].prefabPath]] = tempPrefabList[i];
        }

        return resultPrefabList;
    }

    static private bool isInitBedepend = false;
    static private Dictionary<string, List<string>> bedependDic = new Dictionary<string, List<string>>();

    public class BedependImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets
            (string[] importedAssets, string[] deletedAssets,
             string[] movedAssets, string[] movedFromPath)
        {
            if (EditorApplication.isPlaying) return;
            if (BuildBundles.Building)
            {
                return;
            }

            if (!isInitBedepend) return;

            foreach(var assetPath in importedAssets)
            {
                if (!assetPath.EndsWith(".prefab")) continue;
                if (!assetPath.Contains(_uiPrefabPath)) continue;

                var depends = AssetDatabase.GetDependencies(assetPath);
                var tmpAssetPath = assetPath.Replace(_uiPrefabPath, "");
                foreach (var depend in depends)
                {
                    if (!depend.EndsWith(".prefab")) continue;
                    if (!depend.Contains(_uiPrefabPath)) continue;
                    string shortDepend = depend.Replace(_uiPrefabPath, "");
                    if (shortDepend == tmpAssetPath) continue;

                    if (!bedependDic.ContainsKey(shortDepend))
                        bedependDic.Add(shortDepend, new List<string>());
                    if (!bedependDic[shortDepend].Contains(tmpAssetPath))
                        bedependDic[shortDepend].Add(tmpAssetPath);
                }

            }

        }
    }

    static private void InitBedependPrefab()
    {
        isInitBedepend = true;
        string rootPath = Path.GetFullPath(_uiPrefabPath);
        string[] files = Directory.GetFiles(rootPath, "*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; ++i)
        {
            string path = files[i].Replace(rootPath, "");
            string name = path.Replace("\\", "/");
            var depends = AssetDatabase.GetDependencies(_uiPrefabPath + name);
            foreach(var depend in depends)
            {
                if (!depend.EndsWith(".prefab")) continue;
                if (!depend.Contains(_uiPrefabPath)) continue;
                string shortDepend = depend.Replace(_uiPrefabPath, "");
                if (shortDepend == name) continue;

                if (!bedependDic.ContainsKey(shortDepend))
                    bedependDic.Add(shortDepend, new List<string>());
                if (!bedependDic[shortDepend].Contains(name))
                    bedependDic[shortDepend].Add(name);
            }
        }

        foreach(var item in bedependDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                string depend1 = item.Value[i];
                for (int j = i + 1; j < item.Value.Count; j++)
                {
                    string depend2 = item.Value[j];
                    if(bedependDic.ContainsKey(depend2) && bedependDic[depend2].Contains(depend1))
                    {
                        item.Value[i] = depend2;
                        item.Value[j] = depend1;
                        i--;
                        break;
                    }
                }

            }
        }

    }

    static private void BedepnedGenPrefab(string prafabName)
    {
        if (!isInitBedepend) InitBedependPrefab();

        GenPrefab(prafabName);
        if(bedependDic.ContainsKey(prafabName))
        {
            foreach(var bedepend in bedependDic[prafabName])
            {
                GenPrefab(bedepend);
            }
        }

    }

    static private void GenPrefab(string name)
    {
        //EditorUtility.DisplayProgressBar("生成UI预制", name, 0.8f);

        string inPath = _uiPrefabPath + name;
        string outPath = _uiPrefabOutPath + name;
        string outDir = Path.GetDirectoryName(outPath);

        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        //meta（Before GenPrefab）
        bool metaExist = false;
        string metaPath = Application.dataPath.Replace("Assets", outPath + ".meta");
        byte[] metaContent = null;
        if (File.Exists(metaPath))
        {
            metaExist = true;
            metaContent = File.ReadAllBytes(metaPath);
        }

        Object oj = AssetDatabase.LoadAssetAtPath(inPath, typeof(Object));
        Object ojC = Object.Instantiate(oj);
        GameObject go = ojC as GameObject;

        RewriteUIPrefab(go.transform);
        PrefabUtility.CreatePrefab(outPath, go);
        RewriteUIPrefabDepend(outPath);

        DestroyImmediate(go);

        //EditorUtility.ClearProgressBar();

        //meta（After GenPrefab）
        if (metaExist)
        {
            File.WriteAllBytes(metaPath, metaContent);
        }
        AssetDatabase.ImportAsset(outPath, ImportAssetOptions.ForceUpdate);
    }

    private static void RewriteUIPrefabFileID(string path, GameObject go)
    {
        string srcFileId = "";
        string targetFildId = "";
        string startName = "m_RootGameObject: {fileID: ";

        string srcTxt = File.ReadAllText(path);
        int startIndex = srcTxt.IndexOf(startName) + startName.Length;
        int endIndex = srcTxt.IndexOf("}", startIndex);
        srcFileId = srcTxt.Substring(startIndex, endIndex - startIndex);

        PrefabUtility.CreatePrefab(path, go);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        string targetTxt = File.ReadAllText(path);
        startIndex = targetTxt.IndexOf(startName) + startName.Length;
        endIndex = targetTxt.IndexOf("}", startIndex);
        targetFildId = targetTxt.Substring(startIndex, endIndex - startIndex);
        
        targetTxt = targetTxt.Replace(targetFildId, srcFileId);

        File.WriteAllText(path, targetTxt);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private static void RewriteUIPrefabDepend(string path)
    {
        var depends = AssetDatabase.GetDependencies(path);

        foreach(var depend in depends)
        {
            if (!depend.EndsWith(".prefab")) continue;

            if(depend.Contains("Assets/Editor/BuildBundles/Resources/Prefabs/"))
            {
                ReplaceDependRef(path, depend);
            }
        }
    }

    private static void ReplaceDependRef(string filePath, string srcPath)
    {
        string srcGuid = AssetDatabase.AssetPathToGUID(srcPath);
        string targetGuid = AssetDatabase.AssetPathToGUID(srcPath.Replace("/Prefabs/","/UI/"));

        if (string.IsNullOrEmpty(srcGuid) || string.IsNullOrEmpty(targetGuid))
        {
            EditorUtility.DisplayDialog("失败" + filePath, "请先转换依赖项" + srcPath, "ok");
            return;
        }

        string srcFileId = "";
        string targetFildId = "";
        string startName = "m_RootGameObject: {fileID: ";

        string srcTxt = File.ReadAllText(srcPath);
        int startIndex = srcTxt.IndexOf(startName) + startName.Length;
        int endIndex = srcTxt.IndexOf("}", startIndex);
        srcFileId = srcTxt.Substring(startIndex, endIndex - startIndex);

        string targetTxt = File.ReadAllText(srcPath.Replace("/Prefabs/", "/UI/"));
        startIndex = targetTxt.IndexOf(startName) + startName.Length;
        endIndex = targetTxt.IndexOf("}", startIndex);
        targetFildId = targetTxt.Substring(startIndex, endIndex - startIndex);

        if (string.IsNullOrEmpty(srcFileId) || string.IsNullOrEmpty(targetFildId))
        {
            EditorUtility.DisplayDialog("警告" + filePath, "FildID 为空 " + srcPath, "ok");
            return;
        }

        string txt = File.ReadAllText(filePath);
        txt = txt.Replace(srcGuid, targetGuid);
        txt = txt.Replace(srcFileId, targetFildId);
        File.WriteAllText(filePath, txt);

        Debug.Log("replace depend " + filePath + "\n" + srcPath + "\n" + srcFileId + "\n" + targetFildId);
    }

    private void GenAllPrefab()
    {
        if (_prefabInfos == null || _prefabInfos.Length <= 0)
            return;

        for (int i = 0; i < _prefabInfos.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("生成UI预制("+i+"/"+ _prefabInfos.Length + ")", _prefabInfos[i].prefabName, 1.0f * i / _prefabInfos.Length);
            GenPrefab(_prefabInfos[i].prefabName);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private void ClearPrefab()
    {
        //Clear ui prefab
        string[] names = Directory.GetFiles(_uiPrefabOutFullPath, "*.prefab", SearchOption.AllDirectories);
        string name = null;
        for (int i = 0; i < names.Length; ++i)
        {
            bool isUseful = false;
            string path = names[i].Replace(_uiPrefabOutFullPath, "");
            name = path.Replace("\\", "/");
            EditorUtility.DisplayProgressBar("清理Prefab", name, (float)i / (float)names.Length);
            for (int j = 0; j < _prefabInfos.Length; ++j)
            {
                if (_prefabInfos[j].prefabName.Equals(name))
                {
                    isUseful = true;
                    break;
                }
            }

            if (!isUseful)
                File.Delete(names[i]);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    private void ShowPrefabList()
    {        
        if (!_hasPrefabData)
        {
            ParsePrefab();
        }

        if (_prefabInfos == null || _prefabInfos.Length <= 0)
            return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("预制体", GUILayout.Width(400));
        EditorGUILayout.LabelField("图集数", GUILayout.Width(50));
        EditorGUILayout.LabelField("无图集数", GUILayout.Width(50));
        EditorGUILayout.LabelField("纹理数", GUILayout.Width(50));
        EditorGUILayout.LabelField("字集数", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        _scrAtlas = EditorGUILayout.BeginScrollView(_scrAtlas);

        for (int i = 0; i < _prefabInfos.Length; ++i)
        {
            ShowPrefabInfo(_prefabInfos[i]);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全选"))
        {
            bool isAllTrue = true;
            for (int i = 0; i < _prefabInfos.Length; ++i)
            {
                if (_prefabInfos[i].isSel == false)
                {
                    isAllTrue = false;
                    break;
                }
            }

            for (int i = 0; i < _prefabInfos.Length; ++i)
            {
                _prefabInfos[i].isSel = !isAllTrue;
            }
        }

        if (GUILayout.Button("生成预制"))
        {
            List<UIPrefabInfo> tempPrefabList = new List<UIPrefabInfo>();
            for (int i = 0; i < _prefabInfos.Length; ++i)
            {
                if (_prefabInfos[i].isSel)
                    tempPrefabList.Add(_prefabInfos[i]);
            }

            tempPrefabList = GenPrefabPreSort(tempPrefabList);

            for (int i = 0; i < tempPrefabList.Count; ++i)
            {
                EditorUtility.DisplayProgressBar("生成UI预制(" + i + "/" + tempPrefabList.Count + ")", tempPrefabList[i].prefabName, 1.0f * i / tempPrefabList.Count);
                GenPrefab(tempPrefabList[i].prefabName);
                //Debug.Log(tempPrefabList[i].prefabPath + " " + i);
            }
            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("UIEditor", "预制生成完成", "ok");
        }

        if (GUILayout.Button("清理多余prefab"))
        {
            ClearPrefab();
            EditorUtility.DisplayDialog("UIEditor", "预制清理完成", "ok");
        }

        if (GUILayout.Button("分析预制"))
        {
            ParsePrefab();
            EditorUtility.DisplayDialog("UIEditor", "预制分析完成", "ok");
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ParseAtlas2()
    {
        CalBeReferences();

        for (int i = 0; i < _atlasInfoList.Count; ++i)
        {
            AtlasInfo aInfo = _atlasInfoList[i];
            for (int j = 0; j < aInfo.spriteInfos.Count; ++j)
            {
                SpriteInfo sInfo = aInfo.spriteInfos[j];
                sInfo.refCount = GetRefCount(sInfo.sPath);
            }
        }
    }

    private void ParseAtlas()
    {        
        _atlasInfoDic.Clear();
        _atlasInfoList.Clear();
        _atlasGroupList.Clear();

        List<string> tmp = new List<string>();
        List<string> files = new List<string>();
        string[] files1 = Directory.GetFiles(_uiAssetsFullPath, "*.jpg", SearchOption.AllDirectories);
        string[] files2 = Directory.GetFiles(_uiAssetsFullPath, "*.png", SearchOption.AllDirectories);

        files.AddRange(files1);
        files.AddRange(files2);

        for (int i = 0; i < files.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("刷新", files[i], (float)i / (float)files.Count);

            string path = files[i].Remove(0, Directory.GetCurrentDirectory().Length + 1);      
                 
            TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
            Texture2D text2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            string atlasName = ParseAtlasName(imp.spritePackingTag);

            AtlasInfo _atlasInfo = null;

            if (!_atlasInfoDic.TryGetValue(atlasName, out _atlasInfo))
            {
                _atlasInfo = new AtlasInfo();
                _atlasInfo.isSel = false;
                _atlasInfo.isShowDetails = false;
                _atlasInfoDic[atlasName] = _atlasInfo;
                _atlasInfoList.Add(_atlasInfo);
                _atlasInfo.sName = atlasName;
                string atlasPath = _uiAtlasOutPath + atlasName + ".prefab";
                _atlasInfo.isPrefabExist = File.Exists(atlasPath);
                if(_atlasInfo.isPrefabExist)
                {
                    _atlasInfo.target = AssetDatabase.LoadAssetAtPath<GameObject>(atlasPath);
                    _atlasInfo.uiAtlas = _atlasInfo.target.GetComponent<UIAtlas>();
                    _atlasInfo.newCount = - _atlasInfo.uiAtlas._sprHash.Count;
                    if (_atlasInfo.uiAtlas._sprite != null) _atlasInfo.newCount--;
                }
            }

            SpriteInfo spriteInfo = new SpriteInfo();
            _atlasInfo.spriteInfos.Add(spriteInfo);
            _atlasInfo.imps.Add(imp);
            _atlasInfo.newCount++;
            spriteInfo.sPath = path;
            spriteInfo.sName = Path.GetFileName(spriteInfo.sPath);
            spriteInfo.width = text2D.width;
            spriteInfo.height = text2D.height;
            spriteInfo.target = text2D;
        }

        ExportSpriteInfo();

        _atlasInfoList.Sort(
            (AtlasInfo a, AtlasInfo b) =>
            {
                return a.sName.CompareTo(b.sName);
//                if (a.spriteInfos.Count > 1)
//                {

//                }
//                else
//                { 
//}
//                if (a.spriteInfos.Count == b.spriteInfos.Count)
//                return b.spriteInfos.Count - a.spriteInfos.Count;
            }
        );

        for (int i = 0; i < _atlasInfoList.Count; ++i)
        {
            if (_atlasInfoList[i].spriteInfos.Count != 1) continue;

            AtlasInfo atlasInfo = _atlasInfoList[i];
            string groupName = atlasInfo.sName.Split('/')[0];
            if (!_atlasGroupList.ContainsKey(groupName))
            {
                _atlasGroupList.Add(groupName, new AtlasGroupInfo());
                _atlasGroupList[groupName].name = groupName;
            }

            var atlasGroup = _atlasGroupList[groupName];
            _atlasGroupList[groupName].atlasInfos.Add(_atlasInfoList[i]);
            atlasGroup.newCount += atlasInfo.isPrefabExist ? 0 : 1;
        }

        _hasAtlasData = true;
        EditorUtility.ClearProgressBar();
    }

    public void ExportSpriteInfo()
    {
        string txt = "";
        foreach(var atlasInfo in _atlasInfoList)
        {
            foreach(var sprInfo in atlasInfo.spriteInfos)
            {
                string path = sprInfo.sPath.Replace("\\", "/");
                path = path.Replace(".png", "").Replace(".jpg", "");
                path = path.Replace("Assets/Res/UIImage/", "");
                string name = Path.GetFileNameWithoutExtension(sprInfo.sPath);

                txt += path + " " + atlasInfo.sName + " " + name + "\n";
            }
        }

        string listPath = "Assets/Editor/BuildBundles/Resources/Atlas/spriteinfolist.bytes";
        if (File.Exists(listPath)) File.Delete(listPath);

        File.WriteAllText(listPath, txt);
    }

    private void GenAtlas(string name, List<TextureImporter> imps)
    {
        if (string.IsNullOrEmpty(name) || invalidName.Equals(name))
            return;

        if (imps == null || imps.Count <= 0)
            return;

        //EditorUtility.DisplayProgressBar("生成UI图集", name, 0.8f);

        //老方法
        //AtlasPackTool.PackTextureSingle(imps, name, _uiAtlasOutPath, _uiAtlasTextureOutPath);

        if (imps.Count == 1) AtlasPackTool.PackTextureSingle(imps, name, _uiAtlasOutPath, _uiAtlasTextureOutPath);
        else AtlasPackTool.PackSpriteAtlas(imps, name, _uiAtlasOutPath, _uiAtlasTextureOutPath);
        
        //EditorUtility.ClearProgressBar();
    }

    private void GenAllAtlas()
    {
        for (int i = 0; i < _atlasInfoList.Count; ++i)
        {
            AtlasInfo _atlasInfo = _atlasInfoList[i];
            EditorUtility.DisplayProgressBar("生成UI图集(" + i + "/" + _atlasInfoList.Count + ")", _atlasInfo.sName, 0.8f);
            GenAtlas(_atlasInfo.sName, _atlasInfo.imps);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private void ClearAtlas()
    {
        //Clear Atlas prefab
        string[] names = Directory.GetFiles(_uiAtlasOutFullPath, "*.prefab", SearchOption.AllDirectories);
        string name = null;
        for (int i = 0; i < names.Length; ++i)
        {
            bool isUseful = false;
            string path = names[i].Replace(_uiAtlasOutFullPath, "");
            name = path.Replace("\\", "/");
            name = name.Substring(0, name.IndexOf("."));

            EditorUtility.DisplayProgressBar("清理Atlas", name, (float)i * 0.5f / (float)names.Length);

            for (int j = 0; j < _atlasInfoList.Count; ++j)
            {
                AtlasInfo _atlasInfo = _atlasInfoList[j];
                if (_atlasInfo.sName.Equals(name))
                {
                    isUseful = true;
                    break;
                }
            }

            if (!isUseful) File.Delete(names[i]);
        }
        //Clear Atlas texture
        names = Directory.GetFiles(_uiAtlasTextureOutFullPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < names.Length; ++i)
        {
            bool isUseful = false;
            string path = names[i].Replace(_uiAtlasTextureOutFullPath, "");
            if (path.EndsWith(".meta")) continue;
            name = path.Replace("\\", "/");
            name = name.Substring(0, name.IndexOf("."));

            EditorUtility.DisplayProgressBar("清理Atlas", name, (float)i * 0.5f / (float)names.Length + 0.5f);
            for (int j = 0; j < _atlasInfoList.Count; ++j)
            {
                AtlasInfo _atlasInfo = _atlasInfoList[j];
                if (_atlasInfo.sName.Equals(name))
                {
                    isUseful = true;
                    break;
                }
            }

            if (!isUseful)
                File.Delete(names[i]);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    
    private void ShowAtlasList()
    {
        if (!_hasAtlasData)
        {
            ParseAtlas();
        }

        AtlasInfo _atlasInfo = null;
        if (_atlasInfoList != null && _atlasInfoList.Count > 0)
        {
            _scrAtlas = EditorGUILayout.BeginScrollView(_scrAtlas);
            
            for (int i = 0; i < _atlasInfoList.Count; ++i)
            {
                if (_atlasInfoList[i].spriteInfos.Count == 1) continue;

                ShowAtlasInfo(_atlasInfoList[i]);
            }

            foreach(var atlasGroup in _atlasGroupList.Values)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(atlasGroup.name, GUILayout.Width(100));

                bool allSel = GUILayout.Toggle(atlasGroup.isAllSel, "全选", GUILayout.Width(100));

                if (atlasGroup.isAllSel != allSel)
                {
                    for (int i = 0; i < atlasGroup.atlasInfos.Count; ++i)
                    {
                        atlasGroup.atlasInfos[i].isSel = allSel;
                    }
                    atlasGroup.isAllSel = allSel;
                }

                EditorGUILayout.LabelField(atlasGroup.newCount == 0 ? "" : "新增(" + atlasGroup.newCount + ")", GUILayout.Width(100));
                atlasGroup.isOpen = EditorGUILayout.Foldout(atlasGroup.isOpen, "显示详情（" + atlasGroup.atlasInfos.Count + ")");
                EditorGUILayout.EndHorizontal();

                if(atlasGroup.isOpen)
                {
                    for (int i = 0; i < atlasGroup.atlasInfos.Count; ++i)
                    {
                        ShowAtlasInfo(atlasGroup.atlasInfos[i]);
                    }
                }



            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全选"))
            {
                bool isAllTrue = true;
                for (int i = 0; i < _atlasInfoList.Count; ++i)
                {
                    if (_atlasInfoList[i].isSel == false)
                    {
                        isAllTrue = false;
                        break;
                    }
                }

                for (int i = 0; i < _atlasInfoList.Count; ++i)
                {
                    _atlasInfoList[i].isSel = !isAllTrue;
                }
            }

            if (GUILayout.Button("生成图集"))
            {
                List<AtlasInfo> tempList = new List<AtlasInfo>();

                for (int i = 0; i < _atlasInfoList.Count; ++i)
                {
                    if (_atlasInfoList[i].isSel)
                        tempList.Add(_atlasInfoList[i]);
                }

                for (int i = 0; i < tempList.Count; ++i)
                {
                    _atlasInfo = tempList[i];

                    EditorUtility.DisplayProgressBar("生成UI图集(" + i + "/" + tempList.Count + ")", _atlasInfo.sName, 0.8f);
                    GenAtlas(_atlasInfo.sName, _atlasInfo.imps);
                }
                EditorUtility.ClearProgressBar();
                ParseAtlas();
                EditorUtility.DisplayDialog("UIEditor", "图集生成完成", "ok");
            }

            if (GUILayout.Button("清理多余Atlas"))
            {
                ClearAtlas();
                EditorUtility.DisplayDialog("UIEditor", "图集清理完成", "ok");
            }

            if (GUILayout.Button("图集分析"))
            {
                ParseAtlas2();
                EditorUtility.DisplayDialog("UIEditor", "图集分析完成", "ok");
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("刷新"))
        {
            ParseAtlas();
            EditorUtility.DisplayDialog("UIEditor", "刷新完成", "ok");
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ShowAtlasInfo(AtlasInfo _atlasInfo)
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        _atlasInfo.isSel = GUILayout.Toggle(_atlasInfo.isSel, _atlasInfo.sName, GUILayout.Width(300));
        if (_atlasInfo.isPrefabExist)
        {
            EditorGUILayout.LabelField(_atlasInfo.newCount == 0 ? "" : "新增(" + _atlasInfo.newCount + ")", GUILayout.Width(100));
        }
        else EditorGUILayout.LabelField("未导出", GUILayout.Width(100));

        _atlasInfo.isShowDetails = EditorGUILayout.Foldout(_atlasInfo.isShowDetails, "显示详情（" + _atlasInfo.spriteInfos.Count + ")");
        string atlasTexPath = _uiAtlasTextureOutPath + _atlasInfo.sName + ".png";
        if (_atlasInfo.isPrefabExist && GUILayout.Button("定位", GUILayout.Width(50)))
        {
            EditorGUIUtility.PingObject(_atlasInfo.target);
        }
        EditorGUILayout.EndHorizontal();
        if (_atlasInfo.isShowDetails)
        {
            for (int j = 0; j < _atlasInfo.spriteInfos.Count; ++j)
            {
                ShowSpriteInfo(_atlasInfo.spriteInfos[j], _atlasInfo.uiAtlas);
            }
        }
    }

    private void ShowSpriteInfo(SpriteInfo _info, UIAtlas uiAtlas = null)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(_info.sName, GUILayout.Width(200));
        if (_info.refCount <= 0)
        {
            GUI.color = Color.red;
            EditorGUILayout.LabelField(_info.refCount.ToString(), GUILayout.Width(50));
            GUI.color = Color.white;
        }
        else
        {
            EditorGUILayout.LabelField(_info.refCount.ToString(), GUILayout.Width(50));
        }

        EditorGUILayout.LabelField(_info.width + "x" + _info.height, GUILayout.Width(100));
        if(uiAtlas != null)
        {
            if(!uiAtlas.HasSpriteData(Path.GetFileNameWithoutExtension(_info.sName)))
                EditorGUILayout.LabelField("新增", GUILayout.Width(50));
            else EditorGUILayout.LabelField("", GUILayout.Width(50));
        }
        else EditorGUILayout.LabelField("未导出", GUILayout.Width(50));

        if (GUILayout.Button("定位", GUILayout.Width(50)))
        {
            EditorGUIUtility.PingObject(_info.target);
        }
        EditorGUILayout.EndHorizontal();
    }

    protected bool IsTagPrefixed(string packingTag)
    {
        packingTag = packingTag.Trim();
        if (packingTag.Length < TagPrefix.Length)
            return false;
        return (packingTag.Substring(0, TagPrefix.Length) == TagPrefix);
    }

    private string ParseAtlasName(string packingTag)
    {
        string name = packingTag.Trim();
        if (IsTagPrefixed(name))
            name = name.Substring(TagPrefix.Length).Trim();
        return (name.Length == 0) ? invalidName : name;
    }

    private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
    {
        if (meshType == SpriteMeshType.Tight)
            if (IsTagPrefixed(packingTag) == AllowTightWhenTagged)
                return SpritePackingMode.Tight;
        return SpritePackingMode.Rectangle;
    }

    static private void RewriteUIPrefab(Transform node)
    {
        UIImage _uisprite = node.GetComponent<UIImage>();
        if (_uisprite != null)
        {
            if (_uisprite.sprite != null)
            {
                string path = AssetDatabase.GetAssetPath(_uisprite.sprite);
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

                if (texImp != null)
                {
                    _uisprite.SetInfo(texImp.spritePackingTag, _uisprite.sprite.name);
                    _uisprite.sprite = null;
                    _uisprite.isRender = false;
                }
                else
                {
                    _uisprite.isRender = true;
                    _uisprite.SetInfo(null, null);
                }
            }
            else
            {
                _uisprite.SetInfo(null, null);
                _uisprite.isRender = (_uisprite.color != Color.white);

                Mask mask = node.GetComponent<Mask>();
                if (mask != null) _uisprite.isRender = true;
            }
        }

        UISpriteRenderer _uispriteRdr = node.GetComponent<UISpriteRenderer>();
        if (_uispriteRdr != null)
        {
            _uispriteRdr._spriteRdr = node.GetComponent<SpriteRenderer>();
            if (_uispriteRdr._spriteRdr.sprite != null)
            {
                string path = AssetDatabase.GetAssetPath(_uispriteRdr._spriteRdr.sprite);
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

                if (texImp != null)
                {
                    _uispriteRdr.SetInfo(texImp.spritePackingTag, _uispriteRdr._spriteRdr.sprite.name);
                    _uispriteRdr._spriteRdr.sprite = null;
                }
                else
                {
                    _uispriteRdr.SetInfo(null, null);
                }
            }
            else
            {
                _uispriteRdr.SetInfo(null, null);
            }
        }

        UITexture _uitexture = node.GetComponent<UITexture>();
        if (_uitexture != null)
        {
            if (_uitexture.texture != null)
            {
                string path = AssetDatabase.GetAssetPath(_uitexture.texture);
                path = path.Replace("\\", "/");
                if (path.Contains("Assets/Res/UIImage/"))
                {//在ab目录下
                    string relativePath = path.Replace("Assets/Editor/BuildBundles/Resources/", "");

                    if(!File.Exists(_uiTextureOutPath + relativePath))
                    {
                        //复制文件到目标目录
                        string dirPath = relativePath.Replace(Path.GetFileName(relativePath), "");
                        var dirSps = dirPath.Split('/');
                        var curDir = _uiTextureOutPath;
                        foreach(var dirSp in dirSps)
                        {
                            if (string.IsNullOrEmpty(dirSp)) continue;

                            var nextDir = curDir + dirSp;
                            if(!Directory.Exists(nextDir)) AssetDatabase.CreateFolder(curDir, dirSp);

                            curDir += dirSp + "/";
                        }
                        AssetDatabase.CopyAsset(path, _uiTextureOutPath + relativePath);
                    }

                    TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
                    texImp.textureType = TextureImporterType.Default;
                    texImp.mipmapEnabled = false;
                    texImp.isReadable = false;
                    texImp.SaveAndReimport();

                    //引用转路径
                    relativePath = relativePath.Replace(".png", "").Replace(".jpg", "");
                    _uitexture.SetInfo("Texture/" + relativePath);
                    _uitexture.texture = null;
                }
                else if(path.Contains("Assets/Resources/"))
                {//在resource下面
                    string relativePath = path.Replace("Assets/Editor/BuildBundles/Resources/", "");

                    //引用转路径
                    relativePath = relativePath.Replace(".png", "").Replace(".jpg", "");
                    _uitexture.SetInfo(relativePath);
                    _uitexture.texture = null;
                }

            }
            else
            {
                _uitexture.SetInfo(null);
            }
        }


        BYXText _uitext = node.GetComponent<BYXText>();
        if (_uitext != null)
        {
            if (_uitext.font != null)
            {
                if (_uitext.font.name != "Arial")
                {
                    _uitext.SetInfo("Font/" + _uitext.font.name);
                    _uitext.font = null;
                }
            }
            else
            {
                _uitext.SetInfo(null);
            }
        }

        UIButton _uibutton = node.GetComponent<UIButton>();
        if (_uibutton != null && _uibutton.transition == Selectable.Transition.SpriteSwap)
        {
            if (_uibutton.spriteState.highlightedSprite != null)
            {
                string path = AssetDatabase.GetAssetPath(_uibutton.spriteState.highlightedSprite);
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texImp != null)
                {
                    _uibutton.SetButtonInfo(1, texImp.spritePackingTag, _uibutton.spriteState.highlightedSprite.name);
                }
                else
                {
                    _uibutton.SetButtonInfo(1, null, null);
                }
            }
            else
            {
                _uibutton.SetButtonInfo(1, null, null);
            }

            if (_uibutton.spriteState.pressedSprite != null)
            {
                string path = AssetDatabase.GetAssetPath(_uibutton.spriteState.pressedSprite);
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texImp != null)
                {
                    _uibutton.SetButtonInfo(2, texImp.spritePackingTag, _uibutton.spriteState.pressedSprite.name);
                }
                else
                {
                    _uibutton.SetButtonInfo(2, null, null);
                }
            }
            else
            {
                _uibutton.SetButtonInfo(2, null, null);
            }

            if (_uibutton.spriteState.disabledSprite != null)
            {
                string path = AssetDatabase.GetAssetPath(_uibutton.spriteState.disabledSprite);
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texImp != null)
                {
                    _uibutton.SetButtonInfo(3, texImp.spritePackingTag, _uibutton.spriteState.disabledSprite.name);
                }
                else
                {
                    _uibutton.SetButtonInfo(3, null, null);
                }
            }
            else
            {
                _uibutton.SetButtonInfo(3, null, null);
            }

            _uibutton.spriteState = new SpriteState();
        }

        for (int i = 0; i < node.childCount; ++i)
        {
            RewriteUIPrefab(node.GetChild(i));
        }
    }

    private UIPrefabInfo GetPrefabAtlasInfo(string name)
    {
        UIPrefabInfo info = new UIPrefabInfo();

        info.prefabName = name;
        info.prefabPath = _uiPrefabPath + name;

        string[] _referenceList = AssetDatabase.GetDependencies(info.prefabPath, true);

        for (int i = 0; i < _referenceList.Length; ++i)
        {
            string _ext = Path.GetExtension(_referenceList[i]);
            if (_png.Equals(_ext))
            {
                TextureImporter imp = AssetImporter.GetAtPath(_referenceList[i]) as TextureImporter;
                string packTag = ParseAtlasName(imp.spritePackingTag);

                if (invalidName.Equals(packTag) && !info.noAtlas.Contains(_referenceList[i]))
                {
                    info.noAtlas.Add(_referenceList[i]);
                }
                else
                {
                    if (!info.atlass.ContainsKey(packTag))
                    {
                        info.atlass[packTag] = new List<string>();
                    }

                    info.atlass[packTag].Add(_referenceList[i]);
                }
            }
            else if (_jpg.Equals(_ext) || _tga.Equals(_ext))
            {
                if (!info.textures.Contains(_referenceList[i]))
                {
                    info.textures.Add(_referenceList[i]);
                }
            }
            else if (_ttf.Equals(_ext))
            {
                if (!info.fonts.Contains(_referenceList[i]))
                {
                    info.fonts.Add(_referenceList[i]);
                }
            }
            else
            {
                if (!info.others.Contains(_referenceList[i]))
                {
                    info.others.Add(_referenceList[i]);
                }
            }
        }
        return info;
    }

    private void ShowPrefabInfo(UIPrefabInfo info)
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        info.isSel = EditorGUILayout.ToggleLeft(info.prefabName, info.isSel, GUILayout.Width(400));
        EditorGUILayout.LabelField(info.atlass.Count.ToString(), GUILayout.Width(50));
        EditorGUILayout.LabelField(info.noAtlas.Count.ToString(), GUILayout.Width(50));
        EditorGUILayout.LabelField(info.textures.Count.ToString(), GUILayout.Width(50));
        EditorGUILayout.LabelField(info.fonts.Count.ToString(), GUILayout.Width(50));
        info.isShowDetail = EditorGUILayout.Foldout(info.isShowDetail, "显示详情");
        string prefabPath = _uiPrefabOutPath + info.prefabName;
        if (File.Exists(prefabPath))
        {
            if(GUILayout.Button("定位", GUILayout.Width(50)))
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
        }
        else EditorGUILayout.LabelField("未导出", GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();
        if (info.isShowDetail)
        {
            if (info.atlass.Count > 0)
            {
                EditorGUILayout.LabelField("图集详情");
                EditorGUILayout.BeginVertical(GUI.skin.box);

                foreach (string k in info.atlass.Keys)
                {
                    EditorGUILayout.LabelField(k);
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    List<string> ns = info.atlass[k];
                    for (int i = 0; i < ns.Count; ++i)
                    {
                        if (GUILayout.Button(ns[i]))
                        {
                            Object _object = AssetDatabase.LoadAssetAtPath(ns[i], typeof(Object));
                            EditorGUIUtility.PingObject(_object);
                        }

                        //EditorGUILayout.LabelField(ns[i]);
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

            if (info.noAtlas.Count > 0)
            {
                EditorGUILayout.LabelField("未设置图集精灵");
                EditorGUILayout.BeginVertical(GUI.skin.box);

                GUI.color = Color.red;
                for (int i = 0; i < info.noAtlas.Count; ++i)
                {
                    if (GUILayout.Button(info.noAtlas[i]))
                    {
                        Object _object = AssetDatabase.LoadAssetAtPath(info.noAtlas[i], typeof(Object));
                        EditorGUIUtility.PingObject(_object);
                    }
                    //EditorGUILayout.LabelField(info.noAtlas[i]);
                }
                GUI.color = Color.white;

                EditorGUILayout.EndVertical();
            }

            if (info.textures.Count > 0)
            {
                EditorGUILayout.LabelField("纹理详情");
                EditorGUILayout.BeginVertical(GUI.skin.box);

                for (int i = 0; i < info.textures.Count; ++i)
                {
                    if (GUILayout.Button(info.textures[i]))
                    {
                        Object _object = AssetDatabase.LoadAssetAtPath(info.textures[i], typeof(Object));
                        EditorGUIUtility.PingObject(_object);
                    }
                    //EditorGUILayout.LabelField(info.textures[i]);
                }

                EditorGUILayout.EndVertical();
            }

            if (info.fonts.Count > 0)
            {
                EditorGUILayout.LabelField("字集引用");
                EditorGUILayout.BeginVertical(GUI.skin.box);

                for (int i = 0; i < info.fonts.Count; ++i)
                {
                    EditorGUILayout.LabelField(info.fonts[i]);
                }

                EditorGUILayout.EndVertical();
            }

            if (info.others.Count > 0)
            {
                EditorGUILayout.LabelField("其他引用");
                EditorGUILayout.BeginVertical(GUI.skin.box);

                for (int i = 0; i < info.others.Count; ++i)
                {
                    if (GUILayout.Button(info.others[i]))
                    {
                        Object _object = AssetDatabase.LoadAssetAtPath(info.others[i], typeof(Object));
                        EditorGUIUtility.PingObject(_object);
                    }
                    //EditorGUILayout.LabelField(info.others[i]);
                }

                EditorGUILayout.EndVertical();
            }
        }
    }

    private void PackTagToLower()
    {
        string[] files = Directory.GetFiles(_uiAssetsFullPath, "*.png", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("UI资源解析", files[i], (float)i / (float)files.Length);

            string path = files[i].Remove(0, Application.dataPath.Length - 6);
            TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;

            imp.isReadable = true;
            imp.spritePackingTag = imp.spritePackingTag.ToLower();
            imp.SaveAndReimport();
        }
        EditorUtility.ClearProgressBar();
    }

    private Dictionary<string, List<string>> _refInfos = new Dictionary<string, List<string>>();

    private void CalBeReferences()
    {
        _refInfos.Clear();
        string[] allGuids = AssetDatabase.FindAssets("t:Material t:Prefab");//"t:Material t:Prefab t:Scene t:Model"

        string file = null;

        for (int i = 0; i < allGuids.Length; ++i)
        {
            file = AssetDatabase.GUIDToAssetPath(allGuids[i]);

            EditorUtility.DisplayProgressBar("正在查询引用", file, (float)i / (float)allGuids.Length);
            string[] paths = AssetDatabase.GetDependencies(file, false);
            for (int j = 0; j < paths.Length; ++j)
            {
                string _pathGuid = AssetDatabase.AssetPathToGUID(paths[j]);
                List<string> refs = null;
                if (!_refInfos.TryGetValue(_pathGuid, out refs))
                {
                    refs = new List<string>();
                    _refInfos.Add(_pathGuid, refs);
                }
                refs.Add(file);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private int GetRefCount(string path)
    {
        string _pathGuid = AssetDatabase.AssetPathToGUID(path);
        List<string> refs = null;
        if (_refInfos.TryGetValue(_pathGuid, out refs))
        {
            return refs.Count;
        }
        return 0;
    }
}

public class AtlasPackTool
{
    static float matAtlasSize = 2048;//最大图集尺寸  
    static float padding = 1;//每两个图片之间用多少像素来隔开              

    //判断图片格式不对了，先进性格式的转换
    static private void CheckFormat(List<TextureImporter> imps)
    {
        bool bNeedReImport = false;

        //单张图片不缩放，多张每张不超过1024
        int maxTextureSize = imps.Count == 1 ? 2048 : 1024;

        for (int i = 0; i < imps.Count; ++i)
        {
            TextureImporter imp = imps[i];
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.textureType = TextureImporterType.Sprite;
            imp.isReadable = true;
            imp.mipmapEnabled = false;
            imp.npotScale = TextureImporterNPOTScale.None;//用于非二次幂纹理的缩放模式
            if(imp.assetPath.ToLower().EndsWith(".jpg"))
            {
                imp.SetPlatformTextureSettings("iPhone", maxTextureSize, TextureImporterFormat.RGB24);
                imp.SetPlatformTextureSettings("Android", maxTextureSize, TextureImporterFormat.RGB24);
            }
            else
            {
                imp.SetPlatformTextureSettings("iPhone", maxTextureSize, TextureImporterFormat.RGBA32);
                imp.SetPlatformTextureSettings("Android", maxTextureSize, TextureImporterFormat.RGBA32);
            }
            imp.SaveAndReimport();

            bNeedReImport = true;
        }

        if (bNeedReImport)
        {
            AssetDatabase.Refresh();
        }
    }

    //加载图片资源
    static private Texture2D[] LoadTextures(List<TextureImporter> imps)
    {
        Texture2D[] texs = new Texture2D[imps.Count];
        for (int i = 0; i < imps.Count; i++)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(imps[i].assetPath);
            texs[i] = tex;
            //判断图片命名的合法性
            if (tex.name.StartsWith(" ") || tex.name.EndsWith(" "))
            {
                string newName = tex.name.TrimStart(' ').TrimEnd(' ');
                Debug.LogWarning(string.Format("rename texture'name old name : {0}, new name {1}", tex.name, newName));
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tex), newName);
            }

            string assetPath = AssetDatabase.GetAssetPath(tex);
            //重新把图片导入内存，理论上unity工程中的资源在用到的时候，Unity会自动导入到内存，但有的时候却没有自动导入，为了以防万一，我们手动导入一次  
            AssetDatabase.ImportAsset(assetPath);
        }

        return texs;
    }

    static public void PackTexture(List<TextureImporter> imps, string atlasName, string atlasPath, string texturePath)
    {
        if (imps == null || imps.Count <= 0)
        {
            Debug.Log("选择打包的小图数量为0");
            return;
        }

        //检查文件夹是否存在
        if (!Directory.Exists(atlasPath))
        {
            Directory.CreateDirectory(atlasPath);
        }

        //检查文件夹是否存在
        if (!Directory.Exists(texturePath))
        {
            Directory.CreateDirectory(texturePath);
        }

        //图集预制相对路径
        string relativePath = atlasPath + atlasName + ".prefab";
        //图集预制绝对路径
        string absolutePath = Path.GetFullPath(atlasPath) + atlasName + ".prefab";

        string absoluteDir = Path.GetDirectoryName(absolutePath);
        if (!Directory.Exists(absoluteDir)) Directory.CreateDirectory(absoluteDir);

        //纹理相对路径
        string relativeTexPath = texturePath + atlasName + ".png";
        //纹理绝对路径
        string absoluteTexPath = Path.GetFullPath(texturePath) + atlasName + ".png";

        string absoluteTexDir = Path.GetDirectoryName(absoluteTexPath);
        if (!Directory.Exists(absoluteTexDir)) Directory.CreateDirectory(absoluteTexDir);

        //判断图片格式不对了，先进性格式的转换
        CheckFormat(imps);
        //加载图片资源
        Texture2D[] texs = LoadTextures(imps);

        //创建Atlas实例
        GameObject atlasPrefab = new GameObject();
        UIAtlas atlasInfo = atlasPrefab.AddComponent<UIAtlas>();
        atlasInfo._name = atlasName;

        //打包小图到大纹理并返回uv信息 
        Texture2D atlasTexture = null; 
        Rect[] rs = null;
        if (texs.Length > 1) //数量大于1，合并图集
        {
            atlasTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            rs = atlasTexture.PackTextures(texs, (int)padding, (int)matAtlasSize);

            //把图集写入到磁盘文件
            File.WriteAllBytes(absoluteTexPath, atlasTexture.EncodeToPNG());
        }
        else //只有一张图片，复制图片
        {
            atlasTexture = texs[0];
            rs = new Rect[1] { new Rect(0, 0, 1, 1) };
            if (File.Exists(absoluteTexPath)) File.Delete(absoluteTexPath);

            if (imps[0].assetPath.ToLower().EndsWith(".jpg"))
            {
                relativeTexPath = relativeTexPath.Replace(".png", ".jpg");
                absoluteTexPath = relativeTexPath.Replace(".png", ".jpg");
            }

            if (File.Exists(absoluteTexPath)) File.Delete(absoluteTexPath);
            File.Copy(imps[0].assetPath, absoluteTexPath);
        }

        //刷新图片 
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(relativeTexPath);

        //记录图片的名字，只是用于输出日志用;  
        StringBuilder names = new StringBuilder();

        //SpriteMetaData结构可以让我们编辑图片的一些信息,想图片的name,包围盒border,在图集中的区域rect等  
        SpriteMetaData[] sheet = new SpriteMetaData[rs.Length];
        for (var i = 0; i < sheet.Length; i++)
        {
            SpriteMetaData meta = new SpriteMetaData();
            meta.name = texs[i].name;
            //这里的rect记录的是单个图片在图集中的uv坐标值             
            meta.rect = rs[i];
            meta.rect.Set(
                meta.rect.x * atlasTexture.width,
                meta.rect.y * atlasTexture.height,
                meta.rect.width * atlasTexture.width,
                meta.rect.height * atlasTexture.height
            );


            TextureImporter texImp = imps[i];
            //如果图片有包围盒信息的话  
            if (texImp != null)
            {
                meta.border = texImp.spriteBorder;
                meta.pivot = texImp.spritePivot;
            }

            sheet[i] = meta;

            SpriteData sd = new SpriteData();
            sd.name = meta.name;
            sd.alignment = meta.alignment;
            sd.border = new Vector4(meta.border.x, meta.border.y, meta.border.z, meta.border.w);
            sd.pivot = new Vector2(meta.pivot.x, meta.pivot.y);
            sd.rect = new Rect(meta.rect);

            if (!atlasInfo.HasSpriteData(meta.name))
            {
                atlasInfo._uvs.Add(sd);
                atlasInfo._sprHash.Add(meta.name);
            }
            else
            {
                Debug.LogErrorFormat("{0}图集中存在相同名称的图片：{1}", atlasInfo._name,meta.name);
            }

            //打印日志用
            names.Append(meta.name);
            if (i < sheet.Length - 1)
                names.Append(",");
        }

        //设置图集纹理信息  
        TextureImporter imp = TextureImporter.GetAtPath(relativeTexPath) as TextureImporter;
        imp.textureType = TextureImporterType.Sprite;//图集的类型  
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.alphaIsTransparency = true;
        if(relativeTexPath.EndsWith(".png"))
        {
            imp.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGBA4);
            imp.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC2_RGBA8);
        }
        else
        {
            imp.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGB4);
            imp.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC2_RGB4);
        }
        //imp.ClearPlatformTextureSettings("iPhone");
        imp.textureFormat = TextureImporterFormat.AutomaticTruecolor;//TextureImporterFormat.AutomaticCompressed;//图集的格式  
        imp.spriteImportMode = texs.Length == 1 ? SpriteImportMode.Single: SpriteImportMode.Multiple;//Multiple表示我们这个大图片(图集)中包含很多小图片  
        imp.mipmapEnabled = false;//是否开启mipmap  
        imp.isReadable = false;
        imp.spritesheet = sheet;//设置图集中小图片的信息(每个图片所在的区域rect等)  
        // 保存并刷新  
        imp.SaveAndReimport();

        if(texs.Length == 1)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativeTexPath);
            atlasInfo._sprite = sprite;
        }
        else
        {
            //重新加载导出的纹理，填充图集信息
            Texture2D texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(relativeTexPath);
            atlasInfo._tex2d = texture2d;
        }

        PrefabUtility.CreatePrefab(relativePath, atlasPrefab, ReplacePrefabOptions.Default);
        GameObject.DestroyImmediate(atlasPrefab);

        //输出日志  
        Debug.Log("Atlas create ok. " + names.ToString());
    }

    //请保证imps长度为1
    static public void PackTextureSingle(List<TextureImporter> imps, string atlasName, string atlasPath, string texturePath)
    {
        if (imps == null || imps.Count != 1)
        {
            Debug.Log("选择打包的小图数量不为1");
            return;
        }

        //检查文件夹是否存在
        if (!Directory.Exists(atlasPath))
        {
            Directory.CreateDirectory(atlasPath);
        }

        //检查文件夹是否存在
        if (!Directory.Exists(texturePath))
        {
            Directory.CreateDirectory(texturePath);
        }

        //图集预制相对路径
        string relativePath = atlasPath + atlasName + ".prefab";
        //图集预制绝对路径
        string absolutePath = Path.GetFullPath(atlasPath) + atlasName + ".prefab";

        string absoluteDir = Path.GetDirectoryName(absolutePath);
        if (!Directory.Exists(absoluteDir)) Directory.CreateDirectory(absoluteDir);

        //纹理相对路径
        string relativeTexPath = texturePath + atlasName + ".png";
        //纹理绝对路径
        string absoluteTexPath = Path.GetFullPath(texturePath) + atlasName + ".png";

        string absoluteTexDir = Path.GetDirectoryName(absoluteTexPath);
        if (!Directory.Exists(absoluteTexDir)) Directory.CreateDirectory(absoluteTexDir);

        //判断图片格式不对了，先进性格式的转换
        CheckFormat(imps);
        //加载图片资源
        Texture2D[] texs = LoadTextures(imps);

        //创建Atlas实例
        GameObject atlasPrefab = new GameObject();
        UIAtlas atlasInfo = atlasPrefab.AddComponent<UIAtlas>();
        atlasInfo._name = atlasName;

        //打包小图到大纹理并返回uv信息 
        Texture2D atlasTexture = texs[0];
        Rect[] rs = new Rect[1] { new Rect(0, 0, 1, 1) };
        if (File.Exists(absoluteTexPath)) File.Delete(absoluteTexPath);

        if (imps[0].assetPath.ToLower().EndsWith(".jpg"))
        {
            relativeTexPath = relativeTexPath.Replace(".png", ".jpg");
            absoluteTexPath = relativeTexPath.Replace(".png", ".jpg");
        }

        if (File.Exists(absoluteTexPath)) File.Delete(absoluteTexPath);
        if (File.Exists(absoluteTexPath+".meta")) File.Delete(absoluteTexPath + ".meta");
        //File.Copy(imps[0].assetPath, absoluteTexPath);
        AssetDatabase.CopyAsset(imps[0].assetPath, absoluteTexPath);

        //刷新图片 
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(relativeTexPath);

        //设置图集纹理信息  
        TextureImporter imp = TextureImporter.GetAtPath(relativeTexPath) as TextureImporter;
        imp.spritePackingTag = "";
        imp.textureType = TextureImporterType.Sprite;//图集的类型  
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.alphaIsTransparency = true;
        if (relativeTexPath.EndsWith(".png"))
        {
            imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4);
            imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGBA8);
        }
        else
        {
            imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGB4);
            imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGB4);
        }
        imp.textureFormat = TextureImporterFormat.AutomaticTruecolor;//TextureImporterFormat.AutomaticCompressed;//图集的格式  
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.mipmapEnabled = false;//是否开启mipmap  
        imp.isReadable = false;
        // 保存并刷新  
        imp.SaveAndReimport();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativeTexPath);
        atlasInfo._sprite = sprite;

        PrefabUtility.CreatePrefab(relativePath, atlasPrefab, ReplacePrefabOptions.Default);
        GameObject.DestroyImmediate(atlasPrefab);
    }

    static public void PackSpriteAtlas(List<TextureImporter> imps, string atlasName, string atlasPath, string texturePath)
    {
        if (imps == null || imps.Count <= 0)
        {
            Debug.Log("选择打包的小图数量为0");
            return;
        }

        //检查文件夹是否存在
        if (!Directory.Exists(atlasPath))
        {
            Directory.CreateDirectory(atlasPath);
        }

        //检查文件夹是否存在
        if (!Directory.Exists(texturePath))
        {
            Directory.CreateDirectory(texturePath);
        }

        //图集预制相对路径
        string relativePath = atlasPath + atlasName + ".prefab";
        //图集预制绝对路径
        string absolutePath = Path.GetFullPath(atlasPath) + atlasName + ".prefab";

        string absoluteDir = Path.GetDirectoryName(absolutePath);
        if (!Directory.Exists(absoluteDir)) Directory.CreateDirectory(absoluteDir);

        //纹理相对路径
        string relativeTexPath = texturePath + atlasName + ".spriteatlas";
        //纹理绝对路径
        string absoluteTexPath = Path.GetFullPath(texturePath) + atlasName + ".spriteatlas";

        if (File.Exists(absoluteTexPath)) File.Delete(absoluteTexPath);
        if (File.Exists(absoluteTexPath.Replace(".spriteatlas", ".png"))) File.Delete(absoluteTexPath.Replace(".spriteatlas", ".png"));

        string absoluteTexDir = Path.GetDirectoryName(absoluteTexPath);
        if (!Directory.Exists(absoluteTexDir)) Directory.CreateDirectory(absoluteTexDir);

        //判断图片格式不对了，先进性格式的转换
        CheckFormat(imps);
        //加载图片资源
        Texture2D[] texs = LoadTextures(imps);

        //创建Atlas实例
        GameObject atlasPrefab = new GameObject();
        UIAtlas atlasInfo = atlasPrefab.AddComponent<UIAtlas>();
        atlasInfo._name = atlasName;

        //打包小图到大纹理并返回uv信息 
        SpriteAtlas sprAtlas = new SpriteAtlas();
        sprAtlas.SetIsVariant(false);
        sprAtlas.SetIncludeInBuild(true);

        var sprAtlasPackSettings = new SpriteAtlasPackingSettings();
        sprAtlasPackSettings.enableRotation = false;
        sprAtlasPackSettings.enableTightPacking = false;
        sprAtlasPackSettings.padding = 2;
        sprAtlasPackSettings.blockOffset = 1;
        sprAtlas.SetPackingSettings(sprAtlasPackSettings);

        var sprAtlasTexSettings = new SpriteAtlasTextureSettings();
        sprAtlasTexSettings.readable = false;
        sprAtlasTexSettings.generateMipMaps = false;
        sprAtlasTexSettings.sRGB = true;
        sprAtlasTexSettings.filterMode = FilterMode.Bilinear;
        sprAtlas.SetTextureSettings(sprAtlasTexSettings);

        var sprAtlasPlatSettingsAndroid = new TextureImporterPlatformSettings();
        sprAtlasPlatSettingsAndroid.name = "Android";
        sprAtlasPlatSettingsAndroid.overridden = true;
        sprAtlasPlatSettingsAndroid.format = TextureImporterFormat.ETC2_RGBA8;
        sprAtlasPlatSettingsAndroid.maxTextureSize = 2048;
        sprAtlas.SetPlatformSettings(sprAtlasPlatSettingsAndroid);

        var sprAtlasPlatSettingsIOS = new TextureImporterPlatformSettings();
        sprAtlasPlatSettingsIOS.name = "iPhone";
        sprAtlasPlatSettingsIOS.overridden = true;
        sprAtlasPlatSettingsIOS.format = TextureImporterFormat.ASTC_RGBA_4x4;
        sprAtlasPlatSettingsIOS.maxTextureSize = 2048;
        sprAtlas.SetPlatformSettings(sprAtlasPlatSettingsIOS);

        sprAtlas.Add(texs);

        AssetDatabase.CreateAsset(sprAtlas, relativeTexPath);

        //刷新图片 
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(relativeTexPath);

        atlasInfo._sprAtlas = sprAtlas;

        foreach(var tex in texs)
        {
            atlasInfo._sprHash.Add(tex.name);
        }


        PrefabUtility.CreatePrefab(relativePath, atlasPrefab, ReplacePrefabOptions.Default);
        GameObject.DestroyImmediate(atlasPrefab); 
    }
}