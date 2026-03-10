using Skywalker.Extensions.Emailing;
using Xunit;

namespace Skywalker.Extensions.Emailing.Tests;

public class SmtpEmailSenderConfigurationTests
{
    [Fact]
    public void SmtpEmailSenderConfiguration_DefaultFromAddress_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.DefaultFromAddress = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", config.DefaultFromAddress);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_DefaultFromDisplayName_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.DefaultFromDisplayName = "Test Sender";

        // Assert
        Assert.Equal("Test Sender", config.DefaultFromDisplayName);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_Host_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.Host = "smtp.example.com";

        // Assert
        Assert.Equal("smtp.example.com", config.Host);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_Port_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.Port = 587;

        // Assert
        Assert.Equal(587, config.Port);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_UserName_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.UserName = "user@example.com";

        // Assert
        Assert.Equal("user@example.com", config.UserName);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_Password_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.Password = "secret123";

        // Assert
        Assert.Equal("secret123", config.Password);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_Domain_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.Domain = "example.com";

        // Assert
        Assert.Equal("example.com", config.Domain);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_EnableSsl_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.EnableSsl = true;

        // Assert
        Assert.True(config.EnableSsl);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_UseDefaultCredentials_ShouldBeSettable()
    {
        // Arrange
        var config = new SmtpEmailSenderConfiguration();

        // Act
        config.UseDefaultCredentials = true;

        // Assert
        Assert.True(config.UseDefaultCredentials);
    }

    [Fact]
    public void SmtpEmailSenderConfiguration_DefaultValues_ShouldBeNull()
    {
        // Arrange & Act
        var config = new SmtpEmailSenderConfiguration();

        // Assert
        Assert.Null(config.DefaultFromAddress);
        Assert.Null(config.DefaultFromDisplayName);
        Assert.Null(config.Host);
        Assert.Equal(0, config.Port);
        Assert.Null(config.UserName);
        Assert.Null(config.Password);
        Assert.Null(config.Domain);
        Assert.False(config.EnableSsl);
        Assert.False(config.UseDefaultCredentials);
    }
}

