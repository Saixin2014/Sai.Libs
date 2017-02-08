using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{

    public class ComUtil
    {

        private static string[] biaodian = { ".", "。", "？", "?", "！", "!", "…", ":", "：", ";", "；" };

        private static Boolean IsLast(string word)
        {
            for (int i = 0; i < biaodian.Length; i++)
            {
                if (word.Equals(biaodian[i]))
                {
                    return true;
                }
            }
            return false;

        }

        public static string FormatPdfText(string content)
        {
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\n", "\r\n");

            if (content.Length <= 100)
            {
                return content;
            }

            StringBuilder toReturn = new StringBuilder();
            string[] temp = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            for (int i = 0; i < temp.Length; i++)
            {
                string temp_I = temp[i].Trim();
                if (temp_I.Length >= 1)
                {
                    string temp_I_lastWord = temp_I.Substring(temp_I.Length - 1).Trim();
                    if (IsLast(temp_I_lastWord))
                    {
                        toReturn.Append(temp_I + "\r\n");
                    }
                    else
                    {
                        toReturn.Append(temp_I);
                    }
                }
            }

            return toReturn.ToString().Trim();

        }


        /// <summary>
        /// 替换多个空白字符为一个空白字符
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceSpan(string content)
        {
            RegexOptions options = RegexOptions.None;
            Regex reg1 = new Regex("[\r\n]{2,}", options);
            string str = reg1.Replace(content, "\r\n");
            Regex reg2 = new Regex("[ ]{2,}", options);
            str = reg2.Replace(str, " ");
            Regex reg = new Regex("[ ]{2,}", options);
            str = reg.Replace(str, " ");
            Regex reg3 = new Regex("[\n]{2,}", options);
            str = reg3.Replace(str, "\n");
            Regex reg4 = new Regex("[\t]{2,}", options);
            str = reg4.Replace(str, "\t");
            return str;
        }

        /// <summary>
        /// 小写
        /// </summary>
        /// <param name="chsstr"></param>
        /// <returns></returns>
        public static string LowerConvert(string chsstr)
        {
            string strRet = string.Empty;
            char[] ArrChar = chsstr.ToCharArray();
            foreach (char c in ArrChar)
            {
                strRet += SingleChs2UpLower(c.ToString());
            }
            return strRet;
        }
        /// <summary>
        /// 单个字母变小写
        /// </summary>
        /// <param name="singleChs"></param>
        /// <returns></returns>
        public static string SingleChs2UpLower(string singleChs)
        {
            string strRtn = singleChs;
            //将字母转为大/小写
            if (Regex.IsMatch(singleChs, "[A-Z]"))
            {
                strRtn = singleChs.ToLower();
            }
            //else if (Regex.IsMatch(SingleChs, "[a-z]"))
            //{
            //    strRtn = SingleChs.ToUpper();
            //}
            return strRtn;
        }
        public static string Remove(string str)
        {
            string regexstr = @"<[^>]*>";    //去除所有的标签
            //@"<script[^>]*?>.*?</script>" //去除所有脚本，中间部分也删除

            // string regexstr = @"<img[^>]*>";   //去除图片的正则
            // string regexstr = @"<(?!br).*?>";   //去除所有标签，只剩br
            // string regexstr = @"<table[^>]*?>.*?</table>";   //去除table里面的所有内容
            //string regexstr = @"<(?!img|br|p|/p).*?>";   //去除所有标签，只剩img,br,p

            string str1 = Regex.Replace(str, regexstr, string.Empty, RegexOptions.IgnoreCase);
            return str1;
        }
    }

    public static class StringToUniCode
    {
        /// <summary>  
        /// 字符串转为UniCode码字符串  
        /// </summary>  
        /// <param name="s"></param>  
        /// <returns></returns>  
        public static string StringToUnicode(string s)
        {
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("//u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }
        /// <summary>  
        /// Unicode字符串转为正常字符串  
        /// </summary>  
        /// <param name="srcText"></param>  
        /// <returns></returns>  
        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }
    }

    public class TextUtil
    {
        /// <summary>
        /// 已utf-8读取文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetText(string filePath)
        {
            if (!File.Exists(filePath)) { throw new FileNotFoundException(filePath + "文本文件不存在"); }
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                sr.Close();
            }
            return sb.ToString();
        }
        
        public static string Utf16ToUtf8(string utf16String)
        {
            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Return UTF8 bytes as ANSI string
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        public static void WriteTxt(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                //开始写入
                sw.Write(content);
                //清空缓冲区
                sw.Flush();
                sw.Dispose();
                //关闭流
                sw.Close();
            }
            //using (FileStream fs = new FileStream(path, FileMode.Create))
            //{
            //    StreamWriter sw = new StreamWriter(fs);
            //    //开始写入
            //    sw.Write(content);
            //    //清空缓冲区
            //    sw.Flush();
            //    //关闭流
            //    sw.Close();
            //    fs.Close();
            //}
        }

        public static void Write(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                //开始写入
                sw.WriteLine(content);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
        }
    }
}
