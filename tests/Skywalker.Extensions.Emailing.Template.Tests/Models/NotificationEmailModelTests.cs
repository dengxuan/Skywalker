using Skywalker.Extensions.Emailing.Template.Models;

namespace Skywalker.Extensions.Emailing.Template.Tests.Models;

public class NotificationEmailModelTests
{
    [Fact]
    public void UserName_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.UserName);
    }

    [Fact]
    public void Title_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.Title);
    }

    [Fact]
    public void Message_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.Message);
    }

    [Fact]
    public void ActionText_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.ActionText);
    }

    [Fact]
    public void ActionUrl_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.ActionUrl);
    }

    [Fact]
    public void Timestamp_ShouldBeNullByDefault()
    {
        var model = new NotificationEmailModel();

        Assert.Null(model.Timestamp);
    }

    [Fact]
    public void SetProperties_ShouldUpdateValues()
    {
        var timestamp = DateTime.UtcNow;
        var model = new NotificationEmailModel
        {
            UserName = "User",
            Title = "New Message",
            Message = "You have a new message",
            ActionText = "View Message",
            ActionUrl = "https://example.com/messages/1",
            Timestamp = timestamp,
            AppName = "MyApp"
        };

        Assert.Equal("User", model.UserName);
        Assert.Equal("New Message", model.Title);
        Assert.Equal("You have a new message", model.Message);
        Assert.Equal("View Message", model.ActionText);
        Assert.Equal("https://example.com/messages/1", model.ActionUrl);
        Assert.Equal(timestamp, model.Timestamp);
        Assert.Equal("MyApp", model.AppName);
    }

    [Fact]
    public void InheritsFromEmailModelBase()
    {
        var model = new NotificationEmailModel();

        Assert.IsAssignableFrom<EmailModelBase>(model);
    }
}

