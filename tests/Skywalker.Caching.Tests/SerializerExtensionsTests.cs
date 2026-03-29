using NSubstitute;
using Skywalker.Caching;
using Skywalker.Caching.Abstractions;

namespace Skywalker.Caching.Tests;

public class SerializerExtensionsTests
{
    [Fact]
    public void Deserialize_Generic_DelegatesToNonGenericMethod()
    {
        var serializer = Substitute.For<ICachingSerializer>();
        var bytes = new byte[] { 1, 2, 3 };
        serializer.Deserialize(typeof(string), bytes).Returns("hello");

        var result = serializer.Deserialize<string>(bytes);

        Assert.Equal("hello", result);
        serializer.Received(1).Deserialize(typeof(string), bytes);
    }

    [Fact]
    public void Deserialize_Generic_ReturnsNull_WhenDeserializerReturnsNull()
    {
        var serializer = Substitute.For<ICachingSerializer>();
        serializer.Deserialize(typeof(string), null).Returns((object?)null);

        var result = serializer.Deserialize<string>(null);

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_Generic_ThrowsArgumentNullException_WhenSerializerIsNull()
    {
        ICachingSerializer serializer = null!;

        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize<string>(Array.Empty<byte>()));
    }

    [Fact]
    public void Deserialize_Generic_HandlesValueTypes()
    {
        var serializer = Substitute.For<ICachingSerializer>();
        var bytes = new byte[] { 42 };
        serializer.Deserialize(typeof(int), bytes).Returns(42);

        var result = serializer.Deserialize<int>(bytes);

        Assert.Equal(42, result);
    }
}
