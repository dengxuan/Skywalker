using Skywalker.Extensions.Tokenizer;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class EitherTests
{
    // A side
    [Fact]
    public void A_CreatesAInstance()
    {
        var either = Either<int, string>.A(42);
        var result = either.Fold(a => $"int:{a}", b => $"str:{b}");
        Assert.Equal("int:42", result);
    }

    [Fact]
    public void A_Null_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => Either<string, string>.A(null!));
    }

    // B side
    [Fact]
    public void B_CreatesBInstance()
    {
        var either = Either<int, string>.B("hello");
        var result = either.Fold(a => $"int:{a}", b => $"str:{b}");
        Assert.Equal("str:hello", result);
    }

    [Fact]
    public void B_Null_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => Either<string, string>.B(null!));
    }

    // Fold null checks
    [Fact]
    public void Fold_NullAFunc_ThrowsForA()
    {
        var either = Either<int, string>.A(42);
        Assert.Throws<ArgumentNullException>(() => either.Fold<string>(null!, b => "b"));
    }

    [Fact]
    public void Fold_NullBFunc_ThrowsForA()
    {
        var either = Either<int, string>.A(42);
        Assert.Throws<ArgumentNullException>(() => either.Fold<string>(a => "a", null!));
    }

    [Fact]
    public void Fold_NullAFunc_ThrowsForB()
    {
        var either = Either<int, string>.B("hello");
        Assert.Throws<ArgumentNullException>(() => either.Fold<string>(null!, b => "b"));
    }

    [Fact]
    public void Fold_NullBFunc_ThrowsForB()
    {
        var either = Either<int, string>.B("hello");
        Assert.Throws<ArgumentNullException>(() => either.Fold<string>(a => "a", null!));
    }

    // Equals
    [Fact]
    public void Equals_SameA_ReturnsTrue()
    {
        var a1 = Either<int, string>.A(42);
        var a2 = Either<int, string>.A(42);
        Assert.True(a1.Equals(a2));
    }

    [Fact]
    public void Equals_DifferentA_ReturnsFalse()
    {
        var a1 = Either<int, string>.A(42);
        var a2 = Either<int, string>.A(99);
        Assert.False(a1.Equals(a2));
    }

    [Fact]
    public void Equals_SameB_ReturnsTrue()
    {
        var b1 = Either<int, string>.B("hello");
        var b2 = Either<int, string>.B("hello");
        Assert.True(b1.Equals(b2));
    }

    [Fact]
    public void Equals_DifferentB_ReturnsFalse()
    {
        var b1 = Either<int, string>.B("hello");
        var b2 = Either<int, string>.B("world");
        Assert.False(b1.Equals(b2));
    }

    [Fact]
    public void Equals_AAndB_ReturnsFalse()
    {
        var a = Either<string, string>.A("hello");
        var b = Either<string, string>.B("hello");
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_NullEither_ReturnsFalse()
    {
        var a = Either<int, string>.A(42);
        Assert.False(a.Equals((Either<int, string>?)null));
    }

    [Fact]
    public void Equals_ObjectOverload_Works()
    {
        var a1 = Either<int, string>.A(42);
        object a2 = Either<int, string>.A(42);
        Assert.True(a1.Equals(a2));
    }

    [Fact]
    public void Equals_ObjectOverload_NullReturnsFalse()
    {
        var b = Either<int, string>.B("hello");
        Assert.False(b.Equals((object?)null));
    }

    // GetHashCode
    [Fact]
    public void GetHashCode_SameA_SameHash()
    {
        var a1 = Either<int, string>.A(42);
        var a2 = Either<int, string>.A(42);
        Assert.Equal(a1.GetHashCode(), a2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_SameB_SameHash()
    {
        var b1 = Either<int, string>.B("hello");
        var b2 = Either<int, string>.B("hello");
        Assert.Equal(b1.GetHashCode(), b2.GetHashCode());
    }

    // ToString
    [Fact]
    public void ToString_A_ReturnsValueString()
    {
        var a = Either<int, string>.A(42);
        Assert.Equal("42", a.ToString());
    }

    [Fact]
    public void ToString_B_ReturnsValueString()
    {
        var b = Either<int, string>.B("hello");
        Assert.Equal("hello", b.ToString());
    }
}
