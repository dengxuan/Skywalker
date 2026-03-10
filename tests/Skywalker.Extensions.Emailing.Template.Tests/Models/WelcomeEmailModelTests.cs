using Skywalker.Extensions.Emailing.Template.Models;

namespace Skywalker.Extensions.Emailing.Template.Tests.Models;

public class WelcomeEmailModelTests
{
    [Fact]
    public void UserName_ShouldBeNullByDefault()
    {
        var model = new WelcomeEmailModel();

        Assert.Null(model.UserName);
    }

    [Fact]
    public void ActivationLink_ShouldBeNullByDefault()
    {
        var model = new WelcomeEmailModel();

        Assert.Null(model.ActivationLink);
    }

    [Fact]
    public void ActivationLinkExpirationHours_ShouldDefaultTo24()
    {
        var model = new WelcomeEmailModel();

        Assert.Equal(24, model.ActivationLinkExpirationHours);
    }

    [Fact]
    public void SetProperties_ShouldUpdateValues()
    {
        var model = new WelcomeEmailModel
        {
            UserName = "John Doe",
            ActivationLink = "https://example.com/activate?token=abc123",
            ActivationLinkExpirationHours = 48,
            AppName = "MyApp"
        };

        Assert.Equal("John Doe", model.UserName);
        Assert.Equal("https://example.com/activate?token=abc123", model.ActivationLink);
        Assert.Equal(48, model.ActivationLinkExpirationHours);
        Assert.Equal("MyApp", model.AppName);
    }

    [Fact]
    public void InheritsFromEmailModelBase()
    {
        var model = new WelcomeEmailModel();

        Assert.IsAssignableFrom<EmailModelBase>(model);
    }
}

