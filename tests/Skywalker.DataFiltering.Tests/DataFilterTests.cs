using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Data.Filtering;
using Xunit;

namespace Skywalker.DataFiltering.Tests;

public class DataFilterTests
{
    private readonly IDataFilter _dataFilter;

    public DataFilterTests()
    {
        var services = new ServiceCollection();
        services.AddDataFiltering();
        var provider = services.BuildServiceProvider();
        _dataFilter = provider.GetRequiredService<IDataFilter>();
    }

    [Fact]
    public void SoftDelete_IsEnabledByDefault()
    {
        Assert.True(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [Fact]
    public void MultiTenant_IsEnabledByDefault()
    {
        Assert.True(_dataFilter.IsEnabled<IMultiTenant>());
    }

    [Fact]
    public void Disable_DisablesFilter()
    {
        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());
        }
    }

    [Fact]
    public void Disable_RestoresStateAfterDispose()
    {
        Assert.True(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.True(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [Fact]
    public void Enable_EnablesFilter()
    {
        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());

            using (_dataFilter.Enable<ISoftDelete>())
            {
                Assert.True(_dataFilter.IsEnabled<ISoftDelete>());
            }

            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());
        }
    }

    [Fact]
    public void NestedDisable_WorksCorrectly()
    {
        Assert.True(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());

            using (_dataFilter.Disable<ISoftDelete>())
            {
                Assert.False(_dataFilter.IsEnabled<ISoftDelete>());
            }

            Assert.False(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.True(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [Fact]
    public void CustomFilter_DefaultsToEnabled()
    {
        Assert.True(_dataFilter.IsEnabled<ICustomFilter>());
    }

    public interface ICustomFilter { }
}

