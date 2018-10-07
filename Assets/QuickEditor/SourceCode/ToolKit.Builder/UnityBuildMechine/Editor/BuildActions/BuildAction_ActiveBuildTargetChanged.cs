using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEditor.Builder
{
    public class BuildAction_ActiveBuildTargetChanged : BuildAction
    {
        private BuildTarget BuildTarget;
        //private BuildState BuildState = BuildState.None;

        public BuildAction_ActiveBuildTargetChanged(BuildTarget buildTarget)
        {
            BuildTarget = buildTarget;
            //BuildState = BuildState.None;
            //EditorUserBuildSettings.activeBuildTargetChanged = delegate ()
            //{
            //    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget)
            //    {
            //        BuildState = BuildState.Success;
            //    }
            //};
        }

        public override BuildState OnUpdate()
        {
            return EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget) ? BuildState.Success : BuildState.Failure;
        }

        public override BuildProgress GetProgress()
        {
            return null;
        }
    }
}