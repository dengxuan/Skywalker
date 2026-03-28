using Xunit;

namespace Skywalker.Extensions.Tests;

public class LruCacheTests
{
    [Fact]
    public void GetValue_CachesResult()
    {
        var callCount = 0;
        var cache = new LruCache<string, int>(key => { callCount++; return key.Length; }, 10);
        var v1 = cache.GetValue("hello");
        var v2 = cache.GetValue("hello");
        Assert.Equal(5, v1);
        Assert.Equal(5, v2);
        Assert.Equal(1, callCount); // evaluator called only once
    }

    [Fact]
    public void GetValue_EvictsLru_WhenOverCapacity()
    {
        var callCount = 0;
        var cache = new LruCache<int, int>(key => { callCount++; return key * 10; }, 3);
        cache.GetValue(1);
        cache.GetValue(2);
        cache.GetValue(3);
        Assert.Equal(3, callCount);

        // Adding 4th item should evict "1" (least recently used)
        cache.GetValue(4);
        Assert.Equal(4, callCount);

        // Accessing "1" again should re-evaluate since it was evicted
        cache.GetValue(1);
        Assert.Equal(5, callCount);
    }

    [Fact]
    public void GetValue_RecentlyUsed_NotEvicted()
    {
        var callCount = 0;
        var cache = new LruCache<int, int>(key => { callCount++; return key; }, 3);
        cache.GetValue(1);
        cache.GetValue(2);
        cache.GetValue(3);
        // Touch 1 so it becomes most-recently used
        cache.GetValue(1);
        Assert.Equal(3, callCount); // still cached

        // Adding 4 should evict 2 (LRU), not 1
        cache.GetValue(4);
        cache.GetValue(1);
        Assert.Equal(4, callCount); // 1 still cached; only 4 was new
    }

    [Fact]
    public void Constructor_ZeroCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<int, int>(x => x, 0));
    }

    [Fact]
    public void Constructor_NegativeCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<int, int>(x => x, -1));
    }

    [Fact]
    public void Capacity_GetSet()
    {
        var cache = new LruCache<int, int>(x => x, 5);
        Assert.Equal(5, cache.Capacity);
        cache.Capacity = 10;
        Assert.Equal(10, cache.Capacity);
    }

    [Fact]
    public void Capacity_SetToZero_Throws()
    {
        var cache = new LruCache<int, int>(x => x, 5);
        Assert.Throws<ArgumentOutOfRangeException>(() => cache.Capacity = 0);
    }

    [Fact]
    public void Capacity_Shrink_EvictsExcess()
    {
        var callCount = 0;
        var cache = new LruCache<int, int>(key => { callCount++; return key; }, 5);
        cache.GetValue(1);
        cache.GetValue(2);
        cache.GetValue(3);
        Assert.Equal(3, callCount);

        // Shrink to 2 — should keep the 2 most recently used items
        cache.Capacity = 2;
        Assert.Equal(2, cache.Capacity);

        // Most recently used 2 and 3 should remain cached
        cache.GetValue(3);
        cache.GetValue(2);
        Assert.Equal(3, callCount); // still cached

        // 1 should be evicted
        cache.GetValue(1);
        Assert.Equal(4, callCount); // re-evaluated
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var cache = new LruCache<int, int>(x => x, 5);
        cache.GetValue(1);
        cache.Dispose();
    }
}
