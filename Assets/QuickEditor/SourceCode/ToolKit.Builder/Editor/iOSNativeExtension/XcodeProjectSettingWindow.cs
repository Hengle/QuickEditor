namespace QuickEditor.Builder
{
    using QuickEditor.Common;
    using System;
    using System.Collections;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class XcodeProjectSettingWindow : QEditorWindow
    {
        private ReorderableList mReorderableList;
        private Vector2 mReorderableScrollPosition;
        private Vector2 mSettingsScrollPosition;

        public static void Open()
        {
            WindowTitle = "Xcode Project Setting Configuration";
            var window = GetEditorWindow<XcodeProjectSettingWindow>();
            Undo.undoRedoPerformed += () =>
            {
                window.Repaint();
            };
        }

        protected override void OnGUI()
        {
            minSize = new Vector2(800, 600);
            Rect windowRect = position.WindowRect();
            Rect insetRect = windowRect.WithPadding(3);

            QEditorGUIStaticAPI.DrawRect(windowRect, QEditorColors.ChineseBlack);

            QEditorGUIStaticAPI.BeginChangeCheck();

            var setting = XcodeProjectSetting.Current;
            Rect topLeftPane = QEditorGUIStaticAPI.DrawLeftRect(insetRect, () =>
            {
                QEditorGUIStaticAPI.Toggle("EnableBitCode", ref setting.EnableBitCode);
                QEditorGUIStaticAPI.Toggle("EnableUIStatusBar", ref setting.EnableUIStatusBar);
                QEditorGUIStaticAPI.Toggle("DeleteLaunchImages", ref setting.DeleteLaunchImages);
                QEditorGUIStaticAPI.Toggle("NSAppTransportSecurity", ref setting.NSAppTransportSecurity);

                QEditorGUIStaticAPI.DoReorderableList<XcodeProjectSetting>(setting, setting.XcodeSettingsList, typeof(XcodeProjectConfig), ref mReorderableList, ref mReorderableScrollPosition, ref mSettingsScrollPosition, (Rect rect, int index, bool isActive, bool isFocused) =>
                 {
                     var elem = (AbstractXcodeConfig)setting.XcodeSettingsList[index];
                     GUI.Label(rect, elem.SaveName);
                     rect.x = rect.xMax - 15;
                     elem.Enabled = GUI.Toggle(rect, elem.Enabled, string.Empty);
                 }, () =>
                 {
                     setting.XcodeSettingsList.ForEach(item => item.Enabled = true);
                 }, () =>
                 {
                     setting.XcodeSettingsList.ForEach(item => item.Enabled = false);
                 });
            });
            QEditorGUIStaticAPI.DrawRightRect(insetRect, topLeftPane, () =>
            {
                if (mReorderableList.index < 0) { mReorderableList.index = 0; }
                using (var scroll = new EditorGUILayout.ScrollViewScope(mSettingsScrollPosition))
                {
                    mSettingsScrollPosition = scroll.scrollPosition;
                    if (mReorderableList.index >= setting.XcodeSettingsList.Count)
                    {
                        mReorderableList.index = 0;
                    }
                    if (setting.XcodeSettingsList.Count > 0)
                    {
                        setting.XcodeSettingsList[mReorderableList.index].DrawInnerGUI();
                    }
                }
            });
            QEditorGUIStaticAPI.EndChangeCheck(setting);
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