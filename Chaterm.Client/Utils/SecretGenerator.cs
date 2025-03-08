using System;
using System.Security.Cryptography;
using System.Text;

namespace Chaterm.Client.Utils;

public class SecretGenerator
{
    /// <summary>
    /// 指定された文字列をSHA512でハッシュ化し、Base64エンコードした文字列を返します。
    /// </summary>
    /// <param name="input">ハッシュ化したい文字列</param>
    /// <returns>ハッシュ値</returns>
    public static string ComputeHash(string input)
    {
        using SHA256 sha256Hash = SHA256.Create();

        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = sha256Hash.ComputeHash(bytes);
        string base64Hash = Convert.ToBase64String(hashBytes);

        return base64Hash;
    }
}
