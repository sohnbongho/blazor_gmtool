using System.Security.Cryptography;
using System.Text;

namespace DbConnectionStringEncryptor.Helper;

public static class DbConnectionStringCryptoHelper
{
    private static readonly string _password = "P@ssw0rd!MySecretKey2025$Crypto";
    public static byte[] GetAesKey(string password)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static string Encrypt(string plainText)
    {
        var key = GetAesKey(_password);
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = aes.IV.Concat(cipherBytes).ToArray();
        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string cipherText)
    {
        var fullBytes = Convert.FromBase64String(cipherText);
        var iv = fullBytes.Take(16).ToArray();
        var cipherBytes = fullBytes.Skip(16).ToArray();

        var key = GetAesKey(_password);
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}