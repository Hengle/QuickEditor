namespace QuickEditor.Toolkit
{
    using System.Collections.Generic;
    using QuickEditor.Common;

    public class ProjectCatalogSettings : QScriptableObject<ProjectCatalogSettings>
    {
        public List<string> AssetFolders;
        public List<string> ResourcesFolders;
    }
}