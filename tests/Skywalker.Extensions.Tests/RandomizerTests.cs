using System.Security.Cryptography;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class RandomizerTests
{
    [Fact]
    public void Generate_ValidLength_ReturnsByteArray()
    {
        var bytes = Randomizer.Generate(16);
        Assert.Equal(16, bytes.Length);
    }

    [Fact]
    public void Generate_Length1_ReturnsSingleByte()
    {
        var bytes = Randomizer.Generate(1);
        Assert.Single(bytes);
    }

    [Fact]
    public void Generate_ZeroLength_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Randomizer.Generate(0));
    }

    [Fact]
    public void Generate_NegativeLength_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Randomizer.Generate(-1));
    }

    [Fact]
    public void GenerateInt32_WithMax_ReturnsInRange()
    {
        for (int i = 0; i < 100; i++)
        {
            var result = Randomizer.GenerateInt32(100);
            Assert.InRange(result, 0, 99);
        }
    }

    [Fact]
    public void GenerateInt32_WithRange_ReturnsInRange()
    {
        for (int i = 0; i < 100; i++)
        {
            var result = Randomizer.GenerateInt32(10, 20);
            Assert.InRange(result, 10, 19);
        }
    }

    [Fact]
    public void GenerateInt64_ReturnsValue()
    {
        var result = Randomizer.GenerateInt64();
        // Just verify it returns a value without throwing
        Assert.IsType<long>(result);
    }

    [Fact]
    public void GenerateString_DefaultLength_Returns8Chars()
    {
        var result = Randomizer.GenerateString();
        Assert.Equal(8, result.Length);
    }

    [Fact]
    public void GenerateString_CustomLength_ReturnsCorrectLength()
    {
        var result = Randomizer.GenerateString(16);
        Assert.Equal(16, result.Length);
    }

    [Fact]
    public void GenerateString_DigitOnly_ReturnsOnlyDigits()
    {
        var result = Randomizer.GenerateString(20, digitOnly: true);
        Assert.Equal(20, result.Length);
        Assert.All(result.ToCharArray(), c => Assert.True(char.IsDigit(c)));
    }

    [Fact]
    public void GenerateString_ZeroLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Randomizer.GenerateString(0));
    }

    [Fact]
    public void GenerateString_Over128_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Randomizer.GenerateString(129));
    }

    [Fact]
    public void GenerateString_MaxLength128_Works()
    {
        var result = Randomizer.GenerateString(128);
        Assert.Equal(128, result.Length);
    }

    [Fact]
    public void WeightRandom_WithItems_ReturnsAnItem()
    {
        var items = new[] { "a", "b", "c" };
        var result = items.WeightRandom(x => 1);

        Assert.Contains(result, items);
    }

    [Fact]
    public void WeightRandom_EmptyCollection_ReturnsDefault()
    {
        var items = Array.Empty<string>();
        var result = items.WeightRandom(x => 1);

        Assert.Null(result);
    }

    [Fact]
    public void WeightRandom_SingleItem_ReturnsThatItem()
    {
        var items = new[] { "only" };
        var result = items.WeightRandom(x => 10);

        Assert.Equal("only", result);
    }
}
