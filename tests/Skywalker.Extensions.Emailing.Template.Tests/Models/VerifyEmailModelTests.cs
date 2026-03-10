using Skywalker.Extensions.Emailing.Template.Models;

namespace Skywalker.Extensions.Emailing.Template.Tests.Models;

public class VerifyEmailModelTests
{
    [Fact]
    public void UserName_ShouldBeNullByDefault()
    {
        var model = new VerifyEmailModel();

        Assert.Null(model.UserName);
    }

    [Fact]
    public void VerificationLink_ShouldBeNullByDefault()
    {
        var model = new VerifyEmailModel();

        Assert.Null(model.VerificationLink);
    }

    [Fact]
    public void VerificationCode_ShouldBeNullByDefault()
    {
        var model = new VerifyEmailModel();

        Assert.Null(model.VerificationCode);
    }

    [Fact]
    public void ExpirationHours_ShouldDefaultTo24()
    {
        var model = new VerifyEmailModel();

        Assert.Equal(24, model.ExpirationHours);
    }

    [Fact]
    public void SetProperties_ShouldUpdateValues()
    {
        var model = new VerifyEmailModel
        {
            UserName = "Jane Doe",
            VerificationLink = "https://example.com/verify?token=abc",
            VerificationCode = "123456",
            ExpirationHours = 1,
            AppName = "MyApp"
        };

        Assert.Equal("Jane Doe", model.UserName);
        Assert.Equal("https://example.com/verify?token=abc", model.VerificationLink);
        Assert.Equal("123456", model.VerificationCode);
        Assert.Equal(1, model.ExpirationHours);
        Assert.Equal("MyApp", model.AppName);
    }

    [Fact]
    public void InheritsFromEmailModelBase()
    {
        var model = new VerifyEmailModel();

        Assert.IsAssignableFrom<EmailModelBase>(model);
    }
}

