using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileEncrypt
{
    public class FileHelper
    {

    }
    /// <summary>
    /// 异常处理
    /// </summary>
    public class UtilException:ApplicationException
    {
        public UtilException(string msg) : base(msg) { }
    }

    /// <summary>
    /// CryptHelp
    /// </summary>
    public class DESFileClass
    {
        private const ulong FC_TAG = 0xFC010203040506CF;

        private const int BUFFER_SIZE = 128 * 1024;

        /// <summary>
        /// 检验两个Byte数组是否相同
        /// </summary>
        /// <param name="b1">Byte数组</param>
        /// <param name="b2">Byte数组</param>
        /// <returns>true－相等</returns>
        private static bool CheckByteArrays(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// <param name="password">密码</param>
        /// <param name="salt"></param>
        /// <returns>加密对象</returns>
        private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);

            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        /// <summary>
        /// 生成指定长度的随机Byte数组
        /// </summary>
        /// <param name="count">Byte数组长度</param>
        /// <returns>随机Byte数组</returns>
        private static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            rand.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="inFile">待加密文件</param>
        /// <param name="outFile">加密后输入文件</param>
        /// <param name="password">加密密码</param>
        public static void EncryptFile(string inFile, string outFile, string password)
        {
            using (FileStream fin = File.OpenRead(inFile),
                fout = File.OpenWrite(outFile))
            {
                long lSize = fin.Length; // 输入文件长度
                int size = (int)lSize;
                byte[] bytes = new byte[BUFFER_SIZE]; // 缓存
                int read = -1; // 输入文件读取数量
                int value = 0;

                // 获取IV和salt
                byte[] IV = GenerateRandomBytes(16);
                byte[] salt = GenerateRandomBytes(16);

                // 创建加密对象
                SymmetricAlgorithm sma = DESFileClass.CreateRijndael(password, salt);
                sma.IV = IV;

                // 在输出文件开始部分写入IV和salt
                fout.Write(IV, 0, IV.Length);
                fout.Write(salt, 0, salt.Length);

                // 创建散列加密
                HashAlgorithm hasher = SHA256.Create();
                using (CryptoStream cout = new CryptoStream(fout, sma.CreateEncryptor(), CryptoStreamMode.Write),
                    chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    BinaryWriter bw = new BinaryWriter(cout);
                    bw.Write(lSize);

                    bw.Write(FC_TAG);

                    // 读写字节块到加密流缓冲区
                    while ((read = fin.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        cout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                    }
                    // 关闭加密流
                    chash.Flush();
                    chash.Close();

                    // 读取散列
                    byte[] hash = hasher.Hash;

                    // 输入文件写入散列
                    cout.Write(hash, 0, hash.Length);

                    // 关闭文件流
                    cout.Flush();
                    cout.Close();
                }
            }
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="inFile">待解密文件</param>
        /// <param name="outFile">解密后输出文件</param>
        /// <param name="password">解密密码</param>
        public static void DecryptFile(string inFile, string outFile, string password)
        {
            // 创建打开文件流
            using (FileStream fin = File.OpenRead(inFile),
                fout = File.OpenWrite(outFile))
            {
                int size = (int)fin.Length;
                byte[] bytes = new byte[BUFFER_SIZE];
                int read = -1;
                int value = 0;
                int outValue = 0;

                byte[] IV = new byte[16];
                fin.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fin.Read(salt, 0, 16);

                SymmetricAlgorithm sma = DESFileClass.CreateRijndael(password, salt);
                sma.IV = IV;

                value = 32;
                long lSize = -1;

                // 创建散列对象, 校验文件
                HashAlgorithm hasher = SHA256.Create();

                using (CryptoStream cin = new CryptoStream(fin, sma.CreateDecryptor(), CryptoStreamMode.Read),
                    chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    // 读取文件长度
                    BinaryReader br = new BinaryReader(cin);
                    lSize = br.ReadInt64();
                    ulong tag = br.ReadUInt64();

                    if (FC_TAG != tag)
                        throw new UtilException("文件被破坏");

                    long numReads = lSize / BUFFER_SIZE;

                    long slack = (long)lSize % BUFFER_SIZE;

                    for (int i = 0; i < numReads; ++i)
                    {
                        read = cin.Read(bytes, 0, bytes.Length);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }

                    if (slack > 0)
                    {
                        read = cin.Read(bytes, 0, (int)slack);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }

                    chash.Flush();
                    chash.Close();

                    fout.Flush();
                    fout.Close();

                    byte[] curHash = hasher.Hash;

                    // 获取比较和旧的散列对象
                    byte[] oldHash = new byte[hasher.HashSize / 8];
                    read = cin.Read(oldHash, 0, oldHash.Length);
                    if ((oldHash.Length != read) || (!CheckByteArrays(oldHash, curHash)))
                        throw new UtilException("文件被破坏");
                }

                if (outValue != lSize)
                    throw new UtilException("文件大小不匹配");
            }
        }


        #region AES加密
        /// <summary>       
        /// AES加密         
        /// </summary>        
        /// <param name="text">加密字符</param>        
        /// <param name="password">加密的密码</param>        
        /// <param name="iv">密钥(隨機生成)</param>        
        /// <returns></returns>
        public static string AESEncrypt(string text, string password, string iv)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                    len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(text);
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                return Convert.ToBase64String(cipherBytes);
            }
            catch
            {
                return text;
            }
        }
        #endregion

        #region 随机生成密钥
        /// <summary>        
        /// 随机生成密钥        
        /// </summary>        
        /// <returns></returns>
        public static string GetIv(int n)
        {
            char[] arrChar = new char[] { 'a', 'b', 'd', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 'q', 's', 't', 'u', 'v', 'w', 'z', 'y', 'x', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Q', 'P', 'R', 'T', 'S', 'V', 'U', 'W', 'X', 'Y', 'Z' };
            StringBuilder num = new StringBuilder();
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        #endregion

        #region AES解密
        /// <summary>        
        /// AES解密        
        /// </summary>        
        /// <param name="text"></param>        
        /// <param name="password"></param>        
        /// <param name="iv"></param>        
        /// <returns></returns>
        public static string AESDecrypt(string text, string password, string iv)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] encryptedData = Convert.FromBase64String(text);
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);
            }
            catch
            {
                return text;
            }
        }
        #endregion


    }

}
