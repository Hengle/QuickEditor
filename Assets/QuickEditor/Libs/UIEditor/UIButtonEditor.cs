using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

[CustomEditor(typeof(UIButton))]

public class UIButtonEditor : ButtonEditor
{
    UIButton _button = null;
    UIButton button
    {
        get
        {
            if (_button == null)
                _button = target as UIButton;
            return _button;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ButtonInspector();
    }

    private void ButtonInspector()
    {
        button.useClickAnimation = EditorGUILayout.Toggle("UseClickAnimation", button.useClickAnimation);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        SpriteInfoInspector(button.highlightedSpriteInfo, "Highlighted");
        SpriteInfoInspector(button.pressedSpriteInfo, "Pressed");
        SpriteInfoInspector(button.disabledSpriteInfo, "Disabled");
        EditorGUILayout.EndVertical();
    }

    private void SpriteInfoInspector(SpriteButtonInfo info, string name)
    {
        EditorGUILayout.LabelField(name);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Atlas Name", info._atlasName);
        EditorGUILayout.LabelField("Sprite Name", info._spriteName);
        EditorGUILayout.ObjectField("Atlas", info._atlas, typeof(UIAtlas), false);
        EditorGUILayout.EndVertical();
    }
}
