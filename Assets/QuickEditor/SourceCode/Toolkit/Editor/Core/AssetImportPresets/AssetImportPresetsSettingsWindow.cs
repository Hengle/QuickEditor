using QuickEditor.Common;
using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QuickEditor.Toolkit
{
    public class AssetImportPresetsSettingsWindow : QEditorWindow
    {
        private int mSelectedTabIndex = 0;

        private string[] mAssetTabsText = new string[] { "Model", "Texture", "Audio" };

        private ReorderableList mAssetImportPresetsList;

        private Vector2 mAssetImportPresetsListScroll;
        private Vector2 settingsScroll;

        internal const int AssetImportPresetsNodePriority = 100;
        internal const string AssetImportPresetsNodeName = QEditorDefines.ToolkitRootNodeName + "Asset Import Presets";

        [MenuItem(AssetImportPresetsNodeName, false, AssetImportPresetsNodePriority)]
        private static void Init()
        {
            var window = GetWindow<AssetImportPresetsSettingsWindow>("Asset Import Presets Configuration", true)
                .minSize(new Vector2(800, 600));
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
            EditorGUI.DrawRect(windowRect, QEditorColors.ChineseBlack);

            EditorGUI.BeginChangeCheck();

            Rect topLeftPane = QEditorGUIStaticAPI.DrawLeftRect(insetRect, () =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUI.BeginChangeCheck();
                QEditorGUIStaticAPI.Toolbar(ref mSelectedTabIndex, mAssetTabsText, EditorStyles.toolbarButton);
                if (EditorGUI.EndChangeCheck())
                {
                    mAssetImportPresetsList = null;
                }
                GUILayout.EndHorizontal();

                switch (mSelectedTabIndex)
                {
                    default:
                    case 0:
                        DoReorderableList(AssetImportPresetsSettings.Current.MeshImportSettings, typeof(MeshImportSetting));
                        break;

                    case 1:
                        DoReorderableList(AssetImportPresetsSettings.Current.TextureImportSettings, typeof(TextureImportSetting));
                        break;

                    case 2:
                        DoReorderableList(AssetImportPresetsSettings.Current.AudioImportSettings, typeof(AudioImportSetting));
                        break;
                }
            });
            QEditorGUIStaticAPI.DrawRightRect(insetRect, topLeftPane, () =>
            {
                if (mAssetImportPresetsList.index < 0)
                {
                    mAssetImportPresetsList.index = 0;
                }

                using (var scroll = new EditorGUILayout.ScrollViewScope(settingsScroll))
                {
                    settingsScroll = scroll.scrollPosition;

                    switch (mSelectedTabIndex)
                    {
                        default:
                        case 0:
                            if (mAssetImportPresetsList.index >= AssetImportPresetsSettings.Current.MeshImportSettings.Count)
                            {
                                mAssetImportPresetsList.index = 0;
                            }
                            if (AssetImportPresetsSettings.Current.MeshImportSettings.Count > 0)
                            {
                                AssetImportPresetsSettings.Current.MeshImportSettings[mAssetImportPresetsList.index].DrawInnerGUI();
                            }
                            break;

                        case 1:
                            if (mAssetImportPresetsList.index >= AssetImportPresetsSettings.Current.TextureImportSettings.Count)
                            {
                                mAssetImportPresetsList.index = 0;
                            }
                            if (AssetImportPresetsSettings.Current.TextureImportSettings.Count > 0)
                            {
                                AssetImportPresetsSettings.Current.TextureImportSettings[mAssetImportPresetsList.index].DrawInnerGUI();
                            }
                            break;

                        case 2:
                            if (mAssetImportPresetsList.index >= AssetImportPresetsSettings.Current.AudioImportSettings.Count)
                            {
                                mAssetImportPresetsList.index = 0;
                            }
                            if (AssetImportPresetsSettings.Current.AudioImportSettings.Count > 0)
                            {
                                AssetImportPresetsSettings.Current.AudioImportSettings[mAssetImportPresetsList.index].DrawInnerGUI();
                            }
                            break;
                    }
                }
            });

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(AssetImportPresetsSettings.Current);
            }
        }

        private void DoReorderableList(IList list, Type listType)
        {
            QEditorGUIStaticAPI.DoReorderableList<AssetImportPresetsSettings>(AssetImportPresetsSettings.Current, list, listType, ref mAssetImportPresetsList, ref mAssetImportPresetsListScroll, ref settingsScroll, (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var elem = (AbstractImportSetting)list[index];
                GUI.Label(rect, elem.saveName);
                rect.x = rect.xMax - 15;
                elem.isEnabled = GUI.Toggle(rect, elem.isEnabled, string.Empty);
            }, () =>
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ((AbstractImportSetting)list[i]).isEnabled = true;
                }
            }, () =>
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ((AbstractImportSetting)list[i]).isEnabled = true;
                }
            });
        }
    }
}
