namespace QuickEditor.Builder
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.iOS.Xcode.Custom;

    public static class CInfoPlist
    {
        private static string GetInfoPlistPath(string buildPath)
        {
            return Path.Combine(buildPath, XcodeProjectConst.INFO_PLIST_NAME);
        }

        private static PlistDocument GetInfoPlist(string buildPath)
        {
            string plistPath = GetInfoPlistPath(buildPath);
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            return plist;
        }

        /// <summary>
        /// URLSchemes
        /// </summary>
        public static void SetURLSchemes(string buildPath, string urlIdentifier, List<string> schemeList)
        {
            PlistDocument plist = GetInfoPlist(buildPath);

            PlistElementArray urlTypes;
            if (plist.root.values.ContainsKey(XcodeProjectConst.URL_TYPES_KEY))
            {
                urlTypes = plist.root[XcodeProjectConst.URL_TYPES_KEY].AsArray();
            }
            else
            {
                urlTypes = plist.root.CreateArray(XcodeProjectConst.URL_TYPES_KEY);
            }

            PlistElementDict itmeDict = urlTypes.AddDict();

            itmeDict.SetString(XcodeProjectConst.URL_TYPE_ROLE_KEY, "Editor");
            itmeDict.SetString(XcodeProjectConst.URL_IDENTIFIER_KEY, urlIdentifier);

            PlistElementArray schemesArray = itmeDict.CreateArray(XcodeProjectConst.URL_SCHEMES_KEY);
            if (itmeDict.values.ContainsKey(XcodeProjectConst.URL_SCHEMES_KEY))
            {
                schemesArray = itmeDict[XcodeProjectConst.URL_SCHEMES_KEY].AsArray();
            }
            else
            {
                schemesArray = itmeDict.CreateArray(XcodeProjectConst.URL_SCHEMES_KEY);
            }

            for (int i = 0; i < schemesArray.values.Count; i++)
            {
                schemeList.Remove(schemesArray.values[i].AsString());
            }

            foreach (string scheme in schemeList)
            {
                schemesArray.AddString(scheme);
            }

            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }

        /// <summary>
        /// ApplicationQueriesSchemes
        /// </summary>
        public static void SetApplicationQueriesSchemes(string buildPath, List<string> _applicationQueriesSchemes)
        {
            PlistDocument plist = GetInfoPlist(buildPath);

            //LSApplicationQueriesSchemes 白名单
            PlistElementArray queriesSchemes;
            if (plist.root.values.ContainsKey(XcodeProjectConst.APPLICATION_QUERIES_SCHEMES_KEY))
            {
                queriesSchemes = plist.root[XcodeProjectConst.APPLICATION_QUERIES_SCHEMES_KEY].AsArray();
            }
            else
            {
                queriesSchemes = plist.root.CreateArray(XcodeProjectConst.APPLICATION_QUERIES_SCHEMES_KEY);
            }

            foreach (string queriesScheme in _applicationQueriesSchemes)
            {
                if (!queriesSchemes.values.Contains(new PlistElementString(queriesScheme)))
                {
                    queriesSchemes.AddString(queriesScheme);
                }
            }

            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }

        /// <summary>
        /// 设置开始画面
        /// </summary>
        public static void DeleteLaunchImagesKey(string buildPath)
        {
            PlistDocument plist = GetInfoPlist(buildPath);

            //设置开始画面
            if (plist.root.values.ContainsKey(XcodeProjectConst.UI_LAUNCHI_IMAGES_KEY))
            {
                plist.root.values.Remove(XcodeProjectConst.UI_LAUNCHI_IMAGES_KEY);
            }
            if (plist.root.values.ContainsKey(XcodeProjectConst.UI_LAUNCHI_STORYBOARD_NAME_KEY))
            {
                plist.root.values.Remove(XcodeProjectConst.UI_LAUNCHI_STORYBOARD_NAME_KEY);
            }

            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }

        /// <summary>
        /// ATS
        /// </summary>
        public static void SetATS(string buildPath, bool enableATS)
        {
            PlistDocument plist = GetInfoPlist(buildPath);

            //ATS
            PlistElementDict atsDict = plist.root.CreateDict(XcodeProjectConst.ATS_KEY);
            atsDict.SetBoolean(XcodeProjectConst.ALLOWS_ARBITRARY_LOADS_KEY, enableATS);

            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }

        /// <summary>
        /// 状态栏设置
        /// </summary>
        public static void SetStatusBar(string buildPath, bool enable)
        {
            PlistDocument plist = GetInfoPlist(buildPath);

            //状态栏设置
            plist.root.SetBoolean(XcodeProjectConst.STATUS_HIDDEN_KEY, enable);
            plist.root.SetBoolean(XcodeProjectConst.STATUS_BAR_APPEARANCE_KEY, !enable);

            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }

        public static void AddStringKey(string buildPath, string key, string value)
        {
            PlistDocument plist = GetInfoPlist(buildPath);
            plist.root.SetString(key, value);
            plist.WriteToFile(GetInfoPlistPath(buildPath));
        }
    }
}