using Skywalker.Extensions.Tokenizer;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class TokenTests
{
    // Factory methods
    [Fact]
    public void Eoi_CreatesEoiToken()
    {
        var token = Token.Eoi();
        Assert.Equal(TokenKind.Eoi, token.Kind);
        Assert.Null(token.Text);
    }

    [Fact]
    public void Star_CreatesStarToken()
    {
        var token = Token.Star();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("*", token.Text);
    }

    [Fact]
    public void Dot_CreatesDotToken()
    {
        var token = Token.Dot();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal(".", token.Text);
    }

    [Fact]
    public void Colon_CreatesColonToken()
    {
        var token = Token.Colon();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal(":", token.Text);
    }

    [Fact]
    public void Comma_CreatesCommaToken()
    {
        var token = Token.Comma();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal(",", token.Text);
    }

    [Fact]
    public void Semicolon_CreatesSemicolonToken()
    {
        var token = Token.Semicolon();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal(";", token.Text);
    }

    [Fact]
    public void RightParenthesis_CreatesToken()
    {
        var token = Token.RightParenthesis();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal(")", token.Text);
    }

    [Fact]
    public void Equals_CreatesEqualsToken()
    {
        var token = Token.Equals();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("=", token.Text);
    }

    [Fact]
    public void Pipe_CreatesPipeToken()
    {
        var token = Token.Pipe();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("|", token.Text);
    }

    [Fact]
    public void LeftBracket_CreatesToken()
    {
        var token = Token.LeftBracket();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("[", token.Text);
    }

    [Fact]
    public void RightBracket_CreatesToken()
    {
        var token = Token.RightBracket();
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("]", token.Text);
    }

    [Fact]
    public void NotEqual_CreatesNotEqualToken()
    {
        var token = Token.NotEqual();
        Assert.Equal(TokenKind.NotEqual, token.Kind);
    }

    [Fact]
    public void Plus_CreatesPlusToken()
    {
        var token = Token.Plus();
        Assert.Equal(TokenKind.Plus, token.Kind);
    }

    [Fact]
    public void Greater_CreatesGreaterToken()
    {
        var token = Token.Greater();
        Assert.Equal(TokenKind.Greater, token.Kind);
    }

    [Fact]
    public void Includes_CreatesIncludesToken()
    {
        var token = Token.Includes();
        Assert.Equal(TokenKind.Includes, token.Kind);
    }

    [Fact]
    public void RegexMatch_CreatesRegexMatchToken()
    {
        var token = Token.RegexMatch();
        Assert.Equal(TokenKind.RegexMatch, token.Kind);
    }

    [Fact]
    public void DashMatch_CreatesDashMatchToken()
    {
        var token = Token.DashMatch();
        Assert.Equal(TokenKind.DashMatch, token.Kind);
    }

    [Fact]
    public void PrefixMatch_CreatesPrefixMatchToken()
    {
        var token = Token.PrefixMatch();
        Assert.Equal(TokenKind.PrefixMatch, token.Kind);
    }

    [Fact]
    public void SuffixMatch_CreatesSuffixMatchToken()
    {
        var token = Token.SuffixMatch();
        Assert.Equal(TokenKind.SuffixMatch, token.Kind);
    }

    [Fact]
    public void SubstringMatch_CreatesSubstringMatchToken()
    {
        var token = Token.SubstringMatch();
        Assert.Equal(TokenKind.SubstringMatch, token.Kind);
    }

    [Fact]
    public void Tilde_CreatesTildeToken()
    {
        var token = Token.Tilde();
        Assert.Equal(TokenKind.Tilde, token.Kind);
    }

    [Fact]
    public void Slash_CreatesSlashToken()
    {
        var token = Token.Slash();
        Assert.Equal(TokenKind.Slash, token.Kind);
    }

    // Text-based factory methods
    [Fact]
    public void Ident_WithText_CreatesIdentToken()
    {
        var token = Token.Ident("foo");
        Assert.Equal(TokenKind.Ident, token.Kind);
        Assert.Equal("foo", token.Text);
    }

    [Fact]
    public void Ident_NullText_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => Token.Ident(null));
    }

    [Fact]
    public void Ident_EmptyText_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() => Token.Ident(""));
    }

    [Fact]
    public void Integer_WithText_CreatesIntegerToken()
    {
        var token = Token.Integer("123");
        Assert.Equal(TokenKind.Integer, token.Kind);
        Assert.Equal("123", token.Text);
    }

    [Fact]
    public void Hash_WithText_CreatesHashToken()
    {
        var token = Token.Hash("id");
        Assert.Equal(TokenKind.Hash, token.Kind);
        Assert.Equal("id", token.Text);
    }

    [Fact]
    public void WhiteSpace_WithText_CreatesWhiteSpaceToken()
    {
        var token = Token.WhiteSpace(" ");
        Assert.Equal(TokenKind.WhiteSpace, token.Kind);
        Assert.Equal(" ", token.Text);
    }

    [Fact]
    public void String_WithText_CreatesStringToken()
    {
        var token = Token.String("hello");
        Assert.Equal(TokenKind.String, token.Kind);
        Assert.Equal("hello", token.Text);
    }

    [Fact]
    public void String_NullText_CreatesWithEmpty()
    {
        var token = Token.String(null);
        Assert.Equal(TokenKind.String, token.Kind);
        Assert.Equal(string.Empty, token.Text);
    }

    [Fact]
    public void Function_WithText_CreatesFunctionToken()
    {
        var token = Token.Function("nth-child");
        Assert.Equal(TokenKind.Function, token.Kind);
        Assert.Equal("nth-child", token.Text);
    }

    [Fact]
    public void Char_CreatesCharToken()
    {
        var token = Token.Char('x');
        Assert.Equal(TokenKind.Char, token.Kind);
        Assert.Equal("x", token.Text);
    }

    // Equality
    [Fact]
    public void Equals_SameKindAndText_ReturnsTrue()
    {
        var a = Token.Ident("foo");
        var b = Token.Ident("foo");
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentText_ReturnsFalse()
    {
        var a = Token.Ident("foo");
        var b = Token.Ident("bar");
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentKind_ReturnsFalse()
    {
        var a = Token.Ident("foo");
        var b = Token.Hash("foo");
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_ObjectOverload_ReturnsTrue()
    {
        var a = Token.Star();
        object b = Token.Star();
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        var a = Token.Star();
        Assert.False(a.Equals(null));
    }

    [Fact]
    public void OperatorEquals_SameTokens_ReturnsTrue()
    {
        var a = Token.Star();
        var b = Token.Star();
        Assert.True(a == b);
    }

    [Fact]
    public void OperatorNotEquals_DifferentTokens_ReturnsTrue()
    {
        var a = Token.Star();
        var b = Token.Dot();
        Assert.True(a != b);
    }

    // GetHashCode
    [Fact]
    public void GetHashCode_SameKindAndText_SameHash()
    {
        var a = Token.Ident("foo");
        var b = Token.Ident("foo");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_NoText_UsesKindOnly()
    {
        var a = Token.Eoi();
        var b = Token.Eoi();
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // ToString
    [Fact]
    public void ToString_WithText_IncludesKindAndText()
    {
        var token = Token.Ident("foo");
        Assert.Equal("Ident: foo", token.ToString());
    }

    [Fact]
    public void ToString_WithoutText_ShowsKindOnly()
    {
        var token = Token.Eoi();
        Assert.Equal("Eoi", token.ToString());
    }
}
