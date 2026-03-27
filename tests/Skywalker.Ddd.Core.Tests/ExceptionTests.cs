using Skywalker.Ddd.Exceptions;

namespace Skywalker.Ddd.Core.Tests;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetCode()
    {
        var ex = new EntityNotFoundException();
        Assert.Equal("Skywalker:EntityNotFound", ex.Code);
        Assert.Null(ex.EntityType);
        Assert.Null(ex.EntityId);
    }

    [Fact]
    public void Constructor_WithEntityType_ShouldSetMessageAndType()
    {
        var ex = new EntityNotFoundException(typeof(string));
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Null(ex.EntityId);
        Assert.Contains("System.String", ex.Message);
        Assert.Contains("was not found", ex.Message);
    }

    [Fact]
    public void Constructor_WithEntityTypeAndId_ShouldSetMessageWithId()
    {
        var ex = new EntityNotFoundException(typeof(string), 42);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Equal(42, ex.EntityId);
        Assert.Contains("42", ex.Message);
        Assert.Contains("System.String", ex.Message);
    }

    [Fact]
    public void Constructor_WithEntityTypeIdAndInner_ShouldChainInner()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new EntityNotFoundException(typeof(string), 42, inner);
        Assert.Equal(inner, ex.InnerException);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Equal(42, ex.EntityId);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new EntityNotFoundException("custom message");
        Assert.Equal("custom message", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInner_ShouldChainInner()
    {
        var inner = new Exception("inner");
        var ex = new EntityNotFoundException("msg", inner);
        Assert.Equal("msg", ex.Message);
        Assert.Equal(inner, ex.InnerException);
    }
}

public class AuthorizationExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaults()
    {
        var ex = new AuthorizationException();
        Assert.Equal("Skywalker:Authorization", ex.Code);
        Assert.Equal(Microsoft.Extensions.Logging.LogLevel.Warning, ex.LogLevel);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetCodeAndMessage()
    {
        var ex = new AuthorizationException("forbidden");
        Assert.Equal("forbidden", ex.Message);
        Assert.Equal("Skywalker:Authorization", ex.Code);
    }

    [Fact]
    public void Constructor_WithMessageAndInner_ShouldChainInner()
    {
        var inner = new Exception("inner");
        var ex = new AuthorizationException("msg", inner);
        Assert.Equal("msg", ex.Message);
        Assert.Equal(inner, ex.InnerException);
        Assert.Equal("Skywalker:Authorization", ex.Code);
    }

    [Fact]
    public void Constructor_WithMessageAndCode_ShouldSetCustomCode()
    {
        var ex = new AuthorizationException("msg", "CustomCode");
        Assert.Equal("msg", ex.Message);
        Assert.Equal("CustomCode", ex.Code);
    }

    [Fact]
    public void Constructor_WithMessageCodeAndInner_ShouldSetAll()
    {
        var inner = new Exception("inner");
        var ex = new AuthorizationException("msg", "Code1", inner);
        Assert.Equal("Code1", ex.Code);
        Assert.Equal(inner, ex.InnerException);
    }

    [Fact]
    public void WithData_ShouldSetDataAndReturnSelf()
    {
        var ex = new AuthorizationException("msg");
        var result = ex.WithData("key", "value");
        Assert.Same(ex, result);
        Assert.Equal("value", ex.Data["key"]);
    }

    [Fact]
    public void LogLevel_ShouldBeSettable()
    {
        var ex = new AuthorizationException();
        ex.LogLevel = Microsoft.Extensions.Logging.LogLevel.Error;
        Assert.Equal(Microsoft.Extensions.Logging.LogLevel.Error, ex.LogLevel);
    }
}

public class UserFriendlyExceptionTests
{
    [Fact]
    public void Constructor_WithCode_ShouldSetCode()
    {
        var ex = new UserFriendlyException("ERR001");
        Assert.Equal("ERR001", ex.Code);
        Assert.Equal(string.Empty, ex.Message);
    }

