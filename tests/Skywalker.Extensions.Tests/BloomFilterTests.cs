using Skywalker.Extensions.Filters;
using Xunit;

namespace Skywalker.Extensions.Tests;

public class BloomFilterTests
{
    // Constructor tests
    [Fact]
    public void Constructor_WithCNK_CreatesFilter()
    {
        var filter = new BloomFilter(10.0, 100, 7);

        Assert.Equal(100, filter.GetExpectedNumberOfElements());
        Assert.Equal(7, filter.K);
        Assert.Equal(1000, filter.Size());
        Assert.Equal(0, filter.Count());
    }

    [Fact]
    public void Constructor_WithBitSetSizeAndElements_CreatesFilter()
    {
        var filter = new BloomFilter(1000, 100);

        Assert.Equal(100, filter.GetExpectedNumberOfElements());
        Assert.True(filter.K > 0);
        Assert.Equal(1000, filter.Size());
        Assert.Equal(0, filter.Count());
    }

    [Fact]
    public void Constructor_WithFalsePositiveProbability_CreatesFilter()
    {
        var filter = new BloomFilter(0.01, 100);

        Assert.Equal(100, filter.GetExpectedNumberOfElements());
        Assert.True(filter.K > 0);
        Assert.True(filter.Size() > 0);
    }

    // Add/Contains tests
    [Fact]
    public void Add_ThenContains_ReturnsTrue()
    {
        var filter = new BloomFilter(0.01, 100);

        filter.Add("hello");

        Assert.True(filter.Contains("hello"));
        Assert.Equal(1, filter.Count());
    }

    [Fact]
    public void Contains_NotAdded_ReturnsFalse()
    {
        var filter = new BloomFilter(0.001, 100);

        filter.Add("hello");

        Assert.False(filter.Contains("world"));
    }

    [Fact]
    public void Add_Bytes_ThenContains_ReturnsTrue()
    {
        var filter = new BloomFilter(0.01, 100);
        var bytes = new byte[] { 1, 2, 3, 4, 5 };

        filter.Add(bytes);

        Assert.True(filter.Contains(bytes));
    }

    [Fact]
    public void AddAll_Objects_ThenContainsAll()
    {
        var filter = new BloomFilter(0.01, 100);
        var items = new object[] { "hello", "world", "test" };

        filter.AddAll(items);

        Assert.True(filter.ContainsAll(items));
        Assert.Equal(3, filter.Count());
    }

    [Fact]
    public void AddAll_Bytes_ThenContains()
    {
        var filter = new BloomFilter(0.01, 100);
        var items = new List<byte[]>
        {
            new byte[] { 1, 2, 3 },
            new byte[] { 4, 5, 6 }
        };

        filter.AddAll(items);

        Assert.Equal(2, filter.Count());
    }

    // Clear
    [Fact]
    public void Clear_ResetsFilter()
    {
        var filter = new BloomFilter(0.01, 100);
        filter.Add("hello");
        filter.Add("world");

        Assert.Equal(2, filter.Count());

        filter.Clear();

        Assert.Equal(0, filter.Count());
        Assert.False(filter.Contains("hello"));
    }

    // BitOperations
    [Fact]
    public void GetBit_SetBit_Works()
    {
        var filter = new BloomFilter(1000, 100);

        Assert.False(filter.GetBit(5));

        filter.SetBit(5, true);

        Assert.True(filter.GetBit(5));

        filter.SetBit(5, false);

        Assert.False(filter.GetBit(5));
    }

    [Fact]
    public void GetBitSet_ReturnsBitArray()
    {
        var filter = new BloomFilter(1000, 100);
        var bitSet = filter.GetBitSet();

        Assert.NotNull(bitSet);
        Assert.Equal(1000, bitSet.Length);
    }

    // Size/Count/ExpectedElements
    [Fact]
    public void Size_ReturnsCorrectBitSetSize()
    {
        var filter = new BloomFilter(10.0, 50, 3);
        Assert.Equal(500, filter.Size());
    }

    [Fact]
    public void GetExpectedBitsPerElement_ReturnsCorrectValue()
    {
        var filter = new BloomFilter(10.0, 50, 3);
        Assert.Equal(10.0, filter.GetExpectedBitsPerElement());
    }

    [Fact]
    public void GetBitsPerElement_AfterAdds_ReturnsCorrectValue()
    {
        var filter = new BloomFilter(10.0, 50, 3);
        filter.Add("item1");
        filter.Add("item2");

        Assert.Equal(250.0, filter.GetBitsPerElement());
    }

