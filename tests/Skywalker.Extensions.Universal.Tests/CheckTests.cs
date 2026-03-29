using System;
using System.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class CheckTests
{
    [Fact]
    public void NotNull_WithNonNullValue_ShouldReturnValue()
    {
        var result = "hello".NotNull("param");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NotNull_WithNullValue_ShouldThrowArgumentNullException()
    {
        string? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull("param"));
        Assert.Equal("param", ex.ParamName);
    }

    [Fact]
    public void NotNull_WithMessage_ShouldUseCustomMessage()
    {
        string? value = null;
        var ex = Assert.Throws<ArgumentNullException>(() => value.NotNull("param", "custom message"));
        Assert.Contains("custom message", ex.Message);
    }

    [Fact]
    public void NotNullOrEmpty_WithValidString_ShouldReturnString()
    {
        var result = "hello".NotNullOrEmpty("param");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NotNullOrEmpty_WithNullString_ShouldThrow()
    {
        string? value = null;
        Assert.Throws<ArgumentNullException>(() => value.NotNullOrEmpty("param"));
    }

    [Fact]
    public void NotNullOrEmpty_WithEmptyString_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => "".NotNullOrEmpty("param"));
    }

    [Fact]
    public void NotNullOrEmpty_Collection_WithItems_ShouldReturnCollection()
    {
        var list = new List<int> { 1, 2 };
        var result = Check.NotNullOrEmpty(list, "param");
        Assert.Same(list, result);
    }

    [Fact]
    public void NotNullOrEmpty_Collection_WithEmpty_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => Check.NotNullOrEmpty(new List<int>(), "param"));
    }

    [Fact]
    public void NotNullOrEmpty_Collection_WithNull_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => Check.NotNullOrEmpty<int>(null, "param"));
    }

    [Fact]
    public void NotNullOrWhiteSpace_WithValidString_ShouldReturnString()
    {
        var result = "hello".NotNullOrWhiteSpace("param");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NotNullOrWhiteSpace_WithWhitespace_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => "   ".NotNullOrWhiteSpace("param"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Positive_Int_WithPositive_ShouldReturn(int value)
    {
        Assert.Equal(value, value.Positive("param"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Positive_Int_WithNonPositive_ShouldThrow(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => value.Positive("param"));
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(100L)]
    public void Positive_Long_WithPositive_ShouldReturn(long value)
    {
        Assert.Equal(value, value.Positive("param"));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void Positive_Long_WithNonPositive_ShouldThrow(long value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => value.Positive("param"));
    }

    [Fact]
    public void Positive_Decimal_WithPositive_ShouldReturn()
    {
        Assert.Equal(1m, 1m.Positive("param"));
    }

    [Fact]
    public void Positive_Decimal_WithZero_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 0m.Positive("param"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Nonnegative_Int_WithNonnegative_ShouldReturn(int value)
    {
        Assert.Equal(value, value.Nonnegative("param"));
    }

    [Fact]
    public void Nonnegative_Int_WithNegative_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-1).Nonnegative("param"));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    public void Nonnegative_Long_WithNonnegative_ShouldReturn(long value)
    {
        Assert.Equal(value, value.Nonnegative("param"));
    }

    [Fact]
    public void Nonnegative_Long_WithNegative_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-1L).Nonnegative("param"));
    }

    [Fact]
    public void Nonnegative_Decimal_WithNonnegative_ShouldReturn()
    {
        Assert.Equal(0m, 0m.Nonnegative("param"));
    }

    [Fact]
    public void Nonnegative_Decimal_WithNegative_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-1m).Nonnegative("param"));
    }

    [Fact]
    public void NotEmptyGuid_WithValidGuid_ShouldReturn()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(guid, guid.NotEmptyGuid("param"));
    }

    [Fact]
    public void NotEmptyGuid_WithEmptyGuid_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => Guid.Empty.NotEmptyGuid("param"));
    }

    [Fact]
    public void Equal_Int_WithEqualValues_ShouldReturn()
    {
        Assert.Equal(5, 5.Equal(5, "param"));
    }

    [Fact]
    public void Equal_Int_WithDifferentValues_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => 5.Equal(10, "param"));
    }

    [Fact]
    public void Equal_Long_WithEqualValues_ShouldReturn()
    {
        Assert.Equal(5L, 5L.Equal(5L, "param"));
    }

    [Fact]
    public void Equal_Long_WithDifferentValues_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => 5L.Equal(10L, "param"));
    }

    [Fact]
    public void Equal_Bool_WithEqualValues_ShouldReturn()
    {
        Assert.True(true.Equal(true, "param"));
    }

    [Fact]
    public void Equal_Bool_WithDifferentValues_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => true.Equal(false, "param"));
    }

    [Fact]
    public void LengthOf_WithValidLength_ShouldReturn()
    {
        var result = "hello".LengthOf("param", 10, 1);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void LengthOf_ExceedsMax_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "hello".LengthOf("param", 3));
    }

    [Fact]
    public void LengthOf_BelowMin_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => "hi".LengthOf("param", 10, 5));
    }

    [Fact]
    public void LengthOf_NullWithMinLength_ShouldThrow()
    {
        string? value = null;
        Assert.Throws<ArgumentOutOfRangeException>(() => value!.LengthOf("param", 10, 1));
    }

    [Fact]
    public void Condition_WithTrueCondition_ShouldReturn()
    {
        var result = Check.Condition(5, x => x > 0, "param");
        Assert.Equal(5, result);
    }

    [Fact]
    public void Condition_WithFalseCondition_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Check.Condition(-1, x => x > 0, "param"));
    }

    [Fact]
    public void HasNoNulls_WithNoNulls_ShouldReturn()
    {
        var list = new List<string> { "a", "b" };
        var result = Check.HasNoNulls(list, "param");
        Assert.Same(list, result);
    }

    [Fact]
    public void HasNoNulls_WithNull_ShouldThrow()
    {
        var list = new List<string> { "a", null! };
        Assert.Throws<ArgumentException>(() => Check.HasNoNulls(list, "param"));
    }

    [Fact]
    public void AssignableTo_WithAssignableType_ShouldReturn()
    {
        var result = Check.AssignableTo<IDisposable>(typeof(System.IO.MemoryStream), "param");
        Assert.Equal(typeof(System.IO.MemoryStream), result);
    }

    [Fact]
    public void AssignableTo_WithNonAssignableType_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => Check.AssignableTo<IDisposable>(typeof(string), "param"));
    }
}
