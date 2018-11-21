namespace QuickEditor.Toolkit
{
    using QuickEditor.Core;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class AudioImportSetting : QuickPresetsSetting
    {
        public enum SampleOverrideValues
        {
            _8000Hz = 8000,
            _11025Hz = 11025,
            _22050Hz = 22050,
            _44100Hz = 44100,
            _48000Hz = 48000,
            _96000Hz = 96000,
            _192000Hz = 192000
        }

        #region SavedSettings

        [SerializeField]
        private AudioCompressionFormat m_audioCompressionFormat;

        [SerializeField]
        private AudioClipLoadType m_AudioClipLoadType;

        [SerializeField]
        private int m_audioQuality;

        [SerializeField]
        private AudioSampleRateSetting m_sampleRateSetting;

        [SerializeField]
        private SampleOverrideValues m_sampleRateOverride;

        [SerializeField]
        private bool m_preloadAudioData;

        [SerializeField]
        private bool m_loadInBackground;

        [SerializeField]
        private bool m_forceToMono;

        #endregion SavedSettings

        #region Getter/Setter

        public AudioCompressionFormat AudioCompressionFormat
        {
            get
            {
                return m_audioCompressionFormat;
            }

            set
            {
                m_audioCompressionFormat = value;
            }
        }

        public AudioClipLoadType AudioClipLoadType
        {
            get
            {
                return m_AudioClipLoadType;
            }

            set
            {
                m_AudioClipLoadType = value;
            }
        }

        public int AudioQuality
        {
            get
            {
                return m_audioQuality;
            }

            set
            {
                m_audioQuality = value;
            }
        }

        public AudioSampleRateSetting SampleRateSetting
        {
            get
            {
                return m_sampleRateSetting;
            }

            set
            {
                m_sampleRateSetting = value;
            }
        }

        public SampleOverrideValues SampleRateOverride
        {
            get
            {
                return m_sampleRateOverride;
            }

            set
            {
                m_sampleRateOverride = value;
            }
        }

        public bool PreloadAudioData
        {
            get
            {
                return m_preloadAudioData;
            }

            set
            {
                m_preloadAudioData = value;
            }
        }

        public bool LoadInBackground
        {
            get
            {
                return m_loadInBackground;
            }

            set
            {
                m_loadInBackground = value;
            }
        }

        public bool ForceToMono
        {
            get
            {
                return m_forceToMono;
            }

            set
            {
                m_forceToMono = value;
            }
        }

        #endregion Getter/Setter

        public override void DrawInnerGUI()
        {
            DrawFilterGUI();

            GUILayout.Space(6);
            EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), QuickEditorColors.DarkGrayX11);
            GUILayout.Space(3);

            m_forceToMono = EditorGUILayout.Toggle("Force To Mono", m_forceToMono);
            m_loadInBackground = EditorGUILayout.Toggle("Load In Background", m_loadInBackground);

            GUILayout.Space(6);
            EditorGUILayout.LabelField("Default Import Settings", EditorStyles.boldLabel);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), QuickEditorColors.DarkGrayX11);
            GUILayout.Space(3);

            m_AudioClipLoadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Load Type", m_AudioClipLoadType);

            EditorGUI.BeginDisabledGroup(m_AudioClipLoadType == AudioClipLoadType.Streaming ? true : false);
            m_preloadAudioData = EditorGUILayout.Toggle("Preload Audio Data", m_preloadAudioData);
            EditorGUI.EndDisabledGroup();

            if (m_AudioClipLoadType == AudioClipLoadType.Streaming)
                m_preloadAudioData = false;

            m_audioCompressionFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Compression Format", m_audioCompressionFormat);
            m_audioQuality = EditorGUILayout.IntSlider("Quality", m_audioQuality, 1, 100);
            m_sampleRateSetting = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Sample Rate Setting", m_sampleRateSetting);

            if (m_sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate)
            {
                m_sampleRateOverride = (SampleOverrideValues)EditorGUILayout.EnumPopup("Sample Rate Setting", m_sampleRateOverride);
            }
        }
    }
}
