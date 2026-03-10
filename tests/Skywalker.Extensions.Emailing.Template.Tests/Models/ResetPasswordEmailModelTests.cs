using Skywalker.Extensions.Emailing.Template.Models;

namespace Skywalker.Extensions.Emailing.Template.Tests.Models;

public class ResetPasswordEmailModelTests
{
    [Fact]
    public void UserName_ShouldBeNullByDefault()
    {
        var model = new ResetPasswordEmailModel();

        Assert.Null(model.UserName);
    }

    [Fact]
    public void ResetLink_ShouldBeNullByDefault()
    {
        var model = new ResetPasswordEmailModel();

        Assert.Null(model.ResetLink);
    }

    [Fact]
    public void ResetLinkExpirationHours_ShouldDefaultTo24()
    {
        var model = new ResetPasswordEmailModel();

        Assert.Equal(24, model.ResetLinkExpirationHours);
    }

    [Fact]
    public void SetProperties_ShouldUpdateValues()
    {
        var model = new ResetPasswordEmailModel
        {
            UserName = "John Doe",
            ResetLink = "https://example.com/reset?token=xyz789",
            ResetLinkExpirationHours = 2,
            AppName = "MyApp"
        };

        Assert.Equal("John Doe", model.UserName);
        Assert.Equal("https://example.com/reset?token=xyz789", model.ResetLink);
        Assert.Equal(2, model.ResetLinkExpirationHours);
        Assert.Equal("MyApp", model.AppName);
    }

    [Fact]
    public void InheritsFromEmailModelBase()
    {
        var model = new ResetPasswordEmailModel();

        Assert.IsAssignableFrom<EmailModelBase>(model);
    }
}

