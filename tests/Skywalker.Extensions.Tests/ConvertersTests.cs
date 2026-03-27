using System.Globalization;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class ConvertersTests
{
    private enum TestEnum
    {
        ValueA,
        ValueB,
        ValueC
    }

    private class SimpleObject
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    // ToDictionary
    [Fact]
    public void ToDictionary_NullSource_ReturnsNull()
    {
        var result = Converters.ToDictionary(null!);
        Assert.Null(result);
    }

    [Fact]
    public void ToDictionary_SimpleObject_ReturnsDictionary()
    {
        var obj = new SimpleObject { Name = "Test", Age = 25 };
        var result = Converters.ToDictionary(obj);

        Assert.NotNull(result);
        Assert.Equal("Test", result["name"]);
        Assert.Equal(25, result["age"]);
    }

    [Fact]
    public void ToDictionary_ForceCamelCase_ConvertsToCamelCase()
    {
        var obj = new SimpleObject { Name = "Test", Age = 30 };
        var result = Converters.ToDictionary(obj, forceCamelCase: true);

        Assert.NotNull(result);
        Assert.True(result.ContainsKey("name"));
        Assert.True(result.ContainsKey("age"));
    }

    [Fact]
    public void ToDictionary_NoCamelCase_KeepsOriginalCase()
    {
        var obj = new SimpleObject { Name = "Test", Age = 30 };
        var result = Converters.ToDictionary(obj, forceCamelCase: false);

        Assert.NotNull(result);
        Assert.True(result.ContainsKey("Name"));
        Assert.True(result.ContainsKey("Age"));
    }

    // ChangeType<T>
    [Fact]
    public void ChangeType_IntFromString()
    {
        var result = Converters.ChangeType<int>("42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void ChangeType_EnumFromString()
    {
        var result = Converters.ChangeType<TestEnum>("ValueB");
        Assert.Equal(TestEnum.ValueB, result);
    }

    [Fact]
    public void ChangeType_DoubleFromInt()
    {
        var result = Converters.ChangeType<double>(42);
        Assert.Equal(42.0, result);
    }

    // TryChangeType<T>
    [Fact]
    public void TryChangeType_ValidInt_ReturnsTrue()
    {
        var success = Converters.TryChangeType<int>("42", out var result);
        Assert.True(success);
        Assert.Equal(42, result);
    }

    [Fact]
    public void TryChangeType_InvalidInt_ReturnsFalse()
    {
        var success = Converters.TryChangeType<int>("abc", out var result);
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryChangeType_Guid_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        var success = Converters.TryChangeType<Guid>(guid.ToString(), out var result);
        Assert.True(success);
        Assert.Equal(guid, result);
    }

    [Fact]
    public void TryChangeType_Enum_ReturnsTrue()
    {
        var success = Converters.TryChangeType<TestEnum>("ValueC", out var result);
        Assert.True(success);
        Assert.Equal(TestEnum.ValueC, result);
    }

    [Fact]
    public void TryChangeType_WithCulture_ReturnsTrue()
    {
        var success = Converters.TryChangeType<double>("3.14", out var result, CultureInfo.InvariantCulture);
        Assert.True(success);
        Assert.Equal(3.14, result);
    }

    // EnumTryParse
    [Fact]
    public void EnumTryParse_ValidName_ReturnsTrue()
    {
        var success = Converters.EnumTryParse<TestEnum>("ValueA", typeof(TestEnum), out var result);
        Assert.True(success);
        Assert.Equal(TestEnum.ValueA, result);
    }

    [Fact]
    public void EnumTryParse_CaseInsensitive_ReturnsTrue()
    {
        var success = Converters.EnumTryParse<TestEnum>("valuea", typeof(TestEnum), out var result);
        Assert.True(success);
        Assert.Equal(TestEnum.ValueA, result);
    }

    [Fact]
    public void EnumTryParse_InvalidName_ReturnsFalse()
    {
        var success = Converters.EnumTryParse<TestEnum>("InvalidValue", typeof(TestEnum), out var result);
        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void EnumTryParse_NullInput_ReturnsFalse()
    {
        var success = Converters.EnumTryParse<TestEnum>(null, typeof(TestEnum), out var result);
        Assert.False(success);
        Assert.Equal(default, result);
    }

    // FormatValue overloads
    [Fact]
    public void FormatValue_Byte()
    {
        Assert.Equal("42", Converters.FormatValue((byte)42, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableByte_Null()
    {
        Assert.Null(Converters.FormatValue((byte?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableByte_HasValue()
    {
        Assert.Equal("42", Converters.FormatValue((byte?)42, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Short()
    {
        Assert.Equal("100", Converters.FormatValue((short)100, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableShort_Null()
    {
        Assert.Null(Converters.FormatValue((short?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Int()
    {
        Assert.Equal("42", Converters.FormatValue(42, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableInt_Null()
    {
        Assert.Null(Converters.FormatValue((int?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Long()
    {
        Assert.Equal("100000", Converters.FormatValue(100000L, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableLong_Null()
    {
        Assert.Null(Converters.FormatValue((long?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Float()
    {
        Assert.Equal("3.14", Converters.FormatValue(3.14f, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableFloat_Null()
    {
        Assert.Null(Converters.FormatValue((float?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Double()
    {
        Assert.Equal("3.14", Converters.FormatValue(3.14, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableDouble_Null()
    {
        Assert.Null(Converters.FormatValue((double?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_Decimal()
    {
        Assert.Equal("3.14", Converters.FormatValue(3.14m, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableDecimal_Null()
    {
        Assert.Null(Converters.FormatValue((decimal?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_SByte()
    {
        Assert.Equal("42", Converters.FormatValue((sbyte)42, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableSByte_Null()
    {
        Assert.Null(Converters.FormatValue((sbyte?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_UShort()
    {
        Assert.Equal("100", Converters.FormatValue((ushort)100, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableUShort_Null()
    {
        Assert.Null(Converters.FormatValue((ushort?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_UInt()
    {
        Assert.Equal("42", Converters.FormatValue((uint)42, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableUInt_Null()
    {
        Assert.Null(Converters.FormatValue((uint?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_ULong()
    {
        Assert.Equal("100000", Converters.FormatValue((ulong)100000, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_NullableULong_Null()
    {
        Assert.Null(Converters.FormatValue((ulong?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_DateTime()
    {
        var dt = new DateTime(2023, 1, 15, 10, 30, 0);
        var result = Converters.FormatValue(dt, CultureInfo.InvariantCulture);
        Assert.Contains("01/15/2023", result);
    }

    [Fact]
    public void FormatValue_NullableDateTime_Null()
    {
        Assert.Null(Converters.FormatValue((DateTime?)null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FormatValue_DateTimeOffset()
    {
        var dto = new DateTimeOffset(2023, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var result = Converters.FormatValue(dto, CultureInfo.InvariantCulture);
        Assert.Contains("01/15/2023", result);
    }

    [Fact]
    public void FormatValue_NullableDateTimeOffset_Null()
    {
        Assert.Null(Converters.FormatValue((DateTimeOffset?)null, CultureInfo.InvariantCulture));
    }

    // GetMinMaxValueOfType
    [Fact]
    public void GetMinMaxValueOfType_Int()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<int>();
        Assert.Equal(int.MinValue, min);
        Assert.Equal(int.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Byte()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<byte>();
        Assert.Equal(byte.MinValue, min);
        Assert.Equal(byte.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Long()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<long>();
        Assert.Equal(long.MinValue, min);
        Assert.Equal(long.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Double()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<double>();
        Assert.Equal(double.MinValue, min);
        Assert.Equal(double.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Decimal()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<decimal>();
        Assert.Equal(decimal.MinValue, min);
        Assert.Equal(decimal.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Short()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<short>();
        Assert.Equal(short.MinValue, min);
        Assert.Equal(short.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_Float()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<float>();
        Assert.Equal(float.MinValue, min);
        Assert.Equal(float.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_SByte()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<sbyte>();
        Assert.Equal(sbyte.MinValue, min);
        Assert.Equal(sbyte.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_UShort()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<ushort>();
        Assert.Equal(ushort.MinValue, min);
        Assert.Equal(ushort.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_UInt()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<uint>();
        Assert.Equal(uint.MinValue, min);
        Assert.Equal(uint.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_ULong()
    {
        var (min, max) = Converters.GetMinMaxValueOfType<ulong>();
        Assert.Equal(ulong.MinValue, min);
        Assert.Equal(ulong.MaxValue, max);
    }

    [Fact]
    public void GetMinMaxValueOfType_UnsupportedType_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Converters.GetMinMaxValueOfType<string>());
    }
}
