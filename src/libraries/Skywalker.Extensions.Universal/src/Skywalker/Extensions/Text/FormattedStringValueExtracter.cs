using System.Diagnostics;
using Skywalker.Extensions.Collections;
using Skywalker.Extensions.Text.Formatting;

namespace Skywalker.Extensions.Text;

/// <summary>
/// This class is used to extract dynamic values from a formatted string.
/// It works as reverse of <see cref="string.Format(string,object)"/>
/// </summary>
/// <example>
/// Say that str is "My name is Neo." and format is "My name is {name}.".
/// Then Extract method gets "Neo" as "name".  
/// </example>
public class FormattedStringValueExtracter
{
    /// <summary>
    /// Extracts dynamic values from a formatted string.
    /// </summary>
    /// <param name="str">String including dynamic values</param>
    /// <param name="format">Format of the string</param>
    /// <param name="ignoreCase">True, to search case-insensitive.</param>
    /// <param name="splitformatCharacter">format is splitted using this character when provided.</param>
    public static ExtractionResult Extract(string str, string format, bool ignoreCase = false, char? splitformatCharacter = null)
    {
        var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        if (str == format)
        {
            return new ExtractionResult(true);
        }

        var formatTokens = TokenizeFormat(format, splitformatCharacter);

        if (formatTokens.IsNullOrEmpty())
        {
            return new ExtractionResult(str == "");
        }

        var result = new ExtractionResult(false);

        for (var i = 0; i < formatTokens.Count; i++)
        {
            var currentToken = formatTokens[i];
            var previousToken = i > 0 ? formatTokens[i - 1] : null;

            if (currentToken.Type == FormatStringTokenType.ConstantText)
            {
                if (i == 0)
                {
                    if (str.StartsWith(currentToken.Text, stringComparison))
                    {
#if NETSTANDARD2_0
                        str = str.Substring(currentToken.Text.Length);
#else
                        str = str[currentToken.Text.Length..];
#endif
                    }
                }
                else
                {
                    var matchIndex = str.IndexOf(currentToken.Text, stringComparison);
                    if (matchIndex >= 0)
                    {
                        Debug.Assert(previousToken != null, "previousToken can not be null since i > 0 here");
                        result.IsMatch = true;
#if NETSTANDARD2_0
                        result.Matches.Add(new NameValue(previousToken!.Text, str.Substring(0, matchIndex)));
                        str = str.Substring(matchIndex + currentToken.Text.Length);
#else
                        result.Matches.Add(new NameValue(previousToken!.Text, str[..matchIndex]));
                        str = str[(matchIndex + currentToken.Text.Length)..];
#endif
                    }
                }
            }
        }

        var lastToken = formatTokens.Last();
        if (lastToken.Type == FormatStringTokenType.DynamicValue)
        {
            result.Matches.Add(new NameValue(lastToken.Text, str));
            result.IsMatch = true;
        }

        return result;
    }

    private static List<FormatStringToken> TokenizeFormat(string originalFormat, char? splitformatCharacter = null)
    {
        if (splitformatCharacter == null)
        {
            return new FormatStringTokenizer().Tokenize(originalFormat);
        }

        var result = new List<FormatStringToken>();
        var formats = originalFormat.Split(splitformatCharacter.Value);

        foreach (var format in formats)
        {
            result.AddRange(new FormatStringTokenizer().Tokenize(format));
        }

        return result;
    }

    /// <summary>
    /// Checks if given <see cref="str"/> fits to given <see cref="format"/>.
    /// Also gets extracted values.
    /// </summary>
    /// <param name="str">String including dynamic values</param>
    /// <param name="format">Format of the string</param>
    /// <param name="values">Array of extracted values if matched</param>
    /// <param name="ignoreCase">True, to search case-insensitive</param>
    /// <returns>True, if matched.</returns>
    public static bool IsMatch(string str, string format, out string?[] values, bool ignoreCase = false)
    {
        var result = Extract(str, format, ignoreCase);
        if (!result.IsMatch)
        {
            values = Array.Empty<string>();
            return false;
        }

        values = result.Matches.Select(m => m.Value).ToArray();
        return true;
    }

    /// <summary>
    /// Used as return value of <see cref="Extract"/> method.
    /// </summary>
    public class ExtractionResult
    {
        /// <summary>
        /// Is fully matched.
        /// </summary>
        public bool IsMatch { get; set; }

        /// <summary>
        /// List of matched dynamic values.
        /// </summary>
        public List<NameValue> Matches { get; private set; }

        internal ExtractionResult(bool isMatch)
        {
            IsMatch = isMatch;
            Matches = new List<NameValue>();
        }
    }
}
