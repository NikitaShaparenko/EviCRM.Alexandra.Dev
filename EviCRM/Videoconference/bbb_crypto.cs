using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace EviCRM.Alexandra.EviCRM.Videoconference
{
    class Crypto
    {
 public static string getSha1(string StrValue)
        {
            HashFx md = new HashFx();
            return md.encryptString(StrValue, 1);
        }
    }
    public class HashFx
    {
        public HashFx()
        {

        }
     
        public string encryptString(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

       
        public string encryptString(string strToEncryp, int Algorithm)
        {
            if (Algorithm == 1)
            {
                System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
                byte[] bytes = ue.GetBytes(strToEncryp);

                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                SHA1CryptoServiceProvider SHA = new SHA1CryptoServiceProvider();
                byte[] hashBytes = SHA.ComputeHash(bytes);

                // Convert the encrypted bytes back to a string (base 16)
                string hashString = "";

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
                }

                return hashString.PadLeft(32, '0');
            }
            else
            {
                return encryptString(strToEncryp);
            }
            return null;
        }
       
        public string encryptString(byte[] RawStringBytes)
        {

            string hashString = "";
            for (int i = 0; i < RawStringBytes.Length; i++)
            {
                hashString += Convert.ToString(RawStringBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');

        }
       
        public string Md5File(string filepath)
        {
            FileStream filestrm = new FileStream(filepath, FileMode.Open);
            byte[] md5byte = new byte[filestrm.Length];

            filestrm.Read(md5byte, 0, Convert.ToInt32(filestrm.Length.ToString()));
            byte[] ResultHash = HashByte(md5byte);


            string hashString = "";

            for (int i = 0; i < ResultHash.Length; i++)
            {
                hashString += Convert.ToString(ResultHash[i], 16).PadLeft(2, '0');
            }
            filestrm.Close();
            return hashString.PadLeft(32, '0');

        }

        public byte[] HashByte(byte[] bytes)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)

            return hashBytes;
        }

        public byte[] HashByte(byte[] bytes, int Algorithm)
        {
            byte[] hashBytes = null;

            if (Algorithm == 0)//MD5
            {

                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                hashBytes = md5.ComputeHash(bytes);
            }
            else if (Algorithm == 1)//SHA-1
            {
                SHA1CryptoServiceProvider SHS = new SHA1CryptoServiceProvider();
                hashBytes = SHS.ComputeHash(bytes);
            }
            else
            {
                return null;
            }
            return hashBytes;
        }

    }
}
