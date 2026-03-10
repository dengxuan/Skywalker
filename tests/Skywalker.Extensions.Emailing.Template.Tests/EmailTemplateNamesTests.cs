using Skywalker.Extensions.Emailing.Template;

namespace Skywalker.Extensions.Emailing.Template.Tests;

public class EmailTemplateNamesTests
{
    [Fact]
    public void Welcome_ShouldHaveCorrectValue()
    {
        Assert.Equal("Skywalker.Emailing.Welcome", EmailTemplateNames.Welcome);
    }

    [Fact]
    public void ResetPassword_ShouldHaveCorrectValue()
    {
        Assert.Equal("Skywalker.Emailing.ResetPassword", EmailTemplateNames.ResetPassword);
    }

    [Fact]
    public void VerifyEmail_ShouldHaveCorrectValue()
    {
        Assert.Equal("Skywalker.Emailing.VerifyEmail", EmailTemplateNames.VerifyEmail);
    }

    [Fact]
    public void Notification_ShouldHaveCorrectValue()
    {
        Assert.Equal("Skywalker.Emailing.Notification", EmailTemplateNames.Notification);
    }

    [Fact]
    public void Layout_ShouldHaveCorrectValue()
    {
        Assert.Equal("Skywalker.Emailing.Layout", EmailTemplateNames.Layout);
    }

    [Fact]
    public void AllNames_ShouldHaveConsistentPrefix()
    {
        const string prefix = "Skywalker.Emailing.";

        Assert.StartsWith(prefix, EmailTemplateNames.Welcome);
        Assert.StartsWith(prefix, EmailTemplateNames.ResetPassword);
        Assert.StartsWith(prefix, EmailTemplateNames.VerifyEmail);
        Assert.StartsWith(prefix, EmailTemplateNames.Notification);
        Assert.StartsWith(prefix, EmailTemplateNames.Layout);
    }
}

