using Xunit;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Tests;

public class AjaxResponseTests
{
    [Fact]
    public void DefaultConstructor_ShouldBeSuccess()
    {
        var response = new AjaxResponse();
        Assert.True(response.Success);
        Assert.Null(response.Error);
        Assert.Null(response.Result);
    }

    [Fact]
    public void Constructor_WithBoolFalse_ShouldSetSuccess()
    {
        var response = new AjaxResponse(false);
        Assert.False(response.Success);
    }

    [Fact]
    public void Constructor_WithResult_ShouldBeSuccess()
    {
        var response = new AjaxResponse("hello");
        Assert.True(response.Success);
        Assert.Equal("hello", response.Result);
    }

    [Fact]
    public void Constructor_WithError_ShouldNotBeSuccess()
    {
        var error = new Error("E001", "Something failed");
        var response = new AjaxResponse(error);
        Assert.False(response.Success);
        Assert.Equal(error, response.Error);
        Assert.False(response.UnAuthorizedRequest);
    }

    [Fact]
    public void Constructor_WithError_Unauthorized_ShouldSetFlag()
    {
        var error = new Error("E401", "Unauthorized");
        var response = new AjaxResponse(error, true);
        Assert.False(response.Success);
        Assert.True(response.UnAuthorizedRequest);
    }

    [Fact]
    public void GenericAjaxResponse_WithResult_ShouldBeSuccess()
    {
        var response = new AjaxResponse<int>(42);
        Assert.True(response.Success);
        Assert.Equal(42, response.Result);
    }

    [Fact]
    public void GenericAjaxResponse_DefaultConstructor_ShouldBeSuccess()
    {
        var response = new AjaxResponse<string>();
        Assert.True(response.Success);
        Assert.Null(response.Result);
    }

    [Fact]
    public void GenericAjaxResponse_WithError_ShouldNotBeSuccess()
    {
        var error = new Error("E001", "fail");
        var response = new AjaxResponse<string>(error);
        Assert.False(response.Success);
        Assert.Equal(error, response.Error);
    }
}

public class ErrorTests
{
    [Fact]
    public void Constructor_WithCodeAndMessage_ShouldSetProperties()
    {
        var error = new Error("E001", "Something failed");
        Assert.Equal("E001", error.Code);
        Assert.Equal("Something failed", error.Message);
        Assert.Null(error.Details);
        Assert.Null(error.ValidationErrors);
    }

    [Fact]
    public void Constructor_WithCodeMessageAndDetails_ShouldSetAll()
    {
        var error = new Error("E001", "Something failed", "stack trace here");
        Assert.Equal("E001", error.Code);
        Assert.Equal("Something failed", error.Message);
        Assert.Equal("stack trace here", error.Details);
    }

    [Fact]
    public void ValidationErrors_ShouldBeSettable()
    {
        var error = new Error("E001", "validation failed");
        error.ValidationErrors = new[]
        {
            new ValidationErrorDto("Name", "Name is required"),
        };
        Assert.Single(error.ValidationErrors);
    }
}

public class ValidationErrorDtoTests
{
    [Fact]
    public void Constructor_WithMemberAndMessage_ShouldSetProperties()
    {
        var error = new ValidationErrorDto("Name", "Name is required");
        Assert.Equal("Name", error.Member);
        Assert.Equal("Name is required", error.Message);
    }
}
