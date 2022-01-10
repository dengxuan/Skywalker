using Skywalker;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace System;

/// <summary>
/// Extension methods for String class.
/// </summary>
public static class StringExtensions
{

    private static string ToHash(this string str, HashAlgorithm algorithm, Encoding encoding)
    {
        var bytes = encoding.GetBytes(str);
        var hashedBytes = algorithm.ComputeHash(bytes);
        return hashedBytes.ToHex();
    }

    private static bool VerifyHash(this string str, string hashedValue, HashAlgorithm algorithm, Encoding encoding)
    {
        var bytes = encoding.GetBytes(str);
        var hashedBytes = encoding.GetBytes(hashedValue);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    /// <summary>
    /// Adds a char to end of given string if it does not ends with the char.
    /// </summary>
    public static string EnsureEndsWith(this string str, char c)
    {
        return EnsureEndsWith(str, c, StringComparison.Ordinal);
    }

    /// <summary>
    /// Adds a char to end of given string if it does not ends with the char.
    /// </summary>
    public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.EndsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return str + c;
    }

    /// <summary>
    /// Adds a char to end of given string if it does not ends with the char.
    /// </summary>
    public static string EnsureEndsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.EndsWith(c.ToString(culture), ignoreCase, culture))
        {
            return str;
        }

        return str + c;
    }

    /// <summary>
    /// Adds a char to beginning of given string if it does not starts with the char.
    /// </summary>
    public static string EnsureStartsWith(this string str, char c)
    {
        return EnsureStartsWith(str, c, StringComparison.Ordinal);
    }

    /// <summary>
    /// Adds a char to beginning of given string if it does not starts with the char.
    /// </summary>
    public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.StartsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return c + str;
    }

    /// <summary>
    /// Adds a char to beginning of given string if it does not starts with the char.
    /// </summary>
    public static string EnsureStartsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.StartsWith(c.ToString(culture), ignoreCase, culture))
        {
            return str;
        }

        return c + str;
    }

    /// <summary>
    /// Indicates whether this string is null or an System.String.Empty string.
    /// </summary>
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// indicates whether this string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// indicates whether this string is empty, or consists only of white-space characters.
    /// </summary>
    /// <exception cref="ArgumentNullException">If this string is null</exception>
    public static bool IsEmptyOrWhiteSpace(this string str)
    {
        string trimed = str.NotNull(nameof(str)).Trim();
        return string.Empty == trimed;
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
    public static string Left(this string str, int len)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.Length < len)
        {
            throw new ArgumentException("len argument can not be bigger than given string's length!");
        }
#if NETSTANDARD2_0
        return str.Substring(0, len);
#else
        return str[..len];
#endif
    }

    /// <summary>
    /// Converts line endings in the string to <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string NormalizeLineEndings(this string str)
    {
        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
    }

    /// <summary>
    /// Gets index of nth occurence of a char in a string.
    /// </summary>
    /// <param name="str">source string to be searched</param>
    /// <param name="c">Char to search in <see cref="str"/></param>
    /// <param name="n">Count of the occurence</param>
    public static int NthIndexOf(this string str, char c, int n)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var count = 0;
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] != c)
            {
                continue;
            }

            if ((++count) == n)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes first occurrence of the given postfixes from end of the given string.
    /// Ordering is important. If one of the postFixes is matched, others will not be tested.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="postFixes">one or more postfix.</param>
    /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
    public static string? RemovePostFix(this string str, params string[] postFixes)
    {
        if (str == null)
        {
            return null;
        }

        if (str == string.Empty)
        {
            return string.Empty;
        }

        if (postFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix))
            {
                return str.Left(str.Length - postFix.Length);
            }
        }

        return str;
    }

    /// <summary>
    /// Removes first occurrence of the given prefixes from beginning of the given string.
    /// Ordering is important. If one of the preFixes is matched, others will not be tested.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="preFixes">one or more prefix.</param>
    /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
    public static string? RemovePreFix(this string str, params string[] preFixes)
    {
        if (str == null)
        {
            return null;
        }

        if (str == string.Empty)
        {
            return string.Empty;
        }

        if (preFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var preFix in preFixes)
        {
            if (str.StartsWith(preFix))
            {
                return str.Right(str.Length - preFix.Length);
            }
        }

        return str;
    }

    /// <summary>
    /// Gets a substring of a string from end of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
    public static string Right(this string str, int len)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (str.Length < len)
        {
            throw new ArgumentException("len argument can not be bigger than given string's length!");
        }

        return str.Substring(str.Length - len, len);
    }

    /// <summary>
    /// Uses string.Split method to split given string by given separator.
    /// </summary>
    public static string[] Split(this string str, string separator)
    {
        return str.Split(new[] { separator }, StringSplitOptions.None);
    }

    /// <summary>
    /// Uses string.Split method to split given string by given separator.
    /// </summary>
    public static string[] Split(this string str, string separator, StringSplitOptions options)
    {
        return str.Split(new[] { separator }, options);
    }

    /// <summary>
    /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string str)
    {
        return str.Split(Environment.NewLine);
    }

    /// <summary>
    /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string str, StringSplitOptions options)
    {
        return str.Split(Environment.NewLine, options);
    }

    /// <summary>
    /// Converts PascalCase string to camelCase string.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <param name="invariantCulture">Invariant culture</param>
    /// <returns>camelCase of the string</returns>
    public static string ToCamelCase(this string str, bool invariantCulture = true)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return invariantCulture ? str.ToLowerInvariant() : str.ToLower();
        }
