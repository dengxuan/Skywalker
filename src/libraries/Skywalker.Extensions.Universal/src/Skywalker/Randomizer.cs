using System.Security.Cryptography;

namespace Skywalker;

public static class Randomizer
{
    private const string Digits = "0123456789ABCDEFGHJKMNPRSTUVWXYZ";

#if NETSTANDARD2_0
    private static readonly Random s_random = new();
#endif
    public static byte[] Generate(int length)
    {
        if (length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var bytes = new byte[length];
        var random = RandomNumberGenerator.Create();
        random.GetBytes(bytes);
        return bytes;
    }

    public static int GenerateInt32(int toExclusive = int.MaxValue)
    {
#if NETSTANDARD2_0
        return s_random.Next(0, toExclusive);
#else
        return RandomNumberGenerator.GetInt32(0, toExclusive);
#endif
    }

    public static int GenerateInt32(int fromInclusive = 0, int toExclusive = int.MaxValue)
    {
#if NETSTANDARD2_0
        return s_random.Next(fromInclusive, toExclusive);
#else
        return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
#endif
    }

    public static long GenerateInt64()
    {
        var bytes = new byte[8];
        var random = RandomNumberGenerator.Create();
        random.GetBytes(bytes);
        return BitConverter.ToInt64(bytes, 0);
    }

    public static string GenerateString()
    {
        return GenerateString(8);
    }

    public static string GenerateString(int length, bool digitOnly = false)
    {
        if (length < 1 || length > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var result = new char[length];
        var data = new byte[length];
        var random = RandomNumberGenerator.Create();
        random.GetBytes(data);

        //确保首位字符始终为数字字符
        result[0] = Digits[data[0] % 10];

        for (var i = 1; i < length; i++)
        {
            result[i] = Digits[data[i] % (digitOnly ? 10 : 32)];
        }

        return new string(result);
    }

    public static T? WeightRandom<T>(this IEnumerable<T> enumerable, Func<T, int> weighter)
    {
        // this stores sum of weights of all elements before current
        var totalWeight = 0;
        // currently selected element
        T? selected = default;
        foreach (var data in enumerable)
        {
            // weight of current element
            var weight = weighter(data);
            var r = GenerateInt32(totalWeight + weight + 1);

            // probability of this is weight/(totalWeight+weight)
            if (r >= totalWeight)
            {
                // it is the probability of discarding last selected element and selecting current one instead
                selected = data;
            }
            // increase weight sum
            totalWeight += weight;
        }
        // when iterations end, selected is some element of sequence. 
        return selected;
    }
}
