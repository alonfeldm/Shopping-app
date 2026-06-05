using System.Security.Cryptography; // for aes and rsa 
using System;
using System.Text;
using System.Net.Security;// for encoding
namespace SharedLibraries;

public static class SecurityHelpers
{
    public static Aes CreateAes()// helper to create the aes key and iv
    {
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        return aes;
    }
    public static byte[] EncryptWithPublicKey(byte[] Data, RSA ServerPublicKey)// helper to encrypt with a public
    {
        return ServerPublicKey.Encrypt(Data, RSAEncryptionPadding.OaepSHA256);// ecnrypts with a public key
    }
    public static string DecryptWithPrivateKey(byte[] Data, RSA rsa)// helper to decrypt with a private key
    {
        return Encoding.UTF8.GetString(rsa.Decrypt(Data, RSAEncryptionPadding.OaepSHA256));// decrypts with a private key
    }
    public static byte[] EncryptWithSessionKey(byte[] Data, Aes aes)// helper to encrypt with the aes key
    {
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(Data, 0, Data.Length);
    }
    public static byte[] DecryptWithSessionKey(byte[] Data, Aes aes)// helper to decrypt with the aes key
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
    public static string HashPassword(string Password, string Salt, string Pepper)// helper to hash a password so its not kept in plain text in the database
    {
        byte[] SaltBytes = Convert.FromBase64String(Salt);// uses salt to add randomness to the hashed password
        byte[] HashBytes = Rfc2898DeriveBytes.Pbkdf2(Password + Pepper, SaltBytes, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(HashBytes);
    }
    public static bool VerifyPassword(string Password, string Salt, string Pepper, string Hash)
    // a helper to verify a password when logging in by hasing it and comparing it to the saved password
    {
        string NewHash = HashPassword(Password, Salt, Pepper);
        if (NewHash == Hash)
        {
            return true;
        }
        return false;
    }
}