    // FalsePositiveProbability
    [Fact]
    public void ExpectedFalsePositiveProbability_ReturnsValue()
    {
        var filter = new BloomFilter(0.01, 100);
        var prob = filter.ExpectedFalsePositiveProbability();

        Assert.True(prob > 0);
        Assert.True(prob <= 0.02); // Should be close to the specified probability
    }

    [Fact]
    public void GetFalsePositiveProbability_WithZeroElements_ReturnsZero()
    {
        var filter = new BloomFilter(0.01, 100);
        var prob = filter.GetFalsePositiveProbability();
        Assert.Equal(0.0, prob);
    }

    [Fact]
    public void GetFalsePositiveProbability_WithCustomCount_ReturnsValue()
    {
        var filter = new BloomFilter(0.01, 100);
        var prob = filter.GetFalsePositiveProbability(50);
        Assert.True(prob >= 0);
    }

    // CreateHash
    [Fact]
    public void CreateHash_String_ReturnsDeterministicHash()
    {
        var hash1 = BloomFilter.CreateHash("hello");
        var hash2 = BloomFilter.CreateHash("hello");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CreateHash_DifferentStrings_ReturnsDifferentHashes()
    {
        var hash1 = BloomFilter.CreateHash("hello");
        var hash2 = BloomFilter.CreateHash("world");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void CreateHash_Bytes_ReturnsDeterministicHash()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var hash1 = BloomFilter.CreateHash(data);
        var hash2 = BloomFilter.CreateHash(data);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CreateHashes_ReturnsRequestedNumber()
    {
        var data = new byte[] { 1, 2, 3 };
        var hashes = BloomFilter.CreateHashes(data, 5);

        Assert.Equal(5, hashes.Length);
    }

    // Equals/GetHashCode
    [Fact]
    public void Equals_SameConfiguration_ReturnsTrue()
    {
        var filter1 = new BloomFilter(10.0, 100, 7);
        var filter2 = new BloomFilter(10.0, 100, 7);

        Assert.Equal(filter1, filter2);
    }

    [Fact]
    public void Equals_DifferentConfiguration_ReturnsFalse()
    {
        var filter1 = new BloomFilter(10.0, 100, 7);
        var filter2 = new BloomFilter(10.0, 200, 7);

        Assert.NotEqual(filter1, filter2);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var filter = new BloomFilter(10.0, 100, 7);
        Assert.False(filter.Equals(null));
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var filter = new BloomFilter(10.0, 100, 7);
        Assert.False(filter.Equals("not a filter"));
    }

    [Fact]
    public void GetHashCode_SameConfiguration_ReturnsSameHash()
    {
        var filter1 = new BloomFilter(10.0, 100, 7);
        var filter2 = new BloomFilter(10.0, 100, 7);

        Assert.Equal(filter1.GetHashCode(), filter2.GetHashCode());
    }

    // HashBytes
    [Fact]
    public void HashBytes_DeterministicResult()
    {
        var bits = new System.Collections.BitArray(32);
        bits.Set(0, true);
        bits.Set(5, true);

        var hash1 = BloomFilter.HashBytes(bits);
        var hash2 = BloomFilter.HashBytes(bits);

        Assert.Equal(hash1, hash2);
    }

    // Static Equals for BitArrays
    [Fact]
    public void Equals_BitArrays_SameContent_ReturnsTrue()
    {
        var a = new System.Collections.BitArray(32);
        var b = new System.Collections.BitArray(32);
        a.Set(5, true);
        b.Set(5, true);

        Assert.True(BloomFilter.Equals(a, b));
    }

    [Fact]
    public void Equals_BitArrays_DifferentContent_ReturnsFalse()
    {
        var a = new System.Collections.BitArray(32);
        var b = new System.Collections.BitArray(32);
        a.Set(5, true);
        b.Set(10, true);

        Assert.False(BloomFilter.Equals(a, b));
    }

    [Fact]
    public void Equals_BitArrays_DifferentLength_ReturnsFalse()
    {
        var a = new System.Collections.BitArray(32);
        var b = new System.Collections.BitArray(64);

        Assert.False(BloomFilter.Equals(a, b));
    }

    // Constructor with existing data
    [Fact]
    public void Constructor_WithExistingData_RestoresState()
    {
        var filter = new BloomFilter(1000, 100);
        filter.Add("hello");
        filter.Add("world");

        var bitSet = filter.GetBitSet();
        var restored = new BloomFilter(1000, 100, 2, bitSet);

        Assert.True(restored.Contains("hello"));
        Assert.True(restored.Contains("world"));
        Assert.Equal(2, restored.Count());
    }
}
