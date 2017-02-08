using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Utils
{
    /// <summary>
    /// AES加密解密 与JAVA AES CBC/NoPadding 是一致的
    /// </summary>
    public class MyAES
    {
        //private static byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
        //    0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10 };

        //private static byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
        //    0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10 };

        private static string KeyStr = "OIi7r85RbO0PBCKH";
        private static string IVStr = "H8EBGHcmZND02eP6";
        private static UTF8Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
        private static byte[] key = utf8WithoutBom.GetBytes(KeyStr);

        private static byte[] iv = utf8WithoutBom.GetBytes(IVStr);
        /// <summary>
        /// 加密AES
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AESEncrypt(string text)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.Zeros;//PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = key; //System.Text.Encoding.UTF8.GetBytes(password);

            byte[] keyBytes = new byte[16];//new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length) len = keyBytes.Length;

            System.Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;


            byte[] ivBytes = iv;//System.Text.Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(text);// Convert.FromBase64String(text);//

            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(cipherBytes);

        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AESDecrypt(string text)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.Zeros;//PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(text);

            byte[] pwdBytes = key;//System.Text.Encoding.UTF8.GetBytes(password);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length) len = keyBytes.Length;

            System.Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = iv;
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);


            //string str = utf8WithoutBom.GetString(plainText).TrimEnd("\0".ToCharArray());;
            string str = Encoding.UTF8.GetString(plainText).TrimEnd("\0".ToCharArray());
            return str;
        }

        public static string AESDecrypt(byte[] data)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.Zeros;//PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = data;// Convert.FromBase64String(text);

            byte[] pwdBytes = key;//System.Text.Encoding.UTF8.GetBytes(password);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length) len = keyBytes.Length;

            System.Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = iv;
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);


            //string str = utf8WithoutBom.GetString(plainText).TrimEnd("\0".ToCharArray());;
            string str = Encoding.UTF8.GetString(plainText).TrimEnd("\0".ToCharArray());
            return str;
        }
    }
}
