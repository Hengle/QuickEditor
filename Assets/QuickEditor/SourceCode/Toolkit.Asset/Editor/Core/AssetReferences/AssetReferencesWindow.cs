using QuickEditor.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AssetReferencesWindow : QEditorHorizontalSplitWindow
{

}

public class FindReferences
{

    [MenuItem("Assets/Find References", false, 100)]
    static private void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string dataPath = Application.dataPath + "/";
        if (!string.IsNullOrEmpty(path))
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];

                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(file), typeof(Object)));
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("匹配结束");
                }

            };
        }
    }

    [MenuItem("Assets/Find References", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}

public class FindMissingWindow : EditorWindow
{
    [MenuItem("Tools/检查/检查MissingReference资源")]
    public static void FindMissing()
    {
        GetWindow<FindMissingWindow>().titleContent = new GUIContent("查找Missing资源");
        GetWindow<FindMissingWindow>().Show();
        Find();
    }

    private static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> prefabs = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
    private static Dictionary<UnityEngine.Object, string> refPaths = new Dictionary<UnityEngine.Object, string>();

    private static void Find()
    {
        prefabs.Clear();
        string[] allassetpaths = AssetDatabase.GetAllAssetPaths();//获取所有资源路径
        var gos = allassetpaths
            .Where(a => a.EndsWith("prefab"))//筛选 是以prefab为后缀的 预设体资源
            .Select(a => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(a));//加载这个预设体
                                                                               //gos拿到的是所有加载好的预设体
        foreach (var item in gos)
        {
            GameObject go = item as GameObject;
            if (go)
            {
                Component[] cps = go.GetComponentsInChildren<Component>(true);//获取这个物体身上所有的组件
                foreach (var cp in cps)//遍历每一个组件
                {
                    if (!cp)
                    {
                        if (!prefabs.ContainsKey(go))
                        {
                            prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                        }
                        else
                        {
                            prefabs[go].Add(cp);
                        }
                        continue;
                    }
                    SerializedObject so = new SerializedObject(cp);//生成一个组件对应的S俄日阿里则对Object对象 用于遍历这个组件的所有属性
                    var iter = so.GetIterator();//拿到迭代器
                    while (iter.NextVisible(true))//如果有下一个属性
                    {
                        //如果这个属性类型是引用类型的
                        if (iter.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            //引用对象是null 并且 引用ID不是0 说明丢失了引用
                            if (iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0)
                            {
                                if (!refPaths.ContainsKey(cp)) refPaths.Add(cp, iter.propertyPath);
                                else refPaths[cp] += " | " + iter.propertyPath;
                                if (prefabs.ContainsKey(go))
                                {
                                    if (!prefabs[go].Contains(cp)) prefabs[go].Add(cp);
                                }
                                else
                                {
                                    prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                                }
                            }
                        }
                    }
                }
            }
        }
        EditorUtility.DisplayDialog("", "就绪", "OK");
    }

    //以下只是将查找结果显示
    private Vector3 scroll = Vector3.zero;

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("移除丢失引用的Animator组件")) RemoveAnimatorWithMissReference();
        foreach (var item in prefabs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(item.Key, typeof(GameObject), true, GUILayout.Width(200));
            EditorGUILayout.BeginVertical();
            foreach (var cp in item.Value)
            {
                if (cp)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(cp, cp.GetType(), true, GUILayout.Width(200));
                    if (refPaths.ContainsKey(cp))
                    {
                        GUILayout.Label(refPaths[cp]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void RemoveAnimatorWithMissReference()
    {
        int count = 0;
        foreach (var cps in prefabs.Values)
        {
            foreach (var item in cps)
            {
                Animator a = item as Animator;
                if (a)
                {
                    count++;
                    EditorUtility.SetDirty(a.gameObject);
                    DestroyImmediate(a, true);
                }
            }
        }
        if (count > 0)
        {
            EditorUtility.DisplayDialog("", "一共移除" + count + "个Animator组件", "OK");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Find();
        }
    }
}

public class ResourceTools : Editor
{
    private static Dictionary<string, Object> assetInfoDict = new Dictionary<string, Object>();

    private static string curRootAsset = string.Empty;
    private static float curProgress = 0f;

    [MenuItem("ResourceTools / DeleteOtherResources")]
    private static void DeleteOtherResources()
    {
        string path = GetSelectedAssetPath();
        if (path == null)
        {
            Debug.LogWarning("请先选择目标文件夹");
            return;
        }
        ResourceTools.GetAllAssets(path);

    }

    public static void GetAllAssets(string rootDir)
    {
        assetInfoDict.Clear();

        DirectoryInfo dirinfo = new DirectoryInfo(rootDir);
        FileInfo[] fs = dirinfo.GetFiles("*.*", SearchOption.AllDirectories);
        int ind = 0;
        foreach (var f in fs)
        {
            curProgress = (float)ind / (float)fs.Length;
            curRootAsset = "搜寻中...：" + f.Name;
            EditorUtility.DisplayProgressBar("正在查询其他版本资源", curRootAsset, curProgress);
            ind++;
            int index = f.FullName.IndexOf("Assets");
            if (index != -1)
            {
                string assetPath = f.FullName.Substring(index);
                Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                string upath = AssetDatabase.GetAssetPath(asset);
                //Debug.Log(upath);
                if (assetInfoDict.ContainsKey(assetPath) == false
                    && assetPath.StartsWith("Assets")
                    && !(asset is MonoScript)
                    && !(asset is LightingDataAsset)
                    && asset != null
                    )
                {

                    if (upath.EndsWith("@jp.png") ||
                        upath.EndsWith("@en.png") ||
                        upath.EndsWith("@tw.png") ||
                        upath.EndsWith("@kr.png") ||
                        upath.EndsWith("@hk.png"))
                    {
                        assetInfoDict.Add(upath, asset);
                    }
                }
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
        EditorUtility.ClearProgressBar();

        int setIndex = 0;
        foreach (KeyValuePair<string, Object> kv in assetInfoDict)
        {
            EditorUtility.DisplayProgressBar("正在删除...", kv.Key, (float)setIndex / (float)assetInfoDict.Count);
            setIndex++;
            //这里 开始删除 资源
            AssetDatabase.DeleteAsset(kv.Key);
            Debug.Log(kv.Key);
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.UnloadUnusedAssetsImmediate();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string GetSelectedAssetPath()
    {
        var selected = Selection.activeObject;
        if (selected == null)
        {
            return null;
        }
        Debug.Log(selected.GetType());
        if (selected is DefaultAsset)
        {
            string path = AssetDatabase.GetAssetPath(selected);
            Debug.Log("选中路径： " + path);
            return path;
        }
        else
        {
            return null;
        }
    }

}

/// 查找一个物体拖拽到了哪些物体上。
/// </summary>

public class FindDragObject : ScriptableWizard
{

    public GameObject targetObj;

    public void OnWizardUpdate()
    {
        helpString = "选择你要查看的物体";
        isValid = targetObj != null;
    }

    private void OnWizardCreate()
    {
        GameObject[] roots = Selection.gameObjects;
        Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
        foreach (var item in roots)
        {
            if (!dic.ContainsKey(item.transform.root.name))
                dic.Add(item.transform.root.name, item.transform.root.gameObject);
        }
        foreach (var item in dic)
        {
            GetCompent(item.Value);
        }
    }

    private void GetCompent(GameObject gameObject)
    {
        Component[] components = gameObject.GetComponentsInChildren<Component>();
        foreach (var item in components)
        {
            GetPropertiesInCompent(item);
        }
    }

    private void GetPropertiesInCompent(Component item)
    {

        if (item.gameObject == targetObj)
        {
            return;
        }
        List<PropertyInfo> properties = item.GetType().GetProperties().Where(p => p.PropertyType.IsPublic && !p.PropertyType.IsValueType && p.PropertyType != typeof(Component)).ToList<PropertyInfo>();
        foreach (var propertyItem in properties)
        {
            var value = propertyItem.GetValue(item, null);
            if (value != null)
            {
                if (value == (object)targetObj)
                {
                    Debug.Log("找到名称为下列的物体");
                    Debug.Log(item.gameObject.name);
                }
            }

        }

        GetFieldsInCompent(item);
    }

    private void GetFieldsInCompent(Component item)
    {
        List<FieldInfo> fields = item.GetType().GetFields().Where(p => p.FieldType.IsPublic && !p.FieldType.IsValueType).ToList<FieldInfo>();
        foreach (var fieldItem in fields)
        {

            var value = fieldItem.GetValue(item);
            if (value != null)
            {
                if (value == (object)targetObj)
                {
                    Debug.Log("找到名称为下列的物体");
                    Debug.Log(item.gameObject.name);
                }
            }
        }
    }

    [MenuItem("Tools/开始查询")]
    public static void FindObjectNames()
    {
        ScriptableWizard.DisplayWizard<FindDragObject>("FindNames", "FindObjectNames");
    }

}

public class SearchShader
{

    public static string FilePath = "Assets/Materials";

    //搜索固定文件夹中的所有Material的路径
    public static List<string> listMatrials;

    public static List<string> listTargetMaterial;

    public static string selectedShaderName;

    public static StringBuilder sb;

    [MenuItem("Assets/SearchShader", true)]
    private static bool OptionSelectAvailable()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }
        return Selection.activeObject.GetType() == typeof(Shader);
    }

    [MenuItem("Assets/SearchShader")]
    private static void SearchConstantShader()
    {
        Debug.Log("当前选中的Shader名字：" + Selection.activeObject.name);
        sb = new StringBuilder();

        selectedShaderName = Selection.activeObject.name;

        listMatrials = new List<string>();
        listMatrials.Clear();
        listTargetMaterial = new List<string>();
        listTargetMaterial.Clear();

        //项目路径 eg:projectPath = D:Project/Test/Assets
        string projectPath = Application.dataPath;

        //eg:projectPath = D:Project/Test
        projectPath = projectPath.Substring(0, projectPath.IndexOf("Assets"));

        try
        {
            //获取某一文件夹中的所有Matrial的Path信息
            GetMaterialsPath(projectPath, FilePath, "Material", ref listMatrials);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        for (int i = 0; i < listMatrials.Count; i++)
        {
            EditorUtility.DisplayProgressBar("Check Materials", "Current Material :"
                + i + "/" + listMatrials.Count, (float)i / listMatrials.Count);

            try
            {
                //开始Check
                BegainCheckMaterials(listMatrials[i]);
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError(e);
            }
        }

        PrintToTxt();
        EditorUtility.ClearProgressBar();
        Debug.Log("Check Success");
    }

    //获取某一文件夹中的所有Matrial的Path信息
    public static void GetMaterialsPath(string projectPath, string targetFilePath, string searchType, ref List<string> array)
    {
        if (Directory.Exists(targetFilePath))
        {
            string[] guids;
            //搜索
            guids = AssetDatabase.FindAssets("t:" + searchType, new[] { targetFilePath });
            foreach (string guid in guids)
            {
                string source = AssetDatabase.GUIDToAssetPath(guid);
                listMatrials.Add(source);
            }
        }
    }

    //开始检查Material
    public static void BegainCheckMaterials(string materialPath)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (mat.shader.name == selectedShaderName)
        {
            listTargetMaterial.Add(materialPath);
        }
    }

    public static void PrintToTxt()
    {
        //加入shader的名字
        listTargetMaterial.Add(selectedShaderName);

        FileInfo fi = new FileInfo(Application.dataPath + "/Materials.txt");
        if (!fi.Exists)
        {
            fi.CreateText();
        }
        else
        {
            StreamWriter sw = new StreamWriter(Application.dataPath + "/Materials.txt");
            for (int i = 0; i < listTargetMaterial.Count - 1; i++)
            {
                sb.Append(listTargetMaterial[i] + "\n");
            }
            string useNum = string.Format("共有 {0} 个Material用到：{1}", listTargetMaterial.Count - 1, selectedShaderName);
            sb.Append(useNum + "\n");
            sb.Append("用到的shader名字为：" + selectedShaderName);
            sw.Write(sb.ToString());

            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
