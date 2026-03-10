using Skywalker.ObjectMapping;
using Xunit;

namespace Skywalker.ObjectMapping.Tests;

public class DefaultObjectMapperTests
{
    private readonly IObjectMapper _mapper = new DefaultObjectMapper();

    [Fact]
    public void Map_SimpleProperties_MapsCorrectly()
    {
        var source = new SourceModel { Name = "Test", Age = 25 };

        var result = _mapper.Map<SourceModel, DestinationModel>(source);

        Assert.Equal("Test", result.Name);
        Assert.Equal(25, result.Age);
    }

    [Fact]
    public void Map_NullSource_ReturnsDefault()
    {
        SourceModel? source = null;

        var result = _mapper.Map<SourceModel, DestinationModel>(source!);

        Assert.Null(result);
    }

    [Fact]
    public void Map_ToExistingObject_UpdatesProperties()
    {
        var source = new SourceModel { Name = "Updated", Age = 30 };
        var destination = new DestinationModel { Name = "Original", Age = 20 };

        var result = _mapper.Map(source, destination);

        Assert.Same(destination, result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void Map_WithObjectOverload_MapsCorrectly()
    {
        object source = new SourceModel { Name = "Test", Age = 25 };

        var result = _mapper.Map<DestinationModel>(source);

        Assert.Equal("Test", result.Name);
        Assert.Equal(25, result.Age);
    }

    [Fact]
    public void MapCollection_MapsAllItems()
    {
        var sources = new[]
        {
            new SourceModel { Name = "A", Age = 1 },
            new SourceModel { Name = "B", Age = 2 }
        };

        var results = _mapper.MapList<SourceModel, DestinationModel>(sources);

        Assert.Equal(2, results.Count);
        Assert.Equal("A", results[0].Name);
        Assert.Equal("B", results[1].Name);
    }

    public class SourceModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class DestinationModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}

