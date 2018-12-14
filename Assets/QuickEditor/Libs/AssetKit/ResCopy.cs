using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResCopyConfig
{
    public class ResCopyExcel
    {
        public List<string> t2144;
        public List<string> banshu;
        public List<string> design_test;
        public List<string> shangwu;
        public List<string> test_in_1;
        public List<string> test_out_1;
    }
    public List<List<string>> ui_images;
    public List<List<string>> bg;
    public List<List<string>> cg;
    public List<List<string>> icon;
    public List<List<string>> config;
    public ResCopyExcel excel;
    public List<List<string>> spine;
    public List<List<string>> live2d;
    public List<List<string>> story;
    public List<List<string>> sound;
    public List<List<string>> effect;
}

public class ResCopy
{
    public enum ResPath
    {
        SVN,
    }

    [MenuItem("YLToolKit.Asset/拷贝 UI图片")]
    private static void CopyUIImages()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.ui_images, true);
        OnCopyFinished("UI Image");
    }

    [MenuItem("YLToolKit.Asset/拷贝 CG")]
    private static void CopyCG()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.cg, true);
        OnCopyFinished("CG");
    }

    [MenuItem("YLToolKit.Asset/拷贝 背景图片")]
    private static void CopyBG()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.bg, true);
        OnCopyFinished("BG");
    }

    [MenuItem("YLToolKit.Asset/拷贝 Icon以及其他动态加载的资源")]
    private static void CopyIcon()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.icon, true);
        OnCopyFinished("Icon");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/策划测试")]
    private static void CopyConfig_design_test()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.design_test);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/2144")]
    private static void CopyConfig_2144()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.t2144);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/版署")]
    private static void CopyConfig_banshu()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.banshu);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/商务")]
    private static void CopyConfig_shangwu()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.shangwu);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/内网测试服")]
    private static void CopyConfig_test_in_1()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.test_in_1);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 配置表/外网测试服")]
    private static void CopyConfig_test_out_1()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.config, false);
        CopyWithExcel(svn, Config.excel.test_out_1);
        OnCopyFinished("Excel");
    }

    [MenuItem("YLToolKit.Asset/拷贝 Spine")]
    private static void CopySpine()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.spine, false);
        OnCopyFinished("Spine");
    }

    [MenuItem("YLToolKit.Asset/拷贝 Live2d")]
    private static void CopyLive2D()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.live2d, false);
        OnCopyFinished("Live2D");
    }

    [MenuItem("YLToolKit.Asset/拷贝 剧本")]
    private static void CopyStory()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.story, false);
        OnCopyFinished("Story");
    }

    [MenuItem("YLToolKit.Asset/拷贝 背景音乐和音效")]
    private static void CopySound()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.sound, false);
        OnCopyFinished("BGM");
    }

    [MenuItem("YLToolKit.Asset/拷贝 特效")]
    private static void CopyEffect()
    {
        _config = null;
        if (!ignore_dialog && !SVNUpdate())
        {
            return;
        }
        string svn = GetSvnPath();
        CopyWithConfig(svn, Config.effect,false);
        OnCopyFinished("Effect");
    }

    static void CopyAtlas(string path)
    {
        if (path.Contains("UIImage"))
        {
            string dst = path.Replace("UIImage", "UIAtlas");
            if (File.Exists(dst))
            {
                File.Copy(path, dst);
            }
            else
            {
                Debug.LogError("有新的文件" + path);
            }
        }
    }

    //[MenuItem("YLToolKit.Asset/拷贝 全部资源")]
    //private static void CopyAll()
    //{
    //    _config = null;
    //    ignore_dialog = true;
    //    if (!SVNUpdate())
    //    {
    //        return;
    //    }
    //    CopyBG();
    //    CopyCG();
    //    CopyIcon();
    //    CopySpine();
    //    CopyLive2D();
    //    CopyStory();
    //    CopySound();
    //    CopyEffect();
    //    ignore_dialog = false;
    //    OnCopyFinished("All");
    //}

    [MenuItem("YLToolKit.Asset/清除进度弹窗", false, 8000)]
    private static void ClearProgressBar()
    {
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("YLToolKit.Asset/清除路径配置", false, 8000)]
    private static void ClearPathConfig()
    {
        PlayerPrefs.SetString(ResPath.SVN.ToString(), "");
        PlayerPrefs.Save();
    }

    [MenuItem("YLToolKit.Asset/SVN更新", false, 7000)]
    private static bool SVNUpdate()
    {
        string svn = GetSvnPath();
        if (!ProcessCMD.ProcessCommand("svn", "update \"" + svn + "\""))
        {
            return false;
        }
        return true;
    }

    private static string GetSvnPath()
    {
        string svn = PlayerPrefs.GetString(ResPath.SVN.ToString());
        if (string.IsNullOrEmpty(svn))
        {
            svn = EditorUtility.OpenFolderPanel("设置SVN目录", "", "") + "/";
            PlayerPrefs.SetString(ResPath.SVN.ToString(), svn);
        }
        return svn;
    }

    private static void CopyFile(string src, string dst, bool ignoreMeta, bool copyAtlas)
    {
        if (File.Exists(src))
        {
            if (ignoreMeta)
            {
                if (dst.Contains(".meta") && File.Exists(dst))
                {
                    return;
                }
            }
            File.Copy(src, Application.dataPath + dst, true);
            if (copyAtlas)
            {
                CopyAtlas(src);
            }
        }
        else
        {
            Debug.Log("文件" + src + "未找到");
        }
    }

    private static ResCopyConfig _config = null;

    private static ResCopyConfig Config
    {
        get
        {
            if (_config == null)
            {
                _config = JsonMapper.ToObject<ResCopyConfig>(File.ReadAllText(Application.dataPath + "/Editor/YLToolKit/AssetKit/ResourceCopys.json"));
            }
            return _config;
        }
    }

    private static void CopyDirectorySingle(string src, string dst, bool ignoremeta, bool copyAtlas)
    {
        DirectoryInfo srcDir = new DirectoryInfo(src);
        DirectoryInfo dstDir = new DirectoryInfo(dst);
        DirectoryInfo[] srcSubDirs = srcDir.GetDirectories();
        foreach (DirectoryInfo dir in srcSubDirs)
        {
            CopyDirectorySingle(dir.FullName, dstDir.FullName, ignoremeta, copyAtlas);
        }

        FileInfo[] files = srcDir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (!ignoremeta && file.Extension.Contains("meta"))
            {
                continue;
            }
            Debug.Log(file.FullName + " copy to\n" + dst + "\\" + file.Name);
            file.CopyTo(dst + "\\" + file.Name, true);
            if (copyAtlas)
            {
                CopyAtlas(file.FullName);
            }
        }
    }

    private static void CopyDirectoryTree(string src, string dst, bool ignoremeta, bool copyAtlas)
    {
        DirectoryInfo srcDir = new DirectoryInfo(src);
        DirectoryInfo dstDir = new DirectoryInfo(dst);
        DirectoryInfo[] srcSubDirs = srcDir.GetDirectories();
        foreach (DirectoryInfo dir in srcSubDirs)
        {
            string dirPath = dstDir.FullName.Replace("\\", "/");
            if (!dirPath.EndsWith("/"))
            {
                dirPath += "/";
            }
            if (!Directory.Exists(dirPath + dir.Name))
            {
                Directory.CreateDirectory(dirPath + dir.Name);
            }
            CopyDirectoryTree(dir.FullName, dirPath + dir.Name, ignoremeta, copyAtlas);
        }

        FileInfo[] files = srcDir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (!ignoremeta && file.Extension.Contains("meta"))
            {
                continue;
            }
            string dirPath = dstDir.FullName.Replace("\\", "/");
            if (!dirPath.EndsWith("/"))
            {
                dirPath += "/";
            }
            Debug.Log(file.FullName + " copy to\n" + dirPath + file.Name);
            file.CopyTo(dirPath + file.Name, true);
            if (copyAtlas)
            {
                CopyAtlas(file.FullName);
            }
        }
    }

    private static void LoadExcel(string src, string dst)
    {
        float i = 0;
        string[] files = Directory.GetFiles(src);
        try
        {
            ExcelLoader loader = new ExcelLoader();
            foreach (var path in files)
            {
                EditorUtility.DisplayProgressBar("Parse Excel", path, i / files.Length);
                i++;
                if (path.Contains("~$"))
                {
                    continue;
                }
                FileInfo info = new FileInfo(path);
                string name = info.FullName.Substring(src.Length);
                loader.Load(path, dst);
            }
            loader.CreateLoader();
        }
        catch (System.Exception e)
        {
            Debug.LogError(files[(int)(i - 1)] + "解析失败\n\n" + e.Message + "\n\n" + e.StackTrace);
        }
    }

    private static void CopyWithExcel(string svn ,List<string> list)
    {
        LoadExcel(svn + list[1], list[2]);
        EditorUtility.ClearProgressBar();
    }

    private static void CopyWithConfig(string svn, List<List<string>> lists, bool copyAtlas)
    {
        float i = 0;
        foreach (var list in lists)
        {
            i++;
            if (!EditorUtility.DisplayCancelableProgressBar("Copy Files", list[1], i / lists.Count))
            {
                if (list[0] == "dir")
                {
                    CopyDirectoryTree(svn + list[1], Application.dataPath + "/" + list[2], false, copyAtlas);
                }
                else if (list[0] == "dir_ignoremeta")
                {
                    CopyDirectoryTree(svn + list[1], Application.dataPath + "/" + list[2], true, copyAtlas);
                }
                else if (list[0] == "dir_single")
                {
                    CopyDirectorySingle(svn + list[1], Application.dataPath + "/" + list[2], false, copyAtlas);
                }
                else if (list[0] == "dir_single_ignoremeta")
                {
                    CopyDirectorySingle(svn + list[1], Application.dataPath + "/" + list[2], true, copyAtlas);
                }
                else if (list[0] == "file")
                {
                    CopyFile(svn + list[1], list[2], false, copyAtlas);
                }
                else if (list[0] == "file_ignoremeta")
                {
                    CopyFile(svn + list[1], list[2], true, copyAtlas);
                }
                else if (list[0] == "excel")
                {
                    LoadExcel(svn + list[1], list[2]);
                }
            }
            else
            {
                break;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private static bool ignore_dialog = false;

    private static void OnCopyFinished(string title)
    {
        Debug.Log(title + " Copys Success");
        AssetDatabase.Refresh();
        if (!ignore_dialog)
        {
            EditorUtility.DisplayDialog("CopyFiles", title + " Copys Success", "OK");
        }
    }

}
