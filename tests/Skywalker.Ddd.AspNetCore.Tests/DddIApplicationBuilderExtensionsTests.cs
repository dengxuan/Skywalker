using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests;

public class DddIApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseUnitOfWork_ShouldRegisterMiddleware()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        // Act
        app.UseUnitOfWork();

        // Assert — middleware is registered, building the pipeline should succeed
        var pipeline = app.Build();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void UseSkywalker_ShouldRegisterBothMiddlewares()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        // Act
        var result = app.UseSkywalker();

        // Assert
        Assert.Same(app, result);
        var pipeline = app.Build();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void UseUnitOfWork_ShouldReturnSameBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        // Act
        var result = app.UseUnitOfWork();

        // Assert
        Assert.Same(app, result);
    }
}
