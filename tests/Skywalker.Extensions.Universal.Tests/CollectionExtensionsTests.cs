using System;
using System.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class CollectionExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_NullCollection_ShouldReturnTrue()
    {
        ICollection<int>? collection = null;
        Assert.True(collection.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptyCollection_ShouldReturnTrue()
    {
        var collection = new List<int>();
        Assert.True(collection.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyCollection_ShouldReturnFalse()
    {
        var collection = new List<int> { 1 };
        Assert.False(collection.IsNullOrEmpty());
    }

    [Fact]
    public void AddIfNotContains_NewItem_ShouldAddAndReturnTrue()
    {
        var list = new List<int> { 1, 2 };
        Assert.True(list.AddIfNotContains(3));
        Assert.Contains(3, list);
    }

    [Fact]
    public void AddIfNotContains_ExistingItem_ShouldReturnFalse()
    {
        var list = new List<int> { 1, 2 };
        Assert.False(list.AddIfNotContains(2));
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void AddIfNotContains_NullSource_ShouldThrow()
    {
        ICollection<int>? list = null;
        Assert.Throws<ArgumentNullException>(() => list!.AddIfNotContains(1));
    }

    [Fact]
    public void AddIfNotContains_MultipleItems_ShouldReturnAddedItems()
    {
        var list = new List<int> { 1, 2 };
        var added = list.AddIfNotContains(new[] { 2, 3, 4 });
        Assert.Equal(new[] { 3, 4 }, added);
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void AddIfNotContains_WithPredicate_ShouldAddWhenNotFound()
    {
        var list = new List<string> { "hello" };
        var result = list.AddIfNotContains(s => s == "world", () => "world");
        Assert.True(result);
        Assert.Contains("world", list);
    }

    [Fact]
    public void AddIfNotContains_WithPredicate_ShouldNotAddWhenFound()
    {
        var list = new List<string> { "hello" };
        var result = list.AddIfNotContains(s => s == "hello", () => "hello");
        Assert.False(result);
        Assert.Single(list);
    }

    [Fact]
    public void RemoveAll_ShouldRemoveMatchingItems()
    {
        ICollection<int> list = new List<int> { 1, 2, 3, 4, 5 };
        var removed = list.RemoveAll(x => x > 3);
        Assert.Equal(new[] { 4, 5 }, removed);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void RemoveAll_NoMatches_ShouldReturnEmpty()
    {
        ICollection<int> list = new List<int> { 1, 2, 3 };
        var removed = list.RemoveAll(x => x > 10);
        Assert.Empty(removed);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void ConvertAll_ShouldConvertItems()
    {
        var items = new[] { 1, 2, 3 };
        var result = items.ConvertAll(x => x.ToString());
        Assert.Equal(new[] { "1", "2", "3" }, result);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnEachItem()
    {
        var items = new[] { 1, 2, 3 };
        var sum = 0;
        items.ForEach(x => sum += x);
        Assert.Equal(6, sum);
    }

    [Fact]
    public void Find_ShouldReturnMatchingItem()
    {
        var items = new[] { 1, 2, 3 };
        Assert.Equal(2, items.Find(x => x == 2));
    }

    [Fact]
    public void Find_NoMatch_ShouldReturnDefault()
    {
        var items = new[] { 1, 2, 3 };
        Assert.Equal(0, items.Find(x => x == 10));
    }

    [Fact]
    public void FindAll_ShouldReturnAllMatches()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var result = items.FindAll(x => x > 3);
        Assert.Equal(new[] { 4, 5 }, result);
    }

    [Fact]
    public void GetContentsHashCode_NullList_ShouldReturnZero()
    {
        IList<int>? list = null;
        Assert.Equal(0, list!.GetContentsHashCode());
    }

    [Fact]
    public void GetContentsHashCode_SameItemsDifferentOrder_ShouldReturnSameHash()
    {
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 3, 1, 2 };
        Assert.Equal(list1.GetContentsHashCode(), list2.GetContentsHashCode());
    }

    [Fact]
    public void AreEquivalent_SameItems_ShouldReturnTrue()
    {
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 3, 1, 2 };
        Assert.True(list1.AreEquivalent(list2));
    }

    [Fact]
    public void AreEquivalent_DifferentItems_ShouldReturnFalse()
    {
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = new List<int> { 4, 5, 6 };
        Assert.False(list1.AreEquivalent(list2));
    }

    [Fact]
    public void AreEquivalent_DifferentCounts_ShouldReturnFalse()
    {
        var list1 = new List<int> { 1, 2 };
        var list2 = new List<int> { 1, 2, 3 };
        Assert.False(list1.AreEquivalent(list2));
    }

    [Fact]
    public void AreEquivalent_BothNull_ShouldReturnTrue()
    {
        IList<int>? list1 = null;
        IList<int>? list2 = null;
        Assert.True(list1!.AreEquivalent(list2!));
    }

    [Fact]
    public void AreEquivalent_OneNull_ShouldReturnFalse()
    {
        var list1 = new List<int> { 1 };
        Assert.False(list1.AreEquivalent(null!));
    }

    [Fact]
    public void AreEquivalent_WithDuplicates_ShouldConsiderCounts()
    {
        var list1 = new List<int> { 1, 1, 2 };
        var list2 = new List<int> { 1, 2, 2 };
        Assert.False(list1.AreEquivalent(list2));
    }
}
