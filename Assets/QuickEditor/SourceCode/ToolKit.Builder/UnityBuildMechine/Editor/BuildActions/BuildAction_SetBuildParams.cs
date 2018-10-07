using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickEditor.Builder
{
    public class BuildAction_SetBuildParams : BuildAction
    {
        public BuildTargetGroup Group;
        public string Path;

        public BuildAction_SetBuildParams(string path, BuildTargetGroup group)
        {
            Group = @group;
            this.Path = path;
        }

        public override BuildState OnUpdate()
        {
            var tex = (Texture2D)AssetDatabase.LoadAssetAtPath(Path, typeof(Texture2D));

            if (tex == null)
            {
                Debug.LogError("Icon Not Found : " + Path);
                return BuildState.Failure;
            }

            var count = PlayerSettings.GetIconSizesForTargetGroup(this.Group).Length;

            var textures = new List<Texture2D>();
            for (int i = 0; i < count; i++)
            {
                textures.Add(tex);
            }

            PlayerSettings.SetIconsForTargetGroup(this.Group, textures.ToArray());

            return BuildState.Success;
        }

        public override BuildProgress GetProgress()
        {
            return null;
        }
    }
}