    [Fact]
    public void Constructor_WithCodeAndMessage_ShouldSetBoth()
    {
        var ex = new UserFriendlyException("ERR001", "Something went wrong");
        Assert.Equal("ERR001", ex.Code);
        Assert.Equal("Something went wrong", ex.Message);
    }

    [Fact]
    public void WithData_ShouldSetDataAndReturnSelf()
    {
        var ex = new UserFriendlyException("ERR001", "msg");
        var result = ex.WithData("userId", 123);
        Assert.Same(ex, result);
        Assert.Equal(123, ex.Data["userId"]);
    }
}

public class SkywalkerValidationExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveEmptyErrors()
    {
        var ex = new SkywalkerValidationException();
        Assert.Empty(ex.ValidationErrors);
        Assert.Equal("Skywalker:Validation", ex.Code);
        Assert.Equal(Microsoft.Extensions.Logging.LogLevel.Warning, ex.LogLevel);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new SkywalkerValidationException("invalid input");
        Assert.Equal("invalid input", ex.Message);
        Assert.Empty(ex.ValidationErrors);
    }

    [Fact]
    public void Constructor_WithValidationErrors_ShouldBuildMessage()
    {
        var errors = new[]
        {
            new System.ComponentModel.DataAnnotations.ValidationResult("Name is required"),
            new System.ComponentModel.DataAnnotations.ValidationResult("Age must be positive"),
        };

        var ex = new SkywalkerValidationException(errors);
        Assert.Equal(2, ex.ValidationErrors.Count);
        Assert.Contains("Name is required", ex.Message);
        Assert.Contains("Age must be positive", ex.Message);
    }

    [Fact]
    public void Constructor_WithEmptyValidationErrors_ShouldShowGenericMessage()
    {
        var ex = new SkywalkerValidationException(Array.Empty<System.ComponentModel.DataAnnotations.ValidationResult>());
        Assert.Equal("Validation failed.", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndErrors_ShouldUseProvidedMessage()
    {
        var errors = new[]
        {
            new System.ComponentModel.DataAnnotations.ValidationResult("err1"),
        };

        var ex = new SkywalkerValidationException("custom msg", errors);
        Assert.Equal("custom msg", ex.Message);
        Assert.Single(ex.ValidationErrors);
    }

    [Fact]
    public void Constructor_WithMessageAndInner_ShouldChainInner()
    {
        var inner = new Exception("inner");
        var ex = new SkywalkerValidationException("msg", inner);
        Assert.Equal(inner, ex.InnerException);
        Assert.Empty(ex.ValidationErrors);
    }
}

public class ErrorInfoTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveDefaults()
    {
        var info = new ErrorInfo();
        Assert.Null(info.Code);
        Assert.Equal(string.Empty, info.Message);
        Assert.Null(info.Details);
        Assert.Null(info.ValidationErrors);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var info = new ErrorInfo("error occurred");
        Assert.Equal("error occurred", info.Message);
    }

    [Fact]
    public void Constructor_WithCodeAndMessage_ShouldSetBoth()
    {
        var info = new ErrorInfo("E001", "error occurred");
        Assert.Equal("E001", info.Code);
        Assert.Equal("error occurred", info.Message);
    }

    [Fact]
    public void Constructor_WithCodeMessageAndDetails_ShouldSetAll()
    {
        var info = new ErrorInfo("E001", "error occurred", "stack trace");
        Assert.Equal("E001", info.Code);
        Assert.Equal("error occurred", info.Message);
        Assert.Equal("stack trace", info.Details);
    }
}

public class ValidationErrorInfoTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveDefaults()
    {
        var info = new ValidationErrorInfo();
        Assert.Equal(string.Empty, info.Message);
        Assert.Null(info.Members);
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var info = new ValidationErrorInfo("field required");
        Assert.Equal("field required", info.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndMembers_ShouldSetBoth()
    {
        var members = new[] { "Name", "Email" };
        var info = new ValidationErrorInfo("field required", members);
        Assert.Equal("field required", info.Message);
        Assert.Equal(members, info.Members);
    }
}
