using Skywalker.Ddd.EntityFrameworkCore.ValueConverters;

namespace Skywalker.Ddd.EntityFrameworkCore.Tests;

public class ValueConverterTests
{
    public record TestData(string Name, int Value, List<string> Tags);

    [Fact]
    public void SkywalkerJsonValueConverter_ShouldSerializeObject()
    {
        // Arrange
        var converter = new SkywalkerJsonValueConverter<TestData>();
        var data = new TestData("Test", 42, ["tag1", "tag2"]);

        // Act
        var json = converter.ConvertToProvider(data) as string;

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.Contains("\"Name\":\"Test\"", json);
        Assert.Contains("\"Value\":42", json);
    }

    [Fact]
    public void SkywalkerJsonValueConverter_ShouldDeserializeObject()
    {
        // Arrange
        var converter = new SkywalkerJsonValueConverter<TestData>();
        var json = "{\"Name\":\"Test\",\"Value\":42,\"Tags\":[\"tag1\",\"tag2\"]}";

        // Act
        var data = converter.ConvertFromProvider(json) as TestData;

        // Assert
        Assert.NotNull(data);
        Assert.Equal("Test", data!.Name);
        Assert.Equal(42, data.Value);
        Assert.Equal(2, data.Tags.Count);
        Assert.Contains("tag1", data.Tags);
        Assert.Contains("tag2", data.Tags);
    }

    [Fact]
    public void SkywalkerJsonValueConverter_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var converter = new SkywalkerJsonValueConverter<TestData>();
        var original = new TestData("RoundTrip", 100, ["a", "b", "c"]);

        // Act
        var json = converter.ConvertToProvider(original) as string;
        var restored = converter.ConvertFromProvider(json) as TestData;

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Name, restored!.Name);
        Assert.Equal(original.Value, restored.Value);
        Assert.Equal(original.Tags.Count, restored.Tags.Count);
    }

    [Fact]
    public void SkywalkerJsonValueConverter_ShouldHandleComplexTypes()
    {
        // Arrange
        var converter = new SkywalkerJsonValueConverter<Dictionary<string, object>>();
        var data = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = 123
        };

        // Act
        var json = converter.ConvertToProvider(data) as string;

        // Assert
        Assert.NotNull(json);
        Assert.Contains("key1", json);
        Assert.Contains("value1", json);
    }

    [Fact]
    public void SkywalkerJsonValueConverter_ShouldHandleArrays()
    {
        // Arrange
        var converter = new SkywalkerJsonValueConverter<int[]>();
        var data = new[] { 1, 2, 3, 4, 5 };

        // Act
        var json = converter.ConvertToProvider(data) as string;
        var restored = converter.ConvertFromProvider(json) as int[];

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(data.Length, restored!.Length);
        for (int i = 0; i < data.Length; i++)
        {
            Assert.Equal(data[i], restored[i]);
        }
    }
}

