using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{
    public class Md5Util
    {
        public static String SEED = "0123456789ABCDEF";
        public static string Md5Hex(string str)
        {
            string md5Str = SEED;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string strMD5Value = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                int a = 0xf & buffer[i] >> 4;
                int b = buffer[i] & 0xf;
                strMD5Value += md5Str.Substring(0xf & buffer[i] >> 4, 1) + md5Str[buffer[i] & 0xf];
            }
            return strMD5Value;
        }

        /// <summary>
        /// 小写md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5LowerHex(string str)
        {
            string md5 = Md5Hex(str);
            md5 = ComUtil.LowerConvert(md5);
            return md5;
        }
    }
}
