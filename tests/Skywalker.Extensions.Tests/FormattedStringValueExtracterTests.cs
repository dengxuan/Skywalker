using Skywalker.Extensions.Text;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class FormattedStringValueExtracterTests
{
    [Fact]
    public void Extract_BasicFormat_ExtractsDynamicValue()
    {
        var result = FormattedStringValueExtracter.Extract("My name is Neo.", "My name is {name}.");
        Assert.True(result.IsMatch);
        Assert.Equal("name", result.Matches[0].Name);
        Assert.Equal("Neo", result.Matches[0].Value);
    }

    [Fact]
    public void Extract_SameStrings_MatchesWithNoValues()
    {
        var result = FormattedStringValueExtracter.Extract("Hello", "Hello");
        Assert.True(result.IsMatch);
        Assert.Empty(result.Matches);
    }

    [Fact]
    public void Extract_MultipleDynamicValues()
    {
        var result = FormattedStringValueExtracter.Extract(
            "Hello Neo from Matrix",
            "Hello {name} from {place}");
        Assert.True(result.IsMatch);
        Assert.Equal(2, result.Matches.Count);
        Assert.Equal("Neo", result.Matches[0].Value);
        Assert.Equal("Matrix", result.Matches[1].Value);
    }

    [Fact]
    public void Extract_DynamicValueAtEnd()
    {
        var result = FormattedStringValueExtracter.Extract(
            "Hello World",
            "Hello {name}");
        Assert.True(result.IsMatch);
        Assert.Equal("World", result.Matches[0].Value);
    }

    [Fact]
    public void Extract_IgnoreCase()
    {
        var result = FormattedStringValueExtracter.Extract(
            "HELLO World",
            "hello {name}",
            ignoreCase: true);
        Assert.True(result.IsMatch);
        Assert.Equal("World", result.Matches[0].Value);
    }

    [Fact]
    public void Extract_EmptyFormat_EmptyString_Matches()
    {
        var result = FormattedStringValueExtracter.Extract("", "");
        Assert.True(result.IsMatch);
    }

    [Fact]
    public void Extract_EmptyFormat_NonEmptyString_NoMatch()
    {
        // Empty format tokenized produces empty tokens list, so result depends on str == ""
        var result = FormattedStringValueExtracter.Extract("abc", "");
        Assert.False(result.IsMatch);
    }

    [Fact]
    public void IsMatch_Matching_ReturnsTrueWithValues()
    {
        var matched = FormattedStringValueExtracter.IsMatch(
            "User: admin, Role: superadmin",
            "User: {user}, Role: {role}",
            out var values);
        Assert.True(matched);
        Assert.Equal(2, values.Length);
        Assert.Equal("admin", values[0]);
        Assert.Equal("superadmin", values[1]);
    }

    [Fact]
    public void IsMatch_NonMatching_ReturnsFalse()
    {
        // Use a format with only constant text (no dynamic tokens at end)
        var matched = FormattedStringValueExtracter.IsMatch(
            "Something completely different",
            "User: admin, Role: superadmin",
            out var values);
        Assert.False(matched);
        Assert.Empty(values);
    }

    [Fact]
    public void Extract_WithSplitFormatCharacter()
    {
        var result = FormattedStringValueExtracter.Extract(
            "Hello World",
            "Hello {name}",
            splitformatCharacter: ',');
        Assert.True(result.IsMatch);
    }
}
