using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace System;

/// <summary>
/// Extension methods for String class.
/// </summary>
public static partial class StringExtensions
{
#if NET7_0
    [GeneratedRegex("[a-z][A-Z]")]
    private static partial Regex LettersRegex();
#endif

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
        return str.EnsureEndsWith(c, StringComparison.Ordinal);
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
        return str.EnsureStartsWith(c, StringComparison.Ordinal);
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
        var trimed = str.NotNull(nameof(str)).Trim();
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

            if (++count == n)
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
#if NETSTANDARD2_0
#nullable disable
    public static string RemovePostFix(this string str, params string[] postFixes)
#nullable enable
#else
    [return: NotNullIfNotNull("str")]
    public static string? RemovePostFix([NotNullIfNotNull("str")] this string? str, params string[] postFixes)
#endif
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
#if NETSTANDARD2_0
#nullable disable
    public static string RemovePreFix(this string str, params string[] preFixes)
#nullable enable
#else
    [return: NotNullIfNotNull("str")]
    public static string? RemovePreFix(this string? str, params string[] preFixes)
#endif
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

#if NET7_0
        return LettersRegex().Replace(str, evaluator =>
        {
            return $"{evaluator.Value[0]} {(invariantCulture ? char.ToLowerInvariant(evaluator.Value[1]) : char.ToLower(evaluator.Value[1]))}";
        });
#else
        return new Regex("[a-z][A-Z]").Replace(str, evaluator =>
        {
            return $"{evaluator.Value[0]} {(invariantCulture ? char.ToLowerInvariant(evaluator.Value[1]) : char.ToLower(evaluator.Value[1]))}";
        });
#endif
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
#if NET7_0
        return LettersRegex().Replace(str, evaluator =>
        {
            return $"{evaluator.Value[0]} {char.ToLower(evaluator.Value[1], culture)}";
        });
#else
        return new Regex("[a-z][A-Z]").Replace(str, evaluator =>
        {
            return $"{evaluator.Value[0]} {char.ToLower(evaluator.Value[1], culture)}";
        });
#endif
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
        return str.TruncateWithPostfix(maxLength, "...");
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
#if NETSTANDARD2_0
    public static bool IsMissing(this string value)
#else
    public static bool IsMissing([NotNullWhen(false)] this string? value)
#endif
    {
        return string.IsNullOrWhiteSpace(value);
    }

