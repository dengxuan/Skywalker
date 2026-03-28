using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class EnumerableExtensionsTests
{
    [Fact]
    public void JoinAsString_Strings_ShouldConcatenate()
    {
        var items = new[] { "a", "b", "c" };
        Assert.Equal("a, b, c", items.JoinAsString(", "));
    }

    [Fact]
    public void JoinAsString_Typed_ShouldConvertAndJoin()
    {
        var items = new[] { 1, 2, 3 };
        Assert.Equal("1-2-3", items.JoinAsString("-"));
    }

    [Fact]
    public void JoinAsString_Empty_ShouldReturnEmpty()
    {
        var items = Array.Empty<string>();
        Assert.Equal("", items.JoinAsString(","));
    }

    [Fact]
    public void WhereIf_ConditionTrue_ShouldFilter()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var result = items.WhereIf(true, x => x > 3).ToList();
        Assert.Equal(new[] { 4, 5 }, result);
    }

    [Fact]
    public void WhereIf_ConditionFalse_ShouldNotFilter()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var result = items.WhereIf(false, x => x > 3).ToList();
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void WhereIf_WithIndex_ConditionTrue_ShouldFilter()
    {
        var items = new[] { "a", "b", "c" };
        var result = items.WhereIf(true, (x, i) => i > 0).ToList();
        Assert.Equal(new[] { "b", "c" }, result);
    }

    [Fact]
    public void WhereIf_WithIndex_ConditionFalse_ShouldNotFilter()
    {
        var items = new[] { "a", "b", "c" };
        var result = items.WhereIf(false, (x, i) => i > 0).ToList();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void IsNullOrEmpty_NullEnumerable_ShouldReturnTrue()
    {
        IEnumerable? items = null;
        Assert.True(items.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptyEnumerable_ShouldReturnTrue()
    {
        IEnumerable items = Array.Empty<int>();
        Assert.True(items.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyEnumerable_ShouldReturnFalse()
    {
        IEnumerable items = new[] { 1 };
        Assert.False(items.IsNullOrEmpty());
    }

    [Fact]
    public void HasDuplicates_WithDuplicates_ShouldReturnTrue()
    {
        var list = new[] { "a", "b", "a" };
        Assert.True(list.HasDuplicates(x => x));
    }

    [Fact]
    public void HasDuplicates_WithoutDuplicates_ShouldReturnFalse()
    {
        var list = new[] { "a", "b", "c" };
        Assert.False(list.HasDuplicates(x => x));
    }

    [Fact]
    public void HasDuplicates_ByProperty_ShouldDetect()
    {
        var list = new[] { ("a", 1), ("b", 1), ("c", 2) };
        Assert.True(list.HasDuplicates(x => x.Item2));
    }

    [Fact]
    public void SelectNonNull_ShouldFilterNulls()
    {
        var items = new[] { "a", null, "b", null, "c" };
        var result = items.SelectNonNull(x => x).ToList();
        Assert.Equal(new[] { "a", "b", "c" }, result);
    }

    [Fact]
    public void SelectNonNull_WithTransform_ShouldFilterNullResults()
    {
        var items = new[] { 1, 2, 3, 4 };
        var result = items.SelectNonNull(x => x > 2 ? x.ToString() : null).ToList();
        Assert.Equal(new[] { "3", "4" }, result);
    }

    [Fact]
    public void SortByDependencies_ShouldTopologicalSort()
    {
        // A depends on B, B depends on C => result: C, B, A
        var deps = new Dictionary<string, string[]>
        {
            ["A"] = new[] { "B" },
            ["B"] = new[] { "C" },
            ["C"] = Array.Empty<string>()
        };

        var result = new[] { "A", "B", "C" }.SortByDependencies(
            item => deps.TryGetValue(item, out var d) ? d : Array.Empty<string>());

        Assert.Equal(new[] { "C", "B", "A" }, result);
    }

    [Fact]
    public void SortByDependencies_NoDependencies_ShouldPreserveOrder()
    {
        var result = new[] { "A", "B", "C" }.SortByDependencies(
            _ => Array.Empty<string>());

        Assert.Equal(new[] { "A", "B", "C" }, result);
    }

    [Fact]
    public void SortByDependencies_CyclicDependency_ShouldThrow()
    {
        var deps = new Dictionary<string, string[]>
        {
            ["A"] = new[] { "B" },
            ["B"] = new[] { "A" }
        };

        Assert.Throws<ArgumentException>(() =>
            new[] { "A", "B" }.SortByDependencies(
                item => deps.TryGetValue(item, out var d) ? d : Array.Empty<string>()));
    }
}
