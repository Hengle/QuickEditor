using UnityEditor;

namespace QuickEditor.Builder
{
    public class BuildAction_SaveAndRefresh : BuildAction
    {
        public override BuildState OnUpdate()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return BuildState.Success;
        }

        public override BuildProgress GetProgress()
        {
            return null;
        }
    }
}