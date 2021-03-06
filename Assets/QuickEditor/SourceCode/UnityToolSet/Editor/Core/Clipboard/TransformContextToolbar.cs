﻿namespace QuickEditor.Toolkit
{
    using UnityEditor;
    using UnityEngine;

    public class TransformContextToolbar
    {
        private class TransformClipboard
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public bool isPositionSet = false;
            public bool isRotationSet = false;
            public bool isScaleSet = false;
        }

        private static TransformClipboard clipboard = new TransformClipboard();

        #region Copy Methods

        [MenuItem("CONTEXT/Transform/Copy Transform", false, 150)]
        private static void CopyTransform()
        {
            CopyPosition();
            CopyRotation();
            CopyScale();
        }

        [MenuItem("CONTEXT/Transform/Copy Position", false, 151)]
        private static void CopyPosition()
        {
            clipboard.position = Selection.activeTransform.localPosition;
            clipboard.isPositionSet = true;
        }

        [MenuItem("CONTEXT/Transform/Copy Rotation", false, 152)]
        private static void CopyRotation()
        {
            clipboard.rotation = Selection.activeTransform.localRotation;
            clipboard.isRotationSet = true;
        }

        [MenuItem("CONTEXT/Transform/Copy Scale", false, 153)]
        private static void CopyScale()
        {
            clipboard.scale = Selection.activeTransform.localScale;
            clipboard.isScaleSet = true;
        }

        #endregion Copy Methods

        #region Paste Methods

        [MenuItem("CONTEXT/Transform/Paste Transform", false, 200)]
        private static void PasteTransform()
        {
            PastePosition();
            PasteRotation();
            PasteScale();
        }

        [MenuItem("CONTEXT/Transform/Paste Position", false, 201)]
        private static void PastePosition()
        {
            Undo.RecordObject(Selection.activeTransform, "Paste Position");
            Selection.activeTransform.localPosition = clipboard.position;
        }

        [MenuItem("CONTEXT/Transform/Paste Rotation", false, 202)]
        private static void PasteRotation()
        {
            Undo.RecordObject(Selection.activeTransform, "Paste Rotation");
            Selection.activeTransform.localRotation = clipboard.rotation;
        }

        [MenuItem("CONTEXT/Transform/Paste Scale", false, 203)]
        private static void PasteScale()
        {
            Undo.RecordObject(Selection.activeTransform, "Paste Scale");
            Selection.activeTransform.localScale = clipboard.scale;
        }

        #endregion Paste Methods

        #region Validation

        [MenuItem("CONTEXT/Transform/Paste Transform", true)]
        private static bool ValidatePasteTransform()
        {
            return ValidatePastePosition() && ValidatePasteRotation() && ValidatePasteScale();
        }

        [MenuItem("CONTEXT/Transform/Paste Position", true)]
        private static bool ValidatePastePosition()
        {
            return clipboard.isPositionSet;
        }

        [MenuItem("CONTEXT/Transform/Paste Rotation", true)]
        private static bool ValidatePasteRotation()
        {
            return clipboard.isRotationSet;
        }

        [MenuItem("CONTEXT/Transform/Paste Scale", true)]
        private static bool ValidatePasteScale()
        {
            return clipboard.isScaleSet;
        }

        [MenuItem("CONTEXT/Transform/Push To Children", true)]
        private static bool ValidatePushToChildren()
        {
            return Selection.activeTransform.childCount > 0;
        }

        [MenuItem("CONTEXT/Transform/Push To Parent", true)]
        private static bool ValidatePushToParent()
        {
            return Selection.activeTransform.parent != null;
        }

        #endregion Validation
    }
}
