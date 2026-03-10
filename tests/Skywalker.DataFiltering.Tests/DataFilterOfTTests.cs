using Microsoft.Extensions.DependencyInjection;
using Skywalker.Data.Filtering;
using Xunit;

namespace Skywalker.DataFiltering.Tests;

public class DataFilterOfTTests
{
    private readonly IDataFilter<ISoftDelete> _softDeleteFilter;

    public DataFilterOfTTests()
    {
        var services = new ServiceCollection();
        services.AddDataFiltering();
        var provider = services.BuildServiceProvider();
        _softDeleteFilter = provider.GetRequiredService<IDataFilter<ISoftDelete>>();
    }

    [Fact]
    public void IsEnabled_ReturnsCorrectState()
    {
        Assert.True(_softDeleteFilter.IsEnabled);
    }

    [Fact]
    public void Disable_DisablesFilter()
    {
        using (_softDeleteFilter.Disable())
        {
            Assert.False(_softDeleteFilter.IsEnabled);
        }
    }

    [Fact]
    public void Enable_EnablesFilter()
    {
        using (_softDeleteFilter.Disable())
        {
            Assert.False(_softDeleteFilter.IsEnabled);

            using (_softDeleteFilter.Enable())
            {
                Assert.True(_softDeleteFilter.IsEnabled);
            }
        }
    }
}

