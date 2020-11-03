using System;
using System.Security.Cryptography;

namespace Skywalker
{
    public static class Randomizer
    {
        private const string Digits = "0123456789ABCDEFGHJKMNPRSTUVWXYZ";
        private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();
        public static byte[] Generate(int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException("length");

            var bytes = new byte[length];
            _random.GetBytes(bytes);
            return bytes;
        }

        public static int GenerateInt32()
        {
            var bytes = new byte[4];
            _random.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long GenerateInt64()
        {
            var bytes = new byte[8];
            _random.GetBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static string GenerateString()
        {
            return GenerateString(8);
        }

        public static string GenerateString(int length, bool digitOnly = false)
        {
            if (length < 1 || length > 128)
                throw new ArgumentOutOfRangeException("length");

            var result = new char[length];
            var data = new byte[length];

            _random.GetBytes(data);

            //确保首位字符始终为数字字符
            result[0] = Digits[data[0] % 10];

            for (int i = 1; i < length; i++)
            {
                result[i] = Digits[data[i] % (digitOnly ? 10 : 32)];
            }

            return new string(result);
        }
    }
}
