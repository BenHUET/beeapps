namespace BeeApps.Common.Services;

public interface IHashService
{
    public string Hash(string input, string salt);
    public string Hash(string input, byte[] salt);
    public byte[] GetSalt(int size);
}