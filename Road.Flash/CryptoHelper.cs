using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Road.Flash
{
    public class CryptoHelper
    {
        public static RSACryptoServiceProvider GetRSACrypto(string privateKey)
        {
            CspParameters csp = new CspParameters();
            csp.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
            rsa.FromXmlString(privateKey);
            return rsa;
        }

        /// <summary>
        ///  用RSA解密基于Base64编码的字符串，返回UTF-8编码的字符串
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string privateKey, string src)
        {
            CspParameters csp = new CspParameters();
            csp.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
            rsa.FromXmlString(privateKey);
            return RsaDecrypt(rsa, src);
        }
        public static string RsaDecrypt(RSACryptoServiceProvider rsa, string src)
        {
            byte[] srcData = Convert.FromBase64String(src);
            byte[] destData = rsa.Decrypt(srcData, false);
            return Encoding.UTF8.GetString(destData);
        }

        public static byte[] RsaDecryt2(RSACryptoServiceProvider rsa, string src)
        {
            byte[] srcData = Convert.FromBase64String(src);
            return rsa.Decrypt(srcData, false);
        }
    }
}
