namespace QuickEditor.Builder
{
    using QuickEditor.Common;
    using QuickEditor.Common.ReorderableList;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class XcodeProjectConfig : AbstractXcodeConfig
    {
        public enum FrameworkStatus
        {
            Required,
            Optional
        }

        [Serializable]
        public class Framework
        {
            [Folder(PathType.ProjectPath, ".framework", Editable = true)]
            public string Path;

            public FrameworkStatus Status;

            public bool IsWeak
            {
                get { return this.Status == FrameworkStatus.Optional; }
            }
        }

        [Serializable]
        public class Bundle
        {
            [Folder(PathType.ProjectPath, ".bundle", Editable = true)]
            public string Path;
        }

        [Serializable]
        public class Library
        {
            [FilePath(PathType.ProjectPath, "Static Library", "a,dylib", Editable = true)]
            public string File;
        }

        [Serializable]
        public class CompileFile
        {
            [FilePath(PathType.ProjectPath, "Source Files", "h,m,mm,cpp,c", Editable = true)]
            public string File;

            [Folder(PathType.ProjectPath, Editable = true)]
            public string HeadersDirectory;

            public string CompileFlags;
        }

        [Serializable]
        public class BuildProperty
        {
            public string Name;
            public string Value;
        }

        public List<Framework> Frameworks = new List<Framework>();
        public List<Bundle> Bundles = new List<Bundle>();
        public List<Library> Librarys = new List<Library>();
        public List<CompileFile> CompileFiles = new List<CompileFile>();
        public List<Framework> DependentFrameworks = new List<Framework>();
        public List<Library> DependentLibrarys = new List<Library>();
        public List<BuildProperty> BuildPropertys = new List<BuildProperty>();
        public List<string> ApplicationQueriesSchemes = new List<string>();

        private bool mLibrarysFoldout = false;
        private bool mDependentLibrarysFoldout = false;
        private bool mFrameworksFoldout = false;
        private bool mDependentFrameworksFoldout = false;
        private bool mApplicationQueriesSchemesFoldout = false;
        private bool mBuildPropertysFoldout = false;

        public override void DrawInnerGUI()
        {
            DrawFilterGUI();
            QEditorGUIStaticAPI.DrawFoldableBlock(ref mFrameworksFoldout, "Frameworks", () =>
            {
                ReorderableListGUI.ListField(Frameworks, DrawFramework, 32, ReorderableListFlags.DisableAutoFocus);
            });

            QEditorGUIStaticAPI.DrawFoldableBlock(ref mDependentFrameworksFoldout, "Dependent Frameworks", () =>
            {
                ReorderableListGUI.ListField(DependentFrameworks, DrawFramework, 32, ReorderableListFlags.DisableAutoFocus);
            });
            QEditorGUIStaticAPI.DrawFoldableBlock(ref mLibrarysFoldout, "Librarys", () =>
            {
                ReorderableListGUI.ListField(Librarys, DrawLibrarys, 16, ReorderableListFlags.DisableAutoFocus);
            });
            QEditorGUIStaticAPI.DrawFoldableBlock(ref mDependentLibrarysFoldout, "Dependent Librarys", () =>
            {
                ReorderableListGUI.ListField(DependentLibrarys, DrawLibrarys, 16, ReorderableListFlags.DisableAutoFocus);
            });
            QEditorGUIStaticAPI.DrawFoldableBlock(ref mApplicationQueriesSchemesFoldout, "Application Queries Schemes", () =>
            {
                ReorderableListGUI.ListField(ApplicationQueriesSchemes, DrawApplicationQueriesSchemes, 16, ReorderableListFlags.DisableAutoFocus);
            });

            QEditorGUIStaticAPI.DrawFoldableBlock(ref mBuildPropertysFoldout, "Build Propertys", () =>
            {
                ReorderableListGUI.ListField(BuildPropertys, DrawBuildPropertys, 36, ReorderableListFlags.DisableAutoFocus);
            });
        }

        private Library DrawLibrarys(Rect position, Library entry)
        {
            const float leftWidth = 70;
            const float rowHeight = 16;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;
            if (entry == null) { entry = new Library { File = string.Empty }; }
            QEditorGUIStaticAPI.Label(leftUpper, "File");
            QEditorGUIStaticAPI.FileTextField(rightUpper, entry.File, ref entry.File, "Select Librarys", "png");
            return entry;
        }

        private BuildProperty DrawBuildPropertys(Rect position, BuildProperty entry)
        {
            const float leftWidth = 70;
            const float rowHeight = 16;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var leftLower = position;
            leftLower.xMin += 4;
            leftLower.xMax = leftUpper.xMin + leftWidth;
            leftLower.yMin = leftLower.yMax - rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            var rightLower = position;
            rightLower.yMin = rightLower.yMax - rowHeight;
            rightLower.xMin = leftLower.xMax + 2;

            if (entry == null) { entry = new BuildProperty { Name = string.Empty, Value = string.Empty }; }

            QEditorGUIStaticAPI.Label(leftUpper, "Name");
            QEditorGUIStaticAPI.Label(leftLower, "Value");

            if (entry.Name == null) { entry.Name = string.Empty; }

            QEditorGUIStaticAPI.TextField(rightUpper, ref entry.Name);
            QEditorGUIStaticAPI.TextField(rightLower, ref entry.Value);

            return entry;
        }

        private static string DrawApplicationQueriesSchemes(Rect position, string entry)
        {
            const float leftWidth = 70;
            const float rowHeight = 16;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            QEditorGUIStaticAPI.Label(leftUpper, "Value");
            QEditorGUIStaticAPI.TextField(rightUpper, ref entry);

            return entry;
        }

        private static Framework DrawFramework(Rect position, Framework entry)
        {
            const float leftWidth = 70;
            const float rowHeight = 16;

            var leftUpper = position;
            leftUpper.xMin += 4;
            leftUpper.xMax = leftUpper.xMin + leftWidth;
            leftUpper.yMax = leftUpper.yMin + rowHeight;

            var leftLower = position;
            leftLower.xMin += 4;
            leftLower.xMax = leftUpper.xMin + leftWidth;
            leftLower.yMin = leftLower.yMax - rowHeight;

            var rightUpper = position;
            rightUpper.xMin = leftUpper.xMax + 2;
            rightUpper.yMax = rightUpper.yMin + rowHeight;

            var rightLower = position;
            rightLower.yMin = rightLower.yMax - rowHeight;
            rightLower.xMin = leftLower.xMax + 2;

            if (entry == null) { entry = new Framework { Path = "", Status = FrameworkStatus.Optional }; }

            QEditorGUIStaticAPI.Label(leftUpper, "Path");
            QEditorGUIStaticAPI.Label(leftLower, "Status");

            if (entry.Path == null) { entry.Path = string.Empty; }
            QEditorGUIStaticAPI.FileTextField(rightUpper, entry.Path, ref entry.Path, "Select Framework", "png");
            QEditorGUIStaticAPI.EnumPopup(rightLower, entry.Status, ref entry.Status);
            return entry;
        }
    }
}
