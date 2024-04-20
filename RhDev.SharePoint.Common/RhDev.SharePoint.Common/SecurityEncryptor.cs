using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RhDev.SharePoint.Common
{
    public class SecurityEncryptor
    {
        internal static string usedSkey = "Q($1-5-^";

        public static string Encrypt(string pToEncrypt)
        {

            if (string.IsNullOrEmpty(pToEncrypt)) return string.Empty;

            //DES
            DESCryptoServiceProvider des = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(usedSkey),
                IV = Encoding.ASCII.GetBytes(usedSkey)
            };

            byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();


            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        public static string Decrypt(string pToDecrypt)
        {
            if (string.IsNullOrEmpty(pToDecrypt)) return string.Empty;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte) i;
            }
            // Make sure the key has 8 bytes and as the same value in Encrypt method.  
            des.Key = Encoding.ASCII.GetBytes(usedSkey);
            des.IV = Encoding.ASCII.GetBytes(usedSkey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            // create StringBuild object，createDecrypt use memorystream object
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
