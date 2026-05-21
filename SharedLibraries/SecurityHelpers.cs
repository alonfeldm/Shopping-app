using System.Security.Cryptography;
using System;
using System.Text;
namespace SharedLibraries;

public static class SecurityHelpers
{
    public static Aes CreateAes()
    {
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        return aes;
    }
    // public static byte[] ExportPublicKey(RSA rsa)
    // {
    //     return rsa.ExportRSAPublicKey();
    // }
    public static byte[] EncryptWithPublicKey(byte[] Data, RSA ServerPublicKey)
    {
        return ServerPublicKey.Encrypt(Data, RSAEncryptionPadding.OaepSHA256);
    }
    public static string DecryptWithPrivateKey(byte[] Data, RSA rsa)
    {
        return Encoding.UTF8.GetString(rsa.Decrypt(Data, RSAEncryptionPadding.OaepSHA256));
    }
    public static byte[] EncryptWithSessionKey(byte[] Data, Aes aes)
    {
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(Data, 0, Data.Length);
    }
    public static byte[] DecryptWithSessionKey(byte[] Data, Aes aes)
    {
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(Data, 0, Data.Length);
    }
    public static string CreateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
    }
    public static string HashPassword(string Password, string Salt)
    {
        byte[] SaltBytes = Convert.FromBase64String(Salt);
        byte[] HashBytes = Rfc2898DeriveBytes.Pbkdf2(Password, SaltBytes, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(HashBytes);
    }
    public static bool VerifyPassword(string Password, string Salt, string Hash)
    {
        string NewHash = HashPassword(Password, Salt);
        if (NewHash == Hash)
        {
            return true;
        }
        return false;
    }
}
