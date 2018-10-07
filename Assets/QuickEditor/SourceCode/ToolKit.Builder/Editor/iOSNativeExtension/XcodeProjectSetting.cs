namespace QuickEditor.Builder
{
    using QuickEditor.Common;
    using System.Collections.Generic;
    using UnityEngine;

    public class XcodeProjectSetting : QScriptableObject<XcodeProjectSetting>
    {
        [SerializeField]
        [HideInInspector]
        public bool EnableBitCode = false;

        [SerializeField]
        [HideInInspector]
        public bool EnableUIStatusBar = false;

        [SerializeField]
        [HideInInspector]
        public bool DeleteLaunchImages = false;

        [SerializeField]
        [HideInInspector]
        public bool NSAppTransportSecurity = false;

        [SerializeField]
        [HideInInspector]
        public List<XcodeProjectConfig> XcodeSettingsList = new List<XcodeProjectConfig>();
    }
}
