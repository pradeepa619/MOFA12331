using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MOFAAPI.Utilities
{
    public static class EncryptDecrypt
    {
        static string _RSApublickey = "<RSAKeyValue><Modulus>h/B+luesoLa/PtVujqlxTAhgmq+FZiDa7HDLfU0vtn6/XcsuHPgie6vDqhi5LNTdh/HKvAGoQMDDVmZpSyf5ocgP8x7jAFakgqNqlMWn1W3GmKi7juy+xhtZyzmy7XP/XAFEFJnN8AHA+b84zyKDKqX/JJHGVerFAkfrAw+0NM8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        static string _RSAprivatekey = "<RSAKeyValue><Modulus>h/B+luesoLa/PtVujqlxTAhgmq+FZiDa7HDLfU0vtn6/XcsuHPgie6vDqhi5LNTdh/HKvAGoQMDDVmZpSyf5ocgP8x7jAFakgqNqlMWn1W3GmKi7juy+xhtZyzmy7XP/XAFEFJnN8AHA+b84zyKDKqX/JJHGVerFAkfrAw+0NM8=</Modulus><Exponent>AQAB</Exponent><P>8ua/UYvhn1lvpwZz+q2l/R+2+nEiaE6R1yViC+WqCRShh3jtHm3meh8ogZiEMWZsxYsGMJ5eGNxXIwwHdZgw1w==</P><Q>j0UjVZ7q0nYZMAl35oRRHgPSPKMYGZy+SZBZyPl2O1fPgZLy808bxS+iSedTkDl74bLbUgc3jUpCdCnA4lGEyQ==</Q><DP>wBKk8KfLBsWjG5Fnvq73lbwxZcJ2ccLHYjhmWoAGP933iPefkTNYT/M5hZkD10hl9KNlhqaEQ+ZgZQ9LCrsIyw==</DP><DQ>ciWclGAMKWiMgthYXiH32xkiTMuIrQdUGJpY3qXazEcW/d6NkMrrjjx4abdTvan/CICO+xji6sBKl/CYS/i0+Q==</DQ><InverseQ>ULeW8JRmpoYq9C/3cGzCZn1ptUOmHmFOIGDs4AQxCdUyzIk3H5n6+fIK68KV/xvh3avYkTW8lPz0jBSOU47k6A==</InverseQ><D>J+zKQSfp24nQwXON9Phu/hW3ybajR0t/ANJf8X0p9j9jlPiHopp+BjR8UwjP2grqxXKOSIl0ERCMTmPlqN0kw53DTdLbluIuau9J8TCoLZO3sGGA85osYQosBRln22uNitU8LLGJx0rEnmWWfZFHLarUuA1JYS7GF5lOoOA56QE=</D></RSAKeyValue>";
        
        #region Not Used AESEncryption
        public static string AESEncrypt(string text)
        {
            try
            {
                string textToEncrypt = text;
                string ToReturn = "";
                string publickey = "santhosh";
                string secretkey = "engineer";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
                //Exception will be thrown to calling method  
            }
        }

        public static string AESDecrypt(string text)
        {
            try
            {
                string textToDecrypt = text;
                string ToReturn = "";
                string publickey = "santhosh";
                string privatekey = "engineer";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
                //Exception will be thrown to calling method  
            }
        }
        #endregion

        #region RESEncryptionDecryption
        static public string Encryption(string sTextToEncrypt)
        {
            try
            {
                string sEncryptedData=string.Empty;
                byte[] bTextToEncrypt = Encoding.ASCII.GetBytes(sTextToEncrypt);
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(_RSApublickey);//Assign RSA Public Key generated from online generator to encrypt using public key
                    sEncryptedData = Convert.ToBase64String(RSA.Encrypt(bTextToEncrypt, false));                    
                }
                return sEncryptedData;
            }
            catch (CryptographicException e)
            {
                return null;
                throw e; //Exception will be thrown to calling method  
            }
        }

        static public string Decryption(string sEncryptedText)
        {
            try
            {
                byte[] bytTextToDecrypt = Convert.FromBase64String(sEncryptedText);//getting bytes from base64 encoded string
                string sDecryptedData=string.Empty;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString(_RSAprivatekey);
                    sDecryptedData = Encoding.Default.GetString(RSA.Decrypt(bytTextToDecrypt, false));
                }
                return sDecryptedData;
            }
            catch (CryptographicException e)
            {
                return null;
                throw e;//Exception will be thrown to calling method            
            }
        }
        #endregion
    }
   }