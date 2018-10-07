namespace QuickEditor.Builder
{
    using QuickEditor.Common;
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class XcodeProjectSettingWindow : QEditorHorizontalSplitWindow
    {
        private ReorderableList mReorderableList;
        private Vector2 mReorderableScrollPosition;
        private Vector2 mSettingsScrollPosition;
        private XcodeProjectSetting mCurrentSetting;

        public static void Open()
        {
            WindowTitle = "Xcode Project Setting Configuration";
            var window = GetEditorWindow<XcodeProjectSettingWindow>();
            Undo.undoRedoPerformed += () =>
            {
                window.Repaint();
            };
        }

        protected override void OnEnable()
        {
            mCurrentSetting = XcodeProjectSetting.Current;
        }

        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(mCurrentSetting);
            }
        }

        protected override void DrawLeftRect()
        {
            base.DrawLeftRect();
            QEditorGUIStaticAPI.Toggle("EnableBitCode", ref mCurrentSetting.EnableBitCode);
            QEditorGUIStaticAPI.Toggle("EnableUIStatusBar", ref mCurrentSetting.EnableUIStatusBar);
            QEditorGUIStaticAPI.Toggle("DeleteLaunchImages", ref mCurrentSetting.DeleteLaunchImages);
            QEditorGUIStaticAPI.Toggle("NSAppTransportSecurity", ref mCurrentSetting.NSAppTransportSecurity);

            QEditorGUIStaticAPI.DoReorderableList<XcodeProjectSetting>(mCurrentSetting, mCurrentSetting.XcodeSettingsList, typeof(XcodeProjectConfig), ref mReorderableList, ref mReorderableScrollPosition, ref mSettingsScrollPosition, (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var elem = (AbstractXcodeConfig)mCurrentSetting.XcodeSettingsList[index];
                GUI.Label(rect, elem.SaveName);
                rect.x = rect.xMax - 15;
                elem.Enabled = GUI.Toggle(rect, elem.Enabled, string.Empty);
            }, () =>
            {
                mCurrentSetting.XcodeSettingsList.ForEach(item => item.Enabled = true);
            }, () =>
            {
                mCurrentSetting.XcodeSettingsList.ForEach(item => item.Enabled = false);
            });
        }

        protected override void DrawRightRect()
        {
            if (mReorderableList.index < 0) { mReorderableList.index = 0; }
            using (var scroll = new EditorGUILayout.ScrollViewScope(mSettingsScrollPosition))
            {
                mSettingsScrollPosition = scroll.scrollPosition;
                if (mReorderableList.index >= mCurrentSetting.XcodeSettingsList.Count)
                {
                    mReorderableList.index = 0;
                }
                if (mCurrentSetting.XcodeSettingsList.Count > 0)
                {
                    mCurrentSetting.XcodeSettingsList[mReorderableList.index].DrawInnerGUI();
                }
            }
        }

        private void DoReorderableList(IList list, Type listType)
        {
            if (mReorderableList == null)
            {
                mReorderableList = new ReorderableList(list, listType, true, false, false, false);
                mReorderableList.headerHeight = 0;
                mReorderableList.showDefaultBackground = false;
                mReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var elem = (AbstractXcodeConfig)list[index];
                    GUI.Label(rect, elem.SaveName);
                    rect.x = rect.xMax - 15;
                    elem.Enabled = GUI.Toggle(rect, elem.Enabled, string.Empty);
                };
            }
            QEditorGUIStaticAPI.Space();
            using (new QEditorGUILayout.HorizontalBlock())
            {
                QEditorGUIStaticAPI.Space();
                if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Plus"), GUIStyle.none, GUILayout.Width(16)))
                {
                    list.Add(Activator.CreateInstance(listType));
                    mReorderableList.index = list.Count - 1;
                    mReorderableList.GrabKeyboardFocus();
                    mSettingsScrollPosition.y = float.MaxValue;
                }
                QEditorGUIStaticAPI.Space();
                if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Minus"), GUIStyle.none, GUILayout.Width(16)))
                {
                    if (mReorderableList.index >= 0 && mReorderableList.index <= list.Count - 1)
                    {
                        Undo.RecordObject(XcodeProjectSetting.Current, "Removed Import Preset");
                        list.RemoveAt(mReorderableList.index);
                        mReorderableList.index = Mathf.Max(0, mReorderableList.index - 1);
                        mReorderableList.GrabKeyboardFocus();
                    }
                }
                QEditorGUIStaticAPI.FlexibleSpace();
                QEditorGUIStaticAPI.Button("Disable.All", EditorStyles.miniButtonLeft, () =>
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((AbstractXcodeConfig)list[i]).Enabled = false;
                    }
                });
                QEditorGUIStaticAPI.Button("Enable.All", EditorStyles.miniButtonRight, () =>
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((AbstractXcodeConfig)list[i]).Enabled = true;
                    }
                });
            }

            QEditorGUIStaticAPI.Space();
            QEditorGUIStaticAPI.DrawRect(EditorGUILayout.GetControlRect(false, 2), QEditorColors.DarkGrayX11);

            using (var scroll = new EditorGUILayout.ScrollViewScope(mReorderableScrollPosition))
            {
                mReorderableScrollPosition = scroll.scrollPosition;
                mReorderableList.DoLayoutList();
            }
        }
    }
}