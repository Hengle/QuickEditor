namespace QuickEditor.Builder
{
    public class XcodeProjectConst
    {
        #region 常量keys

        /// <summary>
        /// 工程根目录
        /// </summary>
        public const string PROJECT_ROOT = "$(PROJECT_DIR)/";

        /// <summary>
        /// Images.xcassets 默认的target
        /// </summary>

        public const string IMAGE_XCASSETS_DIRECTORY_NAME = "Unity-iPhone";

        /// <summary>
        /// LinkerFlags 所有链接标签
        /// </summary>
        public const string LINKER_FLAG_KEY = "OTHER_LDFLAGS";

        public const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";
        public const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";
        public const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";

        /// <summary>
        /// infoplist 路径
        /// </summary>
        public const string INFO_PLIST_NAME = "Info.plist";

        /// <summary>
        /// 常用的key
        /// </summary>
        public const string URL_TYPES_KEY = "CFBundleURLTypes";

        public const string URL_TYPE_ROLE_KEY = "CFBundleTypeRole";
        public const string URL_IDENTIFIER_KEY = "CFBundleURLName";
        public const string URL_SCHEMES_KEY = "CFBundleURLSchemes";

        public const string CLASS_PATH = "Classes/{0}";

        /// <summary>
        /// 开场动画
        /// </summary>
        public const string UI_LAUNCHI_IMAGES_KEY = "UILaunchImages";

        public const string UI_LAUNCHI_STORYBOARD_NAME_KEY = "UILaunchStoryboardName~iphone";

        public const string ATS_KEY = "NSAppTransportSecurity";
        public const string ALLOWS_ARBITRARY_LOADS_KEY = "NSAllowsArbitraryLoads";

        public const string APPLICATION_QUERIES_SCHEMES_KEY = "LSApplicationQueriesSchemes";

        public const string STATUS_HIDDEN_KEY = "UIStatusBarHidden";
        public const string STATUS_BAR_APPEARANCE_KEY = "UIViewControllerBasedStatusBarAppearance";

        #endregion 常量keys
    }
}
