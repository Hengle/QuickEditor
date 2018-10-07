namespace QuickEditor.Builder
{
    public class BuildAction_End : BuildAction
    {
        public override BuildState OnUpdate()
        {
            return BuildState.Success;
        }
    }
}