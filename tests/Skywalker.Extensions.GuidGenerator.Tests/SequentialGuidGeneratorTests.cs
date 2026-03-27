using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.GuidGenerator;

namespace Skywalker.Extensions.GuidGenerator.Tests;

public class SequentialGuidGeneratorViaServiceTests
{
    private IGuidGenerator CreateGenerator(SequentialGuidType? type = null)
    {
        var services = new ServiceCollection();
        services.AddGuidGenerator(opts =>
        {
            if (type.HasValue)
                opts.DefaultSequentialGuidType = type;
        });
        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IGuidGenerator>();
    }

    [Fact]
    public void Create_ShouldReturnNonEmptyGuid()
    {
        var generator = CreateGenerator(SequentialGuidType.SequentialAsString);
        var guid = generator.Create();
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void Create_ShouldReturnUniqueGuids()
    {
        var generator = CreateGenerator(SequentialGuidType.SequentialAsString);
        var guids = Enumerable.Range(0, 100)
            .Select(_ => generator.Create())
            .ToList();

        Assert.Equal(guids.Count, guids.Distinct().Count());
    }

    [Fact]
    public void Create_Default_ShouldReturnNonEmptyGuid()
    {
        var generator = CreateGenerator();
        var guid = generator.Create();
        Assert.NotEqual(Guid.Empty, guid);
    }
}

public class SequentialGuidGeneratorOptionsTests
{
    [Fact]
    public void GetDefaultSequentialGuidType_WithNull_ShouldReturnSequentialAsString()
    {
        var options = new SequentialGuidGeneratorOptions();
        Assert.Equal(SequentialGuidType.SequentialAsString, options.GetDefaultSequentialGuidType());
    }

    [Fact]
    public void GetDefaultSequentialGuidType_WithValue_ShouldReturnThatValue()
    {
        var options = new SequentialGuidGeneratorOptions
        {
            DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd
        };
        Assert.Equal(SequentialGuidType.SequentialAtEnd, options.GetDefaultSequentialGuidType());
    }

    [Fact]
    public void SectionName_ShouldBeCorrect()
    {
        Assert.Equal("Skywalker:GuidGenerator", SequentialGuidGeneratorOptions.SectionName);
    }
}
