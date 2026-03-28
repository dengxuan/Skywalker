using System;
using System.Collections.Generic;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class ListExtensionsTests
{
    [Fact]
    public void FindIndex_WithMatch_ShouldReturnIndex()
    {
        var list = new List<int> { 10, 20, 30 };
        Assert.Equal(1, list.FindIndex(x => x == 20));
    }

    [Fact]
    public void FindIndex_NoMatch_ShouldReturnMinusOne()
    {
        var list = new List<int> { 10, 20, 30 };
        Assert.Equal(-1, list.FindIndex(x => x == 99));
    }

    [Fact]
    public void AddFirst_ShouldInsertAtBeginning()
    {
        var list = new List<int> { 2, 3 };
        list.AddFirst(1);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void AddLast_ShouldAppendAtEnd()
    {
        var list = new List<int> { 1, 2 };
        list.AddLast(3);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertAfter_ExistingItem_ShouldInsertAfter()
    {
        var list = new List<int> { 1, 3 };
        list.InsertAfter(1, 2);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertAfter_NonExistingItem_ShouldInsertFirst()
    {
        var list = new List<int> { 2, 3 };
        list.InsertAfter(99, 1);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertAfter_WithPredicate_ShouldInsertAfterMatch()
    {
        var list = new List<int> { 1, 3 };
        list.InsertAfter(x => x == 1, 2);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertAfter_WithPredicate_NoMatch_ShouldInsertFirst()
    {
        var list = new List<int> { 2, 3 };
        list.InsertAfter(x => x == 99, 1);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertBefore_ExistingItem_ShouldInsertBefore()
    {
        var list = new List<int> { 1, 3 };
        list.InsertBefore(3, 2);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertBefore_NonExistingItem_ShouldInsertLast()
    {
        var list = new List<int> { 1, 2 };
        list.InsertBefore(99, 3);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertBefore_WithPredicate_ShouldInsertBeforeMatch()
    {
        var list = new List<int> { 1, 3 };
        list.InsertBefore(x => x == 3, 2);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertBefore_WithPredicate_NoMatch_ShouldInsertLast()
    {
        var list = new List<int> { 1, 2 };
        list.InsertBefore(x => x == 99, 3);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void ReplaceWhile_ShouldReplaceAllMatches()
    {
        var list = new List<int> { 1, 2, 3, 2, 4 };
        list.ReplaceWhile(x => x == 2, 99);
        Assert.Equal(new[] { 1, 99, 3, 99, 4 }, list);
    }

    [Fact]
    public void ReplaceWhile_WithFactory_ShouldReplaceWithFactory()
    {
        var list = new List<int> { 1, 2, 3 };
        list.ReplaceWhile(x => x > 1, x => x * 10);
        Assert.Equal(new[] { 1, 20, 30 }, list);
    }

    [Fact]
    public void ReplaceOne_ShouldReplaceFirstMatch()
    {
        var list = new List<int> { 1, 2, 3, 2, 4 };
        list.ReplaceOne(x => x == 2, 99);
        Assert.Equal(new[] { 1, 99, 3, 2, 4 }, list);
    }

    [Fact]
    public void ReplaceOne_WithFactory_ShouldReplaceFirstMatch()
    {
        var list = new List<int> { 1, 2, 3 };
        list.ReplaceOne(x => x == 2, x => x * 10);
        Assert.Equal(new[] { 1, 20, 3 }, list);
    }

    [Fact]
    public void ReplaceOne_ByEquality_ShouldReplace()
    {
        var list = new List<int> { 1, 2, 3 };
        list.ReplaceOne(2, 99);
        Assert.Equal(new[] { 1, 99, 3 }, list);
    }

    [Fact]
    public void MoveItem_ShouldMoveToTargetIndex()
    {
        var list = new List<int> { 1, 2, 3, 4 };
        list.MoveItem(x => x == 4, 0);
        Assert.Equal(new[] { 4, 1, 2, 3 }, list);
    }

    [Fact]
    public void MoveItem_SameIndex_ShouldNotChange()
    {
        var list = new List<int> { 1, 2, 3 };
        list.MoveItem(x => x == 2, 1);
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void MoveItem_InvalidIndex_ShouldThrow()
    {
        var list = new List<int> { 1, 2, 3 };
        Assert.Throws<IndexOutOfRangeException>(() => list.MoveItem(x => x == 1, 10));
    }

    [Fact]
    public void GetOrAdd_ExistingItem_ShouldReturnExisting()
    {
        var list = new List<string> { "hello", "world" };
        var result = list.GetOrAdd(x => x == "hello", () => "new");
        Assert.Equal("hello", result);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void GetOrAdd_MissingItem_ShouldAddAndReturn()
    {
        var list = new List<string> { "hello" };
        var result = list.GetOrAdd(x => x == "world", () => "world");
        Assert.Equal("world", result);
        Assert.Equal(2, list.Count);
    }
}
