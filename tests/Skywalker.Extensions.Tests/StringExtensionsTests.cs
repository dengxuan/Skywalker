using System.Globalization;
using System.Text;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class StringExtensionsTests
{
    // EnsureEndsWith
    [Fact]
    public void EnsureEndsWith_AddsChar_WhenNotPresent()
    {
        Assert.Equal("hello/", "hello".EnsureEndsWith('/'));
    }

    [Fact]
    public void EnsureEndsWith_DoesNotAdd_WhenAlreadyPresent()
    {
        Assert.Equal("hello/", "hello/".EnsureEndsWith('/'));
    }

    [Fact]
    public void EnsureEndsWith_WithCulture_AddsChar()
    {
        Assert.Equal("hello/", "hello".EnsureEndsWith('/', false, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EnsureEndsWith_NullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).EnsureEndsWith('/'));
    }

    // EnsureStartsWith
    [Fact]
    public void EnsureStartsWith_AddsChar_WhenNotPresent()
    {
        Assert.Equal("/hello", "hello".EnsureStartsWith('/'));
    }

    [Fact]
    public void EnsureStartsWith_DoesNotAdd_WhenAlreadyPresent()
    {
        Assert.Equal("/hello", "/hello".EnsureStartsWith('/'));
    }

    [Fact]
    public void EnsureStartsWith_WithCulture_AddsChar()
    {
        Assert.Equal("/hello", "hello".EnsureStartsWith('/', false, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EnsureStartsWith_NullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => ((string)null!).EnsureStartsWith('/'));
    }

    // IsNullOrEmpty / IsNullOrWhiteSpace
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("a", false)]
    public void IsNullOrEmpty_Tests(string? input, bool expected)
    {
        Assert.Equal(expected, input.IsNullOrEmpty());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("  ", true)]
    [InlineData("a", false)]
    public void IsNullOrWhiteSpace_Tests(string? input, bool expected)
    {
        Assert.Equal(expected, input.IsNullOrWhiteSpace());
    }

    [Fact]
    public void IsEmptyOrWhiteSpace_Tests()
    {
        Assert.True("".IsEmptyOrWhiteSpace());
        Assert.True("  ".IsEmptyOrWhiteSpace());
        Assert.False("a".IsEmptyOrWhiteSpace());
    }

    // Left / Right
    [Fact]
    public void Left_ReturnsSubstring()
    {
        Assert.Equal("hel", "hello".Left(3));
    }

    [Fact]
    public void Left_ThrowsWhenLenTooLarge()
    {
        Assert.Throws<ArgumentException>(() => "hi".Left(5));
    }

    [Fact]
    public void Right_ReturnsSubstring()
    {
        Assert.Equal("llo", "hello".Right(3));
    }

    [Fact]
    public void Right_ThrowsWhenLenTooLarge()
    {
        Assert.Throws<ArgumentException>(() => "hi".Right(5));
    }

    // NormalizeLineEndings
    [Fact]
    public void NormalizeLineEndings_ConvertsAll()
    {
        var result = "a\r\nb\rc\nd".NormalizeLineEndings();
        var expected = $"a{Environment.NewLine}b{Environment.NewLine}c{Environment.NewLine}d";
        Assert.Equal(expected, result);
    }

    // NthIndexOf
    [Fact]
    public void NthIndexOf_FindsCorrectIndex()
    {
        Assert.Equal(5, "a.b.c.d".NthIndexOf('.', 3));
    }

    [Fact]
    public void NthIndexOf_ReturnsMinusOne_WhenNotFound()
    {
        Assert.Equal(-1, "abc".NthIndexOf('.', 1));
    }

    // RemovePostFix / RemovePreFix
    [Fact]
    public void RemovePostFix_RemovesMatching()
    {
        Assert.Equal("hello", "helloWorld".RemovePostFix("World"));
    }

    [Fact]
    public void RemovePostFix_ReturnsNull_ForNull()
    {
        Assert.Null(((string?)null).RemovePostFix("x"));
    }

    [Fact]
    public void RemovePostFix_ReturnsEmpty_ForEmpty()
    {
        Assert.Equal("", "".RemovePostFix("x"));
    }

    [Fact]
    public void RemovePreFix_RemovesMatching()
    {
        Assert.Equal("World", "helloWorld".RemovePreFix("hello"));
    }

    [Fact]
    public void RemovePreFix_ReturnsNull_ForNull()
    {
        Assert.Null(((string?)null).RemovePreFix("x"));
    }

    // Split
    [Fact]
    public void Split_BySeparator()
    {
        var result = "a::b::c".Split("::");
        Assert.Equal(new[] { "a", "b", "c" }, result);
    }

    [Fact]
    public void SplitToLines_Splits()
    {
        var result = $"a{Environment.NewLine}b{Environment.NewLine}c".SplitToLines();
        Assert.Equal(3, result.Length);
    }

    // ToCamelCase / ToPascalCase
    [Theory]
    [InlineData("HelloWorld", "helloWorld")]
    [InlineData("A", "a")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToCamelCase_Tests(string? input, string? expected)
    {
        Assert.Equal(expected, input?.ToCamelCase() ?? input);
    }

    [Fact]
    public void ToCamelCase_WithCulture()
    {
        Assert.Equal("helloWorld", "HelloWorld".ToCamelCase(CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("helloWorld", "HelloWorld")]
    [InlineData("a", "A")]
    [InlineData("", "")]
    public void ToPascalCase_Tests(string input, string expected)
    {
        Assert.Equal(expected, input.ToPascalCase());
    }

    [Fact]
    public void ToPascalCase_WithCulture()
    {
        Assert.Equal("HelloWorld", "helloWorld".ToPascalCase(CultureInfo.InvariantCulture));
    }

    // ToSentenceCase
    [Fact]
    public void ToSentenceCase_SplitsWords()
    {
        var result = "ThisIsSample".ToSentenceCase();
        Assert.Contains(" ", result);
    }

    // ToEnum
    [Fact]
    public void ToEnum_Converts()
    {
        Assert.Equal(DayOfWeek.Monday, "Monday".ToEnum<DayOfWeek>());
    }

    [Fact]
    public void ToEnum_IgnoreCase()
    {
        Assert.Equal(DayOfWeek.Monday, "monday".ToEnum<DayOfWeek>(true));
    }

    // Truncate
    [Fact]
    public void Truncate_TruncatesLongString()
    {
        Assert.Equal("hel", "hello".Truncate(3));
    }

    [Fact]
    public void Truncate_ReturnsOriginal_WhenShort()
    {
        Assert.Equal("hi", "hi".Truncate(5));
    }

    [Fact]
    public void Truncate_Null_ReturnsNull()
    {
        Assert.Null(((string)null!).Truncate(5));
    }

    // TruncateWithPostfix
    [Fact]
    public void TruncateWithPostfix_AddsDots()
    {
        Assert.Equal("he...", "hello world".TruncateWithPostfix(5));
    }

    [Fact]
    public void TruncateWithPostfix_ShortString_ReturnsOriginal()
    {
        Assert.Equal("hi", "hi".TruncateWithPostfix(5));
    }

    [Fact]
    public void TruncateWithPostfix_EmptyString_ReturnsEmpty()
    {
        Assert.Equal("", "".TruncateWithPostfix(5));
    }

    // IsMissing / IsPresent
    [Fact]
    public void IsMissing_TrueForNull() => Assert.True(((string?)null).IsMissing());

    [Fact]
    public void IsPresent_TrueForValue() => Assert.True("hello".IsPresent());

    // ToStringWithoutBom
    [Fact]
    public void ToStringWithoutBom_WithBom()
    {
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF, (byte)'h', (byte)'i' };
        Assert.Equal("hi", bytes.ToStringWithoutBom());
    }

    [Fact]
    public void ToStringWithoutBom_WithoutBom()
    {
        var bytes = Encoding.UTF8.GetBytes("hi");
        Assert.Equal("hi", bytes.ToStringWithoutBom());
    }

    [Fact]
    public void ToStringWithoutBom_NullReturnsNull()
    {
        Assert.Null(((byte[]?)null).ToStringWithoutBom());
    }

    // ToSpaceSeparatedString / FromSpaceSeparatedString
    [Fact]
    public void ToSpaceSeparatedString_JoinsWithSpaces()
    {
        Assert.Equal("a b c", new[] { "a", "b", "c" }.ToSpaceSeparatedString());
    }

    [Fact]
    public void FromSpaceSeparatedString_Splits()
    {
        var result = "a b c".FromSpaceSeparatedString().ToList();
        Assert.Equal(new[] { "a", "b", "c" }, result);
    }

    // ParseScopesString
    [Fact]
    public void ParseScopesString_Parses()
    {
        var result = "read write read".ParseScopesString();
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count); // distinct
    }

    [Fact]
    public void ParseScopesString_NullReturnsNull()
    {
        Assert.Null(((string?)null).ParseScopesString());
    }

    // IsMissingOrTooLong
    [Fact]
    public void IsMissingOrTooLong_Tests()
    {
        Assert.True(((string?)null).IsMissingOrTooLong(5));
        Assert.True("toolong".IsMissingOrTooLong(3));
        Assert.False("ok".IsMissingOrTooLong(5));
    }

    // Slash helpers
    [Fact]
    public void EnsureLeadingSlash_Adds()
    {
        Assert.Equal("/api", "api".EnsureLeadingSlash());
    }

    [Fact]
    public void EnsureTrailingSlash_Adds()
    {
        Assert.Equal("api/", "api".EnsureTrailingSlash());
    }

    [Fact]
    public void RemoveLeadingSlash_Removes()
    {
        Assert.Equal("api", "/api".RemoveLeadingSlash());
    }

    [Fact]
    public void RemoveTrailingSlash_Removes()
    {
        Assert.Equal("api", "api/".RemoveTrailingSlash());
    }

    [Fact]
    public void CleanUrlPath_CleansTrailingSlash()
    {
        Assert.Equal("/api", "/api/".CleanUrlPath());
    }

    [Fact]
    public void CleanUrlPath_EmptyBecomesSlash()
    {
        Assert.Equal("/", "".CleanUrlPath());
    }

    // IsLocalUrl
    [Theory]
    [InlineData("/", true)]
    [InlineData("/foo", true)]
    [InlineData("//foo", false)]
    [InlineData("~/", true)]
    [InlineData("~/foo", true)]
    [InlineData("~//foo", false)]
    [InlineData("", false)]
    [InlineData("http://evil.com", false)]
    public void IsLocalUrl_Tests(string url, bool expected)
    {
        Assert.Equal(expected, url.IsLocalUrl());
    }

    // AddQueryString / AddHashFragment
    [Fact]
    public void AddQueryString_AddsQuestionMark()
    {
        Assert.Equal("http://a.com?key=val", "http://a.com".AddQueryString("key=val"));
    }

    [Fact]
    public void AddQueryString_AppendsAmpersand()
    {
        Assert.Equal("http://a.com?a=1&b=2", "http://a.com?a=1".AddQueryString("b=2"));
    }

    [Fact]
    public void AddHashFragment_AddsHash()
    {
        Assert.Equal("http://a.com#section", "http://a.com".AddHashFragment("section"));
    }

    // GetOrigin
    [Fact]
    public void GetOrigin_ReturnsOrigin()
    {
        Assert.Equal("https://example.com", "https://example.com/path".GetOrigin());
    }

    [Fact]
    public void GetOrigin_InvalidReturnsNull()
    {
        Assert.Null("not-a-url".GetOrigin());
    }

    // Obfuscate
    [Fact]
    public void Obfuscate_LongString()
    {
        Assert.Equal("****cret", "mysecret".Obfuscate());
    }

    [Fact]
    public void Obfuscate_ShortString()
    {
        Assert.Equal("********", "ab".Obfuscate());
    }

    // IsMatch / IsEmail
    [Fact]
    public void IsMatch_MatchesPattern()
    {
        Assert.True("abc123".IsMatch(@"\d+"));
    }

    [Fact]
    public void IsMatch_NullReturnsFalse()
    {
        Assert.False(((string?)null).IsMatch(@"\d+"));
    }

    // Hash methods (just verify roundtrip)
    [Fact]
    public void ToMd5_ProducesConsistentHash()
    {
        var hash = "hello".ToMd5();
        Assert.True("hello".VerifyMd5(hash, Encoding.UTF8));
    }

    [Fact]
    public void ToSha256_ProducesHash()
    {
        var hash = "hello".ToSha256();
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void ToHmacSha256_WithKey_ProducesHash()
    {
        var hash = "hello".ToHmacSha256("mykey");
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }
}
