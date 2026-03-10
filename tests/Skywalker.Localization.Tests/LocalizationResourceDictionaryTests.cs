using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

public class LocalizationResourceDictionaryTests
{
    [Fact]
    public void Add_Generic_CreatesAndAddsResource()
    {
        var dictionary = new LocalizationResourceDictionary();

        var resource = dictionary.Add<TestResource>();

        Assert.NotNull(resource);
        Assert.Equal(typeof(TestResource), resource.ResourceType);
        Assert.True(dictionary.ContainsKey(typeof(TestResource)));
    }

    [Fact]
    public void Add_WithDefaultCulture_SetsDefaultCultureName()
    {
        var dictionary = new LocalizationResourceDictionary();

        var resource = dictionary.Add<TestResource>("zh-CN");

        Assert.Equal("zh-CN", resource.DefaultCultureName);
    }

    [Fact]
    public void Add_ThrowsOnDuplicate()
    {
        var dictionary = new LocalizationResourceDictionary();
        dictionary.Add<TestResource>();

        Assert.Throws<InvalidOperationException>(() => dictionary.Add<TestResource>());
    }

    [Fact]
    public void GetOrNull_Generic_ReturnsResource()
    {
        var dictionary = new LocalizationResourceDictionary();
        var added = dictionary.Add<TestResource>();

        var result = dictionary.GetOrNull<TestResource>();

        Assert.Same(added, result);
    }

    [Fact]
    public void GetOrNull_ReturnsNullForNotFound()
    {
        var dictionary = new LocalizationResourceDictionary();

        var result = dictionary.GetOrNull<TestResource>();

        Assert.Null(result);
    }

    [Fact]
    public void Indexer_ReturnsResource()
    {
        var dictionary = new LocalizationResourceDictionary();
        var added = dictionary.Add<TestResource>();

        var result = dictionary[typeof(TestResource)];

        Assert.Same(added, result);
    }

    [Fact]
    public void Count_ReturnsCorrectValue()
    {
        var dictionary = new LocalizationResourceDictionary();

        Assert.Empty(dictionary);

        dictionary.Add<TestResource>();
        Assert.Single(dictionary);

        dictionary.Add<BaseResource>();
        Assert.Equal(2, dictionary.Count);
    }

    [Fact]
    public void Remove_RemovesResource()
    {
        var dictionary = new LocalizationResourceDictionary();
        dictionary.Add<TestResource>();

        var removed = dictionary.Remove(typeof(TestResource));

        Assert.True(removed);
        Assert.False(dictionary.ContainsKey(typeof(TestResource)));
    }

    [Fact]
    public void Clear_RemovesAllResources()
    {
        var dictionary = new LocalizationResourceDictionary();
        dictionary.Add<TestResource>();
        dictionary.Add<BaseResource>();

        dictionary.Clear();

        Assert.Empty(dictionary);
    }
}

