using System.Security.Cryptography;

namespace BeeApps.Common.Services;

public class HashService : IHashService
{
    private static readonly RandomNumberGenerator _RNG = RandomNumberGenerator.Create();

    public string Hash(string input, string salt)
    {
        return Hash(input, Convert.FromBase64String(salt));
    }

    public string Hash(string input, byte[] salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(input, salt, 10000, HashAlgorithmName.SHA512);
        return Convert.ToBase64String(pbkdf2.GetBytes(512 / 8));
    }

    public byte[] GetSalt(int size)
    {
        var salt = new byte[size];
        _RNG.GetNonZeroBytes(salt);
        return salt;
    }
}