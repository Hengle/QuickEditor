using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(UIAtlas))]
public class UIAtlasInspector : Editor
{
    UIAtlas _atlas = null;
    UIAtlas atlas
    {
        get
        {
            if (_atlas == null)
                _atlas = target as UIAtlas;
            return _atlas;
        }
    }

    public override void OnInspectorGUI()
    {
//         EditorGUILayout.LabelField("Atlas Name", atlas._name);
//         EditorGUILayout.ObjectField("Texture2D", atlas._tex2d, typeof(Texture2D), false);
        //for(int i = 0; i < atlas._uvs.Count; ++i)
        //{
        //    EditorGUILayout.LabelField("sprite " + i, atlas._uvs[i].name);
        //}
    }
}
