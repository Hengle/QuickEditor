namespace QuickEditor.Builder
{
    using System.IO;
    using UnityEngine;

    public partial class CXClass : System.IDisposable
    {
        private string mFilePath;
        private string mContent;
        private int Index;

        public CXClass(string path)
        {
            mFilePath = path;
            if (!System.IO.File.Exists(mFilePath))
            {
                Debug.LogError(mFilePath + "路径下文件不存在");
                return;
            }
        }

        public void Load()
        {
            StreamReader reader = new StreamReader(mFilePath);
            mContent = reader.ReadToEnd();
            reader.Close();
        }

        public void WriteBelow(string below, string text)
        {
            int beginIndex = mContent.IndexOf(below);
            if (beginIndex == -1)
            {
                Debug.LogError(mFilePath + "中没有找到标致" + below);
                return;
            }

            int endIndex = mContent.LastIndexOf("\n", beginIndex + below.Length);

            mContent = mContent.Substring(0, endIndex) + "\n" + text + "\n" + mContent.Substring(endIndex);
        }

        public void Replace(string below, string newText)
        {
            int beginIndex = mContent.IndexOf(below);
            if (beginIndex == -1)
            {
                Debug.LogError(mFilePath + "中没有找到标致" + below);
                return;
            }
            mContent = mContent.Replace(below, newText);
        }

        public void Write()
        {
            StreamWriter writer = new StreamWriter(mFilePath);
            writer.Write(mContent);
            writer.Close();
        }

        public void Dispose()
        {
        }
    }
}
