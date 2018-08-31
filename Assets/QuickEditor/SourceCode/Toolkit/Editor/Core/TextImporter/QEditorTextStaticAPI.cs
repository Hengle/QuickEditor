namespace QuickEditor.Toolkit
{
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class QEditorTextStaticAPI
    {
        public static bool IsFile(string file)
        {
            string extension = Path.GetExtension(file);
            if (extension == ".cs" || extension == ".js" || extension == ".boo" || extension == ".shader")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetFileFormatToUTF8_BOM(string file)
        {
            if (!File.Exists(file))
            {
                Debug.LogWarning(string.Format("不存在文件: {0}", file));
                return;
            }

            string extension = Path.GetExtension(file);
            if (extension == ".cs" || extension == ".js" || extension == ".boo" || extension == ".shader")
            {
                //先检测文件编码格式，防止无用刷新
                //先判断BOM模式，防止无用检测
                bool isUTF8_BOM = QEditorFileEncodingStaticAPI.isUTF8_BOM(file);
                Encoding fileEncoding = null;
                if (extension != ".shader")
                {
                    if (isUTF8_BOM)
                        return;
                }
                //shader脚本不添加签名，因为内置shader编译器暂不支持带签名的UTF8脚本
                else if (!isUTF8_BOM)
                {
                    fileEncoding = QEditorFileEncodingStaticAPI.GetType(file);
                    if (fileEncoding == Encoding.UTF8)
                        return;
                }

                //根据具体编码格式读出内容，再设置对象编码，防止出现乱码
                if (fileEncoding == null)
                    fileEncoding = QEditorFileEncodingStaticAPI.GetType(file);
                UTF8Encoding utf8 = new UTF8Encoding((extension != ".shader"));
                File.WriteAllText(file, File.ReadAllText(file, fileEncoding), utf8);
            }
        }

        public static void SetFolderFormatToUTF8_BOM(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Debug.LogWarning(string.Format("不存在文件夹：{0}", folder));
                return;
            }

            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                SetFileFormatToUTF8_BOM(file);
            }
        }

        public static void SetFileFormat(string file, Encoding encoding)
        {
            if (!File.Exists(file))
            {
                Debug.LogWarning(string.Format("不存在文件：{0}", file));
                return;
            }

            File.WriteAllText(file, File.ReadAllText(file, Encoding.Default), encoding);
        }
    }
}