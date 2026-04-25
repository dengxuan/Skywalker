using System.Collections.Immutable;
using Skywalker.SourceGenerators;
using Xunit;

namespace Skywalker.SourceGenerators.Common.Tests;

public class EquatableArrayTests
{
    [Fact]
    public void Default_IsEmpty()
    {
        EquatableArray<int> array = default;
        Assert.True(array.IsEmpty);
        Assert.Equal(0, array.Length);
    }

    [Fact]
    public void Empty_IsEmpty()
    {
        Assert.True(EquatableArray<int>.Empty.IsEmpty);
        Assert.Equal(0, EquatableArray<int>.Empty.Length);
    }

    [Fact]
    public void ConstructFromArray_PreservesElements()
    {
        var array = new EquatableArray<int>(new[] { 1, 2, 3 });
        Assert.Equal(3, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
        Assert.Equal(3, array[2]);
    }

    [Fact]
    public void ConstructFromImmutableArray_PreservesElements()
    {
        var array = new EquatableArray<int>(ImmutableArray.Create(10, 20));
        Assert.Equal(2, array.Length);
        Assert.Equal(10, array[0]);
        Assert.Equal(20, array[1]);
    }

    [Fact]
    public void Equals_SameContent_IsTrue()
    {
        var a = new EquatableArray<int>(new[] { 1, 2, 3 });
        var b = new EquatableArray<int>(new[] { 1, 2, 3 });
        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Equals_DifferentContent_IsFalse()
    {
        var a = new EquatableArray<int>(new[] { 1, 2, 3 });
        var b = new EquatableArray<int>(new[] { 1, 2, 4 });
        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void Equals_DifferentLength_IsFalse()
    {
        var a = new EquatableArray<int>(new[] { 1, 2, 3 });
        var b = new EquatableArray<int>(new[] { 1, 2 });
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_BothEmpty_IsTrue()
    {
        EquatableArray<int> a = default;
        var b = EquatableArray<int>.Empty;
        Assert.True(a.Equals(b));
        Assert.True(a == b);
    }

    [Fact]
    public void Equals_DefaultVsNonEmpty_IsFalse()
    {
        EquatableArray<int> a = default;
        var b = new EquatableArray<int>(new[] { 1 });
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void GetHashCode_SameContent_IsEqual()
    {
        var a = new EquatableArray<int>(new[] { 1, 2, 3 });
        var b = new EquatableArray<int>(new[] { 1, 2, 3 });
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_Default_IsZero()
    {
        EquatableArray<int> a = default;
        Assert.Equal(0, a.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentOrder_IsDifferent()
    {
        var a = new EquatableArray<int>(new[] { 1, 2, 3 });
        var b = new EquatableArray<int>(new[] { 3, 2, 1 });
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Enumerate_DefaultArray_YieldsNothing()
    {
        EquatableArray<int> a = default;
        var count = 0;
        foreach (var _ in a) count++;
        Assert.Equal(0, count);
    }

    [Fact]
    public void Enumerate_PopulatedArray_YieldsAll()
    {
        var a = new EquatableArray<int>(new[] { 5, 6, 7 });
        var collected = new List<int>();
        foreach (var item in a) collected.Add(item);
        Assert.Equal(new[] { 5, 6, 7 }, collected);
    }

    [Fact]
    public void AsImmutableArray_Default_ReturnsEmpty()
    {
        EquatableArray<int> a = default;
        var imm = a.AsImmutableArray();
        Assert.True(imm.IsEmpty);
        Assert.False(imm.IsDefault);
    }

    [Fact]
    public void AsImmutableArray_Populated_ReturnsSame()
    {
        var source = ImmutableArray.Create(1, 2, 3);
        var a = new EquatableArray<int>(source);
        Assert.Equal(source, a.AsImmutableArray());
    }

    [Fact]
    public void ImplicitConversion_FromImmutableArray()
    {
        EquatableArray<int> a = ImmutableArray.Create(1, 2);
        Assert.Equal(2, a.Length);
    }

    [Fact]
    public void ImplicitConversion_FromArray()
    {
        EquatableArray<int> a = new[] { 4, 5 };
        Assert.Equal(2, a.Length);
    }

    [Fact]
    public void Equals_AsObject()
    {
        var a = new EquatableArray<string>(new[] { "x", "y" });
        object boxed = new EquatableArray<string>(new[] { "x", "y" });
        Assert.True(a.Equals(boxed));
        Assert.False(a.Equals("not-an-array"));
    }

    [Fact]
    public void RecordEquality_WorksAcrossEquatableArrayField()
    {
        var a = new TestModel("foo", new EquatableArray<int>(new[] { 1, 2 }));
        var b = new TestModel("foo", new EquatableArray<int>(new[] { 1, 2 }));
        var c = new TestModel("foo", new EquatableArray<int>(new[] { 1, 3 }));

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    private sealed record TestModel(string Name, EquatableArray<int> Numbers);
}
