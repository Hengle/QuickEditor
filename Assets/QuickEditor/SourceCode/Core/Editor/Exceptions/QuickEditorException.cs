namespace QuickEditor.Core
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class QuickEditorException : Exception
    {
        public QuickEditorException() : base()
        {
        }

        public QuickEditorException(string message) : base(message)
        {
        }

        public QuickEditorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QuickEditorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
