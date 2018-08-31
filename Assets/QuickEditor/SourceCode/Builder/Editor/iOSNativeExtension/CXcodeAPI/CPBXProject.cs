namespace QuickEditor.Builder
{
    using System.IO;
    using UnityEditor.iOS.Xcode.Custom;

    public static class CPBXProject
    {
        private static string GetPBXProjectPath(string buildPath)
        {
            return PBXProject.GetPBXProjectPath(buildPath);
        }

        private static PBXProject GetPBXProject(string buildPath)
        {
            string projPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            return proj;
        }

        public static void AddLibToProject(string buildPath, string lib)
        {
            PBXProject proj = GetPBXProject(buildPath);
            string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            string fileGuid = proj.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
            proj.AddFileToBuild(targetGuid, fileGuid);
            File.WriteAllText(GetPBXProjectPath(buildPath), proj.WriteToString());
        }

        public static void SetBitCode(string buildPath, bool enableBitCode)
        {
            PBXProject proj = GetPBXProject(buildPath);
            string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            proj.SetBuildProperty(targetGuid, XcodeProjectConst.ENABLE_BITCODE_KEY, enableBitCode ? "YES" : "NO");
            File.WriteAllText(GetPBXProjectPath(buildPath), proj.WriteToString());
        }
    }
}
