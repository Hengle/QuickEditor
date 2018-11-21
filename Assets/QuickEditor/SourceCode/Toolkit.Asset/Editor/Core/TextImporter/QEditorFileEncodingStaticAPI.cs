namespace QuickEditor.Toolkit
{
    using UnityEngine;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 获取文件的编码格式
    /// </summary>
    public class QEditorFileEncodingStaticAPI
    {
        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>是否是带签名的UTF8编码</returns>
        public static bool isUTF8_BOM(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            r.Close();
            fs.Close();

            return IsUTF8_BOMBytes(ss);
        }

        /// <summary>
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || IsUTF8_BOMBytes(ss))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();

            return reVal;
        }

        /// <summary>
        /// 通过给定的字节数组，判断其编码类型
        /// </summary>
        /// <param name="ss">字节数组</param>
        /// <returns>字节数组的编码类型</returns>
        public static Encoding GetType(byte[] ss)
        {
            Encoding reVal = Encoding.Default;

            if (IsUTF8Bytes(ss) || IsUTF8_BOMBytes(ss))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }

            return reVal;
        }

        /// <summary>
        /// 将文件格式转换为UTF-8-BOM
        /// </summary>
        /// <param name="FILE_NAME">文件路径</param>
        public static void CovertToUTF8_BOM(string FILE_NAME)
        {
            byte[] BomHeader = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.ReadWrite);

            //按默认编码获取文件内容
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            r.Close();

            bool isBom = false;
            if (ss.Length >= 3)
            {
                if (ss[0] == BomHeader[0] && ss[1] == BomHeader[1] && ss[2] == BomHeader[2])
                {
                    isBom = true;
                }
            }

            //将内容转换为UTF8格式，并添加Bom头
            if (!isBom)
            {
                string content = Encoding.Default.GetString(ss);
                byte[] newSS = Encoding.UTF8.GetBytes(content);

                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(BomHeader, 0, BomHeader.Length);
                fs.Write(newSS, 0, i);
            }

            fs.Close();
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;    //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }

            if (charByteCounter > 1)
            {
                Debug.LogError("非预期的byte格式");
            }

            return true;
        }

        /// <summary>
        /// 判断是否是带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8_BOMBytes(byte[] data)
        {
            if (data.Length < 3)
                return false;

            return ((data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF));
        }
    }
}