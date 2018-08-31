namespace QuickEditor.Common
{
    using System.Collections;
    using UnityEditor;

    public static class QEditorCoroutineExtensions
    {
        public static QEditorCoroutines.QEditorCoroutine StartCoroutine(this EditorWindow thisRef, IEnumerator coroutine)
        {
            return QEditorCoroutines.StartCoroutine(coroutine, thisRef);
        }

        public static QEditorCoroutines.QEditorCoroutine StartCoroutine(this EditorWindow thisRef, string methodName)
        {
            return QEditorCoroutines.StartCoroutine(methodName, thisRef);
        }

        public static QEditorCoroutines.QEditorCoroutine StartCoroutine(this EditorWindow thisRef, string methodName, object value)
        {
            return QEditorCoroutines.StartCoroutine(methodName, value, thisRef);
        }

        public static void StopCoroutine(this EditorWindow thisRef, IEnumerator coroutine)
        {
            QEditorCoroutines.StopCoroutine(coroutine, thisRef);
        }

        public static void StopCoroutine(this EditorWindow thisRef, string methodName)
        {
            QEditorCoroutines.StopCoroutine(methodName, thisRef);
        }

        public static void StopAllCoroutines(this EditorWindow thisRef)
        {
            QEditorCoroutines.StopAllCoroutines(thisRef);
        }
    }
}