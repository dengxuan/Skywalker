using System.Reflection;
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
        var uowAssembly = typeof(Abstractions.IUnitOfWorkManager).Assembly;

        // 诊断：检查程序集是否有 SkywalkerServicesAttribute
        var attr = uowAssembly.GetCustomAttribute<Skywalker.DependencyInjection.SkywalkerServicesAttribute>();
        Assert.NotNull(attr); // 确保 SourceGenerator 生成了该属性

        // Act — 显式传入 Uow 程序集，避免 GetCallingAssembly 在测试环境中行为不一致
        var builder = services.AddSkywalker(uowAssembly);

        // 诊断：检查 PartManager 发现了哪些程序集
        Assert.NotEmpty(builder.PartManager.Assemblies); // 应该至少发现一个程序集

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
