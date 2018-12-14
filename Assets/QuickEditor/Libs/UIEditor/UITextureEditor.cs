using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(UITexture))]
public class UITextureEditor : RawImageEditor
{
    UITexture _texture = null;
    UITexture texture
    {
        get
        {
            if (_texture == null)
                _texture = target as UITexture;
            return _texture;
        }
    }

    private GUIStyle m_PreviewLabelStyle;

    protected GUIStyle previewLabelStyle
    {
        get
        {
            if (m_PreviewLabelStyle == null)
            {
                m_PreviewLabelStyle = new GUIStyle("PreOverlayLabel")
                {
                    richText = true,
                    alignment = TextAnchor.UpperLeft,
                    fontStyle = FontStyle.Normal
                };
            }

            return m_PreviewLabelStyle;
        }
    }

    public override void OnInspectorGUI()
    {
        bool bGray = EditorGUILayout.Toggle("Gray", texture.Gray);
        if (texture.Gray != bGray)
        {
            texture.Gray = bGray;
        }
        base.OnInspectorGUI();
        
        EditorGUILayout.ObjectField("RenderTexture", texture.rtexture, typeof(UnityEngine.RenderTexture), true);        
    }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        base.OnPreviewGUI(rect, background);

        EditorGUILayout.LabelField("Texture Name", texture.GetTextureName(), previewLabelStyle);
    }
}
