namespace QuickEditor.UnityToolSet
{
    using System.Reflection;
    using UnityEditor;

    public class UnityToolSetToolbar
    {
        internal const int ConsoleNodePriority = 9000;
        internal const string ConsoleNodeName = UnityToolSetConstants.UnityToolSetRootNodeName + "Clean Console";

        [MenuItem(ConsoleNodeName)]
        private static void CleanConsole()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            MethodInfo methodInfo = assembly.GetType("UnityEditor.LogEntries").
                GetMethod("Clear");
            methodInfo.Invoke(new object(), null);
        }
    }
}