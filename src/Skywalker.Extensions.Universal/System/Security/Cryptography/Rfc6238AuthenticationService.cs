// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Net;
using System.Text;

namespace System.Security.Cryptography;

/// <summary>
///
/// </summary>
public static class Rfc6238AuthenticationService
{
    private static readonly TimeSpan s_timestep = TimeSpan.FromMinutes(3);
    private static readonly Encoding s_encoding = new UTF8Encoding(false, true);

    /// <summary>
    /// Generates a new 80-bit security token
    /// </summary>
    /// <returns></returns>
    public static byte[] GenerateRandomKey()
    {
        var bytes = new byte[20];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="hashAlgorithm"></param>
    /// <param name="timestepNumber"></param>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public static int ComputeTotp(
        byte[] key,
        ulong timestepNumber,
        string? modifier)
    {
        // # of 0's = length of pin
        const int Mod = 1000000;

        // See https://tools.ietf.org/html/rfc4226
        // We can add an optional modifier
        var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));

        var hash = HMACSHA1.HashData(key, ApplyModifier(timestepAsBytes, modifier));

        // Generate DT string
        var offset = hash[hash.Length - 1] & 0xf;
        Debug.Assert(offset + 4 < hash.Length);
        var binaryCode = (hash[offset] & 0x7f) << 24
                            | (hash[offset + 1] & 0xff) << 16
                            | (hash[offset + 2] & 0xff) << 8
                            | hash[offset + 3] & 0xff;

        return binaryCode % Mod;
    }

    private static byte[] ApplyModifier(byte[] input, string? modifier)
    {
        if (string.IsNullOrEmpty(modifier))
        {
            return input;
        }

        var modifierBytes = s_encoding.GetBytes(modifier);
        var combined = new byte[checked(input.Length + modifierBytes.Length)];
        Buffer.BlockCopy(input, 0, combined, 0, input.Length);
        Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
        return combined;
    }

    // More info: https://tools.ietf.org/html/rfc6238#section-4
    private static ulong GetCurrentTimeStepNumber()
    {
        var delta = DateTimeOffset.UtcNow - DateTimeOffset.UnixEpoch;
        return (ulong)(delta.Ticks / s_timestep.Ticks);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="securityToken"></param>
    /// <param name="modifier"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static int GenerateCode(byte[] securityToken, string? modifier = null)
    {
        if (securityToken == null)
        {
            throw new ArgumentNullException(nameof(securityToken));
        }

        // Allow a variance of no greater than 9 minutes in either direction
        var currentTimeStep = GetCurrentTimeStepNumber();

        return ComputeTotp(securityToken, currentTimeStep, modifier);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="securityToken"></param>
    /// <param name="code"></param>
    /// <param name="modifier"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool ValidateCode(byte[] securityToken, int code, string? modifier = null)
    {
        if (securityToken == null)
        {
            throw new ArgumentNullException(nameof(securityToken));
        }

        // Allow a variance of no greater than 9 minutes in either direction
        var currentTimeStep = GetCurrentTimeStepNumber();

        {
            for (var i = -2; i <= 2; i++)
            {
                var computedTotp = ComputeTotp(securityToken, (ulong)((long)currentTimeStep + i), modifier);
                if (computedTotp == code)
                {
                    return true;
                }
            }
        }

        // No match
        return false;
    }
}
