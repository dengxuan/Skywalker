using System.Reflection;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class TypeHelperTests
{
    private enum TestEnum { A, B }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(bool), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(object), false)]
    public void IsNonNullablePrimitiveType(Type type, bool expected)
    {
        Assert.Equal(expected, TypeHelper.IsNonNullablePrimitiveType(type));
    }

    [Fact]
    public void IsFunc_WithFunc_ReturnsTrue()
    {
        Func<int> func = () => 42;
        Assert.True(TypeHelper.IsFunc(func));
    }

    [Fact]
    public void IsFunc_WithNonFunc_ReturnsFalse()
    {
        Assert.False(TypeHelper.IsFunc("not a func"));
        Assert.False(TypeHelper.IsFunc(null!));
    }

    [Fact]
    public void IsFuncT_WithMatchingType_ReturnsTrue()
    {
        Func<int> func = () => 42;
        Assert.True(TypeHelper.IsFunc<int>(func));
    }

    [Fact]
    public void IsFuncT_WithWrongType_ReturnsFalse()
    {
        Func<int> func = () => 42;
        Assert.False(TypeHelper.IsFunc<string>(func));
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(object), false)]
    public void IsPrimitiveExtended(Type type, bool expected)
    {
        Assert.Equal(expected, TypeHelper.IsPrimitiveExtended(type));
    }

    [Fact]
    public void IsPrimitiveExtended_NullableInt_ReturnsTrue()
    {
        Assert.True(TypeHelper.IsPrimitiveExtended(typeof(int?)));
    }

    [Fact]
    public void IsPrimitiveExtended_Enum_WhenIncluded()
    {
        Assert.True(TypeHelper.IsPrimitiveExtended(typeof(TestEnum), includeEnums: true));
        Assert.False(TypeHelper.IsPrimitiveExtended(typeof(TestEnum), includeEnums: false));
    }

    [Fact]
    public void IsNullable_NullableInt_ReturnsTrue()
    {
        Assert.True(TypeHelper.IsNullable(typeof(int?)));
    }

    [Fact]
    public void IsNullable_Int_ReturnsFalse()
    {
        Assert.False(TypeHelper.IsNullable(typeof(int)));
    }

    [Fact]
    public void GetFirstGenericArgumentIfNullable_NullableInt()
    {
        var result = typeof(int?).GetFirstGenericArgumentIfNullable();
        Assert.Equal(typeof(int), result);
    }

    [Fact]
    public void GetFirstGenericArgumentIfNullable_NonNullable_ReturnsSelf()
    {
        var result = typeof(string).GetFirstGenericArgumentIfNullable();
        Assert.Equal(typeof(string), result);
    }

    [Fact]
    public void IsEnumerable_List_ReturnsTrue()
    {
        Assert.True(TypeHelper.IsEnumerable(typeof(List<int>), out var itemType));
        Assert.Equal(typeof(int), itemType);
    }

    [Fact]
    public void IsEnumerable_String_ReturnsFalse_WhenExcludingPrimitives()
    {
        Assert.False(TypeHelper.IsEnumerable(typeof(string), out _, includePrimitives: false));
    }

    [Fact]
    public void IsDictionary_Dictionary_ReturnsTrue()
    {
        Assert.True(TypeHelper.IsDictionary(typeof(Dictionary<string, int>), out var keyType, out var valueType));
        Assert.Equal(typeof(string), keyType);
        Assert.Equal(typeof(int), valueType);
    }

    [Fact]
    public void IsDictionary_NonDictionary_ReturnsFalse()
    {
        Assert.False(TypeHelper.IsDictionary(typeof(List<int>), out _, out _));
    }

    [Fact]
    public void GetDefaultValue_ValueType_ReturnsDefault()
    {
        Assert.Equal(0, TypeHelper.GetDefaultValue(typeof(int)));
    }

    [Fact]
    public void GetDefaultValue_ReferenceType_ReturnsNull()
    {
        Assert.Null(TypeHelper.GetDefaultValue(typeof(string)));
    }

    [Fact]
    public void GetDefaultValueT_Int()
    {
        Assert.Equal(0, TypeHelper.GetDefaultValue<int>());
    }

    [Fact]
    public void GetFullNameHandlingNullableAndGenerics_NullableInt()
    {
        var name = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(int?));
        Assert.Equal("System.Int32?", name);
    }

    [Fact]
    public void GetFullNameHandlingNullableAndGenerics_GenericType()
    {
        var name = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(List<int>));
        Assert.Contains("System.Int32", name);
    }

    [Fact]
    public void GetFullNameHandlingNullableAndGenerics_PlainType()
    {
        var name = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(int));
        Assert.Equal("System.Int32", name);
    }

    [Theory]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(int), "number")]
    [InlineData(typeof(bool), "boolean")]
    [InlineData(typeof(double), "number")]
    [InlineData(typeof(DateTime), "string")]
    [InlineData(typeof(Guid), "string")]
    public void GetSimplifiedName_KnownTypes(Type type, string expected)
    {
        Assert.Equal(expected, TypeHelper.GetSimplifiedName(type));
    }

    [Fact]
    public void GetSimplifiedName_NullableInt()
    {
        var result = TypeHelper.GetSimplifiedName(typeof(int?));
        Assert.Equal("number?", result);
    }

    [Theory]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(double), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(int), false)]
    public void IsFloatingType(Type type, bool expected)
    {
        Assert.Equal(expected, TypeHelper.IsFloatingType(type));
    }

    [Fact]
    public void IsFloatingType_NullableDouble()
    {
        Assert.True(TypeHelper.IsFloatingType(typeof(double?)));
    }

    [Fact]
    public void StripNullable_NullableInt_ReturnsInt()
    {
        Assert.Equal(typeof(int), TypeHelper.StripNullable(typeof(int?)));
    }

    [Fact]
    public void StripNullable_NonNullable_ReturnsSame()
    {
        Assert.Equal(typeof(int), TypeHelper.StripNullable(typeof(int)));
    }

    [Fact]
    public void ConvertFromString_IntValue()
    {
        var result = TypeHelper.ConvertFromString(typeof(int), "42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void ConvertFromString_NullValue_ReturnsNull()
    {
        Assert.Null(TypeHelper.ConvertFromString(typeof(int), null!));
    }

    [Fact]
    public void ConvertFromString_FloatingWithComma()
    {
        var result = TypeHelper.ConvertFromString(typeof(double), "3,14");
        Assert.Equal(3.14, result);
    }

    [Fact]
    public void ConvertFromStringT_Works()
    {
        var result = TypeHelper.ConvertFromString<int>("42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void ConvertFrom_StringToInt()
    {
        var result = TypeHelper.ConvertFrom(typeof(int), "42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void ConvertFromT_Works()
    {
        var result = TypeHelper.ConvertFrom<int>("42");
        Assert.Equal(42, result);
    }
}
