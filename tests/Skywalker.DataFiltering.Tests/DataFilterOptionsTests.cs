using Microsoft.Extensions.DependencyInjection;
using Skywalker.Data.Filtering;
using Xunit;

namespace Skywalker.DataFiltering.Tests;

public class DataFilterOptionsTests
{
    [Fact]
    public void Configure_SetsDefaultState()
    {
        var services = new ServiceCollection();
        services.AddDataFiltering(options =>
        {
            options.Configure<ISoftDelete>(false);
        });
        var provider = services.BuildServiceProvider();
        var dataFilter = provider.GetRequiredService<IDataFilter>();

        Assert.False(dataFilter.IsEnabled<ISoftDelete>());
    }

    [Fact]
    public void Configure_CustomFilter_SetsDefaultState()
    {
        var services = new ServiceCollection();
        services.AddDataFiltering(options =>
        {
            options.Configure<ICustomFilter>(false);
        });
        var provider = services.BuildServiceProvider();
        var dataFilter = provider.GetRequiredService<IDataFilter>();

        Assert.False(dataFilter.IsEnabled<ICustomFilter>());
    }

    public interface ICustomFilter { }
}

