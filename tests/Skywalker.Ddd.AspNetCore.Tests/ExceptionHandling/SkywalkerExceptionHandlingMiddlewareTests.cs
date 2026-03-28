using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Skywalker.Ddd.AspNetCore.ExceptionHandling;
using Skywalker.Ddd.AspNetCore.Mvc;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using Skywalker.Exceptions;
using Xunit;

namespace Skywalker.Ddd.AspNetCore.Tests.ExceptionHandling;

public class SkywalkerExceptionHandlingMiddlewareTests
{
    private readonly ILogger<SkywalkerExceptionHandlingMiddleware> _logger;

    public SkywalkerExceptionHandlingMiddlewareTests()
    {
        _logger = Substitute.For<ILogger<SkywalkerExceptionHandlingMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_NoException_ShouldCallNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithException_ShouldReturnErrorResponse()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidOperationException("Test error");
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_WithUserFriendlyException_ShouldReturnErrorWithCode()
    {
        // Arrange
        RequestDelegate next = _ => throw new UserFriendlyException("ERR001", "User friendly message");
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_With401Status_ShouldReturnUnauthorizedError()
    {
        // Arrange
        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_With403Status_ShouldReturnForbiddenError()
    {
        // Arrange
        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_With404Status_ShouldReturnNotFoundError()
    {
        // Arrange
        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        };
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_WithWrapResultFalse_ShouldNotWrapStatusCode()
    {
        // Arrange
        RequestDelegate next = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();
        context.Request.Headers["x-wrap-result"] = "false";

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Status code should remain 401, not wrapped
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Null(context.Response.ContentType); // No content written
    }

    [Fact]
    public async Task InvokeAsync_WithWrapResultFalse_ShouldReturnRawStatusCodeOnException()
    {
        // Arrange
        RequestDelegate next = _ => throw new EntityNotFoundException(typeof(object), "123");
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();
        context.Request.Headers["x-wrap-result"] = "false";

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Should return 404 status code instead of 200
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_WithWrapResultTrue_ShouldWrapResponse()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidOperationException("Test error");
        var middleware = new SkywalkerExceptionHandlingMiddleware(next, _logger);
        var context = CreateHttpContext();
        context.Request.Headers["x-wrap-result"] = "true";

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Should return 200 with wrapped response
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal(bool.TrueString, context.Response.Headers["x-wrap-result"]);
    }

    private static HttpContext CreateHttpContext()
    {
        var services = new ServiceCollection();
        var options = new ResponseWrapperOptions();
        services.AddSingleton(options);
        services.AddSingleton<IErrorBuilder>(new ErrorBuilder(options));
        var serviceProvider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider,
            Response = { Body = new MemoryStream() }
        };
        return context;
    }
}

