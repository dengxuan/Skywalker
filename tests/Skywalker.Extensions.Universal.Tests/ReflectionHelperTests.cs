using System.Reflection;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class ReflectionHelperTests
{
    // Test helper types
    private interface IGenericInterface<T> { }
    private class StringImpl : IGenericInterface<string> { }
    private class IntImpl : IGenericInterface<int>, IGenericInterface<string> { }
    private class NoImpl { }

    [Fact]
    public void IsAssignableToGenericType_DirectImpl_ReturnsTrue()
    {
        Assert.True(ReflectionHelper.IsAssignableToGenericType(typeof(StringImpl), typeof(IGenericInterface<>)));
    }

    [Fact]
    public void IsAssignableToGenericType_NoImpl_ReturnsFalse()
    {
        Assert.False(ReflectionHelper.IsAssignableToGenericType(typeof(NoImpl), typeof(IGenericInterface<>)));
    }

    [Fact]
    public void IsAssignableToGenericType_ClosedGeneric_ReturnsTrue()
    {
        Assert.True(ReflectionHelper.IsAssignableToGenericType(typeof(List<int>), typeof(IEnumerable<>)));
    }

    [Fact]
    public void IsAssignableToGenericType_OpenGenericSelf_ReturnsTrue()
    {
        Assert.True(ReflectionHelper.IsAssignableToGenericType(typeof(List<int>), typeof(List<>)));
    }

    [Fact]
    public void GetImplementedGenericTypes_SingleImpl()
    {
        var types = ReflectionHelper.GetImplementedGenericTypes(typeof(StringImpl), typeof(IGenericInterface<>));
        Assert.Contains(typeof(IGenericInterface<string>), types);
    }

    [Fact]
    public void GetImplementedGenericTypes_MultipleImpl()
    {
        var types = ReflectionHelper.GetImplementedGenericTypes(typeof(IntImpl), typeof(IGenericInterface<>));
        Assert.Contains(typeof(IGenericInterface<int>), types);
        Assert.Contains(typeof(IGenericInterface<string>), types);
    }

    [Fact]
    public void GetImplementedGenericTypes_NoImpl_ReturnsEmpty()
    {
        var types = ReflectionHelper.GetImplementedGenericTypes(typeof(NoImpl), typeof(IGenericInterface<>));
        Assert.Empty(types);
    }

    // Attribute tests
    [AttributeUsage(AttributeTargets.All)]
    private class TestAttribute : Attribute
    {
        public string Value { get; }
        public TestAttribute(string value) => Value = value;
    }

    [Test("class")]
    private class DecoratedClass
    {
        [Test("method")]
        public void DecoratedMethod() { }

        public void PlainMethod() { }
    }

    [Fact]
    public void GetSingleAttributeOrDefault_Found()
    {
        var attr = ReflectionHelper.GetSingleAttributeOrDefault<TestAttribute>(typeof(DecoratedClass));
        Assert.NotNull(attr);
        Assert.Equal("class", attr!.Value);
    }

    [Fact]
    public void GetSingleAttributeOrDefault_NotFound_ReturnsDefault()
    {
        var attr = ReflectionHelper.GetSingleAttributeOrDefault<ObsoleteAttribute>(typeof(DecoratedClass));
        Assert.Null(attr);
    }

    [Fact]
    public void GetSingleAttributeOrDefault_WithDefaultValue()
    {
        var defaultAttr = new ObsoleteAttribute("default");
        var attr = ReflectionHelper.GetSingleAttributeOrDefault(typeof(DecoratedClass), defaultAttr);
        Assert.Same(defaultAttr, attr);
    }

    [Fact]
    public void GetSingleAttributeOfMemberOrDeclaringTypeOrDefault_OnMethod()
    {
        var method = typeof(DecoratedClass).GetMethod(nameof(DecoratedClass.DecoratedMethod))!;
        var attr = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TestAttribute>(method);
        Assert.NotNull(attr);
        Assert.Equal("method", attr!.Value);
    }

    [Fact]
    public void GetSingleAttributeOfMemberOrDeclaringTypeOrDefault_FallsBackToDeclaringType()
    {
        var method = typeof(DecoratedClass).GetMethod(nameof(DecoratedClass.PlainMethod))!;
        var attr = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TestAttribute>(method);
        Assert.NotNull(attr);
        Assert.Equal("class", attr!.Value);
    }

    [Fact]
    public void GetAttributesOfMemberOrDeclaringType_CombinesMemberAndDeclaringType()
    {
        var method = typeof(DecoratedClass).GetMethod(nameof(DecoratedClass.DecoratedMethod))!;
        var attrs = ReflectionHelper.GetAttributesOfMemberOrDeclaringType<TestAttribute>(method).ToList();
        Assert.True(attrs.Count >= 1);
    }

    // GetValueByPath / SetValueByPath
    private class Outer
    {
        public string? Name { get; set; }
        public Inner? Inner { get; set; }
    }

    private class Inner
    {
        public int Value { get; set; }
    }

    [Fact]
    public void GetValueByPath_SimpleProperty()
    {
        var obj = new Outer { Name = "test" };
        var result = ReflectionHelper.GetValueByPath(obj, typeof(Outer), "Name");
        Assert.Equal("test", result);
    }

    [Fact]
    public void GetValueByPath_NestedProperty()
    {
        var obj = new Outer { Inner = new Inner { Value = 42 } };
        var result = ReflectionHelper.GetValueByPath(obj, typeof(Outer), "Inner.Value");
        Assert.Equal(42, result);
    }

    // GetPublicConstantsRecursively
    private static class ConstantsHolder
    {
        public const string Foo = "foo";
        public const string Bar = "bar";

        public static class Nested
        {
            public const string Baz = "baz";
        }
    }

    [Fact]
    public void GetPublicConstantsRecursively_ReturnsAllConstants()
    {
        var constants = ReflectionHelper.GetPublicConstantsRecursively(typeof(ConstantsHolder));
        Assert.Contains("foo", constants);
        Assert.Contains("bar", constants);
        Assert.Contains("baz", constants);
    }
}
