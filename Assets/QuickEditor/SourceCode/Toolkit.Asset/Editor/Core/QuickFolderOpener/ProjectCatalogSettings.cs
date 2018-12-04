namespace QuickEditor.Toolkit
{
    using System.Collections.Generic;
    using QuickEditor.Core;

    public class ProjectCatalogSettings : QuickScriptableObject<ProjectCatalogSettings>
    {
        public List<string> AssetFolders;
        public List<string> ResourcesFolders;
    }
}