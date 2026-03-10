// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace System;

/// <summary>
/// 
/// </summary>
public sealed class CorrelationIdGenerator
{
    private const string Encode_32_Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
    private static readonly char[] s_prefix = new char[6];
    private  long _lastId = DateTime.UtcNow.Ticks;

    private static readonly ThreadLocal<char[]> s_charBufferThreadLocal = new(() =>
    {
        var buffer = new char[20];
        buffer[0] = s_prefix[0];
        buffer[1] = s_prefix[1];
        buffer[2] = s_prefix[2];
        buffer[3] = s_prefix[3];
        buffer[4] = s_prefix[4];
        buffer[5] = s_prefix[5];
        buffer[6] = '-';
        return buffer;
    });

    static CorrelationIdGenerator() => PopulatePrefix();

    private CorrelationIdGenerator() { }

    /// <summary>
    /// Returns a single instance of the <see cref="CorrelationIdGenerator"/>.
    /// </summary>
    public static CorrelationIdGenerator Instance { get; } = new CorrelationIdGenerator();

    /// <summary>
    /// Returns an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>
    /// </summary>
    public  string Next => GenerateImpl(Interlocked.Increment(ref _lastId));

    private static string GenerateImpl(long id)
    {
        var buffer = s_charBufferThreadLocal.Value!;

        buffer[7] = Encode_32_Chars[(int)(id >> 60) & 31];
        buffer[8] = Encode_32_Chars[(int)(id >> 55) & 31];
        buffer[9] = Encode_32_Chars[(int)(id >> 50) & 31];
        buffer[10] = Encode_32_Chars[(int)(id >> 45) & 31];
        buffer[11] = Encode_32_Chars[(int)(id >> 40) & 31];
        buffer[12] = Encode_32_Chars[(int)(id >> 35) & 31];
        buffer[13] = Encode_32_Chars[(int)(id >> 30) & 31];
        buffer[14] = Encode_32_Chars[(int)(id >> 25) & 31];
        buffer[15] = Encode_32_Chars[(int)(id >> 20) & 31];
        buffer[16] = Encode_32_Chars[(int)(id >> 15) & 31];
        buffer[17] = Encode_32_Chars[(int)(id >> 10) & 31];
        buffer[18] = Encode_32_Chars[(int)(id >> 5) & 31];
        buffer[19] = Encode_32_Chars[(int)id & 31];

        return new string(buffer, 0, buffer.Length);
    }

    private static void PopulatePrefix()
    {
        var machineHash = Math.Abs(Environment.MachineName.GetHashCode());
        var machineEncoded = Base36.Encode(machineHash);

        var i = s_prefix.Length - 1;
        var j = 0;
        while (i >= 0)
        {
            if (j < machineEncoded.Length)
            {
                s_prefix[i] = machineEncoded[j];
                j++;
            }
            else
            {
                s_prefix[i] = '0';
            }
            i--;
        }
    }

    internal static class Base36
    {
        private const string Base36Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Encode the given number into a <see cref="Base36"/>string.
        /// </summary>
        /// <param name="input">The number to encode.</param>
        /// <returns>Encoded <paramref name="input"/> as string.</returns>
        public static string Encode(long input)
        {
            var arr = Base36Characters.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(arr[input % 36]);
                input /= 36;
            }
            return new string([.. result]);
        }

        /// <summary>
        /// Decode the <see cref="Base36"/> encoded string into a long.
        /// </summary>
        /// <param name="input">The number to decode.</param>
        /// <returns>Decoded <paramref name="input"/> as long.</returns> 
        public static long Decode(string input)
        {
            var reversed = input.ToLower().Reverse();
            long result = 0;
            var pos = 0;
            foreach (var c in reversed)
            {
                result += Base36Characters.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }
    }
}
