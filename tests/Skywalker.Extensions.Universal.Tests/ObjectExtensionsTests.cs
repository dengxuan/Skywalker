using System;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class ObjectExtensionsTests
{
    [Fact]
    public void As_WithCompatibleType_ShouldReturnCasted()
    {
        object value = "hello";
        var result = value.As<string>();
        Assert.Equal("hello", result);
    }

    [Fact]
    public void As_WithIncompatibleType_ShouldReturnNull()
    {
        object value = 42;
        var result = value.As<string>();
        Assert.Null(result);
    }

    [Fact]
    public void To_Int_ShouldConvert()
    {
        object value = "42";
        var result = value.To<int>();
        Assert.Equal(42, result);
    }

    [Fact]
    public void To_Double_ShouldConvert()
    {
        object value = "3.14";
        var result = value.To<double>();
        Assert.Equal(3.14, result);
    }

    [Fact]
    public void To_Guid_ShouldConvert()
    {
        var guid = Guid.NewGuid();
        object value = guid.ToString();
        var result = value.To<Guid>();
        Assert.Equal(guid, result);
    }

    [Fact]
    public void To_Bool_ShouldConvert()
    {
        object value = "true";
        Assert.True(value.To<bool>());
    }

    [Fact]
    public void IsIn_WithItemInList_ShouldReturnTrue()
    {
        Assert.True(3.IsIn(1, 2, 3, 4));
    }

    [Fact]
    public void IsIn_WithItemNotInList_ShouldReturnFalse()
    {
        Assert.False(5.IsIn(1, 2, 3, 4));
    }

    [Fact]
    public void IsIn_String_ShouldWork()
    {
        Assert.True("b".IsIn("a", "b", "c"));
        Assert.False("d".IsIn("a", "b", "c"));
    }

    [Fact]
    public void Locking_Action_ShouldExecute()
    {
        var obj = new object();
        var executed = false;
        obj.Locking(() => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Locking_TypedAction_ShouldExecuteWithSource()
    {
        var list = new System.Collections.Generic.List<int>();
        list.Locking(l => l.Add(42));
        Assert.Single(list);
        Assert.Equal(42, list[0]);
    }

    [Fact]
    public void Locking_Func_ShouldReturnResult()
    {
        var obj = new object();
        var result = obj.Locking(() => 42);
        Assert.Equal(42, result);
    }

    [Fact]
    public void Locking_TypedFunc_ShouldReturnResult()
    {
        var list = new System.Collections.Generic.List<int> { 1, 2, 3 };
        var result = list.Locking(l => l.Count);
        Assert.Equal(3, result);
    }
}
