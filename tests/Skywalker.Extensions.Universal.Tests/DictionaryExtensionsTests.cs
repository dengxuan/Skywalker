using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class DictionaryExtensionsTests
{
    [Fact]
    public void GetOrDefault_Dictionary_ExistingKey_ShouldReturnValue()
    {
        var dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.Equal(42, dict.GetOrDefault("key"));
    }

    [Fact]
    public void GetOrDefault_Dictionary_MissingKey_ShouldReturnDefault()
    {
        var dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.Equal(0, dict.GetOrDefault("missing"));
    }

    [Fact]
    public void GetOrDefault_IDictionary_ExistingKey_ShouldReturnValue()
    {
        IDictionary<string, int> dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.Equal(42, dict.GetOrDefault("key"));
    }

    [Fact]
    public void GetOrDefault_IDictionary_MissingKey_ShouldReturnDefault()
    {
        IDictionary<string, int> dict = new Dictionary<string, int>();
        Assert.Equal(0, dict.GetOrDefault("missing"));
    }

    [Fact]
    public void GetOrDefault_ReadOnlyDictionary_ExistingKey_ShouldReturnValue()
    {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int> { ["key"] = 42 };
        Assert.Equal(42, dict.GetOrDefault("key"));
    }

    [Fact]
    public void GetOrDefault_ReadOnlyDictionary_MissingKey_ShouldReturnDefault()
    {
        IReadOnlyDictionary<string, int> dict = new Dictionary<string, int>();
        Assert.Equal(0, dict.GetOrDefault("missing"));
    }

    [Fact]
    public void GetOrDefault_ConcurrentDictionary_ExistingKey_ShouldReturnValue()
    {
        var dict = new ConcurrentDictionary<string, int>();
        dict["key"] = 42;
        Assert.Equal(42, dict.GetOrDefault("key"));
    }

    [Fact]
    public void GetOrDefault_ConcurrentDictionary_MissingKey_ShouldReturnDefault()
    {
        var dict = new ConcurrentDictionary<string, int>();
        Assert.Equal(0, dict.GetOrDefault("missing"));
    }

    [Fact]
    public void GetOrAdd_ExistingKey_ShouldReturnExistingValue()
    {
        IDictionary<string, int> dict = new Dictionary<string, int> { ["key"] = 42 };
        var factoryCalled = false;
        var result = dict.GetOrAdd("key", _ => { factoryCalled = true; return 99; });
        Assert.Equal(42, result);
        Assert.False(factoryCalled);
    }

    [Fact]
    public void GetOrAdd_MissingKey_ShouldCallFactoryAndAdd()
    {
        IDictionary<string, int> dict = new Dictionary<string, int>();
        var result = dict.GetOrAdd("key", k => k.Length);
        Assert.Equal(3, result);
        Assert.Equal(3, dict["key"]);
    }

    [Fact]
    public void GetOrAdd_ParameterlessFactory_ShouldWork()
    {
        IDictionary<string, int> dict = new Dictionary<string, int>();
        var result = dict.GetOrAdd("key", () => 99);
        Assert.Equal(99, result);
        Assert.Equal(99, dict["key"]);
    }

    [Fact]
    public void GetOrDefault_ReferenceType_MissingKey_ShouldReturnNull()
    {
        var dict = new Dictionary<string, string> { ["key"] = "value" };
        Assert.Null(dict.GetOrDefault("missing"));
    }
}
