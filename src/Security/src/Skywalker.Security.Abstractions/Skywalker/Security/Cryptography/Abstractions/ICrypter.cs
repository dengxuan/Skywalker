namespace Skywalker.Security.Cryptography.Abstractions
{
    public interface ICrypter
    {
        byte[] Encrypt(byte[] bytes);

        byte[] Decrypt(byte[] bytes);
    }
}