    [DebuggerStepThrough]

#if NETSTANDARD2_0
    public static bool IsPresent(this string value)
#else
    public static bool IsPresent([NotNullWhen(true)] this string? value)
#endif
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 使用指定编码<paramref name="encoding"/>将 byte[] 转换为字符串, 使用不带 BOM（字节顺序标记）的 <paramref name="encoding"/> 编码。
    /// </summary>
    /// <param name="bytes">要转换为字符串的 byte[]</param>
    /// <param name="encoding">获取字符串的编码.</param>
    /// <returns>使用指定编码<paramref name="encoding"/>转换后的字符串</returns>
    [DebuggerStepThrough]
    public static string? ToStringWithoutBom(this byte[]? bytes, Encoding encoding)
    {
        if (bytes == null)
        {
            return null;
        }

        var hasBom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

        if (hasBom)
        {
            return encoding.GetString(bytes, 3, bytes.Length - 3);
        }
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// 将 byte[] 转换为字符串，使用不带 BOM（字节顺序标记）的 UTF 8 编码。
    /// </summary>
    /// <param name="bytes">要转换为字符串的 byte[]</param>
    /// <returns>使用指定<see cref="Encoding.UTF8"/>编码转换后的字符串</returns>
    [DebuggerStepThrough]
    public static string? ToStringWithoutBom(this byte[]? bytes)
    {
        return bytes.ToStringWithoutBom(Encoding.UTF8);
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
#if NETSTANDARD2_0
    public static List<string>? ParseScopesString(this string scopes)
#else
    public static List<string>? ParseScopesString(this string? scopes)
#endif
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
    public static bool IsMissingOrTooLong(this string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }
        if (value!.Length > maxLength)
        {
            return true;
        }

        return false;
    }

    [DebuggerStepThrough]
#if NETSTANDARD2_0
    public static string? EnsureLeadingSlash(this string url)
#else
    [return: NotNullIfNotNull("url")]
    public static string? EnsureLeadingSlash(this string? url)
#endif
    {
        if (url != null && !url.StartsWith("/"))
        {
            return "/" + url;
        }

        return url;
    }

    [DebuggerStepThrough]
#if NETSTANDARD2_0
    public static string? EnsureTrailingSlash(this string url)
#else
    [return: NotNullIfNotNull("url")]
    public static string? EnsureTrailingSlash(this string? url)
#endif
    {
        if (url != null && !url.EndsWith("/"))
        {
            return url + "/";
        }

        return url;
    }

    [DebuggerStepThrough]
#if NETSTANDARD2_0
    public static string? RemoveLeadingSlash(this string url)
#else
    [return: NotNullIfNotNull("url")]
    public static string? RemoveLeadingSlash(this string? url)
#endif
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
#if NETSTANDARD2_0
    public static string? RemoveTrailingSlash(this string url)
#else
    [return: NotNullIfNotNull("url")]
    public static string? RemoveTrailingSlash(this string? url)
#endif
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
#if NETSTANDARD2_0
    public static string CleanUrlPath(this string url)
#else
    [return: NotNullIfNotNull("url")]
    public static string? CleanUrlPath(this string? url)
#endif
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            url = "/";
        }

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

    public static bool IsMatch(this string? value, string pattern)
    {
        if(value == null)
        {
            return false;
        }
        return Regex.IsMatch(value, pattern);
    }

    public static bool IsMatch(this string? value, string pattern, RegexOptions regexOptions)
    {
        if (value == null)
        {
            return false;
        }
        return Regex.IsMatch(value, pattern, regexOptions);
    }

    public static bool IsEmail(this string? value) => value.IsMatch(RegexConstants.Email.Any);

    public static bool IsIp4Address(this string? value) => value.IsMatch(RegexConstants.Ipv4Address.Any);

    public static bool IsMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.Any);

    public static bool IsAustraliaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.AU);

    public static bool IsBelgiumMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.BE);

    public static bool IsBrazilMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.BR);

    public static bool IsChinaMainlandMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.CN);

    public static bool IsCzechMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.CZ);

    public static bool IsGermanyMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.DE);

    public static bool IsDenmarkMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.DK);

    public static bool IsAlgeriaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.DZ);

    public static bool IsSpainMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.ES);

    public static bool IsFinlandMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.FI);

    public static bool IsFranceMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.FR);

    public static bool IsUnitedKingdomMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.GB);

    public static bool IsGreeceMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.GR);

    public static bool IsHongKongOfChinaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.HK);

    public static bool IsHungaryMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.HU);

    public static bool IsIndiaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.IN);

    public static bool IsItalyMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.IT);

    public static bool IsJapanMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.JP);

    public static bool IsMalaysiaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.MY);

    public static bool IsNorwayMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.NO);

    public static bool IsNewZealandMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.NZ);

    public static bool IsPolandMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.PL);

    public static bool IsPortugalMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.PT);

    public static bool IsSerbiaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.RS);

    public static bool IsRussianFederationMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.RU);

    public static bool IsSaudiArabiaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.SA);

    public static bool IsSyrianArabMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.SY);

    public static bool IsTurkeyMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.TR);

    public static bool IsTaiwanOfChinaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.TW);

    public static bool IsUnitedStatesMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.US);

    public static bool IsVietNamMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.VN);

    public static bool IsSouthAfricaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.ZA);

    public static bool IsZambiaMobileNumber(this string? value) => value.IsMatch(RegexConstants.MobileNumber.ZM);

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