#if NETSTANDARD2_0
        return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
#else
        return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str[1..];
#endif
    }

    /// <summary>
    /// Converts PascalCase string to camelCase string in specified culture.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <param name="culture">An object that supplies culture-specific casing rules</param>
    /// <returns>camelCase of the string</returns>
    public static string ToCamelCase(this string str, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return str.ToLower(culture);
        }

#if NETSTANDARD2_0
        return char.ToLower(str[0], culture) + str.Substring(1);
#else
        return char.ToLower(str[0], culture) + str[1..];
#endif
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
    /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
    /// </summary>
    /// <param name="str">String to convert.</param>
    /// <param name="invariantCulture">Invariant culture</param>
    public static string ToSentenceCase(this string str, bool invariantCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        return Regex.Replace(str, "[a-z][A-Z]", evaluator =>
        {
            return $"{evaluator.Value[0]} {(invariantCulture ? char.ToLowerInvariant(evaluator.Value[1]) : char.ToLower(evaluator.Value[1]))}";
        });
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
    /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
    /// </summary>
    /// <param name="str">String to convert.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    public static string ToSentenceCase(this string str, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        return Regex.Replace(str, "[a-z][A-Z]", evaluator =>
        {
            return $"{evaluator.Value[0]} {char.ToLower(evaluator.Value[1], culture)}";
        });
    }

    /// <summary>
    /// Converts string to enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">String value to convert</param>
    /// <returns>Returns enum object</returns>
    public static T ToEnum<T>(this string value) where T : struct
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return (T)Enum.Parse(typeof(T), value);
    }

    /// <summary>
    /// Converts string to enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">String value to convert</param>
    /// <param name="ignoreCase">Ignore case</param>
    /// <returns>Returns enum object</returns>
    public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    /// <summary>
    /// Converts camelCase string to PascalCase string.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <param name="invariantCulture">Invariant culture</param>
    /// <returns>PascalCase of the string</returns>
    public static string ToPascalCase(this string str, bool invariantCulture = true)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return invariantCulture ? str.ToUpperInvariant() : str.ToUpper();
        }
#if NETSTANDARD2_0
        return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str.Substring(1);
#else
        return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str[1..];
#endif
    }

    /// <summary>
    /// Converts camelCase string to PascalCase string in specified culture.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <param name="culture">An object that supplies culture-specific casing rules</param>
    /// <returns>PascalCase of the string</returns>
    public static string ToPascalCase(this string str, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return str.ToUpper(culture);
        }
#if NETSTANDARD2_0
        return char.ToUpper(str[0], culture) + str.Substring(1);
#else
        return char.ToUpper(str[0], culture) + str[1..];
