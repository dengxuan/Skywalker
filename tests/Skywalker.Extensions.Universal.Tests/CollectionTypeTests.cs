using Skywalker.Extensions.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class CollectionTypeTests
{
    // PagedList
    [Fact]
    public void PagedList_StoresValues()
    {
        var items = new[] { 1, 2, 3 };
        var paged = new PagedList<int>(100, items);
        Assert.Equal(100, paged.TotalCount);
        Assert.Equal(items, paged.Items);
    }

    // CopyOnWriteDictionary
    [Fact]
    public void CopyOnWriteDictionary_ReadsFromSource()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.Equal(1, cow["a"]);
        Assert.Single(cow);
        Assert.True(cow.ContainsKey("a"));
    }

    [Fact]
    public void CopyOnWriteDictionary_WriteCreatesInnerCopy()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        cow["b"] = 2;
        Assert.Equal(2, cow.Count);
        Assert.False(source.ContainsKey("b")); // source not modified
    }

    [Fact]
    public void CopyOnWriteDictionary_Add_Works()
    {
        var source = new Dictionary<string, int>();
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        cow.Add("x", 10);
        Assert.True(cow.ContainsKey("x"));
    }

    [Fact]
    public void CopyOnWriteDictionary_Remove_Works()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.True(cow.Remove("a"));
    }

    [Fact]
    public void CopyOnWriteDictionary_TryGetValue_Works()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.True(cow.TryGetValue("a", out var val));
        Assert.Equal(1, val);
    }

    [Fact]
    public void CopyOnWriteDictionary_Clear_Works()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        cow.Clear();
        Assert.Empty(cow);
    }

    [Fact]
    public void CopyOnWriteDictionary_IsNotReadOnly()
    {
        var source = new Dictionary<string, int>();
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.False(cow.IsReadOnly);
    }

    [Fact]
    public void CopyOnWriteDictionary_KeysAndValues()
    {
        var source = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.Equal(2, cow.Keys.Count);
        Assert.Equal(2, cow.Values.Count);
    }

    [Fact]
    public void CopyOnWriteDictionary_Enumerable()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        var count = 0;
        foreach (var _ in cow) count++;
        Assert.Equal(1, count);
    }

    [Fact]
    public void CopyOnWriteDictionary_Contains_KVP()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        Assert.Contains(new KeyValuePair<string, int>("a", 1), cow);
    }

    [Fact]
    public void CopyOnWriteDictionary_CopyTo()
    {
        var source = new Dictionary<string, int> { { "a", 1 } };
        var cow = new CopyOnWriteDictionary<string, int>(source, StringComparer.Ordinal);
        var arr = new KeyValuePair<string, int>[1];
        cow.CopyTo(arr, 0);
        Assert.Equal("a", arr[0].Key);
    }

    // TypeList
    [Fact]
    public void TypeList_AddGeneric_Works()
    {
        var list = new TypeList();
        list.Add<string>();
        Assert.Contains(typeof(string), list);
    }

    [Fact]
    public void TypeList_AddType_Works()
    {
        var list = new TypeList();
        list.Add(typeof(int));
        Assert.Single(list);
    }

    [Fact]
    public void TypeList_ContainsGeneric_Works()
    {
        var list = new TypeList();
        list.Add<string>();
        Assert.True(list.Contains<string>());
    }

    [Fact]
    public void TypeList_RemoveGeneric_Works()
    {
        var list = new TypeList();
        list.Add<string>();
        list.Remove<string>();
        Assert.Empty(list);
    }

    [Fact]
    public void TypeList_Indexer_Works()
    {
        var list = new TypeList();
        list.Add<string>();
        Assert.Equal(typeof(string), list[0]);
    }

    [Fact]
    public void TypeList_Insert_Works()
    {
        var list = new TypeList();
        list.Add<string>();
        list.Insert(0, typeof(int));
        Assert.Equal(typeof(int), list[0]);
    }

    [Fact]
    public void TypeList_WithBaseType_RejectsInvalidType()
    {
        var list = new TypeList<IDisposable>();
        Assert.Throws<ArgumentException>(() => list.Add(typeof(string)));
    }

    // Base32
    [Fact]
    public void Base32_RoundTrip()
    {
        var input = new byte[] { 1, 2, 3, 4, 5 };
        var encoded = Base32.ToBase32(input);
        var decoded = Base32.FromBase32(encoded);
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Base32_NullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => Base32.ToBase32(null!));
    }

    [Fact]
    public void Base32_EmptyInput()
    {
        var encoded = Base32.ToBase32(Array.Empty<byte>());
        Assert.Equal(string.Empty, encoded);
    }
}
