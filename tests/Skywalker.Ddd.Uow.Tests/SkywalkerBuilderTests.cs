using Microsoft.Extensions.DependencyInjection;
using Skywalker;

namespace Skywalker.Ddd.Uow.Tests;

public class SkywalkerBuilderTests
{
    [Fact]
    public void AddSkywalker_ShouldReturnISkywalkerBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddSkywalker();

        // Assert
        Assert.NotNull(builder);
        Assert.IsAssignableFrom<ISkywalkerBuilder>(builder);
        Assert.Same(services, builder.Services);
    }

    [Fact]
    public void AddSkywalker_ShouldRegisterUnitOfWorkServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSkywalker();

        // Assert — UoW 核心服务应已注册
        Assert.Contains(services, d => d.ServiceType == typeof(Abstractions.IUnitOfWorkManager));
        Assert.Contains(services, d => d.ServiceType == typeof(Abstractions.IUnitOfWorkAccessor));
    }

    [Fact]
    public void AddSkywalker_Builder_Services_ShouldBeSameCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddSkywalker();

        // Assert — Builder.Services 是同一个 IServiceCollection
        Assert.Same(services, builder.Services);
    }
}
