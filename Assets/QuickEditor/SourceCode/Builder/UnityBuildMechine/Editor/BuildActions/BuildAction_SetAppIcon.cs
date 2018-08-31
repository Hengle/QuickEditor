using System.Collections.Generic;
using QuickEditor.Common;
using UnityEditor;
using UnityEngine;

namespace QuickEditor.Builder
{
    /// <summary>
    /// 设置图标
    /// </summary>
    public class BuildAction_SetAppIcon : BuildAction
    {
        public BuildTargetGroup Group;
        public Texture2D Icon;
        public string Path;

        public BuildAction_SetAppIcon(string path, BuildTargetGroup group)
        {
            Group = @group;
            this.Path = path;
        }

        public BuildAction_SetAppIcon(Texture2D texture, BuildTargetGroup group)
        {
            Group = @group;
            this.Icon = texture;
        }

        public override BuildState OnUpdate()
        {
            var tex = (Texture2D)AssetDatabase.LoadAssetAtPath(Path, typeof(Texture2D));

            if (tex == null)
            {
                Debug.LogError("Icon Not Found : " + Path);
                return BuildState.Failure;
            }
            QEditorStaticAPI.SetAppIcon(Group, Icon);

            return BuildState.Success;
        }

        public override BuildProgress GetProgress()
        {
            return null;
        }
    }
}