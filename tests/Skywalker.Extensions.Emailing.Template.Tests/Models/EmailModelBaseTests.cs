using Skywalker.Extensions.Emailing.Template.Models;

namespace Skywalker.Extensions.Emailing.Template.Tests.Models;

public class EmailModelBaseTests
{
    private class TestEmailModel : EmailModelBase { }

    [Fact]
    public void Year_ShouldDefaultToCurrentYear()
    {
        var model = new TestEmailModel();

        Assert.Equal(DateTime.Now.Year, model.Year);
    }

    [Fact]
    public void AppName_ShouldBeNullByDefault()
    {
        var model = new TestEmailModel();

        Assert.Null(model.AppName);
    }

    [Fact]
    public void AppUrl_ShouldBeNullByDefault()
    {
        var model = new TestEmailModel();

        Assert.Null(model.AppUrl);
    }

    [Fact]
    public void SetProperties_ShouldUpdateValues()
    {
        var model = new TestEmailModel
        {
            AppName = "TestApp",
            AppUrl = "https://example.com",
            Year = 2024
        };

        Assert.Equal("TestApp", model.AppName);
        Assert.Equal("https://example.com", model.AppUrl);
        Assert.Equal(2024, model.Year);
    }
}