#endif
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    public static string? Truncate(this string str, int maxLength)
    {
        if (str == null)
        {
            return null;
        }

        if (str.Length <= maxLength)
        {
            return str;
        }

        return str.Left(maxLength);
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    /// It adds a "..." postfix to end of the string if it's truncated.
    /// Returning string can not be longer than maxLength.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    public static string? TruncateWithPostfix(this string str, int maxLength)
    {
        return TruncateWithPostfix(str, maxLength, "...");
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
    /// It adds given <paramref name="postfix"/> to end of the string if it's truncated.
    /// Returning string can not be longer than maxLength.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    public static string? TruncateWithPostfix(this string str, int maxLength, string postfix)
    {
        if (str == null)
        {
            return null;
        }

        if (str == string.Empty || maxLength == 0)
        {
            return string.Empty;
        }

        if (str.Length <= maxLength)
        {
            return str;
        }

        if (maxLength <= postfix.Length)
        {
            return postfix.Left(maxLength);
        }

        return str.Left(maxLength - postfix.Length) + postfix;
    }

    [DebuggerStepThrough]
    public static bool IsMissing(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    [DebuggerStepThrough]
    public static bool IsPresent(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    [DebuggerStepThrough]
    public static string ToSpaceSeparatedString(this IEnumerable<string> list)
    {
        if (list == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(100);

        foreach (var element in list)
        {
            sb.Append(element + " ");
        }

        return sb.ToString().Trim();
    }

    [DebuggerStepThrough]
    public static IEnumerable<string> FromSpaceSeparatedString(this string input)
    {
        input = input.Trim();
        return input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string>? ParseScopesString(this string scopes)
    {
        if (scopes.IsMissing())
        {
            return null;
        }

        scopes = scopes.Trim();
        var parsedScopes = scopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();

        if (parsedScopes.Any())
        {
            parsedScopes.Sort();
            return parsedScopes;
        }

        return null;
    }

    [DebuggerStepThrough]
    public static bool IsMissingOrTooLong(this string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }
        if (value.Length > maxLength)
        {
            return true;
        }

        return false;
    }

    [DebuggerStepThrough]
    public static string? EnsureLeadingSlash(this string url)
    {
        if (url != null && !url.StartsWith("/"))
        {
            return "/" + url;
        }

        return url;
    }

    [DebuggerStepThrough]
    public static string? EnsureTrailingSlash(this string url)
    {
        if (url != null && !url.EndsWith("/"))
        {
            return url + "/";
        }

        return url;
    }

    [DebuggerStepThrough]
    public static string? RemoveLeadingSlash(this string url)
    {
        if (url != null && url.StartsWith("/"))
        {
#if NETSTANDARD2_0
            url = url.Substring(1);
#else
            url = url[1..];
#endif
        }

        return url;
    }

    [DebuggerStepThrough]
    public static string? RemoveTrailingSlash(this string url)
    {
        if (url != null && url.EndsWith("/"))
        {
#if NETSTANDARD2_0
            url = url.Substring(0, url.Length - 1);
#else
            url = url[0..^1];
#endif
        }

        return url;
    }

    [DebuggerStepThrough]
    public static string CleanUrlPath(this string url)
    {
        if (string.IsNullOrWhiteSpace(url)) url = "/";

        if (url != "/" && url.EndsWith("/"))
        {
#if NETSTANDARD2_0
            url = url.Substring(0, url.Length - 1);
#else
            url = url[0..^1];
#endif
        }

        return url;
    }

    [DebuggerStepThrough]
    public static bool IsLocalUrl(this string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        // Allows "/" or "/foo" but not "//" or "/\".
        if (url[0] == '/')
        {
            // url is exactly "/"
            if (url.Length == 1)
            {
                return true;
            }

            // url doesn't start with "//" or "/\"
            if (url[1] != '/' && url[1] != '\\')
            {
                return true;
            }

            return false;
        }

        // Allows "~/" or "~/foo" but not "~//" or "~/\".
        if (url[0] == '~' && url.Length > 1 && url[1] == '/')
        {
            // url is exactly "~/"
            if (url.Length == 2)
            {
                return true;
            }

            // url doesn't start with "~//" or "~/\"
            if (url[2] != '/' && url[2] != '\\')
            {
                return true;
            }

            return false;
        }

        return false;
    }

    [DebuggerStepThrough]
    public static string AddQueryString(this string url, string query)
    {
        if (!url.Contains('?'))
        {
            url += '?';
        }
        else if (!url.EndsWith("&"))
        {
            url += "&";
        }

        return url + query;
    }

    [DebuggerStepThrough]
    public static string AddQueryString(this string url, string name, string value)
    {
        return url.AddQueryString(name + "=" + UrlEncoder.Default.Encode(value));
    }

    [DebuggerStepThrough]
    public static string AddHashFragment(this string url, string query)
    {
        if (!url.Contains('#'))
        {
            url += '#';
        }

        return url + query;
    }

    public static string? GetOrigin(this string url)
    {
        if (url != null)
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }

            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                return $"{uri.Scheme}://{uri.Authority}";
            }
        }

        return null;
    }

    public static string Obfuscate(this string value)
    {
        var last4Chars = "****";
        if (value.IsPresent() && value.Length > 4)
        {
#if NETSTANDARD2_0
            last4Chars = value.Substring(value.Length - 4);
#else
            last4Chars = value[^4..];
#endif
        }

        return $"****{last4Chars}";
    }

    public static bool IsMatch(this string value, string pattern)
    {
        return Regex.IsMatch(value, pattern);
    }

    public static bool IsMatch(this string value, string pattern, RegexOptions regexOptions)
    {
        return Regex.IsMatch(value, pattern, regexOptions);
    }

    public static bool IsEmail(this string value)
    {
        return Regex.IsMatch(value, RegexConstants.Email.Any);
    }

    public static bool IsChineseMainlandMobileNumber(this string value)
    {
        return Regex.IsMatch(value, RegexConstants.MobileNumber.CN);
    }

    public static string ToMd5(this string str)
    {
        return str.ToMd5(Encoding.UTF8);
    }

    public static string ToMd5(this string str, Encoding encoding)
    {
        using var algorithm = MD5.Create();
        return str.ToHash(algorithm, encoding);
    }

    public static bool VerifyMd5(this string str, string hashedValue)
    {
        return str.VerifyMd5(hashedValue, Encoding.Default);
    }

    public static bool VerifyMd5(this string str, string hashedValue, Encoding encoding)
    {
        using var algorithm = MD5.Create();
        return str.VerifyHash(hashedValue, algorithm, encoding);
    }

    public static string ToSha1(this string str)
    {
        using var algorithm = SHA1.Create();
        return str.ToHash(algorithm, Encoding.Default);
    }

    public static string ToSha1(this string str, Encoding encoding)
    {
        using var algorithm = SHA1.Create();
        return str.ToHash(algorithm, encoding);
    }

    public static bool VerifySha1(this string str, string hashedValue)
    {
        return str.VerifySha1(hashedValue, Encoding.Default);
    }

    public static bool VerifySha1(this string str, string hashedValue, Encoding encoding)
    {
        using var algorithm = SHA1.Create();
        return str.VerifyHash(hashedValue, algorithm, encoding);
    }

    public static string ToSha256(this string str)
    {
        using var algorithm = SHA256.Create();
        return str.ToHash(algorithm, Encoding.Default);
    }

    public static string ToSha256(this string str, Encoding encoding)
    {
        using var algorithm = SHA256.Create();
        return str.ToHash(algorithm, encoding);
    }

    public static bool VerifySha256(this string str, string hashedValue)
    {
        return str.VerifySha256(hashedValue, Encoding.Default);
    }

    public static bool VerifySha256(this string str, string hashedValue, Encoding encoding)
    {
        using var algorithm = SHA256.Create();
        return str.VerifyHash(hashedValue, algorithm, encoding);
    }

    public static string ToSha384(this string str)
    {
        using var algorithm = SHA384.Create();
        return str.ToHash(algorithm, Encoding.Default);
    }

    public static string ToSha384(this string str, Encoding encoding)
    {
        using var algorithm = SHA384.Create();
        return str.ToHash(algorithm, encoding);
    }

    public static bool VerifySha384(this string str, string hashedValue)
    {
        return str.VerifySha384(hashedValue, Encoding.Default);
    }

    public static bool VerifySha384(this string str, string hashedValue, Encoding encoding)
    {
        using var algorithm = SHA384.Create();
        return str.VerifyHash(hashedValue, algorithm, encoding);
    }

    public static string ToSha512(this string str)
    {
        using var algorithm = SHA512.Create();
        return str.ToHash(algorithm, Encoding.Default);
    }

    public static string ToSha512(this string str, Encoding encoding)
    {
        using var algorithm = SHA512.Create();
        return str.ToHash(algorithm, encoding);
    }

    public static bool VerifySha512(this string str, string hashedValue)
    {
        return str.VerifySha512(hashedValue, Encoding.Default);
    }

    public static bool VerifySha512(this string str, string hashedValue, Encoding encoding)
    {
        using var algorithm = SHA512.Create();
        return str.VerifyHash(hashedValue, algorithm, encoding);
    }

    public static string ToHmac(this string str)
    {
        using var algorithm = HMAC.Create("HMACSHA1");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmac(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACSHA1");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacSha1(this string str)
    {
        using var algorithm = HMAC.Create("HMACSHA1");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacSha1(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACSHA1");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacMd5(this string str)
    {
        using var algorithm = HMAC.Create("HMACMD5");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacMd5(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACMD5");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacRipemd160(this string str)
    {
        using var algorithm = HMAC.Create("HMACRIPEMD160");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacRipemd160(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACRIPEMD160");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacSha256(this string str)
    {
        using var algorithm = HMAC.Create("HMACSHA256");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacSha256(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACSHA256");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacSha384(this string str)
    {
        using var algorithm = HMAC.Create("HMACSHA384");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacSha384(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACSHA384");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToHmacSha512(this string str)
    {
        using var algorithm = HMAC.Create("HMACSHA512");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToHmacSha512(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("HMACSHA512");
        return str.ToHash(algorithm!, encoding);
    }

    public static string ToMacTripleDES(this string str)
    {
        using var algorithm = HMAC.Create("MACTripleDES");
        return str.ToHash(algorithm!, Encoding.Default);
    }

    public static string ToMacTripleDES(this string str, Encoding encoding)
    {
        using var algorithm = HMAC.Create("MACTripleDES");
        return str.ToHash(algorithm!, encoding);
    }
}
