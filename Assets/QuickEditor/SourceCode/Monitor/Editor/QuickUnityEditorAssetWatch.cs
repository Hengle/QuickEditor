namespace QuickEditor.Monitor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal sealed partial class QuickUnityEditorAssetWatch : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            Debug.Log("OnWillCreateAsset " + path);
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            List<string> result = new List<string>();
            foreach (var path in paths)
            {
                if (IsUnlocked(path))
                    result.Add(path);
                else
                    Debug.LogError(path + " is read-only.");
                if (path.EndsWith(".unity"))
                {
                    Scene scene = SceneManager.GetSceneByPath(path);
                    Debug.Log("当前保存的场景名称是 ：" + scene.name);
                }
            }
            return result.ToArray();
        }

        public static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            AssetMoveResult result = AssetMoveResult.DidNotMove;
            if (IsLocked(oldPath))
            {
                Debug.LogError(string.Format("Could not move {0} to {1} because {0} is locked!", oldPath, newPath));
                result = AssetMoveResult.FailedMove;
            }
            else if (IsLocked(newPath))
            {
                Debug.LogError(string.Format("Could not move {0} to {1} because {1} is locked!", oldPath, newPath));
                result = AssetMoveResult.FailedMove;
            }
            return result;
        }

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            if (IsLocked(assetPath))
            {
                Debug.LogError(string.Format("Could not delete {0} because it is locked!", assetPath));
                return AssetDeleteResult.FailedDelete;
            }
            return AssetDeleteResult.DidNotDelete;
        }

        public static bool IsOpenForEdit(string assetPath, out string message)
        {
            if (IsLocked(assetPath))
            {
                message = "File is locked for editing!";
                return false;
            }
            else
            {
                message = null;
                return true;
            }
        }

        private static bool IsUnlocked(string path)
        {
            return !IsLocked(path);
        }

        private static bool IsLocked(string path)
        {
            if (!File.Exists(path))
                return false;
            FileInfo fi = new FileInfo(path);
            return fi.IsReadOnly;
        }
    }

    internal sealed partial class QuickEditorAssetPostprocessorWatch : UnityEditor.AssetPostprocessor
    {
        public static bool IsInited { get; private set; }

        public delegate void OnPostprocessAllAssetsFunction(String[] importedAssets,
            String[] deletedAssets,
            String[] movedAssets,
            String[] movedFromAssetPaths);

        private static OnPostprocessAllAssetsFunction mOnPostprocessAllAssetsEvent;

        public static event OnPostprocessAllAssetsFunction OnPostprocessAllAssetsEvent
        {
            add { mOnPostprocessAllAssetsEvent += value; }
            remove { mOnPostprocessAllAssetsEvent -= value; }
        }

        static QuickEditorAssetPostprocessorWatch()
        {
            IsInited = true;
        }

        //        //a.Asset Auditing
        //        //在资源导入时进行检测，避免资源出错，规范资源命名等，具体做法可以参考AssetPostprocessor类，直接在文档里检索就可以了
        //        //b.Common Rules : Texture
        //        //1.Make sure Read/Write is disable
        //        //2.Disable mipmap if possible
        //        //3.Make sure textures are Compressed
        //        //4.Ensure sizes aren’t too large
        //        //   2048*2048  / 1024*1024  for UI atlas
        //        //   512*512 or smaller for model textures
        //        //c.Common Rules : Model
        //        //1.Make sure Read/Write is disable
        //        //2.Disable rig on non-character models
        //        //3.Enable mesh compression

        //        //d.Common Rules : Audio
        //        //1.MP3 compression on IOS
        //        //2.Vorbis compression on Android
        //        //3.”Force Mono”for mobile games
        //        //d.set bitrate as low as possible

        //        //模型导入之前调用
        //        public void OnPreprocessModel()
        //        {
        //            ModelImporter modelImporter = (ModelImporter)assetImporter;
        //            if (assetImporter.assetPath.Contains(".fbx"))
        //            {
        //                modelImporter.globalScale = 1.0f;
        //                //modelImporter.importMaterials = false;
        //            }
        //            Debug.Log("OnPreprocessModel = " + this.assetPath);
        //        }

        //        //模型导入之后调用
        //        public void OnPostprocessModel(GameObject go)
        //        {
        //            Debug.Log("OnPostprocessModel = " + this.assetPath);
        //        }

        //        //音频导入之前
        //        public void OnPreprocessAudio()
        //        {
        //            AudioImporter mAudioImporter = this.assetImporter as AudioImporter;
        //            if (mAudioImporter == null) return;

        //            AudioImporterSampleSettings settings = new AudioImporterSampleSettings();
        //            settings.compressionFormat = AudioCompressionFormat.Vorbis;
        //            settings.loadType = AudioClipLoadType.Streaming;
        //            settings.quality = 100;
        //            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        //            mAudioImporter.defaultSampleSettings = settings;

        //            mAudioImporter.preloadAudioData = true;
        //            var mOverrideSampleSettings = mAudioImporter.GetOverrideSampleSettings(AssetPostprocessorDefine.PlatformName_Android);
        //            mOverrideSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
        //            mOverrideSampleSettings.loadType = AudioClipLoadType.Streaming;
        //            mAudioImporter.SetOverrideSampleSettings(AssetPostprocessorDefine.PlatformName_Android, mOverrideSampleSettings);

        //            mOverrideSampleSettings = mAudioImporter.GetOverrideSampleSettings(AssetPostprocessorDefine.PlatformName_IOS);
        //            mOverrideSampleSettings.compressionFormat = AudioCompressionFormat.MP3;
        //            mOverrideSampleSettings.loadType = AudioClipLoadType.Streaming;
        //            mAudioImporter.SetOverrideSampleSettings(AssetPostprocessorDefine.PlatformName_IOS, mOverrideSampleSettings);

        //            mAudioImporter.forceToMono = false;
        //            mAudioImporter.loadInBackground = false;
        //            mAudioImporter.preloadAudioData = false;
        //            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        //            Debug.Log("OnPreprocessAudio = " + this.assetPath);
        //        }

        //        //音频导入之后
        //        public void OnPostprocessAudio(AudioClip clip)
        //        {
        //            Debug.Log("OnPostprocessAudio = " + this.assetPath);
        //        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (mOnPostprocessAllAssetsEvent != null)
            {
                mOnPostprocessAllAssetsEvent(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
            }
        }
    }
}