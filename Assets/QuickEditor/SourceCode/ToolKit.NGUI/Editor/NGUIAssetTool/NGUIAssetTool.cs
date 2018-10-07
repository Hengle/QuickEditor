#if UNITY_NGUI

namespace QuickEditor.NGUIToolKit
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class NGUIAssetTool : EditorWindow
    {
        //图集存放位置
        private string AtlasWorkSpace = "Assets/GAssets/UIAtlas/";

        //图集小图资源存放位置
        private string AtlasAssetpath = "Assets/UI/UITexture/";

        //UI预制体资源位置
        private string PrefabAssetpath = "Assets/Resources/UIPrefab/";

        public enum OperateType
        {
            atlasFilter = 1,            //图集资源查重
            assetReferenceFind = 2,     //资源引用查询
        }

        [MenuItem("Tools/UIAssetTool")]
        private static void RunTool()
        {
            Rect re = new Rect(0, 0, 1000, 500);
            NGUIAssetTool window = (NGUIAssetTool)EditorWindow.GetWindowWithRect(typeof(NGUIAssetTool), re, true, "UI资源管理");
            window.Show();
            window.InitData();
        }

        //图集搜索文本
        private string searchAtlas = string.Empty;

        //资源搜索文本
        private string searchSprite = string.Empty;

        //当前选中图集
        private string selecitionAtlas = string.Empty;

        //当前选中资源
        private string selecitionSprite = string.Empty;

        //当前选中引用
        private string selectionRef = string.Empty;

        //图集列表
        private List<string> AtlasList = new List<string>();

        //图集资源容器
        private Dictionary<string, List<string>> AtlasAssetDic = new Dictionary<string, List<string>>();

        //<图片名称---图集>字典
        private Dictionary<string, List<string>> SpriteLinkDic = new Dictionary<string, List<string>>();

        //图片的引用
        private Dictionary<string, List<string>> SpriteReference = new Dictionary<string, List<string>>();

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns><c>true</c>, if data was inited, <c>false</c> otherwise.</returns>
        private bool InitData()
        {
            AtlasList.Clear();
            AtlasAssetDic.Clear();

            DirectoryInfo dic = new DirectoryInfo(AtlasWorkSpace);
            FileInfo[] atlases = dic.GetFiles();

            if (atlases == null)
                return false;

            for (int i = 0; i < atlases.Length; i++)
            {
                if (atlases[i].Name.Contains("meta"))
                    continue;

                AtlasList.Add(atlases[i].Name);
                if (!AtlasAssetDic.ContainsKey(atlases[i].Name))
                {
                    UIAtlas atlas = AssetDatabase.LoadAssetAtPath<UIAtlas>(AtlasWorkSpace + atlases[i].Name);
                    if (atlas == null)
                        continue;
                    BetterList<string> sprites = atlas.GetListOfSprites();
                    List<string> spriteName = new List<string>();
                    foreach (var sp in sprites)
                    {
                        spriteName.Add(sp);
                        if (!SpriteLinkDic.ContainsKey(sp))
                            SpriteLinkDic.Add(sp, new List<string>() { atlases[i].Name });
                        else
                            SpriteLinkDic[sp].Add(atlases[i].Name);
                    }
                    AtlasAssetDic.Add(atlases[i].Name, spriteName);
                }
                else
                {
                    UnityEngine.Debug.LogError("重复图集：" + atlases[i].Name);
                }
            }

            if (!LoadPrefab())
                return false;

            return true;
        }

        private bool LoadPrefab()
        {
            DirectoryInfo dirRoot = new DirectoryInfo(PrefabAssetpath);
            if (dirRoot != null)
            {
                LoadDirectory(dirRoot, PrefabAssetpath);
                return true;
            }

            return false;
        }

        //遍历文件夹
        private void LoadDirectory(DirectoryInfo dirRoot, string dirPath)
        {
            FileInfo[] files = dirRoot.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Contains(".meta"))
                    continue;

                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(dirPath + files[i].Name);
                if (obj != null)
                {
                    PushOnePrefab(obj, string.Empty);
                }
            }

            DirectoryInfo[] dirs = dirRoot.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                LoadDirectory(dirs[i], dirPath + dirs[i].Name + "/");
            }
        }

        //遍历预制体
        private void PushOnePrefab(GameObject obj, string path)
        {
            path = path + obj.name;
            UISprite sp = obj.GetComponent<UISprite>();
            if (sp != null)
            {
                if (sp.atlas != null && sp.spriteName != null)
                {
                    string name = sp.atlas.name + "/" + sp.spriteName;
                    if (!SpriteReference.ContainsKey(name))
                        SpriteReference.Add(name, new List<string>() { path });
                    else
                        SpriteReference[name].Add(path);
                }
                else
                    UnityEngine.Debug.Log("sprite empty" + path);
            }

            if (obj.transform.childCount > 0)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    PushOnePrefab(obj.transform.GetChild(i).gameObject, path + "/");
                }
            }
        }

        private Vector2 atlasPos = Vector2.zero;
        private Vector2 spritePos = Vector2.zero;
        private Vector2 refPos = Vector2.zero;
        private bool treeOpen = false;

        private void OnGUI()
        {
            //      NGUIEditorTools.DrawSeparator ();
            //      NGUIEditorTools.DrawOutline (new Rect(0,10,200,200));
            //      NGUIEditorTools.DrawList ("1", new string[]{ "A", "B" }, selecition);
            //      NGUIEditorTools.DrawPrefixButton ("lalalalal");

            GUILayout.BeginVertical();
            DrawSearchList();
            DrawAtlasList();

            if (!string.IsNullOrEmpty(selecitionAtlas) && !string.IsNullOrEmpty(selecitionSprite))
                DrawTexture(SpriteAssetPath(selecitionAtlas, selecitionSprite));
            DrawReference();
        }

        private void DrawAtlasList()
        {
            GUILayout.BeginArea(new Rect(210, 0, 205, 150));
            GUILayout.BeginVertical();
            NGUIEditorTools.DrawHeader("所在图集列表");
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            GUIContent d = new GUIContent("Atlas", AssetDatabase.GetCachedIcon("Assets/AssetBundles/UI/Prefab/Atlas/CommonAtlas.prefab"));
            atlasPos = GUILayout.BeginScrollView(atlasPos, GUILayout.Width(200), GUILayout.Height(130));
            if (SpriteLinkDic.ContainsKey(selecitionSprite))
            {
                //默认展示第一个图集中的资源
                if (SpriteLinkDic[selecitionSprite].Count > 0 && string.IsNullOrEmpty(selecitionAtlas))
                    selecitionAtlas = SpriteLinkDic[selecitionSprite][0];

                for (int i = 0; i < SpriteLinkDic[selecitionSprite].Count; i++)
                {
                    GUI.backgroundColor = selecitionAtlas.Equals(SpriteLinkDic[selecitionSprite][i]) ? new Color(0f, 1f, 1f, 1f) : Color.white;
                    GUILayout.BeginHorizontal();
                    d = new GUIContent(SpriteLinkDic[selecitionSprite][i], AssetDatabase.GetCachedIcon(AtlasWorkSpace + SpriteLinkDic[selecitionSprite][i]));
                    if (GUILayout.Button(d, GUILayout.Width(170), GUILayout.Height(20)))
                    {
                        selecitionAtlas = SpriteLinkDic[selecitionSprite][i];
                    }
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                }
            }
            GUILayout.EndScrollView();
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void DrawSearchList()
        {
            GUILayout.BeginArea(new Rect(0, 0, 205, 480));
            GUILayout.BeginVertical();
            GUILayout.Label("请输入资源名称：");
            searchSprite = GUILayout.TextField(searchSprite, 50, new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(20) });
            GUILayout.Space(3);
            NGUIEditorTools.DrawHeader("资源列表 (" + SpriteLinkDic.Count.ToString() + ")");
            GUILayout.Space(2);
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            GUIContent d = new GUIContent("Atlas", AssetDatabase.GetCachedIcon("Assets/AssetBundles/UI/Prefab/Atlas/CommonAtlas.prefab"));
            spritePos = GUILayout.BeginScrollView(spritePos, GUILayout.Width(200), GUILayout.Height(400));

            foreach (var spr in SpriteLinkDic)
            {
                if (!spr.Key.Contains(searchSprite))
                    continue;

                GUI.backgroundColor = selecitionSprite.Equals(spr.Key) ? new Color(0f, 1f, 1f, 1f) : Color.white;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(spr.Key, GUILayout.Width(170), GUILayout.Height(20)))
                {
                    selecitionSprite = spr.Key;
                    selecitionAtlas = string.Empty;
                }
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
            GUILayout.EndScrollView();
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private void DrawTexture(string path)
        {
            Texture tx = AssetDatabase.LoadAssetAtPath<Texture>(path);//"Assets/UI/UITexture/Common/btn_ditutanchuang_daanniu_weixuanzhong.png"

            if (tx != null)
            {
                GUILayout.BeginArea(new Rect(210, 160, 300, 340));

                GUILayout.BeginVertical();
                GUILayout.Label(tx.name, GUILayout.Width(300), GUILayout.Height(20));
                GUILayout.Label(string.Concat("大小：", tx.width, " x ", tx.height));
                GUILayout.EndVertical();
                GUILayout.EndArea();
                int width = 300;
                int height = 240;
                if (tx.height < height)
                    height = tx.height;
                if (tx.width < width)
                    width = tx.width;
                GUI.DrawTexture(new Rect(210, 200, width, height), tx);

                GUILayout.BeginArea(new Rect(210, 200 + height, 300, 60));
                GUILayout.Label("资源路径:");
                GUILayout.TextArea(path, GUILayout.Width(300), GUILayout.Height(40));
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginArea(new Rect(210, 160, 300, 80));
                GUILayout.BeginVertical();
                GUILayout.Label("未找到该资源！");
                GUILayout.Label("资源路径:");
                GUILayout.TextArea(path, GUILayout.Width(300), GUILayout.Height(40));
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void DrawReference()
        {
            GUILayout.BeginArea(new Rect(510, 0, 450, 500));
            GUILayout.BeginVertical();
            NGUIEditorTools.DrawHeader("引用列表：");
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            refPos = GUILayout.BeginScrollView(refPos, GUILayout.Width(450), GUILayout.Height(380));
            string refKey = selecitionAtlas.Split('.')[0] + "/" + selecitionSprite;
            if (SpriteReference.ContainsKey(refKey))
            {
                for (int i = 0; i < SpriteReference[refKey].Count; i++)
                {
                    GUI.backgroundColor = selectionRef.Equals(SpriteReference[refKey][i]) ? new Color(0f, 1f, 1f, 1f) : Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(SpriteReference[refKey][i], GUILayout.Width(430), GUILayout.Height(20)))
                    {
                        if (!selectionRef.Equals(SpriteReference[refKey][i]))
                            treeOpen = true;
                        else
                            treeOpen = !treeOpen;

                        selectionRef = SpriteReference[refKey][i];
                    }
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    if (treeOpen && selectionRef.Equals(SpriteReference[refKey][i]))
                    {
                        InitTreeState(selectionRef);
                        DrawAReferenceTree(selectionRef);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private Dictionary<string, bool> treeState = new Dictionary<string, bool>();

        //引用树形结构
        private void DrawAReferenceTree(string path)
        {
            string[] nodes = path.Split('/');
            GUILayout.BeginVertical();
            string sign = string.Empty;
            for (int i = 0; i < nodes.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (string.IsNullOrEmpty(nodes[i]))
                    continue;
                GUILayout.Space((i + 1) * 12);
                sign = treeState[nodes[i]] ? "\u25BC" + (char)0x200a : "\u25BA" + (char)0x200a;

                //          treeState [nodes [i]] = GUILayout.Toggle (treeState [nodes [i]], sign + nodes [i], GUILayout.Width (430 - (i + 1) * 10));

                if (!GUILayout.Toggle(true, sign + nodes[i], "PreToolbar2", GUILayout.Width(430 - (i + 1) * 10)))
                    treeState[nodes[i]] = !treeState[nodes[i]];
                //          treeState [nodes [i]] = GUILayout.Toggle (treeState [nodes [i]], GetText (nodes [i], treeState [nodes [i]]), "PreToolbar2", GUILayout.Width (430 - (i + 1) * 10));
                GUILayout.EndHorizontal();

                if (!treeState[nodes[i]])
                    break;
            }
            GUILayout.EndVertical();

            //      string text = "AAAAAA";
            //      bool state = true;
            //      if (state) text = "\u25BC" + (char)0x200a + text;
            //      else text = "\u25BA" + (char)0x200a + text;
            //
            //      GUILayout.BeginHorizontal();
            //      GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            //      if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
        }

        private string GetText(string text, bool state)
        {
            string returnValue = text;

            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            return returnValue;
        }

        private string curTree = string.Empty;

        private void InitTreeState(string path)
        {
            if (curTree.Equals(path))
                return;

            treeState.Clear();
            string[] nodes = path.Split('/');
            for (int i = 0; i < nodes.Length; i++)
            {
                if (!treeState.ContainsKey(nodes[i]))
                    treeState.Add(nodes[i], true);
                else
                    treeState.Add(nodes[i] + "(重复命名" + i + "）", true);
            }
            curTree = path;
        }

        //获取图片资源位置
        private string SpriteAssetPath(string atlasName, string spriteName)
        {
            atlasName = atlasName.Replace("Atlas", string.Empty);
            atlasName = atlasName.Replace("_", string.Empty);
            if (string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
                return string.Empty;

            if (!Directory.Exists(AtlasAssetpath + atlasName.Split('.')[0] + "/"))
                return AtlasAssetpath + atlasName.Split('.')[0] + "/";

            string path = AtlasAssetpath + atlasName.Split('.')[0] + "/" + spriteName + ".png";

            if (File.Exists(path))
                return path;
            else
                path = AtlasAssetpath + atlasName.Split('.')[0];

            DirectoryInfo resPath = new DirectoryInfo(path);
            FileInfo[] files = resPath.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Contains(spriteName) && !files[i].Name.Contains(".meta"))
                {
                    path = path + files[i].Name;
                }
            }

            return path;
        }
    }
}

#endif