using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(UIImage))]
public class UIImageEditor : ImageEditor
{
    UIImage _sprite = null;
    UIImage sprite
    {
        get
        {
            if (_sprite == null)
                _sprite = target as UIImage;
            return _sprite;
        }
    }

    string[] allSprites = null;
    int index = 0;

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
        base.OnInspectorGUI();
        SpriteInspector();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _RefreshSpriteArray();
        _CheckCurrentIndex();
    }

    private void SpriteInspector()
    {
        bool isRender = EditorGUILayout.Toggle("IsRender", sprite.isRender);
        if(sprite.isRender != isRender)
        {
            sprite.isRender = isRender;
            sprite.SetAllDirty();
        }

        UIAtlas atlas = (UIAtlas)EditorGUILayout.ObjectField("Atlas", sprite._atlas, typeof(UIAtlas), false);
        EditorGUILayout.LabelField("AtlasName: " + sprite.GetAtlasName());
        EditorGUILayout.LabelField("SpriteName: " + sprite.GetSpriteName());

        //sprite.SetSpriteOffset(EditorGUILayout.RectField("Sprite Offset", sprite.GetSpriteOffset()));
        if (atlas != sprite._atlas)
        {
            sprite.SetAtlas(atlas);
            _RefreshSpriteArray();
            _CheckCurrentIndex();
            if (allSprites != null)
            {
                sprite.SetSpriteName(allSprites[index]);
            }
            else
            {
                sprite.SetSpriteName(null);
            }

        }

        _DrawSpriteNames();
    }

    private void _RefreshSpriteArray()
    {
        UIAtlas atlas = sprite._atlas;
        if (atlas != null && atlas._sprHash != null && atlas._sprHash.Count > 0)
        {
            if (allSprites == null || allSprites.Length != atlas._sprHash.Count)
            {
                allSprites = new string[atlas._sprHash.Count];
            }

            for (int i = 0; i < allSprites.Length; ++i)
            {
                allSprites[i] = atlas._sprHash[i];
            }
        }
        else
        {
            allSprites = null;
        }
    }

    private void _DrawSpriteNames()
    {
        if (allSprites == null || allSprites.Length <= 0)
            return;

        int newIndex = EditorGUILayout.Popup("Sprite", index, allSprites);

        if (index == newIndex)
            return;

        index = newIndex;
        sprite.SetSpriteName(allSprites[index]);
    }

    private void _CheckCurrentIndex()
    {
        if (allSprites == null || allSprites.Length <= 0)
            return;

        index = 0;
        if (sprite.sprite != null && !string.IsNullOrEmpty(sprite.sprite.name))
        {
            for (int i = 0; i < allSprites.Length; ++i)
            {
                if (sprite.sprite.name.Equals(allSprites[i]))
                {
                    index = i;
                    break;
                }
            }
        }
    }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        base.OnPreviewGUI(rect, background);

        if (sprite.sprite != null && sprite.sprite.texture != null)
        {            
            EditorGUILayout.LabelField("atlas name", sprite.GetAtlasName(), previewLabelStyle);
            EditorGUILayout.LabelField("sprite name", sprite.GetSpriteName(), previewLabelStyle);
            EditorGUILayout.LabelField("texture2D.name", sprite.sprite.texture.name, previewLabelStyle);
            EditorGUILayout.LabelField("texture2D.instanceId", sprite.sprite.texture.GetInstanceID().ToString(), previewLabelStyle);
            EditorGUILayout.LabelField("sprite.name", sprite.sprite.name, previewLabelStyle);

            EditorGUILayout.RectField("sprite.rect", sprite.sprite.textureRect);
        }        
    }
}
