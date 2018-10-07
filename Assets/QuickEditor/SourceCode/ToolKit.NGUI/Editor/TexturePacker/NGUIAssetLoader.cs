namespace QuickEditor.NGUIToolKit
{
    using UnityEditor;
    using UnityEngine;

    public class NGUIAssetLoader
    {
        public static GameObject LoadAtlasPrefab(string prefabFile)
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFile) as GameObject;
            if (prefabAsset == null)
            {
                GameObject temp = new GameObject();
                prefabAsset = PrefabUtility.CreatePrefab(prefabFile, temp);
                GameObject.DestroyImmediate(temp);
            }
            return prefabAsset;
        }

        public static Material LoadTransparentMaterial(string matFile)
        {
            Material matAsset = AssetDatabase.LoadAssetAtPath<Material>(matFile) as Material;
            if (matAsset == null)
            {
                matAsset = new Material(Shader.Find("Unlit/Transparent Colored"));
                AssetDatabase.CreateAsset(matAsset, matFile);
            }
            else
            {
                matAsset.shader = Shader.Find("Unlit/Transparent Colored");
            }
            return matAsset;
        }

        public static Material LoadETCMaterial(string matFile)
        {
            Material matAsset = AssetDatabase.LoadAssetAtPath<Material>(matFile) as Material;
            if (matAsset == null)
            {
                matAsset = new Material(Shader.Find("UI/UI_ETC"));
                AssetDatabase.CreateAsset(matAsset, matFile);
            }
            else
            {
                matAsset.shader = Shader.Find("UI/UI_ETC");
            }
            return matAsset;
        }

        public static Texture2D LoadTexture2D(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            return AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
        }

        public static Object CreatePrefab(GameObject go, string name, string path)
        {
            Object tmpPrefab = PrefabUtility.CreateEmptyPrefab(path.Replace(".png", ".prefab"));
            tmpPrefab = PrefabUtility.ReplacePrefab(go, tmpPrefab, ReplacePrefabOptions.ConnectToPrefab);
            Object.DestroyImmediate(go);
            return tmpPrefab;
        }
    }
}