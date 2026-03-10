using Skywalker.Sms;
using Xunit;

namespace Skywalker.Sms.Tests;

public class SmsMessageTests
{
    [Fact]
    public void SmsMessage_ShouldSetPhoneNumber()
    {
        // Arrange & Act
        var message = new SmsMessage("13800138000", "Test message");

        // Assert
        Assert.Equal("13800138000", message.PhoneNumber);
    }

    [Fact]
    public void SmsMessage_ShouldSetText()
    {
        // Arrange & Act
        var message = new SmsMessage("13800138000", "Test message");

        // Assert
        Assert.Equal("Test message", message.Text);
    }

    [Fact]
    public void SmsMessage_Properties_ShouldBeInitialized()
    {
        // Arrange & Act
        var message = new SmsMessage("13800138000", "Test message");

        // Assert
        Assert.NotNull(message.Properties);
        Assert.Empty(message.Properties);
    }

    [Fact]
    public void SmsMessage_Properties_ShouldBeModifiable()
    {
        // Arrange
        var message = new SmsMessage("13800138000", "Test message");

        // Act
        message.Properties["templateId"] = "SMS_001";
        message.Properties["signName"] = "TestSign";

        // Assert
        Assert.Equal(2, message.Properties.Count);
        Assert.Equal("SMS_001", message.Properties["templateId"]);
        Assert.Equal("TestSign", message.Properties["signName"]);
    }

    [Fact]
    public void SmsMessage_ShouldSupportInternationalPhoneNumber()
    {
        // Arrange & Act
        var message = new SmsMessage("+8613800138000", "Test message");

        // Assert
        Assert.Equal("+8613800138000", message.PhoneNumber);
    }

    [Fact]
    public void SmsMessage_ShouldSupportLongText()
    {
        // Arrange
        var longText = new string('a', 1000);

        // Act
        var message = new SmsMessage("13800138000", longText);

        // Assert
        Assert.Equal(longText, message.Text);
        Assert.Equal(1000, message.Text.Length);
    }
}

