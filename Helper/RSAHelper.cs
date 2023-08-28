using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MOFAAPI.Helpers
{
    public interface IRSAHelper
    {
        string Decrypt(string value);
    }
    public class RSAHelper : IRSAHelper
    {
        private readonly RSACryptoServiceProvider PrivateKey;
        private readonly string privateKeyName = "private.key.pem";
        private readonly string privateKeyPath = "RSA";

        public RSAHelper() => PrivateKey = GetPrivateKeyFromPemFile();

        public string Decrypt(string encrypted)
        {
            var decryptedBytes = PrivateKey.Decrypt(Convert.FromBase64String(encrypted), false);
            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }

        public string Encrypt(string textToEncrypt)
        {
            byte[] result = PrivateKey.Encrypt(Encoding.UTF8.GetBytes(textToEncrypt), false);
            return Convert.ToBase64String(result);
        }

        private RSACryptoServiceProvider GetPrivateKeyFromPemFile()
        {
            using TextReader privateKeyStringReader = new StringReader(File.ReadAllText(GetPath(privateKeyName, privateKeyPath)));
            AsymmetricCipherKeyPair pemReader = (AsymmetricCipherKeyPair)new PemReader(privateKeyStringReader).ReadObject();
            RSAParameters rsaPrivateCrtKeyParameters = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)pemReader.Private);
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.ImportParameters(rsaPrivateCrtKeyParameters);
            return rsaCryptoServiceProvider;
        }

        private string GetPath(string fileName, string filePath)
        {
            var path = Path.Combine(".", filePath);
            return Path.Combine(path, fileName);
        }
    }
}
