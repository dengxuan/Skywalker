using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Skywalker;
using Skywalker.Ddd.AspNetCore.ExceptionHandling;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using Skywalker.Exceptions;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests;

public class AddAspNetCoreTests
{
    [Fact]
    public void AddAspNetCore_ShouldReturnSameBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = Substitute.For<ISkywalkerBuilder>();
        builder.Services.Returns(services);

        // Act
        var result = builder.AddAspNetCore();

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void AddAspNetCore_ShouldRegisterExceptionHandlingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = Substitute.For<ISkywalkerBuilder>();
        builder.Services.Returns(services);

        // Act
        builder.AddAspNetCore();

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IHttpExceptionStatusCodeFinder));
        Assert.Contains(services, d => d.ServiceType == typeof(IExceptionToErrorInfoConverter));
    }

    [Fact]
    public void AddAspNetCore_ShouldRegisterResponseWrapperServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = Substitute.For<ISkywalkerBuilder>();
        builder.Services.Returns(services);

        // Act
        builder.AddAspNetCore();

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IErrorBuilder));
    }
}
