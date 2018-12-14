using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class TempUIFormatting : ScriptableObject
{
    [MenuItem("Assets/Temp/SpriteRenderer转UISpriteRenderer")]
    static void PrefabFormatting()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("PrefabFormatting"+ "(" + i + "/" + objs.Length + ")", objs[i].name , 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".prefab")) continue;
                DoPrefabNode(objs[i]);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".prefab")) return;
            DoPrefabNode(Selection.activeObject as GameObject);
        }

        EditorUtility.ClearProgressBar();
    }

    private static void DoPrefabNode(Object obj)
    {
        bool refresh = false;
        GameObject go = GameObject.Instantiate(obj) as GameObject;

        var tempGo = new GameObject("Temp");

        GameObject.DestroyImmediate(tempGo);

        SpriteRenderer[] srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        foreach(var sr in srs)
        {
            var usr = sr.GetComponent<UISpriteRenderer>();
            if (usr == null)
            {
                sr.gameObject.AddComponent<UISpriteRenderer>();
                refresh = true;
            }
        }

        if (refresh)
        {
            PrefabUtility.ReplacePrefab(go, obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("DoPrefabNode=" + go.name);
        }
        else DestroyImmediate(go);
    }

    [MenuItem("Assets/Temp/Text替换成BYXText")]
    static void PrefabReplaceUIText()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("PrefabFormatting" + "(" + i + "/" + objs.Length + ")", objs[i].name, 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".prefab")) continue;
                DoReplaceUITextNode(path);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".prefab")) return;
            DoReplaceUITextNode(path);
        }

        EditorUtility.ClearProgressBar();
    }

    static void DoReplaceUITextNode(string file)
    {
        StringBuilder txt = new StringBuilder();
        string[] lines = File.ReadAllLines(file);

        foreach (var line in lines)
        {
            if (line.Contains("fileID: 708705254"))
            {
                txt.AppendLine("  m_Script: {fileID: 11500000, guid: 366f20df73f2bbc4395f6bc332434712, type: 3}");
            }
            else txt.AppendLine(line);
        }

        try
        {
            File.WriteAllText(file, txt.ToString());
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);
            Debug.Log(file);
        }
        catch (System.Exception e)
        {
            Utils.LogError(e.Message);
            return;
        }

    }


    [MenuItem("Assets/Temp/Image替换成UIImage")]
    static void PrefabReplaceUIImage()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("PrefabFormatting" + "(" + i + "/" + objs.Length + ")", objs[i].name, 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".prefab")) continue;
                DoReplaceUIImageNode(path);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".prefab")) return;
            DoReplaceUIImageNode(path);
        }

        EditorUtility.ClearProgressBar();
    }

    static void DoReplaceUIImageNode(string file)
    {
        StringBuilder txt = new StringBuilder();
        string[] lines = File.ReadAllLines(file);

        foreach(var line in lines)
        {
            if (line.Contains("fileID: -765806418"))
            {
                txt.AppendLine("  m_Script: {fileID: 11500000, guid: 67c824add1fc3614cb39da6c6581647d, type: 3}");
            }
            else txt.AppendLine(line);
        }

        try
        {
            File.WriteAllText(file, txt.ToString());
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);
            Debug.Log(file);
        }
        catch(System.Exception e)
        {
            Utils.LogError(e.Message);
            return;
        }

    }

    [MenuItem("Assets/Temp/UIImage黑底问题修复")]
    static void PrefabUIImageBlock()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("PrefabFormatting" + "(" + i + "/" + objs.Length + ")", objs[i].name, 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".prefab")) continue;
                DoUIImageBlockNode(objs[i]);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".prefab")) return;
            DoUIImageBlockNode(Selection.activeObject as GameObject);
        }

        EditorUtility.ClearProgressBar();
    }

    private static void DoUIImageBlockNode(Object obj)
    {
        bool refresh = false;
        GameObject go = GameObject.Instantiate(obj) as GameObject;

        if(go == null)
        {
            Debug.Log("name=" + AssetDatabase.GetAssetPath(obj));
            return ;
        }

        UIImage[] imgs = go.GetComponentsInChildren<UIImage>(true);
        foreach (var img in imgs)
        {
            if(img.sprite == null && img.color != Color.white && img.isRender == false)
            {
                img.isRender = true;

                refresh = true;
            }
        }

        if (refresh)
        {
            PrefabUtility.ReplacePrefab(go, obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("ImageToUISprite=" + go.name);
        }
        else DestroyImmediate(go);
    }


    [MenuItem("Assets/Temp/碎图设置tag为目录导出")]
    static void ImageFormatting()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("ImageFormatting" + "(" + i + "/" + objs.Length + ")" , objs[i].name , 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg")) continue;
                DoImageNode(objs[i]);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg")) return;
            DoImageNode(Selection.activeObject);
        }

        EditorUtility.ClearProgressBar();
    }

    private static void DoImageNode(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        Texture2D tex = obj as Texture2D;
        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

        path = path.Replace("\\", "/");
        string tagName = Path.GetDirectoryName(path.Replace("Assets/Res/UIImage/", ""));
        if (tex.width * tex.height >= 512 * 512
            || path.EndsWith("_big.png"))
            tagName = tagName + "/" + tex.name;

        texImp.spritePackingTag = tagName;
        texImp.SaveAndReimport();

    }

    [MenuItem("Assets/Temp/碎图设置tag为单张导出")]
    static void ImageFormattingSingle()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("ImageFormatting" + "(" + i + "/" + objs.Length + ")", objs[i].name, 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg")) continue;
                DoImageNodeSingle(objs[i]);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg")) return;
            DoImageNodeSingle(Selection.activeObject);
        }

        EditorUtility.ClearProgressBar();
    }

    private static void DoImageNodeSingle(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        Texture2D tex = obj as Texture2D;
        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

        path = path.Replace("\\", "/");
        string tagName = Path.GetDirectoryName(path.Replace("Assets/Res/UIImage/", ""));
        tagName = tagName + "/" + tex.name;

        texImp.spritePackingTag = tagName;
        texImp.SaveAndReimport();

    }

    [MenuItem("Assets/Temp/特效图片格式化为ETC2和PVRTC")]
    static void ImageFormattingPlatform()
    {
        UnityEngine.Object[] objs = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
        Debug.Log("objs count=" + objs.Length);
        if (objs != null && objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("ImageFormatting" + "(" + i + "/" + objs.Length + ")", objs[i].name, 1.0f * i / objs.Length);
                string path = AssetDatabase.GetAssetPath(objs[i]);
                if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg") && !path.ToLower().EndsWith(".tga")) continue;
                DoImageNodePlatform(objs[i]);
            }
        }
        else
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.ToLower().EndsWith(".png") && !path.ToLower().EndsWith(".jpg") && !path.ToLower().EndsWith(".tga")) return;
            DoImageNodePlatform(Selection.activeObject);
        }

        EditorUtility.ClearProgressBar();
    }

    private static void DoImageNodePlatform(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        Texture2D tex = obj as Texture2D;
        TextureImporter imp = TextureImporter.GetAtPath(path) as TextureImporter;
        imp.textureType = TextureImporterType.Default;//图集的类型  
        imp.textureShape = TextureImporterShape.Texture2D;
        imp.sRGBTexture = true;
        imp.alphaSource = TextureImporterAlphaSource.FromInput;
        imp.alphaIsTransparency = true;
        imp.isReadable = false;
        imp.mipmapEnabled = false;
        imp.filterMode = FilterMode.Bilinear;
        if (path.EndsWith(".jpg"))
        {
            imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGB4);
            imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGB4);
        }
        else
        {
            imp.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4);
            imp.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC2_RGBA8);
        }

        imp.SaveAndReimport();

    }

}